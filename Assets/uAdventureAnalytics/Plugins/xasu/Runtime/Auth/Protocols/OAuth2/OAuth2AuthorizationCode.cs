
namespace Xasu.Auth.Protocols.OAuth2
{

    internal class OAuth2AuthorizationCode
    {
        public OAuth2AuthorizationCode() { }

        public string Code { get; set; }

        public string SessionState { get; set; }
    }
}
