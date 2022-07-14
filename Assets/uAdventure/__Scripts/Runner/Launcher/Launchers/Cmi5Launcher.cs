using System.Collections.Generic;

namespace uAdventure.Runner
{
    public class Cmi5Launcher : ILauncher
    {

#if UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern string uAdventureGetCompleteURL();
#endif

        public static Cmi5Helper.Cmi5LaunchConfig config = null;

        public static Dictionary<string, string> parameters;
        
        public int Priority { get { return 1; } }

        public virtual bool WantsControl()
        {
            string launchData = "";
#if UNITY_WEBGL
            launchData = uAdventureGetCompleteURL();
#else
            var args = System.Environment.GetCommandLineArgs();
            if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
            {
                launchData = args[1];
            }
#endif
            if (launchData.Contains("?"))
            {
                launchData = launchData.Split('?')[1];
            }

            var urlParams = launchData.Split('&');

            foreach(var param in urlParams)
            {
                var paramParts = param.Split('=');
                if(paramParts.Length == 1)
                {
                    parameters.Add(paramParts[0], "true");
                }
                else
                {
                    parameters.Add(paramParts[0], UnityEngine.Networking.UnityWebRequest.UnEscapeURL(paramParts[1]));
                }
            }

            return parameters.ContainsKey("endpoint") &&
                parameters.ContainsKey("fetch") &&
                parameters.ContainsKey("actor") &&
                parameters.ContainsKey("registration") &&
                parameters.ContainsKey("activityId");
        }

        public virtual bool Launch()
        {
            config = new Cmi5Helper.Cmi5LaunchConfig
            {
                Endpoint = parameters["endpoint"],
                Fetch = parameters["fetch"],
                Actor = parameters["actor"],
                Registration = parameters["registration"],
                ActivityId = parameters["activityId"],
            };

            return false;
        }
    }
}
