using System;
using System.Collections.Generic;
using System.Linq;
using TinCan;
using UnityEngine;
using Xasu.HighLevel;

namespace uAdventure.Geo
{
       

    /// <summary>
    /// Movement tracker class used to extend the normal tracker
    /// </summary>
    public class MovementTracker : AbstractHighLevelTracker<MovementTracker>
    { 
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
        protected override Dictionary<Enum, string> VerbIds => new Dictionary<Enum, string>()
        {
            { Verb.Entered,  "https://beaconing.e-ucm.es/xapi/geolocated/verbs/entered"  },
            { Verb.Exited,   "https://beaconing.e-ucm.es/xapi/geolocated/verbs/exited"   },
            { Verb.Followed, "https://beaconing.e-ucm.es/xapi/geolocated/verbs/followed" },
            { Verb.Looked,   "https://beaconing.e-ucm.es/xapi/geolocated/verbs/looked"   },
            { Verb.Moved,    "https://beaconing.e-ucm.es/xapi/geolocated/verbs/moved"    }
        };

        protected override Dictionary<Enum, string> TypeIds => new Dictionary<Enum, string>()
        {
            { Type.Building,        "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/building"             },
            { Type.Direction,       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/direction "           },
            { Type.GreenZone,       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/green-zone"           },
            { Type.Place,           "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/place"                },
            { Type.PointOfInterest, "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/point-of-interest"    },
            { Type.UrbanArea,       "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/urban-area"           },
            { Type.Water,           "https://beaconing.e-ucm.es/xapi/geolocated/activity-types/water"                }
        };

        protected override Dictionary<Enum, string> ExtensionIds => new Dictionary<Enum, string>()
        {
            { Extension.Guide,         "https://beaconing.e-ucm.es/xapi/geolocated/extensions/guide"       },
            { Extension.Location,      "https://beaconing.e-ucm.es/xapi/geolocated/extensions/location"    },
            { Extension.Orientation,   "https://beaconing.e-ucm.es/xapi/geolocated/extensions/orientation" }
        };

        #region Moved
        /// <summary>
        /// Player moved in the element identified by id.
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Moved(string id, Vector2d latLon)
        {
            return Moved(id, Type.Place, latLon);
        }

        /// <summary>
        /// Player moved in the id
        /// </summary>
        /// <param name="id">Place identifier.</param>
        /// <param name="type">Place type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Moved(string id, Type type, Vector2d latLon)
        {
            return Enqueue(new Statement
                {
                    verb = GetVerb(Verb.Moved),
                    target = GetTargetActivity(id, type)
                })
                .WithResultExtensions(new Dictionary<string, object>
                {
                    { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y }
                });
        }
        #endregion

        #region Entered

        /// <summary>
        /// Player entered the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        public StatementPromise Entered(string id)
        {
            return Entered(id, Type.Place);
        }

        /// <summary>
        /// Player entered the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        public StatementPromise Entered(string id, Type type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Entered),
                target = GetTargetActivity(id, type)
            });
        }

        /// <summary>
        /// Player entered the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Entered(string id, Vector2d latLon)
        {
            return Entered(id, Type.Place, latLon);
        }

        /// <summary>
        /// Player entered the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Entered(string id, Type type, Vector2d latLon)
        {
            return Entered(id, type).WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y }
            });
        }

        /// <summary>
        /// Player entered the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Entered(string id, Vector2d latLon, Vector3d orientation)
        {
            return Entered(id, Type.Place, latLon, orientation);
        }

        /// <summary>
        /// Player entered the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Entered(string id, Type type, Vector2d latLon, Vector3d orientation)
        {
            return Entered(id, type).WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y },
                { Extension.Orientation.ToString().ToLower(), "{\"yaw\":" + orientation.x + ", \"pitch\": " + orientation.y + ", \"roll\": " + orientation.z + "}" }
            });
        }

        #endregion

        #region Exited

        /// <summary>
        /// Player Exited the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        public StatementPromise Exited(string id)
        {
            return Exited(id, Type.Place);
        }

        /// <summary>
        /// Player Exited the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        public StatementPromise Exited(string id, Type type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Exited),
                target = GetTargetActivity(id, type)
            });
        }

        /// <summary>
        /// Player exited the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Exited(string id, Vector2d latLon)
        {
            return Exited(id, Type.Place, latLon);
        }

        /// <summary>
        /// Player exited the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Exited(string id, Type type, Vector2d latLon)
        {
            return Exited(id, type)
            .WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y }
            });
        }

        /// <summary>
        /// Player exited the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Exited(string id, Vector2d latLon, Vector3d orientation)
        {
            return Exited(id, Type.Place, latLon, orientation);
        }

        /// <summary>
        /// Player exited the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Exited(string id, Type type, Vector2d latLon, Vector3d orientation)
        {
            return Exited(id, type).WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y },
                { Extension.Orientation.ToString().ToLower(), "{\"yaw\":" + orientation.x + ", \"pitch\": " + orientation.y + ", \"roll\": " + orientation.z + "}" }
            });
        }

        #endregion

        #region Looked

        /// <summary>
        /// Player looked the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Looked(string id, Vector3d orientation)
        {
            return Looked(id, Type.Place, orientation);
        }

        /// <summary>
        /// Player looked the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        public StatementPromise Looked(string id, Type type, Vector3d orientation)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Looked),
                target = GetTargetActivity(id, type)
            })
            .WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Orientation.ToString().ToLower(), "{\"yaw\":" + orientation.x + ", \"pitch\": " + orientation.y + ", \"roll\": " + orientation.z + "}" }
            });
        }

        /// <summary>
        /// Player looked the place id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Looked(string id, Vector3d orientation, Vector2d latLon)
        {
            return Looked(id, Type.Place, orientation, latLon);
        }

        /// <summary>
        /// Player looked the place id
        /// </summary>
        /// <param name="id">Reachable identifier.</param> 
        /// <param name="type">Reachable type.</param>
        /// <param name="latLon">Actor latitude and longitude.</param>
        /// <param name="orientation">Actor orientation.</param>
        public StatementPromise Looked(string id, Type type, Vector3d orientation, Vector2d latLon)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Looked),
                target = GetTargetActivity(id, type)
            })
            .WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Location.ToString().ToLower(), latLon.x + "," + latLon.y },
                { Extension.Orientation.ToString().ToLower(), "{\"yaw\":" + orientation.x + ", \"pitch\": " + orientation.y + ", \"roll\": " + orientation.z + "}" }
            });
        }

        #endregion

        #region Followed

        /// <summary>
        /// Player followed the element id
        /// Type = Place 
        /// </summary>
        /// <param name="id">Element identifier.</param> 
        public StatementPromise Followed(string id)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Followed),
                target = GetTargetActivity(id, Type.Direction)
            });
        }

        /// <summary>
        /// Player followed the element id with indications
        /// Type = Place 
        /// </summary>
        /// <param name="id">Element identifier.</param> 
        /// <param name="guide">Array of indications the user is following.</param> 
        public StatementPromise Followed(string id, string[] guide)
        {
            return Followed(id).WithResultExtensions(new Dictionary<string, object>
            {
                { Extension.Guide.ToString().ToLower(), "[" + guide.Select(i => "\"" + i + "\"").Aggregate((i1,i2) => i1 + ", " + i2) + "]" }
            });
        }

        #endregion
    }
}