using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UniRx;
using UnityFx.Async;
using Newtonsoft.Json;
using JWT;
using JWT.Serializers;
using JWT.Algorithms;
using JWT.Exceptions;

namespace Simva
{
    public class AuthorizationInfo
    {
        private Dictionary<string, object> decodedJWT;
        private string access_token;

        public AuthorizationInfo()
        {
            Created = DateTime.Now;
        }

        [JsonIgnore]
        public string ClientId { get; set; }

        [JsonIgnore]
        public DateTime Created { get; private set; }

        [JsonProperty("access_token")]
        public string AccessToken
        {
            get { return access_token; }
            set
            {
                access_token = value;
                try
                {
                    IJsonSerializer serializer = new JsonNetSerializer();
                    var provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                    decodedJWT = decoder.DecodeToObject<Dictionary<string, object>>(access_token);
                    Debug.Log("JWT Token: " + JsonConvert.SerializeObject(decodedJWT));
                }
                catch (TokenExpiredException)
                {
                    Debug.LogError("Token has expired");
                }
                catch (SignatureVerificationException)
                {
                    Debug.LogError("Token has invalid signature");
                }
            }
        }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("expires_in")]
        public int Expires { get; set; }

        [JsonIgnore]
        public bool Expired { get { return Created + new TimeSpan(0, 0, Expires) > DateTime.Now; } }

