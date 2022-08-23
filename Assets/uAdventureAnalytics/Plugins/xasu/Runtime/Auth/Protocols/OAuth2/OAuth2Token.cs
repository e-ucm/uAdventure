using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xasu.Auth.Protocols.OAuth2
{
    public class OAuth2Token
    {
        private Dictionary<string, object> decodedJWT;
        private string access_token;

        public OAuth2Token()
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
                var payload = Encoding.UTF8.GetString(Base64Url.Decode(access_token.Split('.')[1]));
                decodedJWT = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
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
        public bool Expired { get { return Created + new TimeSpan(0, 0, Expires) < DateTime.Now; } }

        [JsonIgnore]
        public string Username { get { return (string)decodedJWT["preferred_username"]; } }
    }
}
