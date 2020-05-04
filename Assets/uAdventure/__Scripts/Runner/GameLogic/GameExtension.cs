using UnityEngine;

namespace uAdventure.Runner
{
    public abstract class GameExtension : MonoBehaviour
    {
        public abstract void Restart();
        public abstract void OnGameReady();
        public abstract void OnAfterGameLoad();
        public abstract void OnBeforeGameSave();
        public abstract bool OnGameFinished();
    }
}
