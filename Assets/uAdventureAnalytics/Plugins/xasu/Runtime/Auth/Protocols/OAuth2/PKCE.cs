using System.Security.Cryptography;
using System.Text;
using System;
using Xasu.Util;
using UnityEngine;

namespace Xasu.Auth.Protocols.OAuth2
{
    public static class PKCE
    {
        private const string pkceCodeVerifierKey = "pkce_code_verifier";
        private const string pkceCodeChallengeKey = "pkce_code_challenge";

        private const string pkceTypeNotSupportedMessage = "PKCE type \"{0}\" not supported. Please use \"S256\" type.";

        public static void GenerateOrGetSaved(PKCETypes pkceType, out string codeVerifier, out string codeChallenge)
        {
            if (WebGLUtility.IsWebGLListening())
            {
                Debug.Log("Getting saved PKCE");
                codeVerifier = PlayerPrefs.GetString(pkceCodeVerifierKey);
                codeChallenge = PlayerPrefs.GetString(pkceCodeChallengeKey);
                return;
            }

            // Create random code verifier
            var codeVerifierBytes = new byte[32];
            RandomNumberGenerator.Create().GetBytes(codeVerifierBytes);
            codeVerifier = Base64Url.Encode(codeVerifierBytes);

            // Create code challenge
            switch (pkceType)
            {
                case PKCETypes.S256:
                    using (var sha256 = SHA256.Create())
                    {
                        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
                        codeChallenge = Base64Url.Encode(challengeBytes);

                        // We save the codes in case we need them later
                        PlayerPrefs.SetString(pkceCodeVerifierKey, codeVerifier);
                        PlayerPrefs.SetString(pkceCodeChallengeKey, codeChallenge);
                        PlayerPrefs.Save();
                    }
                    break;
                default:
                    throw new NotSupportedException(string.Format(pkceTypeNotSupportedMessage, pkceType));
            }
            
        }
    }
}
