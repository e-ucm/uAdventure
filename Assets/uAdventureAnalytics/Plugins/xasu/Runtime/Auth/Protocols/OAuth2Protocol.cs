using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using TinCan;
using UnityEngine;
using UnityEngine.Networking;
using Xasu.Auth.Protocols.OAuth2;
using Xasu.Auth.Utils;
using Xasu.Exceptions;
using Xasu.Requests;

namespace Xasu.Auth.Protocols
{
    public class OAuth2Protocol : IAuthProtocol
    {
        private const string fieldMissingMessage = "Field \"{0}\" required for \"OAuth 2.0\" authentication is missing!";
        private const string unsupportedGrantTypeMessage = "Grant type \"{0}\" not supported. Please use either \"code\" type or \"password\" type.";
        private const string unsupportedCodeChallengeMethodMessage = "Code challenge (PKCE) method \"{0}\" not supported. Please use \"S256\" method or disable it.";

        private static IDictionary<string, string> initConfig;

        // Standard fields
        private const string authEndpointField = "auth_endpoint";
        private const string tokenEndpointField = "token_endpoint";
        private const string grantTypeField = "grant_type";
        private const string usernameField = "username";
        private const string refreshTokenField = "refresh_token";
        private const string passwordField = "password";
        private const string clientIdField = "client_id";
        private const string scopeField = "scope";
        private const string stateField = "state";
        private const string codeChallengeMethodField = "code_challenge_method";

        private string authEndpoint;
        private string tokenEndpoint;
        private string grantType;
        private string username;
        private string password;
        private string clientId;
        private string scope;
        private string state;
        private PKCETypes codeChallengeMethod;
        private OAuth2Token token;

        public OAuth2Token Token { get { return token; } }

        /* a ro no ma men
        private readonly string audienceField = "audience";
        private readonly string connectionField = "connection";
        private string connection;
        private string audience;
        audience = config.Value<string>(audienceField);
        connection = config.Value<string>(connectionField);
        */

        public IAsyncPolicy Policy { get; set; }

