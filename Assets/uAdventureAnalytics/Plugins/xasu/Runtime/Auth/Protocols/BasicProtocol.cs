using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Polly;
using TinCan;
using Xasu.Auth.Utils;
using Xasu.Exceptions;
using Xasu.Requests;

// 1: Workaround for task result from: https://stackoverflow.com/questions/13127177/if-my-interface-must-return-task-what-is-the-best-way-to-have-a-no-operation-imp

namespace Xasu.Auth.Protocols
{
    public class BasicProtocol : IAuthProtocol
    {
        private readonly string fieldMissingMessage = "Field \"{0}\" required for \"Basic\" authentication is missing!";
        private readonly string headerNullMessage = "Param \"headerParams\" required for \"Basic\" authentication is null!";
        private readonly string usernameField = "username";
        private readonly string passwordField = "password";
        private readonly string realmField = "realm";
        private string token;
        private string realm;

        public IAsyncPolicy Policy { get; set; }

        public Agent Agent { get; private set; }

        public AuthState State { get; private set; }

        public string ErrorMessage { get; private set; }

        public Task Init(IDictionary<string, string> config)
        {
            if (!config.ContainsKey(usernameField))
            {
                throw new MissingFieldException(string.Format(fieldMissingMessage, usernameField));
            }

            if (!config.ContainsKey(passwordField))
            {
                throw new MissingFieldException(string.Format(fieldMissingMessage, passwordField));
            }

            var username = config.Value(usernameField);
            var password = config.Value(passwordField);

            if (config.ContainsKey(realmField))
            {
                realm = config.Value(realmField);
            }
            token = ToBase64UTF8(username + ":" + password);
            State = AuthState.Working;
            Agent = new Agent
            {
                name = username
            };

            // #1
            return Task.FromResult(0);
        }

        public Task UpdateParamsForAuth(MyHttpRequest request)
        {
            if (request.headers == null)
            {
                throw new ArgumentNullException(headerNullMessage);
            }

            // Authorization
            request.headers.Add("Authorization", "Basic " + token);

            return Task.FromResult(0);
        }

        private static string ToBase64UTF8(string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return System.Convert.ToBase64String(bytes);
        }

        public void Unauthorized(APIException apiException)
        {
            State = AuthState.Errored;
            ErrorMessage = "Basic credentials unauthorized: " + apiException.ToString();
        }

        public void Forbidden(APIException apiException)
        {
            State = AuthState.Errored;
            ErrorMessage = "Basic credentials forbidden: " + apiException.ToString();
        }
    }
}
