using UnityFx.Async;
using Xasu.Auth.Protocols;
using Xasu.Auth.Protocols.OAuth2;

namespace Simva
{
    public interface ISimvaBridge
    {
        void RunScene(string name);
        void StartGameplay();
        IAsyncOperation StartTracker(Xasu.Config.TrackerConfig config, IAuthProtocol onlineProtocol, IAuthProtocol backupProtocol);
        void OnAuthUpdated(OAuth2Token token);
        void Demo();
    }
}