        public AuthState State { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public Agent Agent { get; protected set; }

        public delegate void OnAuthorizationInfoUpdate(OAuth2Token info);
        private OnAuthorizationInfoUpdate onAuthorizationInfoUpdate;

        public async Task Init(IDictionary<string, string> config)
        {
            initConfig = config;

            Debug.Log("[OAuth2] Starting");
            // Main params
            tokenEndpoint = config.GetRequiredValue(tokenEndpointField, fieldMissingMessage);
            grantType = config.GetRequiredValue(grantTypeField, fieldMissingMessage).ToLower();
            clientId = config.GetRequiredValue(clientIdField, fieldMissingMessage);

            // Parse PKCE
            codeChallengeMethod = PKCETypes.None;
            // We only support "S256" (SHA256) code challenge / PKCE method
            if (config.ContainsKey(codeChallengeMethodField))
            {
                var codeChallengeMethodString = config.Value(codeChallengeMethodField)?.ToUpper();
                switch (codeChallengeMethodString)
                {
                    case "S256": codeChallengeMethod = PKCETypes.S256; break;
                    default:
                        throw new NotSupportedException(string.Format(unsupportedCodeChallengeMethodMessage, codeChallengeMethodString));
                }
            }

            // Optional parameters
            scope = config.Value(scopeField);
            state = config.Value(stateField);

            // Grant type specific params
            switch (grantType)
            {
                case "refresh_token":
                    var refresh_token = config.GetRequiredValue(refreshTokenField, fieldMissingMessage);
                    token = await DoRefreshToken(tokenEndpoint, clientId, refresh_token);
                    break;

                case "code":
                    // TODO: support client secret
                    authEndpoint = config.GetRequiredValue(authEndpointField, fieldMissingMessage);
                    token = await DoAccessCodeFlow(authEndpoint, tokenEndpoint, clientId, codeChallengeMethod, scope, state);
                    break;

                // TODO: Implement Client flow

                case "password":
                    // We need the username and password fields
                    username = config.GetRequiredValue(usernameField, fieldMissingMessage);
                    password = config.GetRequiredValue(passwordField, fieldMissingMessage);
                    token = await DoResourceOwnedPasswordCredentialsFlow(tokenEndpoint, clientId, username, password, scope, state);
                    break;
                default:
                    throw new NotSupportedException(string.Format(unsupportedGrantTypeMessage, grantType));
            }

            if(token != null)
            {
                Debug.Log("[OAuth2] Token obtained: " + token.AccessToken);
                Agent = new Agent
                {
                    name = token.Username
                };
            }
        }
        
        public async Task UpdateParamsForAuth(MyHttpRequest request)
        {
            if (token.Expired)
            {
                token = await DoRefreshToken(tokenEndpoint, clientId, token.RefreshToken);
                onAuthorizationInfoUpdate?.Invoke(token);
            }

            // Capitalize
            var tokenType = token.TokenType.First().ToString().ToUpper() + token.TokenType.Substring(1).ToLower();

            // Add authorization header
            request.headers.Add("Authorization", string.Format("{0} {1}", tokenType, token.AccessToken));
        }

        public void RegisterAuthInfoUpdate(OnAuthorizationInfoUpdate toRegister)
        {
            if (toRegister == null) return;

            onAuthorizationInfoUpdate += toRegister;
            if(token != null)
            {
                toRegister(token);
            }
        }

        private static async Task<OAuth2Token> DoAccessCodeFlow(string authUrl, string tokenUrl, string clientId, PKCETypes pkceType,
            string scope, string state)
        {
            // Generate PKCE
            string codeVerifier = null, codeChallenge = null;
            if (pkceType != PKCETypes.None)
            {
                // When coming from WebGL redirect, it retrieves last codes
                PKCE.GenerateOrGetSaved(pkceType, out codeVerifier, out codeChallenge);
            }

            var cancellationToken = new CancellationToken();
            var port = UnityEngine.Random.Range(25525, 65535);
            var listener = new OAuth2Listener();
            var redirectUrl = AuthUtility.ListenForCallback(port, listener, cancellationToken);

            // Do Authorize Request (In WebGL this is a redirect)
            var authCode = await DoAuthorizeRequest(authUrl, clientId, scope, state, redirectUrl, listener, pkceType, codeChallenge);
            
            var form = new Dictionary<string, string>()
            {
                { "code", authCode.Code },
                { "redirect_uri", redirectUrl }
            };

            if (!string.IsNullOrEmpty(codeVerifier))
            {
                form.Add("code_verifier", codeVerifier);
            };
            
            // Do Token Request (without scope or state)
            return await DoTokenRequest(tokenUrl, clientId, "authorization_code", form);
        }

        private static async Task<OAuth2Token> DoResourceOwnedPasswordCredentialsFlow(string tokenUrl, string clientId, string username, string password, 
            string scope, string state)
        {
            var form = new Dictionary<string, string>()
                {
                    { "username", username },
                    { "password", password }
                };

            if (!string.IsNullOrEmpty(scope))
            {
                form.Add("scope", scope);
            }

            if (!string.IsNullOrEmpty(state))
            {
                form.Add("state", state);
            }

            return await DoTokenRequest(tokenUrl, clientId, "password", form);
        }


        private static async Task<OAuth2AuthorizationCode> DoAuthorizeRequest(string authorizeEndpoint, string clientId, string scope, string state, string redirectUrl, OAuth2Listener listener, PKCETypes pkceType, string codeChallenge = null)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "response_type", "code" },
                { "client_id", clientId }
            };

            if (pkceType != PKCETypes.None)
            {
                parameters.Add("code_challenge", codeChallenge);
                parameters.Add("code_challenge_method", pkceType.ToString());
            }

