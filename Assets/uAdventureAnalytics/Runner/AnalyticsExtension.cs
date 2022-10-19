using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;
using Xasu;
using Xasu.Auth.Protocols;
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

        [Priority(1)]
        public override IEnumerator OnGameFinished()
        {
            // TODO Remember the player to connect in order to flush everything
            yield return new WaitUntil(() => !XasuTracker.Instance.Status.IsNetworkRequired);

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
                yield return StartCoroutine(StartTracker(ConvertConfig(trackerConfig)));
                // TODO wait till start tracker is ready
            }
        }

        #endregion GameExtension

        private Xasu.Config.TrackerConfig ConvertConfig(TrackerConfig config)
        {
            Debug.Log("[ANALYTICS] Setting up tracker config...");

            //trackerConfig = config;
            string domain = "";
            int port = 80;
            bool secure = false; 
            
            try
            {
                if (!string.IsNullOrEmpty(config.getHost()))
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
                Debug.Log("[ANALYTICS] Config: " + JsonConvert.SerializeObject(config));
            }
            catch (System.Exception)
            {
                Debug.LogError("[ANALYTICS] Tracker error: Host bad format");
                throw;
            }

            var currentTimestamp = DateTime.Now.ToFileTime().ToString();
            var logName = currentTimestamp + ".log";
            if (string.IsNullOrEmpty(config.getBackupFileName()))
            {
                config.setBackupFileName(DateTime.Now.ToFileTime().ToString() + "_backup.log");
            }

            var simva = config.getHost()?.Contains("simva");
            var trackerConfig = new Xasu.Config.TrackerConfig
            {
                //Simva
                Simva = simva.HasValue ? simva.Value : false,

                FlushInterval = 3,
                BatchSize = 256,

                Online = config.getStorageType() == TrackerConfig.StorageType.NET,
                Fallback = true,
                LRSEndpoint = config.getHost() + config.getTrackEndpoint(),

                Offline = config.getStorageType() == TrackerConfig.StorageType.LOCAL,
                FileName = logName,
                TraceFormat = Xasu.Config.TraceFormats.XAPI,

                Backup = config.getRawCopy(),
                BackupFileName = config.getBackupFileName(),
                BackupEndpoint = config.getHost() + config.getBackupEndpoint()
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

            return trackerConfig;
        }
                
        public IEnumerator StartTracker(Xasu.Config.TrackerConfig config, IAuthProtocol onlineProtocol = null, IAuthProtocol backupProtocol = null)
        {
            Debug.Log("[ANALYTICS] Settings: " + JsonConvert.SerializeObject(config));

            var done = false;
            XasuTracker.Instance.Init(config, onlineProtocol, backupProtocol)
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
