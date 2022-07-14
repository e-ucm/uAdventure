using Newtonsoft.Json;
using System;

namespace Xasu.Auth.Protocols.OAuth2
{
    public class OAuth2AuthorizationError : Exception
    {
        public OAuth2AuthorizationError() { }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
