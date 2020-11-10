using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UniRx;
using UnityEngine.Networking;
using UnityFx.Async;
using Newtonsoft.Json;
using System.IO;
using AssetPackage;
using System.Linq;
using uAdventure.Simva;
using uAdventure.Runner;
using System.Collections;
using UnityFx.Async.Promises;
using System.Runtime.InteropServices;

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
                /* try
                 {*/
                /*IJsonSerializer serializer = new JsonNetSerializer();
                var provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                decodedJWT = decoder.DecodeToObject<Dictionary<string, object>>(access_token);*/
                Debug.Log(access_token);
                var payload = Encoding.UTF8.GetString(Base64Url.Decode(access_token.Split('.')[1]));
                decodedJWT = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                Debug.Log(payload);
                /*}
                catch (TokenExpiredException)
                {
                    Debug.LogError("Token has expired");
                }
                catch (SignatureVerificationException)
                {
                    Debug.LogError("Token has invalid signature");
                }*/
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

	public class AuthorizationError
	{
		public AuthorizationError()	{}

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }
	}

	public static class OpenIdUtility
    {
        [DllImport("__Internal")]
        private static extern void OpenUrl(string url);

        [DllImport("__Internal")]
        private static extern string GetParameter(string name);

        [DllImport("__Internal")]
        private static extern void ClearUrl();

        [DllImport("__Internal")]
        private static extern string GetUrl();

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
            bool windowOpened = false;
            // window.open("https://www.youraddress.com", "_self")

            /*if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.OSXPlayer )
            {
                // create a configuration with OS-specific defaults
                var config = DefaultConfiguration.CreateForRuntimePlatform();

                // your configuration
                config.StartUrl = url;
                config.WindowOptions.Title = "Simva Login";
                //..
                windowOpened = true;

                // application builder
                AppBuilder
                    .Create()
                    .UseApp<ChromelyBasicApp>()
                    .UseConfiguration<IChromelyConfiguration>(config)
                    .Build()
                    .Run(new string[0]);
            }
            else */
            
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                windowProcess = new System.Diagnostics.Process();
                if (tokenLogin)
                {
                    url += "&token";
                }

                System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo("chrome.exe", "--app=\"" + url + "\"" + (tokenLogin ? " -incognito" : ""));
                windowProcess.StartInfo = proc;

                try
                {
                    windowOpened = windowProcess.Start();
                }
                catch(Exception ex)
                {
                    windowOpened = false;
                }

                if (!windowOpened)
                {
                    windowProcess = null;
                }
            }
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
#if UNITY_WEBGL
                OpenUrl(url);
