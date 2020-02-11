using AssetPackage;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using uAdventure.Core;
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
        private List<Completable> completables;
        private List<CompletableController> completableControllers = new List<CompletableController>();
        private TrackerConfig trackerConfig;
        private bool awake = false;

        public void Awake()
        {
            if (awake)
            {
                return;
            }

            awake = true;
            instance = this;
            //Create Main game completable
            var trackerConfigs = Game.Instance.GameState.Data.getObjects<TrackerConfig>();
            PrepareTracker(trackerConfigs.Count == 0 ? new TrackerConfig() : trackerConfigs[0]);

            Completable mainGame = new Completable();

            Completable.Milestone gameStart = new Completable.Milestone();
            gameStart.setType(Completable.Milestone.MilestoneType.SCENE);
            gameStart.setId(Game.Instance.GameState.InitialChapterTarget.getId());
            mainGame.setStart(gameStart);
            mainGame.setId(Game.Instance.GameState.Data.getTitle());
            mainGame.setType(Completable.TYPE_GAME);

            Completable.Milestone gameEnd = new Completable.Milestone();
            gameEnd.setType(Completable.Milestone.MilestoneType.ENDING);
            mainGame.setEnd(gameEnd);

            Completable.Progress gameProgress = new Completable.Progress();
            gameProgress.setType(Completable.Progress.ProgressType.SUM);

            Completable.Score mainScore = new Completable.Score();
            mainScore.setMethod(Completable.Score.ScoreMethod.AVERAGE);

            completables = new List<Completable>(Game.Instance.GameState.GetObjects<Completable>());

            foreach (Completable part in completables)
            {
                Completable.Milestone tmpMilestone = new Completable.Milestone();
                tmpMilestone.setType(Completable.Milestone.MilestoneType.COMPLETABLE);
                tmpMilestone.setId(part.getId());
                gameProgress.addMilestone(tmpMilestone);

                Completable.Score tmpScore = new Completable.Score();
                tmpScore.setMethod(Completable.Score.ScoreMethod.SINGLE);
                tmpScore.setType(Completable.Score.ScoreType.COMPLETABLE);
                tmpScore.setId(part.getId());
                mainScore.addSubScore(tmpScore);
            }
            mainGame.setProgress(gameProgress);
            mainGame.setScore(mainScore);

            completables.Insert(0, mainGame);

            SetCompletables(completables);

            Game.Instance.GameState.OnConditionChanged += (_, __) => ConditionChanged();
            Game.Instance.OnTargetChanged += TargetChanged;
            Game.Instance.OnElementInteracted += ElementInteracted;
        }

        void Update()
        {
            CheckTrackerFlush();
        }

        public override void Restart() { awake = false; Awake(); }

        public override void OnBeforeGameSave() 
        {
            var analyticsMemory = Game.Instance.GameState.GetMemory("analytics"); 
            if (analyticsMemory == null)
            {
                analyticsMemory = new Memory();
                Game.Instance.GameState.SetMemory("analytics", analyticsMemory);
            }
            analyticsMemory.Set("completables", SerializeToString(completableControllers));
        }
        public override void OnGameReady() { }

        public override void OnAfterGameLoad()
        {
            if (!awake)
            {
                Awake();
            }

            var analyticsMemory = Game.Instance.GameState.GetMemory("analytics");
            if(analyticsMemory == null)
            {
                analyticsMemory = new Memory();
                Game.Instance.GameState.SetMemory("analytics", analyticsMemory);
            }
            else
            {
                var serializedCompletables = analyticsMemory.Get<string>("completables");
                if (!string.IsNullOrEmpty(serializedCompletables))
                {
                    completableControllers = DeserializeFromString<List<CompletableController>>(serializedCompletables);
                    for (int i = 0; i < completableControllers.Count; i++)
                    {
                        completableControllers[i].SetCompletable(this.completables[i]);
                        completableControllers[i].Start.SetMilestone(this.completables[i].getStart());
                        completableControllers[i].End.SetMilestone(this.completables[i].getEnd());
                        for (int j = 0; j < this.completables[i].getProgress().getMilestones().Count; j++)
                        {
                            completableControllers[i].ProgressControllers[j].SetMilestone(this.completables[i].getProgress().getMilestones()[j]);
                        }
                    }
                }
            }
        }

        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = System.Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        private void PrepareTracker(TrackerConfig config)
        {
            trackerConfig = config;
            string domain = "";
            int port = 80;
            bool secure = false;

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

            TrackerAssetSettings tracker_settings = new TrackerAssetSettings()
            {
                Host = domain,
                TrackingCode = config.getTrackingCode(),
                BasePath = "/api",
                Port = port,
                Secure = secure,
                StorageType = storage,
                TraceFormat = format,
                BackupStorage = config.getRawCopy(),
            };
            TrackerAsset.Instance.StrictMode = false;
            TrackerAsset.Instance.Bridge = new UnityBridge();
            TrackerAsset.Instance.Settings = tracker_settings;
            TrackerAsset.Instance.StrictMode = false;

            if (PlayerPrefs.HasKey("LimesurveyToken") && PlayerPrefs.GetString("LimesurveyToken") != "ADMIN")
            {
                TrackerAsset.Instance.Login(PlayerPrefs.GetString("LimesurveyToken"), PlayerPrefs.GetString("LimesurveyToken"));
            }
                

            TrackerAsset.Instance.Start();
            this.nextFlush = config.getFlushInterval();
        }

        public CompletableController GetCompletable(string id)
        {
            return completableControllers.Find(c => c.GetCompletable().getId().Equals(id));
        }

        public void SetCompletables(List<Completable> value)
        {
            this.completableControllers.Clear();
            this.completableControllers.AddRange(value.ConvertAll(c => new CompletableController(c)));
        }

        private void UpdateCompletables(System.Func<CompletableController, bool> updatefunction)
        {

            bool somethingCompleted;
            do
            {
                somethingCompleted = false;
                foreach (var completableController in completableControllers)
                {
                    somethingCompleted |= updatefunction(completableController);

                }

                if (somethingCompleted)
                {
                    CompletableCompleted();
                }
            }
            while (somethingCompleted);



            RestartFinished();
        }

        public void ConditionChanged()
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones());
        }

        public void TargetChanged(IChapterTarget target)
        {
            if (!string.IsNullOrEmpty(target.getXApiClass()) && target.getXApiClass() == "accesible")
            {
                TrackerAsset.Instance.Accessible.Accessed(target.getId(), ExParsers.ParseEnum<AccessibleTracker.Accessible>(target.getXApiType()));
                TrackerAsset.Instance.Flush();
            }

            UpdateCompletables(completableController => completableController.UpdateMilestones(target));
        }

        public void ElementInteracted(bool finished, Interactuable interactuable, Action action)
        {
            var interactiveElement = interactuable as InteractiveElement;
            if (interactiveElement == null)
            {
                return;
            }

            if (!finished)
            {
                UpdateElementsInteracted(interactiveElement, action.getType().ToString(), interactiveElement.Element.getId());

                Game.Instance.GameState.BeginChangeAmbit();
            }
            else
            {
                string actionType = string.Empty;
                switch (action.getType())
                {
                    case Action.CUSTOM: actionType = (action as CustomAction).getName(); break;
                    case Action.CUSTOM_INTERACT: actionType = (action as CustomAction).getName(); break;
                    case Action.DRAG_TO: actionType = "drag_to"; break;
                    case Action.EXAMINE: actionType = "examine"; break;
                    case Action.GIVE_TO: actionType = "give_to"; break;
                    case Action.GRAB: actionType = "grab"; break;
                    case Action.TALK_TO: actionType = "talk_to"; break;
                    case Action.USE: actionType = "use"; break;
                    case Action.USE_WITH: actionType = "use_with"; break;
                }

                if (!string.IsNullOrEmpty(action.getTargetId()))
                {
                    TrackerAsset.Instance.setVar("action_target", action.getTargetId());
                }

                if (!string.IsNullOrEmpty(actionType))
                {
                    TrackerAsset.Instance.setVar("action_type", actionType);
                }

                Game.Instance.GameState.EndChangeAmbitAsExtensions();
                var element = interactiveElement.Element;

                if (element is NPC)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Npc);
                }
                else if (element is Item)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Item);
                }
                else if (element is ActiveArea)
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.Item);
                }
                else
                {
                    TrackerAsset.Instance.GameObject.Interacted(element.getId(), GameObjectTracker.TrackedGameObject.GameObject);
                }
            }
        }

        public void UpdateElementsInteracted(Interactuable interactuable, string interaction, string targetId)
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones(interactuable, interaction, targetId));
        }

        public void CompletableCompleted()
        {
            UpdateCompletables(completableController => completableController.UpdateMilestones());
        }

        public void TrackStarted(CompletableController completableController)
        {
            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;


            TrackerAsset.Instance.Completable.Initialized(completableId, completableType);
            TrackerAsset.Instance.Completable.Progressed(completableId, completableType, 0);
        }

        public void TrackProgressed(CompletableController completableController)
        {
            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;
            var completableProgress = completableController.Progress;

            TrackerAsset.Instance.Completable.Progressed(completableId, completableType, completableProgress);
        }


        public void TrackCompleted(CompletableController completableController, System.TimeSpan timeElapsed)
        {
            var completableId = completableController.GetCompletable().getId();
            var completableType = (CompletableTracker.Completable)completableController.GetCompletable().getType() - 1;
            var completableScore = completableController.Score;

            TrackerAsset.Instance.setVar("time", timeElapsed.TotalSeconds);
            TrackerAsset.Instance.Completable.Completed(completableId, completableType, true, completableScore);
        }

        public void RestartFinished()
        {
            foreach (var completableController in completableControllers)
            {
                if (completableController.Completed)
                {
                    completableController.Reset();
                }
            }
        }

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
