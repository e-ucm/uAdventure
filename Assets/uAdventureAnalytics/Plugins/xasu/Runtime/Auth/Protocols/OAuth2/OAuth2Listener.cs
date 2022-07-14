using System;
using System.Collections.Specialized;
using UnityEngine;
using Xasu.Auth.Utils;

namespace Xasu.Auth.Protocols.OAuth2
{
    internal class OAuth2Listener : IAuthListener
    {
        public delegate void OnAuthorizationCodeResponse(OAuth2AuthorizationCode authorizationCode);
        private OnAuthorizationCodeResponse onAuthorizeResponse;
        private NameValueCollection authReply;

        public OAuth2Listener() { }

        public void OnAuthReply(NameValueCollection query)
        {
            authReply = query;

            if (Array.IndexOf(query.AllKeys, "code") != -1 
                && Array.IndexOf(query.AllKeys, "session_state") != -1)
            {
                onAuthorizeResponse?.Invoke(new OAuth2AuthorizationCode
                {
                    Code = query.Get("code"),
                    SessionState = query.Get("session_state")
                });
            }
            else
            {
                Debug.Log("[OAuth2] No code or session-state found, ignoring previous login attempt...");
            }
        }

        public void RegisterListener(OnAuthorizationCodeResponse onAuthorizeResponse)
        {
            this.onAuthorizeResponse += onAuthorizeResponse;

            if (authReply != null)
            {
                OnAuthReply(authReply);
            }
        }
    }
}
