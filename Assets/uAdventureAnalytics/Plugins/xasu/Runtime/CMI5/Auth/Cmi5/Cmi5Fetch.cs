using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.Auth.Protocols.Cmi5
{
    internal class Cmi5Fetch
    {
        [JsonProperty("auth-token")]
        public string AuthToken { get; set; }
    }
}
