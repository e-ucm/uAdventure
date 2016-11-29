using UnityEngine;
using System.Collections;

namespace uAdventure.RageTracker
{
    public class AccessibleTracker : Tracker.IGameObjectTracker
    {

        private Tracker tracker;

        public void setTracker(Tracker tracker)
        {
            this.tracker = tracker;
        }


        /* ACCESSIBLES */

        public enum Accessible
        {
            Screen,
            Area,
            Zone,
            Cutscene,
            Accessible
        }

        /// <summary>
        /// Player accessed a reachable.
        /// Type = Accessible 
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        public void Accessed(string reachableId)
        {
            tracker.Trace(Tracker.Verb.Accessed.ToString().ToLower(), Accessible.Accessible.ToString().ToLower(), reachableId);
        }

        /// <summary>
        /// Player accessed a reachable.
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        /// <param name="type">Reachable type.</param>
        public void Accessed(string reachableId, Accessible type)
        {
            tracker.Trace(Tracker.Verb.Accessed.ToString().ToLower(), type.ToString().ToLower(), reachableId);
        }

        /// <summary>
        /// Player skipped a reachable.
        /// Type = Accessible
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        public void Skipped(string reachableId)
        {
            tracker.Trace(Tracker.Verb.Skipped.ToString().ToLower(), Accessible.Accessible.ToString().ToLower(), reachableId);
        }

        /// <summary>
        /// Player skipped a reachable.
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        /// <param name="type">Reachable type.</param>
        public void Skipped(string reachableId, Accessible type)
        {
            tracker.Trace(Tracker.Verb.Skipped.ToString().ToLower(), type.ToString().ToLower(), reachableId);
        }
    }
}