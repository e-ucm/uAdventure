using AssetPackage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uAdventure.Geo
{
    public static class TrackerExtension
    {

        /**********************
         * Static attributes
         * *******************/

        private static ExtensionTransformations TR;
        private static MovementTracker MT;


        /**********************
         * Extension enums and classes
         * *******************/
        public enum Verb
        {
            Entered,
            Exited,
            Followed,
            Looked,
            Moved
        }
        public enum Type
        {
            Building,
            Direction,
            GreenZone,
            Place,
            PointOfInterest,
            UrbanArea,
            Water
        }

        public enum Extension
        {
            Guide,
            Location,
            Orientation
        }

        // Transformations
        public class ExtensionTransformations
        {
            public Dictionary<string, string> verbIds = new Dictionary<string, string>()
            {
                { Verb.Entered.ToString().ToLower(),  "https://beaconing.e-ucm.es/xapi/geolocated/verbs/entered"  },
                { Verb.Exited.ToString().ToLower(),   "https://beaconing.e-ucm.es/xapi/geolocated/verbs/exited"   },
                { Verb.Followed.ToString().ToLower(), "https://beaconing.e-ucm.es/xapi/geolocated/verbs/followed" },
                { Verb.Looked.ToString().ToLower(),   "https://beaconing.e-ucm.es/xapi/geolocated/verbs/looked"   },
                { Verb.Moved.ToString().ToLower(),    "https://beaconing.e-ucm.es/xapi/geolocated/verbs/moved"    }
            };

            public Dictionary<string, string> typeIds = new Dictionary<string, string>()
            {
                { Type.Building.ToString().ToLower(),        "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/building"             },
                { Type.Direction.ToString().ToLower(),       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/direction "           },
                { Type.GreenZone.ToString().ToLower(),       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/green-zone"           },
                { Type.Place.ToString().ToLower(),           "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/place"                },
                { Type.PointOfInterest.ToString().ToLower(), "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/point-of-interest"    },
                { Type.UrbanArea.ToString().ToLower(),       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/urban-area"           },
                { Type.Water.ToString().ToLower(),           "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/water"                }
            };

            public Dictionary<string, string> objectIds = new Dictionary<string, string>()
            {
                { Extension.Guide.ToString().ToLower(),         "https://beaconing.e-ucm.es/xapi/geolocated/extensions/guide"       },
                { Extension.Location.ToString().ToLower(),      "https://beaconing.e-ucm.es/xapi/geolocated/extensions/location"    },
                { Extension.Orientation.ToString().ToLower(),   "https://beaconing.e-ucm.es/xapi/geolocated/extensions/orientation" }
            };
        }

        /// <summary>
        /// Movement tracker class used to extend the normal tracker
        /// </summary>
        public class MovementTracker : TrackerAsset.IGameObjectTracker
        {

            private TrackerAsset tracker;

            public void setTracker(TrackerAsset tracker)
            {
                this.tracker = tracker;
            }

            #region Moved
            /// <summary>
            /// Player moved in the element identified by id.
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Moved(string id, Vector2d latLon)
            {
                Moved(id, Type.Place, latLon);
            }

            /// <summary>
            /// Player moved in the id
            /// </summary>
            /// <param name="id">Place identifier.</param>
            /// <param name="type">Place type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Moved(string id, Type type, Vector2d latLon)
            {
                tracker.setPosition(latLon.x, latLon.y);
                tracker.ActionTrace(Verb.Moved.ToString().ToLower(), type.ToString().ToLower(), id);
            }
            #endregion

            #region Entered

            /// <summary>
            /// Player entered the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            public void Entered(string id)
            {
                Entered(id, Type.Place);
            }

            /// <summary>
            /// Player entered the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            public void Entered(string id, Type type)
            {
                tracker.ActionTrace(Verb.Entered.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            /// <summary>
            /// Player entered the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Entered(string id, Vector2d latLon)
            {
                Entered(id, Type.Place, latLon);
            }

            /// <summary>
            /// Player entered the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Entered(string id, Type type, Vector2d latLon)
            {
                tracker.setPosition(latLon.x, latLon.y);
                Entered(id, type);
            }

            /// <summary>
            /// Player entered the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Entered(string id, Vector2d latLon, Vector3d orientation)
            {
                Entered(id, Type.Place, latLon, orientation);
            }

            /// <summary>
            /// Player entered the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Entered(string id, Type type, Vector2d latLon, Vector3d orientation)
            {
                tracker.setPosition(latLon.x, latLon.y);
                tracker.setOrientation(orientation.x, orientation.y, orientation.z);
                Entered(id, type);
            }

            #endregion

            #region Exited

            /// <summary>
            /// Player Exited the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            public void Exited(string id)
            {
                Exited(id, Type.Place);
            }

            /// <summary>
            /// Player Exited the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            public void Exited(string id, Type type)
            {
                tracker.ActionTrace(Verb.Exited.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            /// <summary>
            /// Player exited the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Exited(string id, Vector2d latLon)
            {
                Exited(id, Type.Place, latLon);
            }

            /// <summary>
            /// Player exited the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Exited(string id, Type type, Vector2d latLon)
            {
                tracker.setPosition(latLon.x, latLon.y);
                Exited(id, type);
            }

            /// <summary>
            /// Player exited the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Exited(string id, Vector2d latLon, Vector3d orientation)
            {
                Exited(id, Type.Place, latLon, orientation);
            }

            /// <summary>
            /// Player exited the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Exited(string id, Type type, Vector2d latLon, Vector3d orientation)
            {
                tracker.setPosition(latLon.x, latLon.y);
                tracker.setOrientation(orientation.x, orientation.y, orientation.z);
                Exited(id, type);
            }

            #endregion

            #region Looked

            /// <summary>
            /// Player looked the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Looked(string id, Vector3d orientation)
            {
                Looked(id, Type.Place, orientation);
            }

            /// <summary>
            /// Player looked the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            public void Looked(string id, Type type, Vector3d orientation)
            {
                tracker.setOrientation(orientation.x, orientation.y, orientation.z);
                tracker.ActionTrace(Verb.Looked.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            /// <summary>
            /// Player looked the place id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Looked(string id, Vector3d orientation, Vector2d latLon)
            {
                Looked(id, Type.Place, orientation, latLon);
            }

            /// <summary>
            /// Player looked the place id
            /// </summary>
            /// <param name="id">Reachable identifier.</param> 
            /// <param name="type">Reachable type.</param>
            /// <param name="latLon">Actor latitude and longitude.</param>
            /// <param name="orientation">Actor orientation.</param>
            public void Looked(string id, Type type, Vector3d orientation, Vector2d latLon)
            {
                tracker.setOrientation(orientation.x, orientation.y, orientation.z);
                tracker.setPosition(latLon.x, latLon.y);
                tracker.ActionTrace(Verb.Looked.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            #endregion

            #region Followed

            /// <summary>
            /// Player followed the element id
            /// Type = Place 
            /// </summary>
            /// <param name="id">Element identifier.</param> 
            public void Followed(string id)
            {
                tracker.ActionTrace(Verb.Followed.ToString().ToLower(), Type.Direction.ToString().ToLower(), id);
            }

            /// <summary>
            /// Player followed the element id with indications
            /// Type = Place 
            /// </summary>
            /// <param name="id">Element identifier.</param> 
            /// <param name="guide">Array of indications the user is following.</param> 
            public void Followed(string id, string[] guide)
            {
                tracker.setGuide(guide);
                tracker.ActionTrace(Verb.Followed.ToString().ToLower(), Type.Direction.ToString().ToLower(), id);
            }

            #endregion
        }

        /**********************
         * Static methods
         * **********************/

        /// <summary>
        /// This method allows to set a Geopoint as extension to the current trace.
        /// </summary>
        /// <param name="t">tracker to use</param>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        public static void setPosition(this TrackerAsset t, double lat, double lon)
        {
            if (TR == null)
            {
                TR = new ExtensionTransformations();
            }

            t.setVar(Extension.Location.ToString().ToLower(), lat + "," + lon);
        }

        /// <summary>
        /// This method allows to set a orientation extension to the current trace
        /// </summary>
        /// <param name="t">tracker to use</param>
        /// <param name="yaw">Yaw in radians (vertical torsion)</param>
        /// <param name="pitch">Pitch in radians (horizontal torsion)</param>
        /// <param name="roll">Roll in radians (straight torsion)</param>
        public static void setOrientation(this TrackerAsset t, double yaw, double pitch, double roll)
        {
            if (TR == null)
            {
                TR = new ExtensionTransformations();
            }
            
            t.setVar(Extension.Orientation.ToString().ToLower(),
                "{\"yaw\":" + yaw + ", \"pitch\": " + pitch + ", \"roll\": " + roll + "}");
        }

        /// <summary>
        /// This method allows to set a Geopoint as extension to the current trace.
        /// </summary>
        /// <param name="t">tracker to use</param>
        /// <param name="lat">latitude</param>
        /// <param name="lon">longitude</param>
        public static void setGuide(this TrackerAsset t, string[] indications)
        {
            if (TR == null)
            {
                TR = new ExtensionTransformations();
            }

            t.setVar(Extension.Guide.ToString().ToLower(),
                "[" + indications.Select(i => "\"" + i + "\"").Aggregate((i1,i2) => i1 + ", " + i2) + "]");
        }

        /// <summary>
        /// Movement tracker static accessor
        /// </summary>
        public static MovementTracker Movement
        {
            get
            {
                if (MT == null)
                {
                    MT = new MovementTracker();
                    MT.setTracker(TrackerAsset.Instance);
                }

                return MT;
            }
        }
    }
}