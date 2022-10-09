using System;
using System.Collections.Generic;
using TinCan;


namespace Xasu.HighLevel
{
    public class AccessibleTracker : AbstractHighLevelTracker<AccessibleTracker>
    {


        /**********************
        *       Verbs
        * *******************/
        public enum Verb
        {
            Accessed,
            Skipped
        }

        public Dictionary<Enum, string> verbIds = new Dictionary<Enum, string>()
    {
        { Verb.Accessed,      "https://w3id.org/xapi/seriousgames/verbs/accessed"    },
        { Verb.Skipped,       "http://id.tincanapi.com/verb/skipped"                 }
    };
        protected override Dictionary<Enum, string> VerbIds => verbIds;

        public enum AccessibleType
        {
            Screen,
            Area,
            Zone,
            Cutscene,
            Inventory,
            Accessible
        }

        /**********************
        *   Activity Types 
        * *******************/
        public Dictionary<Enum, string> typeIds = new Dictionary<Enum, string>()
    {
        { AccessibleType.Screen,      "https://w3id.org/xapi/seriousgames/activity-types/screen"     },
        { AccessibleType.Area,        "https://w3id.org/xapi/seriousgames/activity-types/area"       },
        { AccessibleType.Zone,        "https://w3id.org/xapi/seriousgames/activity-types/zone"       },
        { AccessibleType.Cutscene,    "https://w3id.org/xapi/seriousgames/activity-types/cutscene"   },
        { AccessibleType.Inventory,   "https://w3id.org/xapi/seriousgames/custom-types/inventory"    }, // WARN: Not in profile server
        { AccessibleType.Accessible,  "https://w3id.org/xapi/seriousgames/activity-types/accessible" }  // WARN: Not in profile server
    };
        protected override Dictionary<Enum, string> TypeIds => typeIds;


        /**********************
        *     Extensions 
        * *******************/

        protected override Dictionary<Enum, string> ExtensionIds => null;


        /// <summary>
        /// Player accessed a reachable.
        /// Type = Accessible 
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        public StatementPromise Accessed(string reachableId)
        {
            return Accessed(reachableId, AccessibleType.Accessible);
        }

        /// <summary>
        /// Player accessed a reachable.
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        /// <param name="type">Reachable type.</param>
        public StatementPromise Accessed(string reachableId, AccessibleType type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Accessed),
                target = GetTargetActivity(reachableId, type)
            });
        }

        /// <summary>
        /// Player skipped a reachable.
        /// Type = Accessible
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        public StatementPromise Skipped(string reachableId)
        {
            return Skipped(reachableId, AccessibleType.Accessible);
        }

        /// <summary>
        /// Player skipped a reachable.
        /// </summary>
        /// <param name="reachableId">Reachable identifier.</param>
        /// <param name="type">Reachable type.</param>
        public StatementPromise Skipped(string reachableId, AccessibleType type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Skipped),
                target = GetTargetActivity(reachableId, type)
            });
        }


    }
}

