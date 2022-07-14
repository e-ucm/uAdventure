using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using TinCan;
using UnityEngine.Auth.Protocols.Cmi5;
using UnityEngine.Networking;
using Xasu.CMI5;
using Xasu.Exceptions;
using Xasu.Requests;

namespace Xasu.Auth.Protocols
{
    public class Cmi5Protocol : IAuthProtocol
    {
        public IAsyncPolicy Policy { get; set; }

        public Agent Agent => Cmi5Helper.Actor;

        public AuthState State { get; protected set; }

        public string ErrorMessage { get; protected set; }

        private Cmi5Fetch auth;

        public async Task Init(IDictionary<string, string> config)
        {
            // This initializes the Query parameters
            Cmi5Helper.SetQuery();
            auth = await DoFetch(Cmi5Helper.Fetch, Policy);
        }

        private static async Task<Cmi5Fetch> DoFetch(System.Uri fetchUrl, IAsyncPolicy policy)
        {
            return await RequestsUtility.DoRequest<Cmi5Fetch>(UnityWebRequest.Post(fetchUrl, ""));
        }

        public Task UpdateParamsForAuth(MyHttpRequest request)
        {
            request.headers.Add("Authorization", string.Format("Basic {0}", auth.AuthToken));
            return Task.FromResult(0);
        }

        // CMI-5 cannot recover from Unauthorized or Forbidden exceptions
        public void Unauthorized(APIException apiException)
        {
            State = AuthState.Errored;
            ErrorMessage = apiException.Message;
        }

        public void Forbidden(APIException apiException)
        {
            State = AuthState.Errored;
            ErrorMessage = apiException.Message;
        }
    }
}