        [JsonIgnore]
        public string Username { get { return (string)decodedJWT["preferred_username"]; } }
    }

    public static class OpenIdUtility
    {
        private static System.Diagnostics.Process windowProcess;
        private static Thread httpListener;
        public static bool tokenLogin;

        public delegate void UpdateDelegate();
        public static UpdateDelegate onUpdate;

        private class ResponseTransfer
        {
            public bool done;
            public bool result;
            public string code;
            public string status;
        }

        public static void GeneratePKCE(out string codeVerifier, out string codeChallenge)
        {
            var codeVerifierBytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(codeVerifierBytes);
            codeVerifier = Base64Url.Encode(codeVerifierBytes);
            // create code_challenge
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                codeChallenge = Base64Url.Encode(challengeBytes);
            }
        }

        public static void OpenBrowser(string url)
        {
            windowProcess = new System.Diagnostics.Process();

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (tokenLogin)
                {
                    url += "&token";
                }

                System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo("chrome.exe", "--app=\"" + url + "\"" + (tokenLogin ? " -incognito" : ""));
                windowProcess.StartInfo = proc;
            }

            if (windowProcess == null || !windowProcess.Start())
            {
                Application.OpenURL(url);
            }
        }

        public static void ListenForCode(int port, Action<bool, string, string> onLogin)
        {
            var responseTransfer = new ResponseTransfer();

            httpListener = new Thread(() =>
            {
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
                listener.Prefixes.Add("http://localhost:" + port + "/");
                listener.Start();

                // Note: The GetContext method blocks while waiting for a request. 
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                // Get the code
                var code = context.Request.QueryString.Get("code");
                var state = context.Request.QueryString.Get("session-state");

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString = "<HTML><BODY onload=\"javascript: setTimeout('self.close()', 10); \">Please close this window and go back to uAdventure</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
                if (windowProcess != null)
                {
                    if (!windowProcess.HasExited)
                    {
                        windowProcess.Kill();
                    }
                    windowProcess = null;
                }

                responseTransfer.result = true;
                responseTransfer.status = state;
                responseTransfer.code = code;
                responseTransfer.done = true;

                listener.Close();
            });

            httpListener.Start();

            responseTransfer.ObserveEveryValueChanged(r => r.done).Subscribe(done =>
            {
                if (done)
                {
                    onLogin(responseTransfer.result, responseTransfer.code, responseTransfer.status);
                }
            });
        }

        public static IAsyncOperation<AuthorizationInfo> LoginWithAccessCode(string authUrl, string tokenUrl, string clientId,
            string audience = null, string scope = null, bool usePKCE = false, string codeChallengeMethod = "S256")
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();

            var port = UnityEngine.Random.Range(25525, 65535);

            var url = authUrl + "?" +
                "response_type=code" +
                "&client_id=" + clientId +
                "&redirect_uri=http://127.0.0.1:" + port + "/redirect";

            string codeVerifier = null, codeChallenge = null;
            if (usePKCE)
            {
                GeneratePKCE(out codeVerifier, out codeChallenge);
                url += "&code_challenge=" + codeChallenge +
                       "&code_challenge_method=" + codeChallengeMethod;
            }

            if (!string.IsNullOrEmpty(scope))
            {
                url += "&scope=" + scope;
            }

            if (!string.IsNullOrEmpty(audience))
            {
                url += "&audience=" + audience;
            }

            ListenForCode(port, (logged, code, state) =>
            {
                var token = GetToken(tokenUrl, clientId, code, port, codeVerifier);
                result.SetResult(token);
            });
            OpenBrowser(url);
            return result;
        }

        public static IAsyncOperation<AuthorizationInfo> LoginWithROPC(string username, string password, string authUrl, string tokenUrl, string clientId,
            string audience, string scope = null)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();

            var port = UnityEngine.Random.Range(25525, 65535);

            var url = authUrl + "?" +
                "grant_type=password" +
                "&username=" + username +
                "&password=" + password +
                "&client_id=" + clientId;

            if (!string.IsNullOrEmpty(scope))
            {
                url += "&scope=" + scope;
            }

            if (!string.IsNullOrEmpty(audience))
            {
                url += "&audience=" + audience;
            }

            ListenForCode(port, (logged, code, state) =>
            {
                var token = GetToken(tokenUrl, clientId, code, port, null);
                result.SetResult(token);
            });
            OpenBrowser(url);
            return result;
        }

        public static AuthorizationInfo GetToken(string tokenUrl, string clientId, string authCode, int port, string codeVerifier)
        {
            var form = new Dictionary<string, string>()
                {
                    { "grant_type", "authorization_code" },
                    { "code", authCode },
                    { "redirect_uri", "http://127.0.0.1:" + port + "/redirect" },
                    { "client_id", clientId },
                };

            if (!string.IsNullOrEmpty(codeVerifier))
            {
                form.Add("code_verifier", codeVerifier);
            }

            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl, form);

            uwr.SendWebRequest();

            while (!uwr.isDone) { }

            var authInfo = JsonConvert.DeserializeObject<AuthorizationInfo>(uwr.downloadHandler.text);
            authInfo.ClientId = clientId;
            return authInfo;
        }

        public static AuthorizationInfo RefreshToken(string tokenUrl, string clientId, string refresh_token)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl,
                new Dictionary<string, string>()
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refresh_token },
                    { "client_id", clientId }
                });

            uwr.SendWebRequest();

            while (!uwr.isDone) { }

            var authInfo = JsonConvert.DeserializeObject<AuthorizationInfo>(uwr.downloadHandler.text);
            if (authInfo.AccessToken == null)
            {
                authInfo = null;
            }
            else
            {
                authInfo.ClientId = clientId;
            }
            return authInfo;
        }
    }

    /// <summary>
    /// Base64Url encoder/decoder
    /// </summary>
    public static class Base64Url
    {
        /// <summary>
        /// Encodes the specified byte array.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        public static string Encode(byte[] arg)
        {
            var s = Convert.ToBase64String(arg); // Standard base64 encoder

            s = s.Split('=')[0]; // Remove any trailing '='s
            s = s.Replace('+', '-'); // 62nd char of encoding
            s = s.Replace('/', '_'); // 63rd char of encoding

            return s;
        }

        /// <summary>
        /// Decodes the specified string.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Illegal base64url string!</exception>
        public static byte[] Decode(string arg)
        {
            var s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding

            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new Exception("Illegal base64url string!");
            }

            return Convert.FromBase64String(s); // Standard base64 decoder
        }
    }
}