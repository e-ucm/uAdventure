using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;
using Xasu;
using Xasu.Util;

namespace uAdventure.Analytics
{
    public class AnalyticsExtension : GameExtension
    {
        //##################################################################
        //########################### CONTROLLER ###########################
        //##################################################################
        #region Controller

        public CompletablesController CompletablesController { get; private set; }
        //private TrackerConfig trackerConfig;
        private bool inited = false;

        public bool AutoStart { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        protected override void Awake()
        {
            base.Awake();
            AutoStart = true;
        }

        public void Init()
        {
            if (inited)
            {
                return;
            }

            inited = true;

            CompletablesController = new CompletablesController();
        }

        #region GameExtension

        public override IEnumerator Restart() 
        { 
            inited = false;
            Init();
            yield return true;
        }

        public override void OnBeforeGameSave() 
        {
            var analyticsMemory = Game.Instance.GameState.GetMemory("analytics"); 
            if (analyticsMemory == null)
            {
                analyticsMemory = new Memory();
                Game.Instance.GameState.SetMemory("analytics", analyticsMemory);
            }
            analyticsMemory.Set("completables", JsonUtility.ToJson(CompletablesController));
        }

        public override IEnumerator OnGameReady()
        {
            yield return true;
        }

        private bool afterFlush;
        public void AfterFlush()
        {
            afterFlush = true;
        }

        [Priority(1)]
        public override IEnumerator OnGameFinished()
        {

            if (XasuTracker.Instance.Status.State == TrackerState.NetworkRequired || XasuTracker.Instance.Status.State == TrackerState.Fallback)
            {
                // TODO Remember the player to connect in order to flush everything
            }

            if (XasuTracker.Instance.Status.State == TrackerState.Normal)
            {
                var finalized = false;
                PartialStatements.CompleteAllStatements();
                XasuTracker.Instance.Finalize()
                    .ContinueWith(t =>
                    {
                        // TODO check if flush failed
                        finalized = true;
                    });
                var time = Time.time;
                yield return new WaitUntil(() => finalized);
            }
        }

        [Priority(1)]
        public override IEnumerator OnAfterGameLoad()
        {
            if (!inited)
            {
                Init();
            }

            var analyticsMemory = Game.Instance.GameState.GetMemory("analytics");
            if(analyticsMemory == null)
            {
                analyticsMemory = new Memory();
                Game.Instance.GameState.SetMemory("analytics", analyticsMemory);
            }
            else
            {
                CompletablesController.RestoreCompletables(analyticsMemory);
            }

            if (AutoStart)
            {
                Debug.Log("[TRACKER] AutoStart!");
                // Get the tracker config from the game settings
                var trackerConfigs = Game.Instance.GameState.Data.getObjects<TrackerConfig>();
                var trackerConfig = trackerConfigs.Count == 0 ? new TrackerConfig() : trackerConfigs[0];
                yield return StartCoroutine(StartTracker(trackerConfig, null));
                // TODO wait till start tracker is ready
            }
        }

        #endregion GameExtension
                
        public IEnumerator StartTracker(TrackerConfig config, string backupFilename)
        {
            //trackerConfig = config;
            string domain = "";
            int port = 80;
            bool secure = false;

            Debug.Log("[ANALYTICS] Setting up tracker...");
            try
            {
                if (config.getHost() != "")
                {
                    string[] splitted = config.getHost().Split('/');

                    if (splitted.Length > 1)
                    {
                        string[] host_splitted = splitted[2].Split(':');
                        if (host_splitted.Length > 0)
                        {
                            domain = host_splitted[0];
                            port = (host_splitted.Length > 1) ? int.Parse(host_splitted[1]) : (splitted[0] == "https:" ? 443 : 80);
                            secure = splitted[0] == "https:";
                        }
                    }
                }
                else
                {
                    config.setHost("localhost");
                }
                Debug.Log("[ANALYTICS] Config: " + JsonConvert.SerializeObject(config));
            }
            catch (System.Exception e)
            {
                Debug.Log("Tracker error: Host bad format");
            }

            var trackerConfig = new Xasu.Config.TrackerConfig
            {
                FlushInterval = 3,
                BatchSize = 256,

                Online = config.getStorageType() == TrackerConfig.StorageType.NET,
                Fallback = true,
                LRSEndpoint = config.getHost() + config.getTrackEndpoint(),

                Offline = config.getStorageType() == TrackerConfig.StorageType.LOCAL,
                TraceFormat = Xasu.Config.TraceFormats.XAPI,

                // TODO backup
            };


            if (config.getStorageType() == TrackerConfig.StorageType.NET && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password))
            {
                Debug.Log("[ANALYTICS] Setting up basic Loging...");
                trackerConfig.AuthProtocol = "basic";
                trackerConfig.AuthParameters = new Dictionary<string, string>
                {
                    {"username", User },
                    {"password", Password }
                };
            }
            else
            {
                Debug.Log("[ANALYTICS] Starting tracker without login...");
            }


            Debug.Log("[ANALYTICS] Settings: " + JsonConvert.SerializeObject(trackerConfig));

            var done = false;
            XasuTracker.Instance.Init(trackerConfig)
                .ContinueWith(t =>
                {
                    // TODO fix 
                    done = true;
                });

            Debug.Log("[ANALYTICS] Waiting until start");
            yield return new WaitUntil(() => done);
            Debug.Log("[ANALYTICS] Start done, result: " + XasuTracker.Instance.Status.State);
        }
        
        #endregion Controller
    }
}
