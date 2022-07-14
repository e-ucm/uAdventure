using System.Collections.Specialized;
using Xasu.Auth.Utils;

namespace Xasu.Auth.Protocols.OAuth
{
    internal class OAuthListener : IAuthListener
    {
        public delegate void OnAuthorizeResponse(AuthorizeResponse response);
        public OnAuthorizeResponse onAuthorizeResponse;

        public OAuthListener() { }

        public void OnAuthReply(NameValueCollection query)
        {
            onAuthorizeResponse?.Invoke(new AuthorizeResponse
            {
                OAuthToken = query.Get("oauth_token"),
                OAuthVerifier = query.Get("oauth_verifier")
            });
        }
    }
}
