using System.Collections.Generic;
using System;
using TinCan;

namespace Xasu.HighLevel
{
    public class AlternativeTracker : AbstractHighLevelTracker<AlternativeTracker>
    {

        /**********************
        *       Verbs
        * *******************/
        public enum Verb
        {
            Selected,
            Unlocked
        }

        public Dictionary<Enum, string> verbIds = new Dictionary<Enum, string>()
    {
        { Verb.Selected, "http://id.tincanapi.com/verb/selected"             },
        { Verb.Unlocked, "https://w3id.org/xapi/seriousgames/verbs/unlocked" },
    };
        protected override Dictionary<Enum, string> VerbIds => verbIds;

        public enum AlternativeType
        {
            Question,
            Menu,
            Dialog,
            Path,
            Arena,
            Alternative
        }

        /**********************
        *   Activity Types 
        * *******************/
        public Dictionary<Enum, string> typeIds = new Dictionary<Enum, string>()
    {
        { AlternativeType.Question,    "http://adlnet.gov/expapi/activities/question"                   },
        { AlternativeType.Menu,        "https://w3id.org/xapi/seriousgames/activity-types/menu"         },
        { AlternativeType.Dialog,      "https://w3id.org/xapi/seriousgames/activity-types/dialog-tree"  },
        { AlternativeType.Path,        "https://w3id.org/xapi/seriousgames/activity-types/path"         }, // WARN: Not in profile server
        { AlternativeType.Arena,       "https://w3id.org/xapi/seriousgames/activity-types/arena"        }, // WARN: Not in profile server
        { AlternativeType.Alternative, "https://w3id.org/xapi/seriousgames/activity-types/alternative"  }  // WARN: Not in profile server
    };
        protected override Dictionary<Enum, string> TypeIds => typeIds;

        /**********************
        *     Extensions
        * *******************/
        protected override Dictionary<Enum, string> ExtensionIds => null;


        /**********************
        *     Templates
        * *******************/

        /// <summary>
        /// Player selected an option in a presented alternative
        /// Type = Alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        public StatementPromise Selected(string alternativeId, string optionId)
        {
            return Selected(alternativeId, optionId, AlternativeType.Alternative);
        }

        /// <summary>
        /// Player selected an option in a presented alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        /// <param name="type">Alternative type.</param>
        public StatementPromise Selected(string alternativeId, string optionId, AlternativeType type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Selected),
                target = GetTargetActivity(alternativeId, type),
                result = new Result
                {
                    response = optionId
                }
            });
        }

        /// <summary>
        /// Player unlocked an option
        /// Type = Alternative
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        public StatementPromise Unlocked(string alternativeId, string optionId)
        {
            return Unlocked(alternativeId, optionId, AlternativeType.Alternative);
        }

        /// <summary>
        /// Player unlocked an option
        /// </summary>
        /// <param name="alternativeId">Alternative identifier.</param>
        /// <param name="optionId">Option identifier.</param>
        /// <param name="type">Alternative type.</param>
        public StatementPromise Unlocked(string alternativeId, string optionId, AlternativeType type)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Unlocked),
                target = GetTargetActivity(alternativeId, type),
                result = new Result
                {
                    response = optionId
                }
            });
        }

    }
}