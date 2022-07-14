using Polly;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinCan;
using Xasu.Exceptions;
using Xasu.Requests;

namespace Xasu.Auth.Protocols
{
    public interface IAuthProtocol
    {
        Agent Agent { get; }
        AuthState State { get; }
        string ErrorMessage { get; }
        void Unauthorized(APIException apiException);
        void Forbidden(APIException apiException);
        IAsyncPolicy Policy { get; set; }
        Task Init(IDictionary<string, string> config);
        Task UpdateParamsForAuth(MyHttpRequest request);
    }
}
