using Newtonsoft.Json;

namespace Xasu.Auth.Protocols.OAuth
{
    internal class OAuthAuthorization
    {
        [JsonProperty("oauth_token")]
        public string OAuthToken { get; set; }

        [JsonProperty("oauth_token_secret")]
        public string OAuthTokenSecret { get; set; }
    }
}
