using UnityEngine;
using System.Collections;

namespace uAdventure.RageTracker
{
    public class GameObjectTracker : Tracker.IGameObjectTracker
    {

        private Tracker tracker;

        public void setTracker(Tracker tracker)
        {
            this.tracker = tracker;
        }


        /* GAMEOBJECT */

        public enum TrackedGameObject
        {
            Enemy,
            Npc,
            Item,
            GameObject
        }

        /// <summary>
        /// Player interacted with a game object.
        /// Type = GameObject 
        /// </summary>
        /// <param name="gameobjectId">Reachable identifier.</param>
        public void Interacted(string gameobjectId)
        {
            tracker.Trace(Tracker.Verb.Interacted.ToString().ToLower(), TrackedGameObject.GameObject.ToString().ToLower(), gameobjectId);
        }

        /// <summary>
        /// Player interacted with a game object.
        /// </summary>
        /// <param name="gameobjectId">TrackedGameObject identifier.</param>
        public void Interacted(string gameobjectId, TrackedGameObject type)
        {
            tracker.Trace(Tracker.Verb.Interacted.ToString().ToLower(), type.ToString().ToLower(), gameobjectId);
        }

        /// <summary>
        /// Player interacted with a game object.
        /// Type = GameObject 
        /// </summary>
        /// <param name="gameobjectId">Reachable identifier.</param>
        public void Used(string gameobjectId)
        {
            tracker.Trace(Tracker.Verb.Used.ToString().ToLower(), TrackedGameObject.GameObject.ToString().ToLower(), gameobjectId);
        }

        /// <summary>
        /// Player interacted with a game object.
        /// </summary>
        /// <param name="gameobjectId">TrackedGameObject identifier.</param>
        public void Used(string gameobjectId, TrackedGameObject type)
        {
            tracker.Trace(Tracker.Verb.Used.ToString().ToLower(), type.ToString().ToLower(), gameobjectId);
        }
    }
}