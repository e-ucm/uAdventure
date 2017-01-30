using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RAGE.Analytics.Formats
{

    public class MovementTracker : Tracker.IGameObjectTracker
    {

        private Tracker tracker;

        public void setTracker(Tracker tracker)
        {
            this.tracker = tracker;
        }


        /* MOVEMENTS */

        public enum Movement
        {
            Geoposition
        }

        /// <summary>
        /// Player accessed a reachable.
        /// Type = Accessible 
        /// </summary>
        /// <param name="id">Reachable identifier.</param>
        public void Geoposition(string id, Vector2 latLon)
        {
            tracker.setGeopoint(latLon.x, latLon.y);
            tracker.ActionTrace(Tracker.Verb.Moved.ToString().ToLower(), Movement.Geoposition.ToString().ToLower(), id);
        }
        
    }
}
