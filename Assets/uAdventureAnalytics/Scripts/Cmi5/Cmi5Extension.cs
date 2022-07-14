using AssetPackage;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace uAdventure.Cmi5
{
    public static class Cmi5Extension
    {

        /**********************
         * Static attributes
         * *******************/

        private static ExtensionTransformations TR;
        private static AUTracker auTracker;


        /**********************
         * Extension enums and classes
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
        public enum Type
        {
            Object,
            AU,
            Block,
            Course
        }

        public enum Extension
        {
            Progress,
            Reason
        }

        // Transformations
        public class ExtensionTransformations
        {
            public Dictionary<string, string> verbIds = new Dictionary<string, string>()
            {
                { Verb.Launched.ToString().ToLower(),      "http://adlnet.gov/expapi/verbs/launched"    },
                { Verb.Initialized.ToString().ToLower(),   "http://adlnet.gov/expapi/verbs/initialized" },
                { Verb.Completed.ToString().ToLower(),     "http://adlnet.gov/expapi/verbs/completed"   },
                { Verb.Passed.ToString().ToLower(),        "http://adlnet.gov/expapi/verbs/passed"      },
                { Verb.Failed.ToString().ToLower(),        "http://adlnet.gov/expapi/verbs/failed"      },
                { Verb.Abandoned.ToString().ToLower(),     "https://w3id.org/xapi/adl/verbs/abandoned"  },
                { Verb.Waived.ToString().ToLower(),        "https://w3id.org/xapi/adl/verbs/waived"     },
                { Verb.Terminated.ToString().ToLower(),    "http://adlnet.gov/expapi/verbs/terminated"  },
                { Verb.Satisfied.ToString().ToLower(),     "https://w3id.org/xapi/adl/verbs/satisfied"  }
            };

            public Dictionary<string, string> typeIds = new Dictionary<string, string>()
            {
                { Type.Object.ToString().ToLower(),        "/object" },
                { Type.AU.ToString().ToLower(),            "/au"     },
                { Type.Block.ToString().ToLower(),         "/block"  },
                { Type.Course.ToString().ToLower(),        "/course" }
            };

            public Dictionary<string, string> objectIds = new Dictionary<string, string>()
            {
                { Extension.Progress.ToString().ToLower(), "https://w3id.org/xapi/cmi5/result/extensions/progress"  },
                { Extension.Reason.ToString().ToLower(),   "https://w3id.org/xapi/cmi5/result/extensions/reason"    }
            };
        }

        /// <summary>
        /// Movement tracker class used to extend the normal tracker
        /// </summary>
        public class AUTracker : TrackerAsset.IGameObjectTracker
        {

            private TrackerAsset tracker;

            public void setTracker(TrackerAsset tracker)
            {
                this.tracker = tracker;
            }

            #region Initialized
            /// <summary>
            /// An "Initialized" statement is used by the AU to indicate that it has
            /// been fully initialized and MUST follow the "Launched" statement created
            /// by the LMS within a reasonable period of time.
            /// Type == AU
            /// </summary>
            /// <param name="id">AU id.</param>
            public void Initialized(string id)
            {
                Initialized(id, Type.AU);
            }

            /// <summary>
            /// An "Initialized" statement is used by the AU to indicate that it has
            /// been fully initialized and MUST follow the "Launched" statement created
            /// by the LMS within a reasonable period of time.
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="type">AU type.</param>
            public void Initialized(string id, Type type)
            {
                tracker.ActionTrace(Verb.Initialized.ToString().ToLower(), type.ToString().ToLower(), id);
            }
            #endregion

            #region Completed

            /// <summary>
            /// The verb "Completed" indicates the learner viewed or did all of the
            /// relevant activities in an AU presentation. The use of the Completed 
            /// verb indicates progress of 100%.
            /// Type = AU. 
            /// </summary>
            /// <param name="id">AU id.</param> 
            public void Completed(string id)
            {
                Completed(id, Type.AU);
            }


            /// <summary>
            /// The verb "Completed" indicates the learner viewed or did all of the
            /// relevant activities in an AU presentation. The use of the Completed 
            /// verb indicates progress of 100%.
            /// Type = AU. 
            /// </summary>
            /// <param name="id">AU id.</param> 
            /// <param name="type">AU type.</param>
            public void Completed(string id, Type type)
            {
                tracker.ActionTrace(Verb.Completed.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            #endregion

            #region Passed

            /// <summary>
            /// The learner attempted and succeeded in a judged activity in the AU.
            /// Type = AU 
            /// </summary>
            /// <param name="id">AU id.</param>
            public void Passed(string id)
            {
                Passed(id, Type.AU);
            }

            /// <summary>
            /// The learner attempted and succeeded in a judged activity in the AU.
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="type">AU type.</param>
            public void Passed(string id, Type type)
            {
                tracker.ActionTrace(Verb.Passed.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            /// <summary>
            /// The learner attempted and succeeded in a judged activity in the AU.
            /// The (scaled) score MUST be equal to or greater than the "masteryScore"
            /// indicated in the LMS Launch Data.
            /// Type = AU 
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="score">The score scaled.</param>
            public void Passed(string id, float score)
            {
                Passed(id, Type.AU, score);
            }

            /// <summary>
            /// The learner attempted and succeeded in a judged activity in the AU.
            /// The (scaled) score MUST be equal to or greater than the "masteryScore"
            /// indicated in the LMS Launch Data.
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="type">AU type.</param>
            /// <param name="score">The score scaled.</param>
            public void Passed(string id, Type type, float score)
            {
                tracker.setScore(score);
                tracker.ActionTrace(Verb.Passed.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            #endregion

            #region Failed

            /// <summary>
            /// The learner attempted and failed in a judged activity in the AU.
            /// Type = AU 
            /// </summary>
            /// <param name="id">AU id.</param>
            public void Failed(string id)
            {
                Failed(id, Type.AU);
            }

            /// <summary>
            /// The learner attempted and failed in a judged activity in the AU.
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="type">AU type.</param>
            public void Failed(string id, Type type)
            {
                tracker.ActionTrace(Verb.Failed.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            /// <summary>
            /// The learner attempted and failed in a judged activity in the AU.
            /// The (scaled) score MUST be lower than the "masteryScore"
            /// indicated in the LMS Launch Data.
            /// Type = AU 
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="score">The score scaled.</param>
            public void Failed(string id, float score)
            {
                Failed(id, Type.AU, score);
            }

            /// <summary>
            /// The learner attempted and failed in a judged activity in the AU.
            /// The (scaled) score MUST be lower than the "masteryScore"
            /// indicated in the LMS Launch Data.
            /// </summary>
            /// <param name="id">AU id.</param>
            /// <param name="type">AU type.</param>
            /// <param name="score">The score scaled.</param>
            public void Failed(string id, Type type, float score)
            {
                tracker.setScore(score);
                tracker.ActionTrace(Verb.Failed.ToString().ToLower(), type.ToString().ToLower(), id);
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
            public void Terminated(string id)
            {
                tracker.ActionTrace(Verb.Terminated.ToString().ToLower(), Type.AU.ToString().ToLower(), id);
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
            public void Terminated(string id, Type type)
            {
                tracker.ActionTrace(Verb.Terminated.ToString().ToLower(), type.ToString().ToLower(), id);
            }

            #endregion
        
        }

        /**********************
         * Static methods
         * **********************/


        /// <summary>
        /// AU tracker static accessor
        /// </summary>
        public static AUTracker AU
        {
            get
            {
                if (auTracker == null)
                {
                    auTracker = new AUTracker();
                    auTracker.setTracker(TrackerAsset.Instance);
                }

                return auTracker;
            }
        }
    }
}