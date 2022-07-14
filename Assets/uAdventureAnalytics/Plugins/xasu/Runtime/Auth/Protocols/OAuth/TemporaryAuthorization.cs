using Newtonsoft.Json;

namespace Xasu.Auth.Protocols.OAuth
{

    internal class TemporaryAuthorization : OAuthAuthorization
    {
        [JsonProperty("oauth_callback_confirmed")]
        public bool OAuthCallbackConfirmed { get; set; }
    }
}