#endif
            }

            if (!windowOpened)
            {
                Application.OpenURL(url);
            }
        }

        public class LoginResponse
        {
            public bool IsError
            {
                get { return !string.IsNullOrEmpty(Error); }
            }

            public string Error { get; set; }
            public string ErrorDescription { get; set; }

            public string Code { get; set; }
            public string SessionState { get; set; }
        }

        public static IAsyncOperation<LoginResponse> ListenForCode(int port, out string redirectUrl)
        {
            var result = new AsyncCompletionSource<LoginResponse>(); 

            if (Application.isEditor)
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

                    var error = context.Request.QueryString.Get("error");
                    var error_description = context.Request.QueryString.Get("error_description");
                    var code = context.Request.QueryString.Get("code");
                    var state = context.Request.QueryString.Get("session-state");

                    Debug.Log(context.Request.Url.Query);
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
                    if (!string.IsNullOrEmpty(error))
                    {
                        responseTransfer.result = false;
                        responseTransfer.code = error;
                        responseTransfer.status = error_description;
                    }
                    else
                    {
                        responseTransfer.result = true;
                        responseTransfer.code = code;
                        responseTransfer.status = state;
                    }
                    responseTransfer.done = true;

                    listener.Close();
                });

                httpListener.Start();

                responseTransfer.ObserveEveryValueChanged(r => r.done).Subscribe(done =>
                {
                    if (done)
                    {
                        if (responseTransfer.result)
                        {
                            result.SetResult(new LoginResponse
                            {
                                Code = responseTransfer.code,
                                SessionState = responseTransfer.status
                            });
                        }
                        else
                        {
                            result.SetResult(new LoginResponse
                            {
                                Error = responseTransfer.code,
                                ErrorDescription = responseTransfer.status
                            });
                        }
                    }
                });

                redirectUrl = "http://127.0.0.1:" + port + "/redirect";
            }
            else if(Application.platform == RuntimePlatform.WindowsPlayer)
            {
                int ipcport = SimvaUriHandler.Start(uri =>
                {
                    Debug.Log("uri received!");
                    var query = uri.Split('?')[1].Split('&');
                    var d = new Dictionary<string, string>();
                    foreach (var q in query.Select(q => q.Split('=')))
                    {
                        d.Add(q[0], q[1]);
                    }

                    SimvaUriHandler.Stop();
                    result.SetResult(new LoginResponse
                    {
                        Code = d["code"],
                        SessionState = d["session_state"]
                    });
                });

                redirectUrl =  Game.Instance.GameState.Data.getApplicationIdentifier() + "://" + ipcport + "/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                var objectName = "OpenIdListener";
                var methodName = "OnAuthReply";
                var openIdListener = new GameObject(objectName, typeof(OpenIdListener)).GetComponent<OpenIdListener>();

                openIdListener.onAuthReceived += uri =>
                {
                    Debug.Log("uri received!");
                    var query = uri.Split('?')[1].Split('&');
                    var d = new Dictionary<string, string>();
                    foreach (var q in query.Select(q => q.Split('=')))
                    {
                        d.Add(q[0], q[1]);
                    }

                    GameObject.DestroyImmediate(openIdListener.gameObject);
                    result.SetResult(new LoginResponse
                    {
                        Code = d["code"],
                        SessionState = d["session_state"]
                    });
                };

                redirectUrl = Game.Instance.GameState.Data.getApplicationIdentifier() + "://" + objectName + "/" + methodName + "/";
            }
            else if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                PlayerPrefs.SetInt("OpenIdTryContinueLogin", 1);
                PlayerPrefs.SetString("OpenIdRedirectUrl", GetUrl());
                PlayerPrefs.Save();
                Debug.Log("RedirectURL: " + GetUrl());
                redirectUrl = GetUrl();
            }
            else
            {
                throw new NotImplementedException("Platform: " + Application.platform + " is not yet implemented!");
            }

            return result;
        }


        public static IAsyncOperation<AuthorizationInfo> TryContinueLogin(string tokenUrl, string clientId)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();


            var loginResponse = ExtractLoginresponseFromUrl();
            Debug.Log("Login response read: " + loginResponse.Code + " - " + loginResponse.SessionState);

            var redirectUrl = PlayerPrefs.GetString("OpenIdRedirectUrl");
            if (string.IsNullOrEmpty(redirectUrl))
            {
                redirectUrl = GetUrl();
            }

            PlayerPrefs.DeleteKey("OpenIdTryContinueLogin");
            PlayerPrefs.DeleteKey("OpenIdRedirectUrl");

            if (loginResponse.IsError)
            {
                result.SetException(new ApiException(500, loginResponse.Error + ": " + loginResponse.ErrorDescription));
            }
            else
            {
                Debug.Log("Trying to get token from response...");
                GetToken(tokenUrl, clientId, loginResponse.Code, redirectUrl)
                .Then(token =>
                {
                    result.SetResult(token);
                })
                .Catch(ex => result.SetException(ex));
            }

            return result;
        }

        private static LoginResponse ExtractLoginresponseFromUrl()
        {
            var error = GetParameter("error");
            if (!string.IsNullOrEmpty(error))
            {
                return new LoginResponse
                {
                    Error = error,
                    ErrorDescription = GetParameter("error_description")
                };
            }
            else
            {
                return new LoginResponse
                {
                    Code = GetParameter("code"),
                    SessionState = GetParameter("session_state")
                };
            }
        }

        public static IAsyncOperation<AuthorizationInfo> LoginWithAccessCode(string authUrl, string tokenUrl, string clientId,
            string audience = null, string scope = null, bool usePKCE = false, string codeChallengeMethod = "S256")
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();

            var port = UnityEngine.Random.Range(25525, 65535);

            var url = authUrl + "?" +
                "response_type=code" +
                "&client_id=" + clientId;

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

            var redirectUri = string.Empty; 
                
            ListenForCode(port, out redirectUri)
            .Then(loginResponse => {
                if (loginResponse.IsError)
                {
                    result.SetException(new ApiException(500, loginResponse.Error + ": " + loginResponse.ErrorDescription));
                }
                else
                {
                    GetToken(tokenUrl, clientId, loginResponse.Code, redirectUri, codeVerifier)
                    .Then(token =>
                    {
                        result.SetResult(token);
                    })
                    .Catch(ex => result.SetException(ex));
                }

			});


            url += "&redirect_uri=" + redirectUri;
            Debug.Log(redirectUri);
            Debug.Log(url);

            OpenBrowser(url);
            return result;
        }

        public static IAsyncOperation<AuthorizationInfo> LoginWithROPC(string username, string password, string authUrl, string tokenUrl, string clientId,
            string audience, string scope = null)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();

            var port = UnityEngine.Random.Range(25525, 65535);

			var url = authUrl;
			var formUrlEncoded = "grant_type=password" +
                "&username=" + username +
                "&password=" + password +
                "&client_id=" + clientId;

            if (!string.IsNullOrEmpty(scope))
            {
				formUrlEncoded += "&scope=" + scope;
            }

            if (!string.IsNullOrEmpty(audience))
            {
				formUrlEncoded += "&audience=" + audience;
            }

			return GetToken(tokenUrl, formUrlEncoded, clientId);
        }

        public static bool HasLoginInfo()
        {
            if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                var state = GetParameter("session_state");
                var code = GetParameter("code");
                return !string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(code);
            }
            else
            {
                return false;
            }

        }

        public static IAsyncOperation<AuthorizationInfo> GetToken(string tokenUrl, string clientId, string authCode, int port, string codeVerifier)
        {
            return GetToken(tokenUrl, clientId, authCode, "http://127.0.0.1:" + port + "/redirect", codeVerifier);
        }

        public static IAsyncOperation<AuthorizationInfo> GetToken(string tokenUrl, string clientId, string authCode, string redirect_uri, string codeVerifier = null)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();
            var form = new Dictionary<string, string>()
                {
                    { "grant_type", "authorization_code" },
                    { "code", authCode },
                    { "redirect_uri", redirect_uri },
                    { "client_id", clientId },
                };

            Debug.Log("A - Redirect uri: " + redirect_uri);
            //Debug.Log(JsonConvert.SerializeObject(form, Formatting.Indented));

            if (!string.IsNullOrEmpty(codeVerifier))
            {
                form.Add("code_verifier", codeVerifier);
                Debug.Log("A2 - Code Verifier: " + codeVerifier);
            }

            Debug.Log("B");
            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl, form);

            Debug.Log("C");
            Observable.FromCoroutine(() => DoRequest(result, uwr)).Subscribe();

            var wrapper = new AsyncCompletionSource<AuthorizationInfo>();

            result.Then(authInfo =>
            {
                Debug.Log("D");
                authInfo.ClientId = clientId;
                wrapper.SetResult(authInfo);
            }).Catch(ex => wrapper.SetException(ex));

            return wrapper;
        }

		public static IAsyncOperation<AuthorizationInfo> GetToken(string tokenUrl, string formUrlEncoded, string clientId)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();
            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl, "");
			byte[] bytes = Encoding.UTF8.GetBytes(formUrlEncoded);
			UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
			uH.contentType = "application/x-www-form-urlencoded";
			uwr.uploadHandler = uH;

			uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            Observable.FromCoroutine(() => DoRequest(result, uwr)).Subscribe();

            var wrapper = new AsyncCompletionSource<AuthorizationInfo>();

            result.Then(authInfo =>
            {
                authInfo.ClientId = clientId;
                wrapper.SetResult(authInfo);
            }).Catch(ex => wrapper.SetException(ex));

            return wrapper;
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

            while(!uwr.isDone) { }

            ThrowErrors(uwr);

            var authInfo = JsonConvert.DeserializeObject<AuthorizationInfo>(uwr.downloadHandler.text);
            authInfo.ClientId = clientId;
            return authInfo;
        }

        public static IAsyncOperation<AuthorizationInfo> RefreshTokenAsync(string tokenUrl, string clientId, string refresh_token)
        {
            var result = new AsyncCompletionSource<AuthorizationInfo>();

            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl,
                new Dictionary<string, string>()
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refresh_token },
                    { "client_id", clientId }
                });

            Observable.FromCoroutine(() => DoRequest(result, uwr)).Subscribe();

            return result.Then(authInfo =>
            {
                var wrapper = new AsyncCompletionSource<AuthorizationInfo>();
                authInfo.ClientId = clientId;
                wrapper.SetResult(authInfo);
                return wrapper;
            });
        }

        private static void ThrowErrors(UnityWebRequest uwr)
		{
			if (uwr.isHttpError)
			{
				var error = uwr.downloadHandler.text;
				try
				{
					var authError = JsonConvert.DeserializeObject<AuthorizationError>(uwr.downloadHandler.text);
					error = authError.ErrorDescription;
				}
				catch { }
				throw new ApiException((int)uwr.responseCode, error);
			}
		}

		/// <summary>
		/// Web service request.
		/// </summary>
		///
		/// <param name="requestSettings">Options for controlling the operation. </param>
		///
		/// <returns>
		/// A RequestResponse.
		/// </returns>
		private static RequestResponse WebServiceRequest(RequestSetttings requestSettings)
        {
            RequestResponse result = new RequestResponse(requestSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestSettings.uri);

                request.Method = requestSettings.method;

                // Both Accept and Content-Type are not allowed as Headers in a HttpWebRequest.
                // They need to be assigned to a matching property.

                if (requestSettings.requestHeaders.ContainsKey("Accept"))
                {
                    request.Accept = requestSettings.requestHeaders["Accept"];
                }

                if (!String.IsNullOrEmpty(requestSettings.body))
                {
                    byte[] data = Encoding.UTF8.GetBytes(requestSettings.body);

                    if (requestSettings.requestHeaders.ContainsKey("Content-Type"))
                    {
                        request.ContentType = requestSettings.requestHeaders["Content-Type"];
                    }

                    foreach (KeyValuePair<string, string> kvp in requestSettings.requestHeaders)
                    {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }

                    request.ContentLength = data.Length;

                    // See https://msdn.microsoft.com/en-us/library/system.net.servicepoint.expect100continue(v=vs.110).aspx
                    // A2 currently does not support this 100-Continue response for POST requets.
                    request.ServicePoint.Expect100Continue = false;

                    Stream stream = request.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                }
                else
                {
                    foreach (KeyValuePair<string, string> kvp in requestSettings.requestHeaders)
                    {
                        if (kvp.Key.Equals("Accept") || kvp.Key.Equals("Content-Type"))
                        {
                            continue;
                        }
                        request.Headers.Add(kvp.Key, kvp.Value);
                    }
                }

                WebResponse response = request.GetResponse();
                if (response.Headers.HasKeys())
                {
                    foreach (string key in response.Headers.AllKeys)
                    {
                        result.responseHeaders.Add(key, response.Headers.Get(key));
                    }
                }

                result.responseCode = (int)(response as HttpWebResponse).StatusCode;

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result.body = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                result.responsMessage = e.Message;

                Debug.Log(String.Format("{0} - {1}", e.GetType().Name, e.Message));
            }

            return result;
        }

        private static IEnumerator DoRequest<T>(IAsyncCompletionSource<T> op, UnityWebRequest webRequest)
        {
            yield return webRequest.SendWebRequest();

            // Sometimes the webrequest is finished but the download is not
            while (!webRequest.isNetworkError && !webRequest.isHttpError && webRequest.downloadProgress != 1)
            {
                yield return new WaitForFixedUpdate();
            }

            if (webRequest.isNetworkError)
            {
                op.SetException(new ApiException((int)webRequest.responseCode, webRequest.error, webRequest.downloadHandler.text));
            }
            else if (webRequest.isHttpError)
            {
                Debug.Log(webRequest.downloadHandler.text);
                op.SetException(new ApiException((int)webRequest.responseCode, webRequest.error, webRequest.downloadHandler.text));
            }
            else
            {
                Debug.Log("Deserializing...");
                var deserialized = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                op.SetResult(deserialized);
            }

            webRequest.Dispose();
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