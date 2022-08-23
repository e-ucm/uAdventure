using Newtonsoft.Json;

namespace Xasu.Auth.Protocols.OAuth
{

    internal class AuthorizeResponse
    {
        [JsonProperty("oauth_token")]
        public string OAuthToken { get; set; }

        [JsonProperty("oauth_verifier")]
        public string OAuthVerifier { get; set; }
    }
}
