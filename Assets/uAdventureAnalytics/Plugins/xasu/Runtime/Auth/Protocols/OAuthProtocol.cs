using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Xasu.Auth.Utils;
using System.Threading;
using Xasu.Util;
using Xasu.Auth.Protocols.OAuth;
using Xasu.Requests;
using TinCan;
using Xasu.Exceptions;

namespace Xasu.Auth.Protocols
{
    public class OAuthProtocol : IAuthProtocol
    {
        private readonly string fieldMissingMessage = "Field \"{0}\" required for \"OAuth 1.0a\" authentication is missing!";
        private readonly string requestNullMessage = "Param \"headerParams\" required for \"OAuth 1.0a\" authentication is null!";

        // Standard fields
        private readonly string consumerKeyField = "oauth_consumer_key";
        private readonly string consumerSecretField = "oauth_consumer_secret";
        private readonly string signatureMethodField = "oauth_signature_method";
        //private readonly string signatureField = "oauth_signature";
        //private readonly string timestampField = "oauth_timestamp";
        //private readonly string callbackField = "oauth_callback";

        // Custom fields
        private readonly string requestTokenEndpointField = "request_token_endpoint"; // AKA "initiate" endpoint
        private readonly string authorizeEndpointField = "authorize_endpoint";
        private readonly string accessTokenEndpointField = "access_token_endpoint";

        // Bearer
        private string consumerKey;
        private string consumerSecret;
        private SignatureTypes signatureMethod;
        private string requestTokenEndpoint;
        private string authorizeEndpoint;
        private string accessTokenEndpoint;
        private OAuthAuthorization token;

        public IAsyncPolicy Policy { get; set; }

        public Agent Agent { get; private set; }

        public AuthState State { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public async Task Init(IDictionary<string, string> config)
        {
            // Main params
            consumerKey = config.GetRequiredValue(consumerKeyField, fieldMissingMessage);
            consumerSecret = config.GetRequiredValue(consumerSecretField, fieldMissingMessage);
            var signatureName = config.GetRequiredValue(signatureMethodField, fieldMissingMessage).ToUpperInvariant();

            switch (signatureName)
            {
                case "HMAC-SHA1": signatureMethod = SignatureTypes.HMACSHA1; break;
                case "RSA-SHA1": signatureMethod = SignatureTypes.RSASHA1; break;
                case "PLAINTEXT": signatureMethod = SignatureTypes.PLAINTEXT; break;
                default: throw new NotSupportedException("Method \"" + signatureName + "\" not supported, please use HMAC-SHA1, RSA-SHA1, or PLAINTEXT");
            }

            // Endpoints
            requestTokenEndpoint = config.Value(requestTokenEndpointField);
            authorizeEndpoint = config.Value(authorizeEndpointField);
            accessTokenEndpoint = config.Value(accessTokenEndpointField);

            var port = UnityEngine.Random.Range(25525, 65535);
            var cancelationToken = new CancellationToken();

            try
            {
                // Prepare recepcion
                var oauthListener = new OAuthListener();
                string callbackUrl = AuthUtility.ListenForCallback(port, oauthListener, cancelationToken);

                // Get Temporary Token and check if our callback is ok
                var temporaryToken = await DoTokenRequest(requestTokenEndpoint, consumerKey, callbackUrl);
                if (!temporaryToken.OAuthCallbackConfirmed)
                {
                    throw new Exception("Callback not confirmed!");
                }

                // Get authorize token
                var authorizeResponse = await DoAuthorizeRequest(authorizeEndpoint, temporaryToken, oauthListener);
                var doAccessTokenRequest = await DoAccessTokenRequest(accessTokenEndpoint, consumerKey, authorizeResponse);

                Agent = new Agent
                {
                    name = "OAuth with token " + doAccessTokenRequest.OAuthToken
                };
            } 
            catch (NetworkException nex)
            {
                State = AuthState.Errored;
                ErrorMessage = "Network is missing! " + nex.ToString();
            }
            catch (APIException apiEx)
            {
                State = AuthState.Errored;
                ErrorMessage = "Auhtorization failed with API exception! " + apiEx.ToString();
            }
        }

        private static async Task<TemporaryAuthorization> DoTokenRequest(string requestTokenEndpoint, string consumerKey, string callbackUrl)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(requestTokenEndpoint, new Dictionary<string, string>()
            {
                { "oauth_consumer_key", consumerKey },
#if UNITY_WEBGL && !UNITY_EDITOR
                { "oauth_callback", WebGLUtility.GetUrl() }, // Returns to itself
#else
                { "oauth_callback", callbackUrl } // We listen for the code
#endif
            });

            return await RequestsUtility.DoRequest<TemporaryAuthorization>(uwr);
        }

        private static async Task<AuthorizeResponse> DoAuthorizeRequest(string authorizeEndpoint, TemporaryAuthorization tempAuth, OAuthListener listener)
        {
            var url = RequestsUtility.AppendParamsToExistingQueryString(authorizeEndpoint, new Dictionary<string, string>()
            {
                { "oauth_token", tempAuth.OAuthToken }
            });

#if !UNITY_WEBGL || UNITY_EDITOR
            AuthorizeResponse authorizeResponse = null;
            listener.onAuthorizeResponse += (auth) =>
            {
                authorizeResponse = auth;
            };
#endif
            AuthUtility.OpenUrl(url);

#if !UNITY_WEBGL || UNITY_EDITOR
            while (authorizeResponse == null)
            {
                await Task.Yield();
            }
            return authorizeResponse;
#else
            return null;
#endif
        }

        private static async Task<OAuthAuthorization> DoAccessTokenRequest(string accessTokenEndpoint, string consumerKey, AuthorizeResponse authorizeResponse)
        {
            UnityWebRequest uwr = UnityWebRequest.Post(accessTokenEndpoint, new Dictionary<string, string>()
            {
                { "oauth_consumer_key", consumerKey },
                { "oauth_token", authorizeResponse.OAuthToken },
                { "oauth_verifier", authorizeResponse.OAuthVerifier }
            });

            return await RequestsUtility.DoRequest<OAuthAuthorization>(uwr);
        }

        public Task UpdateParamsForAuth(MyHttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(requestNullMessage);
            }

            OAuthBase oAuthBase = new OAuthBase();
            var timestamp = oAuthBase.GenerateTimeStamp();
            var nonce = oAuthBase.GenerateNonce();
            var signature = oAuthBase.GenerateSignature(new Uri(request.url), consumerKey,
                                                        consumerSecret, token.OAuthToken, token.OAuthTokenSecret, request.method, timestamp, nonce, signatureMethod,
                                                        out string normalizedUrl,
                                                        out string normalizedRequestParameters);

            // Here we use the Query authorization instead of the Header authorization
            // More info: https://datatracker.ietf.org/doc/html/rfc5849#section-3.5.3
            request.url = string.Format("{0}?{1}&oauth_signature={2}", normalizedUrl, normalizedRequestParameters,
                                          signature);

            // #1
            return Task.FromResult(0);
        }

        public void Unauthorized(APIException apiException)
        {
            State = AuthState.RequiresInteraction;
            ErrorMessage = "The authorization is invalid or has expired. Please Log in again!";
        }

        public void Forbidden(APIException apiException)
        {
            State = AuthState.RequiresInteraction;
            ErrorMessage = "The current authorization has insufficient permissions for one required action. Please Log in again!";
        }
    }
}
