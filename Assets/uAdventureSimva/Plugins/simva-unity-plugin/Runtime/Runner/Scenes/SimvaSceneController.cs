using UnityEngine;

namespace Simva
{
    public abstract class SimvaSceneController : MonoBehaviour
    {
        public abstract void Render();
        public abstract void Destroy();

        public bool Ready { get; protected set; }
    }
}
