using System.Collections;
using UnityEngine;

namespace uAdventure.Runner
{
    public abstract class GameExtension : MonoBehaviour
    {
        public abstract IEnumerator Restart();
        public abstract IEnumerator OnGameReady();
        public abstract IEnumerator OnAfterGameLoad();
        public abstract IEnumerator OnBeforeGameSave();
        public abstract IEnumerator OnGameFinished();
    }
}
