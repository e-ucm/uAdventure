using System;
using System.Collections.Generic;
using TinCan;
using Xasu.CMI5;
using Xasu.Exceptions;
using Xasu.Processors;

namespace Xasu.HighLevel
{

    public class Cmi5Tracker : AbstractHighLevelTracker<Cmi5Tracker>
    {
        /**********************
        *       Verbs
        * *******************/
        public enum Verb
        {
            Launched,
            Initialized,
            Completed,
            Passed,
            Failed,
            Abandoned,
            Waived,
            Terminated,
            Satisfied
        }

        public Dictionary<Enum, string> verbIds = new Dictionary<Enum, string>()
        {
            { Verb.Launched,      "http://adlnet.gov/expapi/verbs/launched"    },
            { Verb.Initialized,   "http://adlnet.gov/expapi/verbs/initialized" },
            { Verb.Completed,     "http://adlnet.gov/expapi/verbs/completed"   },
            { Verb.Passed,        "http://adlnet.gov/expapi/verbs/passed"      },
            { Verb.Failed,        "http://adlnet.gov/expapi/verbs/failed"      },
            { Verb.Abandoned,     "https://w3id.org/xapi/adl/verbs/abandoned"  },
            { Verb.Waived,        "https://w3id.org/xapi/adl/verbs/waived"     },
            { Verb.Terminated,    "http://adlnet.gov/expapi/verbs/terminated"  },
            { Verb.Satisfied,     "https://w3id.org/xapi/adl/verbs/satisfied"  }
        };
        protected override Dictionary<Enum, string> VerbIds => verbIds;

        public enum Type
        {
            Object,
            AU,
            Block,
            Course
        }

        /**********************
        *   Activity Types 
        * *******************/
        public Dictionary<Enum, string> typeIds = new Dictionary<Enum, string>()
            {
                { Type.Object,        "/object" },
                { Type.AU,            "/au"     },
                { Type.Block,         "/block"  },
                { Type.Course,        "/course" }
            };
        protected override Dictionary<Enum, string> TypeIds => typeIds;


        /**********************
        *     Extensions 
        * *******************/

        public enum Extension
        {
            Progress,
            Reason
        }

        public Dictionary<Enum, string> objectIds = new Dictionary<Enum, string>()
            {
                { Extension.Progress, "https://w3id.org/xapi/cmi5/result/extensions/progress"  },
                { Extension.Reason,   "https://w3id.org/xapi/cmi5/result/extensions/reason"    }
            };
        protected override Dictionary<Enum, string> ExtensionIds => objectIds;


        /**********************
            * Static attributes
            * *******************/

        private static Dictionary<string, long> initializedTimes = new Dictionary<string, long>();



        #region Initialized
        /// <summary>
        /// An "Initialized" statement is used by the AU to indicate that it has
        /// been fully initialized and MUST follow the "Launched" statement created
        /// by the LMS within a reasonable period of time.
        /// Type == AU
        /// </summary>
        public StatementPromise Initialized()
        {
            CheckCmi5Enabled();

            return Initialized(Cmi5Helper.Activity.id);
        }

