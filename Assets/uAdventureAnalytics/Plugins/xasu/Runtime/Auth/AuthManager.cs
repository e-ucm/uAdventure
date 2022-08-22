using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xasu.Auth.Protocols;

namespace Xasu.Auth
{
    /// <summary>
    /// Auth Manager manages the available authorization protocols and their initialization and continuation.
    /// </summary>
    public static class AuthManager
    {
        private const string notSupportedAuthMessage = "Authorization type \"{0}\" not supported. Accepted types: basic, oauth and oauth2.";

        private static Dictionary<string, IAuthProtocol> authProtocols = new Dictionary<string, IAuthProtocol>()
        {
            { "basic", new BasicProtocol() },
            { "oauth", new OAuthProtocol() },
            { "oauth2", new OAuth2Protocol() },
            { "cmi5", new Cmi5Protocol() }
        };

        public static async Task<IAuthProtocol> InitAuth(string authName, IDictionary<string, string> parameters, IAsyncPolicy policy)
        {
            if (authName == null || authName == "none" || authName == "disabled")
            {
                return null;
            }

            if (!authProtocols.ContainsKey(authName))
            {
                throw new NotSupportedException(string.Format(notSupportedAuthMessage, authName));
            }
            
            if (policy != null)
            {
                authProtocols[authName].Policy = policy;
            }

            await authProtocols[authName].Init(parameters);

            return authProtocols[authName];
        }

    }
}
