using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uAdventure.Runner
{
    public abstract class GameExtension : MonoBehaviour
    {
        public static T GetInstance<T>() where T : GameExtension
        {
            return (T)GameExtensions.Where(e => e is T).First();
        }

        public static List<GameExtension> GameExtensions { get; } = new List<GameExtension>();

        protected virtual void Awake()
        {
            GameExtensions.Add(this);
        }

        protected virtual void OnDestroy()
        {
            GameExtensions.Remove(this);
        }

        public abstract IEnumerator Restart();
        public abstract IEnumerator OnGameReady();
        public abstract IEnumerator OnAfterGameLoad();
        public abstract void OnBeforeGameSave();
        public abstract IEnumerator OnGameFinished();
    }
}
