using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using TinCan;
using TinCan.Documents;
using TinCan.Json;
using Xasu.CMI5.Model;
using Xasu.Exceptions;
using Xasu.Util;

namespace Xasu.CMI5
{
    public static class Cmi5Helper
    {
        public static bool IsEnabled { get; private set; }

        /******************
         * QUERY PARAMETERS
         * ******************/

        public static Uri Endpoint { get; private set; }
        public static Agent Actor { get; private set; }
        public static Activity Activity { get; private set; }
        public static Uri Fetch { get; private set; }
        public static Guid Registration { get; set; }

        public static void SetQuery()
        {
            Endpoint = new Uri(Cmi5Utility.GetParam("endpoint"));
            Actor = new Agent(new StringOfJSON(Cmi5Utility.GetParam("actor")));
            Activity = new Activity
            {
                id = Cmi5Utility.GetParam("activityId")
            };
            Fetch = new Uri(Cmi5Utility.GetParam("fetch"));
            Registration = new Guid(Cmi5Utility.GetParam("registration"));
        }


        /******************
         * STATE DOCUMENT
         * ******************/
        public static Context ContextTemplate { get; set; }
        public static Context Cmi5Allowed { get; set; }
        public static Context Cmi5Context { get; private set; }
        public static Context MoveOnContext { get; private set; }
        public static Context MasteryScoreContext { get; private set; }
        public static LaunchMode LaunchMode { get; set; }
        public static Uri ReturnURL { get; set; }
        public static string LaunchParameters { get; set; }
        public static MoveOn MoveOn { get; set; }
        public static float MasteryScore { get; set; }

        public static void SetStateDocument(StateDocument stateDocument)
        {
            if (stateDocument == null && stateDocument.content == null)
            {
                throw new Cmi5Exception("Null state document!");
            }

            var stateDocumentData = JObject.Parse(Encoding.UTF8.GetString(stateDocument.content));

            // Extract context template
            if (stateDocumentData.ContainsKey("contextTemplate"))
            {
                ContextTemplate = new Context(stateDocumentData["contextTemplate"] as JObject);
            }

            // Extract launch mode
            if (stateDocumentData.ContainsKey("launchMode"))
            {
                LaunchMode = ParseLaunchMode(stateDocumentData["launchMode"].Value<string>());
            }

            // Extract return url
            if (stateDocumentData.ContainsKey("returnURL"))
            {
                ReturnURL = new Uri(stateDocumentData["returnURL"].Value<string>());
            }

            // Extract move on
            if (stateDocumentData.ContainsKey("moveOn"))
            {
                MoveOn = ParseMoveOn(stateDocumentData["moveOn"].Value<string>());
            }

            // Extract mastery score
            if (stateDocumentData.ContainsKey("masteryScore"))
            {
                MasteryScore = stateDocumentData["masteryScore"].Value<float>();
            }

            // Additional contexts for Cmi5 Tracker statements
            Cmi5Allowed = SetUpCmi5Allowed(ContextTemplate, Registration);
            Cmi5Context = SetUpCmi5Context(ContextTemplate, Registration);
            MoveOnContext = SetUpMoveOnContext(ContextTemplate, Registration);
            MasteryScoreContext = SetUpMasteryScoreContext(ContextTemplate, Registration, MasteryScore);

            IsEnabled = true;
        }

        private static LaunchMode ParseLaunchMode(string launchModeValue)
        {
            LaunchMode launchMode;
            switch (launchModeValue)
            {
                case "Normal": launchMode = LaunchMode.Normal; break;
                case "Browse": launchMode = LaunchMode.Browse; break;
                case "Review": launchMode = LaunchMode.Review; break;
                default:
                    throw new Cmi5Exception("Unknown LaunchMode value: " + launchModeValue);
            }

            return launchMode;
        }

        private static MoveOn ParseMoveOn(string moveOnValue)
        {
            MoveOn moveOn;
            switch (moveOnValue)
            {
                case "NotApplicable": moveOn = MoveOn.NotApplicable; break;
                case "Completed": moveOn = MoveOn.Completed; break;
                case "CompletedOrPassed": moveOn = MoveOn.CompletedOrPassed; break;
                case "CompletedAndPassed": moveOn = MoveOn.CompletedAndPassed; break;
                case "Passed": moveOn = MoveOn.Passed; break;
                default:
                    throw new Cmi5Exception("Unknown MoveOn value: " + moveOnValue);
            }

            return moveOn;
        }

        private static Context SetUpContext(Context contextTemplate, Guid registration)
        {
            Context context = new Context(contextTemplate.ToJObject());

            if (context.contextActivities == null)
            {
                context.contextActivities = new ContextActivities();
            }

            if (context.contextActivities.category == null)
            {
                context.contextActivities.category = new List<Activity>();
            }

            // Setup cmi5 'defined' required values
            context.registration = registration;

            return context;
        }

        private static Context SetUpCmi5Allowed(Context contextTemplate, Guid registration)
        {
            Context context = SetUpContext(contextTemplate, registration);

            // Setup cmi5 'allowed' required values
            context.registration = registration;

            return context;
        }

        private static Context SetUpCmi5Context(Context contextTemplate, Guid registration)
        {
            Context context = SetUpCmi5Allowed(contextTemplate, registration);

            // Setup cmi5 'defined' required values
            context.contextActivities.category.Add(new Activity
            {
                id = "https://w3id.org/xapi/cmi5/context/categories/cmi5"
            });

            return context;
        }

        private static Context SetUpMoveOnContext(Context contextTemplate, Guid registration)
        {
            Context context = SetUpCmi5Context(contextTemplate, registration);

            context.contextActivities.category.Add(new Activity
            {
                id = "https://w3id.org/xapi/cmi5/context/categories/moveon"
            });

            return context;
        }

        private static Context SetUpMasteryScoreContext(Context contextTemplate, Guid registration, float masteryScore)
        {
            Context context = SetUpMoveOnContext(contextTemplate, registration);

            // Simple add workaround
            var extensions = context.extensions.ToJObject();
            extensions.Add("https://w3id.org/xapi/cmi5/context/extensions/masteryscore", masteryScore);
            context.extensions = new TinCan.Extensions(extensions);

            return context;
        }
    }
}