            if (!string.IsNullOrEmpty(scope))
            {
                parameters.Add("scope", scope);
            }

            if (!string.IsNullOrEmpty(state))
            {
                parameters.Add("state", state);
            }

            OAuth2AuthorizationCode authorizeResponse = null;

            try
            {
                parameters.Add("redirect_uri", redirectUrl);

                // If the listener already has a response, the listener provides it immediately
                // This handles the cases where the redirect could have been done already such as in WebGL
                listener.RegisterListener(auth => authorizeResponse = auth);

                if(authorizeResponse == null)
                {
                    var url = RequestsUtility.AppendParamsToExistingQueryString(authorizeEndpoint + "?", parameters);
                    AuthUtility.OpenUrl(url);
                    while (authorizeResponse == null)
                    {
                        await Task.Yield();
                    }
                }

                return authorizeResponse;
            }
            catch
            {
                // TODO: Authorization exceptions
                throw;
            }
        }

        private static async Task<OAuth2Token> DoTokenRequest(string tokenUrl, string clientId, string grantType, Dictionary<string, string> otherParams)
        {
            if (grantType != "authorization_code" && grantType != "password" && grantType != "refresh_token")
            {
                throw new NotSupportedException(string.Format(unsupportedGrantTypeMessage, grantType));
            }

            // Basic form
            var form = new Dictionary<string, string>()
            {
                { "grant_type", grantType },
                { "client_id", clientId },
            };

            // Add the rest of the parameters
            foreach (var param in otherParams)
            {
                form.Add(param.Key, param.Value);
            }

            // TODO: We might need to manually encode the form

            /*
            var formUrlEncoded = form
                .Select(kv => string.Format("{0}={1}", kv.Key, kv.Value))
                .Aggregate((p1, p2) => string.Format("{0}&{1}", p1, p2));
            UnityWebRequest uwr = UnityWebRequest.Post(tokenUrl, "");
            byte[] bytes = Encoding.UTF8.GetBytes(formUrlEncoded);
            UploadHandlerRaw uH = new UploadHandlerRaw(bytes);
            uH.contentType = "application/x-www-form-urlencoded";
            uwr.uploadHandler = uH;
            uwr.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            */
            var req = UnityWebRequest.Post(tokenUrl, form);

            return await DoAuthorizationRequest(clientId, req);
        }

        private async Task<OAuth2Token> DoRefreshToken(string tokenUrl, string clientId, string refresh_token)
        {
            return await DoTokenRequest(tokenUrl, clientId, "refresh_token",
                new Dictionary<string, string>()
                {
                    { "refresh_token", refresh_token }
                });
        }

        private static async Task<OAuth2Token> DoAuthorizationRequest(string clientId, UnityWebRequest uwr)
        {
            try
            {
                var authInfo = await RequestsUtility.DoRequest<OAuth2Token>(uwr);
                authInfo.ClientId = clientId;
                return authInfo;
            }
            catch (APIException ex)
            {
                OAuth2AuthorizationError error = null;
                try
                {
                    error = JsonConvert.DeserializeObject<OAuth2AuthorizationError>(ex.Message);
                }
                catch { }

                // If there is a parsing exception we just raise the previous exception
                if (error != null)
                {
                    throw error;
                }
                else
                {
                    throw;
                }
            }
        }

        public void Unauthorized(APIException apiException)
        {
            if (grantType != "code")
            {
                State = AuthState.Errored;
                ErrorMessage = apiException.Message;
                return;
            }

            State = AuthState.RequiresInteraction;
            ErrorMessage = "The authorization is invalid or has expired. Please Log in again!";
        }

        public void Forbidden(APIException apiException)
        {
            if (grantType != "code")
            {
                State = AuthState.Errored;
                ErrorMessage = apiException.Message;
                return;
            }

            State = AuthState.RequiresInteraction;
            ErrorMessage = "The current authorization has insufficient permissions for one required action. Please Log in again!";
        }
    }
}