        /// <summary>
        /// An "Initialized" statement is used by the AU to indicate that it has
        /// been fully initialized and MUST follow the "Launched" statement created
        /// by the LMS within a reasonable period of time.
        /// </summary>
        /// <param name="id">AU id.</param>
        public StatementPromise Initialized(string id)
        {
            CheckCmi5Enabled();
            if (initializedTimes.ContainsKey(id))
            {
                throw new Cmi5Exception("The initialized statement for the specified id has already been sent!");
            }

            initializedTimes.Add(id, DateTime.Now.Ticks);
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Initialized),
                target = id == Cmi5Helper.Activity.id ? Cmi5Helper.Activity : new Activity { id = id },
                actor = Cmi5Helper.Actor,
                context = Cmi5Helper.Cmi5Context
            });
        }

        #endregion

        #region Completed

        /// <summary>
        /// The verb "Completed" indicates the learner viewed or did all of the
        /// relevant activities in an AU presentation. The use of the Completed 
        /// verb indicates progress of 100%.
        /// Type = AU. 
        /// </summary>
        public StatementPromise Completed(double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Completed(Cmi5Helper.Activity.id, durationInSeconds);
        }


        /// <summary>
        /// The verb "Completed" indicates the learner viewed or did all of the
        /// relevant activities in an AU presentation. The use of the Completed 
        /// verb indicates progress of 100%.
        /// Type = AU. 
        /// </summary>
        /// <param name="id">AU id.</param> 
        /// <param name="type">AU type.</param>
        public StatementPromise Completed(string id, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Completed),
                target = id == Cmi5Helper.Activity.id ? Cmi5Helper.Activity : new Activity { id = id },
                actor = Cmi5Helper.Actor,
                context = Cmi5Helper.MoveOnContext,
                result = new Result
                {
                    completion = true,
                    duration = TimeSpan.FromSeconds(durationInSeconds)
                }
            });
        }

        #endregion

        #region Passed

        /// <summary>
        /// The learner attempted and succeeded in a judged activity in the AU.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        public StatementPromise Passed(float durationInSeconds)
        {
            CheckCmi5Enabled();
            return Passed(Cmi5Helper.Activity.id, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and succeeded in a judged activity in the AU.
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="type">AU type.</param>
        public StatementPromise Passed(string id, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Passed(id, float.NaN, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and succeeded in a judged activity in the AU.
        /// The (scaled) score MUST be equal to or greater than the "masteryScore"
        /// indicated in the LMS Launch Data.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="score">The score scaled.</param>
        public StatementPromise Passed(float score, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Passed(Cmi5Helper.Activity.id, score, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and succeeded in a judged activity in the AU.
        /// The (scaled) score MUST be equal to or greater than the "masteryScore"
        /// indicated in the LMS Launch Data.
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="type">AU type.</param>
        /// <param name="score">The score scaled.</param>
        public StatementPromise Passed(string id, float score, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Passed),
                target = id == Cmi5Helper.Activity.id ? Cmi5Helper.Activity : new Activity { id = id },
                actor = Cmi5Helper.Actor,
                context = Cmi5Helper.MasteryScoreContext,
                result = new Result
                {
                    success = true,
                    score = float.IsNaN(score) ? null : new Score { scaled = score },
                    duration = TimeSpan.FromSeconds(durationInSeconds)
                }
            });
        }

        #endregion

        #region Failed

        /// <summary>
        /// The learner attempted and failed in a judged activity in the AU.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        public StatementPromise Failed(double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Failed(Cmi5Helper.Activity.id, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and failed in a judged activity in the AU.
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="type">AU type.</param>
        public StatementPromise Failed(string id, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Failed(id, float.NaN, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and failed in a judged activity in the AU.
        /// The (scaled) score MUST be lower than the "masteryScore"
        /// indicated in the LMS Launch Data.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="score">The score scaled.</param>
        public StatementPromise Failed(float score, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Failed(Cmi5Helper.Activity.id, score, durationInSeconds);
        }

        /// <summary>
        /// The learner attempted and failed in a judged activity in the AU.
        /// The (scaled) score MUST be lower than the "masteryScore"
        /// indicated in the LMS Launch Data.
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="type">AU type.</param>
        /// <param name="score">The score scaled.</param>
        public StatementPromise Failed(string id, float score, double durationInSeconds)
        {
            CheckCmi5Enabled();
            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Failed),
                target = id == Cmi5Helper.Activity.id ? Cmi5Helper.Activity : new Activity { id = id },
                actor = Cmi5Helper.Actor,
                context = Cmi5Helper.MasteryScoreContext,
                result = new Result
                {
                    success = false,
                    score = score == float.NaN ? null : new Score { scaled = score },
                    duration = TimeSpan.FromSeconds(durationInSeconds)
                }
            });
        }

        #endregion

        #region Terminated

        /// <summary>
        /// The verb "Terminated" indicates that the AU was terminated by the
        /// Learner and that the AU will not be sending any more statements for
        /// the launch session.
        /// This statement MUST be the last statement (of any kind) sent by the AU in a session.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        public StatementPromise Terminated()
        {
            CheckCmi5Enabled();
            return Terminated(Cmi5Helper.Activity.id);
        }

        /// <summary>
        /// The verb "Terminated" indicates that the AU was terminated by the
        /// Learner and that the AU will not be sending any more statements for
        /// the launch session.
        /// This statement MUST be the last statement (of any kind) sent by the AU in a session.
        /// Type = AU 
        /// </summary>
        /// <param name="id">AU id.</param>
        /// <param name="type">AU type.</param>
        public StatementPromise Terminated(string id)
        {
            CheckCmi5Enabled();
            if (!initializedTimes.ContainsKey(id))
            {
                throw new Cmi5Exception("The completed statement for the specified id has not been initialized!");
            }

            // Get the initialized statement time to calculate the duration
            long ticks = initializedTimes[id];
            initializedTimes.Remove(id);

            return Enqueue(new Statement
            {
                verb = GetVerb(Verb.Terminated),
                target = id == Cmi5Helper.Activity.id ? Cmi5Helper.Activity : new Activity { id = id },
                actor = Cmi5Helper.Actor,
                context = Cmi5Helper.Cmi5Context,
                result = new Result
                {
                    duration = DateTime.Now - new DateTime(ticks)
                }
            });
        }

        #endregion

        private static void CheckCmi5Enabled()
        {
            if (!Cmi5Helper.IsEnabled)
            {
                throw new Cmi5Exception("CMI-5 traces are not available because is not enabled! Check the authorization " +
                    "configuration to enable it and launch the game using the CMI-5 protocol (URL/URIScheme).");
            }
        }
    }
}