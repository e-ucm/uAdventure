using System;
using System.Collections.Generic;
using Xasu.Exceptions;
using TinCan;
using UnityEngine;

namespace Xasu.HighLevel
{

    public class CompletableTracker : AbstractHighLevelTracker<CompletableTracker>
    {

        /**********************
        *       Verbs
        * *******************/
        public enum Verb
        {
            Initialized,
            Progressed,
            Completed
        }

        public Dictionary<Enum, string> verbIds = new Dictionary<Enum, string>()
        {
            { Verb.Initialized,   "http://adlnet.gov/expapi/verbs/initialized"  },
            { Verb.Progressed,    "http://adlnet.gov/expapi/verbs/progressed"   },
            { Verb.Completed,     "http://adlnet.gov/expapi/verbs/completed"    }
        };

        protected override Dictionary<Enum, string> VerbIds => verbIds;

        /**********************
        *   Completable Types
        * *******************/

        public enum CompletableType
        {
            Game,
            Session,
            Level,
            Quest,
            Stage,
            Combat,
            StoryNode,
            Race,
            Completable,
            // Dialog completables extension
            DialogNode,
            DialogFragment
        }

        private readonly Dictionary<Enum, string> typeIds = new Dictionary<Enum, string>
        {
            { CompletableType.Game,            "https://w3id.org/xapi/seriousgames/activity-types/serious-game"    },
            { CompletableType.Session,         "https://w3id.org/xapi/seriousgames/activity-types/session"         },
            { CompletableType.Level,           "https://w3id.org/xapi/seriousgames/activity-types/level"           },
            { CompletableType.Quest,           "https://w3id.org/xapi/seriousgames/activity-types/quest"           },
            { CompletableType.Stage,           "https://w3id.org/xapi/seriousgames/activity-types/stage"           },
            { CompletableType.Combat,          "https://w3id.org/xapi/seriousgames/activity-types/combat"          },
            { CompletableType.StoryNode,       "https://w3id.org/xapi/seriousgames/activity-types/story-node"      },
            { CompletableType.Race,            "https://w3id.org/xapi/seriousgames/activity-types/race"            },
            { CompletableType.Completable,     "https://w3id.org/xapi/seriousgames/activity-types/completable"     },
            { CompletableType.DialogNode,      "https://w3id.org/xapi/seriousgames/activity-types/dialog-node"     },
            { CompletableType.DialogFragment,  "https://w3id.org/xapi/seriousgames/activity-types/dialog-fragment" }
        };

        protected override Dictionary<Enum, string> TypeIds => typeIds;

        public enum Extensions
        {
            Progress
        }

        private readonly Dictionary<Enum, string> extensionIds = new Dictionary<Enum, string>
        {
            { Extensions.Progress,            "https://w3id.org/xapi/seriousgames/extensions/progress"    }
        };

        protected override Dictionary<Enum, string> ExtensionIds => extensionIds;

        /**********************
            * Static attributes
            * *******************/

        private static Dictionary<string, DateTime> initializedTimes = new Dictionary<string, DateTime>();

        /// <summary>
        /// Player initialized a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        public StatementPromise Initialized(string completableId)
        {
            return Initialized(completableId, CompletableType.Completable);
        }

        /// <summary>
        /// Player initialized a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        public StatementPromise Initialized(string completableId, CompletableType type)
        {
            bool addInitializedTime = true;
            if (initializedTimes.ContainsKey(completableId))
            {
                if (XasuTracker.Instance.TrackerConfig.StrictMode)
                {
                    throw new XApiException("The initialized statement for the specified id has already been sent!");
                }
                else
                {
                    Debug.LogWarning("The initialized statement for the specified id has already been sent!");
                    addInitializedTime = false;
                }
            }

            if(addInitializedTime)
                initializedTimes.Add(completableId, DateTime.Now);
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Initialized),
                target = GetTargetActivity(completableId, type)
            });
        }

        /// <summary>
        /// Player progressed a completable.
        /// Type = Completable
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="value">New value for the completable's progress.</param>
        public StatementPromise Progressed(string completableId, float value)
        {
            return Progressed(completableId, CompletableType.Completable, value);
        }

        /// <summary>
        /// Player progressed a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="value">New value for the completable's progress.</param>
        /// <param name="type">Completable type.</param>
        public StatementPromise Progressed(string completableId, CompletableType type, float value)
        {
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Progressed),
                target = GetTargetActivity(completableId, type),
                result = SetResultExtensions(new Result(), new Dictionary<Enum, object>
                {
                    { Extensions.Progress, value }
                })
            });
        }

        /// <summary>
        /// Player completed a completable.
        /// Type = Completable
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        public StatementPromise Completed(string completableId)
        {
            return Completed(completableId, CompletableType.Completable, false, 0);
        }

        /// <summary>
        /// Player completed a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        public StatementPromise Completed(string completableId, CompletableType type)
        {
            return Completed(completableId, type, false, 0);
        }

        /// <summary>
        /// Player completed a completable.
        /// Type = Completable
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        public StatementPromise Completed(string completableId, float durationInSeconds)
        {
            return Completed(completableId, CompletableType.Completable, true, durationInSeconds);
        }

        /// <summary>
        /// Player completed a completable.
        /// </summary>
        /// <param name="completableId">Completable identifier.</param>
        /// <param name="type">Completable type.</param>
        public StatementPromise Completed(string completableId, CompletableType type, float durationInSeconds)
        {
            return Completed(completableId, type, true, durationInSeconds);
        }

        private StatementPromise Completed(string completableId, CompletableType type, bool hasDuration, float durationInSeconds)
        {
            if (!hasDuration && !initializedTimes.ContainsKey(completableId))
            {
                if (XasuTracker.Instance.TrackerConfig.StrictMode)
                {
                    throw new XApiException("The completed statement for the specified id has not been initialized!");
                }
                else
                {
                    hasDuration = true;
                    durationInSeconds = 0f;
                    Debug.LogWarning("The completed statement for the specified id has not been initialized and" +
                        " therefore the duration is going to be 0.");
                }
            }

            // Get the initialized statement time to calculate the duration
            TimeSpan duration = hasDuration ? TimeSpan.FromSeconds(durationInSeconds) : DateTime.Now - initializedTimes[completableId];
            if (initializedTimes.ContainsKey(completableId))
            {
                initializedTimes.Remove(completableId);
            }

            var result = new Result{ completion = true };
            if (!hasDuration || durationInSeconds > 0f)
            {
                result.duration = duration;
            }

            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Completed),
                target = GetTargetActivity(completableId, type),
                result = result
            });
        }

    }
}