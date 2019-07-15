using UnityEngine;

namespace uAdventure.Runner
{
    public abstract class GameExtension : MonoBehaviour
    {
        public abstract void OnReset();
        public abstract void OnAfterGameLoad();
        public abstract void OnBeforeGameSave();
    }
}
