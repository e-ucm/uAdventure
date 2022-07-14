
using System.Collections.Specialized;

namespace Xasu.Auth.Utils
{
    public interface IAuthListener
    {
        void OnAuthReply(NameValueCollection query);
    }
}
