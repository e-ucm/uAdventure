using AssetPackage;
using Newtonsoft.Json;
using System.Collections;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Analytics
{
    public class AnalyticsExtension : GameExtension
    {

        //#################################################################
        //########################### SINGLETON ###########################
        //#################################################################
        #region Singleton
        static AnalyticsExtension instance;

        public static AnalyticsExtension Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion Singleton

        //##################################################################
        //########################### CONTROLLER ###########################
        //##################################################################
        #region Controller


        private float nextFlush = 0;
        private bool flushRequested = true;
        public CompletablesController CompletablesController { get; private set; }
        private TrackerConfig trackerConfig;
        private bool inited = false;

        public bool AutoStart { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        protected void Awake()
        {
            AutoStart = true;
            instance = this;
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

        void Update()
        {
            CheckTrackerFlush();
        }

        #region GameExtension

        public override IEnumerator Restart() 
        { 
            inited = false;
            Init();
            yield return true;
        }

        public override IEnumerator OnBeforeGameSave() 
        {
            var analyticsMemory = Game.Instance.GameState.GetMemory("analytics"); 
            if (analyticsMemory == null)
            {
                analyticsMemory = new Memory();
                Game.Instance.GameState.SetMemory("analytics", analyticsMemory);
            }
            analyticsMemory.Set("completables", JsonUtility.ToJson(CompletablesController));
            yield return true;
        }

        public override IEnumerator OnGameReady()
        {
            yield return true;
        }

        public override IEnumerator OnGameFinished()
        {
            yield return true;
        }

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
                // Get the tracker config from the game settings
                var trackerConfigs = Game.Instance.GameState.Data.getObjects<TrackerConfig>();
                trackerConfig = trackerConfigs.Count == 0 ? new TrackerConfig() : trackerConfigs[0];

                yield return StartCoroutine(StartTracker(trackerConfig));
                // TODO wait till start tracker is ready
            }
        }

        #endregion GameExtension
                
        public IEnumerator StartTracker(TrackerConfig config, IBridge bridge = null)
        {
            trackerConfig = config;
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

            TrackerAsset.TraceFormats format;
            switch (config.getTraceFormat())
            {
                case TrackerConfig.TraceFormat.XAPI:
                    format = TrackerAsset.TraceFormats.xapi;
                    break;
                default:
                    format = TrackerAsset.TraceFormats.csv;
                    break;
            }
            Debug.Log("[ANALYTICS] Format: " + format);

            TrackerAsset.StorageTypes storage;
            switch (config.getStorageType())
            {
                case TrackerConfig.StorageType.NET:
                    storage = TrackerAsset.StorageTypes.net;
                    break;
                default:
                    storage = TrackerAsset.StorageTypes.local;
                    break;
            }
            Debug.Log("[ANALYTICS] Storage: " + storage);

            TrackerAssetSettings tracker_settings = new TrackerAssetSettings()
            {
                Host = domain,
                TrackingCode = config.getTrackingCode(),
                BasePath = trackerConfig.getBasePath() ?? "/api",
                LoginEndpoint = trackerConfig.getLoginEndpoint() ?? "login",
                StartEndpoint = trackerConfig.getStartEndpoint() ?? "proxy/gleaner/collector/start/{0}",
                TrackEndpoint = trackerConfig.getStartEndpoint() ?? "proxy/gleaner/collector/track",
                Port = port,
                Secure = secure,
                StorageType = storage,
                TraceFormat = format,
                BackupStorage = config.getRawCopy(),
                UseBearerOnTrackEndpoint = trackerConfig.getUseBearerOnTrackEndpoint()
            };
            Debug.Log("[ANALYTICS] Settings: " + JsonConvert.SerializeObject(tracker_settings));
            TrackerAsset.Instance.StrictMode = false;
            TrackerAsset.Instance.Bridge = bridge ?? new UnityBridge();
            TrackerAsset.Instance.Settings = tracker_settings;
            TrackerAsset.Instance.StrictMode = false;

            var done = false;

            if (storage == TrackerAsset.StorageTypes.net && !string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password))
            { 
                Debug.Log("[ANALYTICS] Loging in...");
                TrackerAsset.Instance.LoginAsync(User, Password, logged =>
                {
                    Debug.Log("[ANALYTICS] Logged");
                    if (logged)
                    {
                        Debug.Log("[ANALYTICS] Starting tracker...");
                        TrackerAsset.Instance.StartAsync(() => done = true);
                    }
                    else
                    {
                        done = true;
                    }
                });
            }
            else
            {
                Debug.Log("[ANALYTICS] Starting tracker without login...");
                TrackerAsset.Instance.StartAsync(() => done = true);
            }


            this.nextFlush = config.getFlushInterval();

            Debug.Log("[ANALYTICS] Waiting until start");
            yield return new WaitUntil(() => done);
            Debug.Log("[ANALYTICS] Start done, result: " + TrackerAsset.Instance.Started);
        }
        
        [System.Obsolete]
        private void LoadTrackerSettings()
        {
            //Load tracker data
            SimpleJSON.JSONNode hostfile = new SimpleJSON.JSONClass();
            bool loaded = false;

            if (!Application.isMobilePlatform && Application.platform != RuntimePlatform.WebGLPlayer /*&& useSystemIO*/)
            {
                if (!System.IO.File.Exists("host.cfg"))
                {
                    hostfile.Add("host", new SimpleJSON.JSONData("http://192.168.175.117:3000/api/proxy/gleaner/collector/"));
                    hostfile.Add("trackingCode", new SimpleJSON.JSONData("57d81d5585b094006eab04d6ndecvjlvjss8aor"));
                    System.IO.File.WriteAllText("host.cfg", hostfile.ToString());
                }
                else
                {
                    hostfile = SimpleJSON.JSON.Parse(System.IO.File.ReadAllText("host.cfg"));
                }
                loaded = true;
            }

            try
            {
                if (loaded)
                {
                    var settings = TrackerAsset.Instance.Settings as TrackerAssetSettings;
                    settings.Host = hostfile["host"];
                    settings.TrackingCode = hostfile["trackingCode"];
                    //End tracker data loading
                }
            }
            catch
            {
                Debug.Log("Error loading the tracker settings");
            }
        }

        private void CheckTrackerFlush()
        {
            if (!TrackerAsset.Instance.Started)
            {
                return;
            }

            float delta = Time.deltaTime;
            if (trackerConfig.getFlushInterval() >= 0)
            {
                nextFlush -= delta;
                if (nextFlush <= 0)
                {
                    flushRequested = true;
                }
                while (nextFlush <= 0)
                {
                    nextFlush += trackerConfig.getFlushInterval();
                }
            }
            if (flushRequested)
            {
                flushRequested = false;
                TrackerAsset.Instance.Flush();
            }
        }
        
        #endregion Controller
    }
}
