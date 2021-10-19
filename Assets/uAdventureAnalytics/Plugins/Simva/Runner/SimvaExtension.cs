using AssetPackage;
using SimpleJSON;
using System;
using System.IO;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityFx.Async;
using UnityFx.Async.Promises;
using uAdventure.Analytics;
using SimvaPlugin;
using Simva.Api;
using Simva.Model;
using Simva;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using uAdventure.Core;

namespace uAdventure.Simva
{
    public class SimvaExtension : GameExtension, Interactuable
    {
        private static SimvaExtension instance;

        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;
        private string savedGameTarget;
        private bool wasAutoSave;
        private bool firstTimeDisabling = true;
        private FlushAllScene flushAllScene;

        private AuthorizationInfo auth;
        private Schedule schedule;
        private SimvaApi<IStudentsApi> simvaController;

        protected void Awake()
        {
            instance = this;
        }

        public static SimvaExtension Instance
        {
            get
            {
                return instance;
            }
        }

        public Schedule Schedule
        {
            get
            {
                return schedule;
            }
        }

        public SimvaApi<IStudentsApi> API
        {
            get
            {
                return simvaController;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.simvaController != null && this.auth != null && Schedule != null;
            }
        }

        public string CurrentActivityId
        {
            get
            {
                if (schedule != null)
                {
                    return Schedule.Next;
                }
                return null;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(SimvaConf.Local.Study);
            }
        }

        public SimvaBridge SimvaBridge { get; private set; }

        [Priority(10)]
        public override IEnumerator OnAfterGameLoad()
        {

            Debug.Log("[SIMVA] Starting...");
            if(SimvaConf.Local == null)
            {
                SimvaConf.Local = new SimvaConf();
                yield return StartCoroutine(SimvaConf.Local.LoadAsync());
                Debug.Log("[SIMVA] Conf Loaded...");
            }

            if (Cmi5Launcher.config != null)
            {
                Debug.Log("[SIMVA] CMI-5 config detected. Stopping...");
                yield return null;
            }
            else if (!IsEnabled)
            {
                Debug.Log("[SIMVA] Study is not set! Stopping...");
                yield return null;
            }
            else if (IsActive)
            {
                Debug.Log("[SIMVA] Simva is already started...");
                // No need to restart
                yield return null;
            }
            else
            {
                Debug.Log("[SIMVA] Disabling tracker autostart...");
                AnalyticsExtension.Instance.AutoStart = false;

                //if(PlayerPrefs.GetString())
                Debug.Log("[SIMVA] Adding scenes...");
                Game.Instance.GameState.Data.getChapters()[0].getObjects<SimvaScene>().AddRange(new SimvaScene[]
                {
                    new LoginScene(),
                    new SurveyScene(),
                    flushAllScene = new FlushAllScene(),
                    new BackupScene(),
                    new EndScene()
                });
                Debug.Log("[SIMVA] Setting current target to Simva.Login...");
                DisableAutoSave();
                savedGameTarget = Game.Instance.GameState.CurrentTarget;
                Game.Instance.GameState.CurrentTarget = "Simva.Login";
            }
        }

        private void DisableAutoSave()
        {
            if (firstTimeDisabling)
            {
                wasAutoSave = Game.Instance.GameState.Data.isAutoSave();
                firstTimeDisabling = false;
            }
            Game.Instance.GameState.Data.setAutoSave(false);
        }

        private void RestoreAutoSave()
        {
            firstTimeDisabling = true;
            Game.Instance.GameState.Data.setAutoSave(wasAutoSave);
        }

        public override void OnBeforeGameSave()
        {
            if(auth != null)
            {
                PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
            }
        }

        public IAsyncOperation backupOperation;
        public Activity backupActivity;

        private bool afterFlush;
        public void AfterFlush()
        {
            afterFlush = true;
        }

        private bool afterBackup;
        public void AfterBackup()
        {
            afterBackup = true;
        }


        [Priority(10)]
        public override IEnumerator OnGameFinished()
        {
            yield return new WaitWhile(() => Game.Instance.isSomethingRunning());
            if (IsActive)
            {
                var readyToClose = false;
                DisableAutoSave();
                Game.Instance.RunTarget("Simva.FlushAll", null, false);
                yield return new WaitUntil(() => afterFlush);
                Continue(CurrentActivityId, true)
                    .Then(() => readyToClose = true);

                yield return new WaitUntil(() => readyToClose);
            }
            else
            {
                yield return AnalyticsExtension.Instance.OnGameFinished();
            }
        }

        public void OnGameCompleted()
        {
            StartCoroutine(OnGameCompletedRoutine());
        }

        private IEnumerator OnGameCompletedRoutine()
        {
            yield return new WaitWhile(() => Game.Instance.isSomethingRunning());

            if (IsActive)
            {
                DisableAutoSave();
                flushAllScene.onlyFlushAndBackup = true;
                Game.Instance.RunTarget("Simva.FlushAll", null, false);
                yield return new WaitUntil(() => afterFlush);
                flushAllScene.onlyFlushAndBackup = false;
                StartBackupIfNeeded();

                UpdateSchedule()
                    .Then(schedule =>
                    {
                        var result = new AsyncCompletionSource();
                        StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                        return result;
                    })
                    .Finally(() =>
                    {
                        NotifyLoading(false);
                    })
                    .Catch(error =>
                    {
                        NotifyLoading(false);
                        NotifyManagers(error.Message);
                    });
            }
        }

        public override IEnumerator OnGameReady()
        {
            if (PlayerPrefs.HasKey("simva_auth"))
            {
                NotifyLoading(true);
                this.auth = JsonConvert.DeserializeObject<AuthorizationInfo>(PlayerPrefs.GetString("simva_auth"));
                this.auth.ClientId = "uadventure";
                SimvaApi<IStudentsApi>.Login(this.auth)
                    .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(() =>
                {
                    OpenIdUtility.tokenLogin = false;
                });

            }
            else if (HasLoginInfo())
            {
                ContinueLoginAndSchedule();
            }
            yield return null;
        }

        public override IEnumerator Restart()
        {
            yield return null;
        }

        public void InitUser()
        {
            LoginAndSchedule();
        }

        public void LoginAndSchedule()
        {
            NotifyLoading(true);
            OpenIdUtility.tokenLogin = true;
            SimvaApi<IStudentsApi>.Login()
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(()=>
                {
                    OpenIdUtility.tokenLogin = false;
                });
        }

        public void LoginAndSchedule(string token)
        {
            NotifyLoading(true);
            SimvaApi<IStudentsApi>.LoginWithToken(token)
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
                    PlayerPrefs.Save();
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                });
        }

        public void ContinueLoginAndSchedule()
        {
            NotifyLoading(true);
            SimvaApi<IStudentsApi>.ContinueLogin()
                .Then(simvaController =>
                {
                    this.auth = simvaController.AuthorizationInfo;
                    this.simvaController = simvaController;
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                })
                .Finally(() =>
                {
                    OpenIdUtility.tokenLogin = false;
                });
        }

        public IAsyncOperation<Schedule> UpdateSchedule()
        {
            var result = new AsyncCompletionSource<Schedule>(); 
                
            simvaController.Api.GetSchedule(simvaController.SimvaConf.Study)
                .Then(schedule =>
                {
                    this.schedule = schedule;
                    foreach(var a in schedule.Activities)
                    {
                        schedule.Activities[a.Key].Id = a.Key;
                    }
                    Debug.Log("[SIMVA] Schedule: " + JsonConvert.SerializeObject(schedule));
                    result.SetResult(schedule);
                })
                .Catch(result.SetException);
            return result;
        }


        public IAsyncOperation SaveActivity(string activityId, string traces, bool completed)
        {
            NotifyLoading(true);

            var body = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(traces))
            {
                body.Add("tofile", true);
                body.Add("result", traces);
            }

            var result = new AsyncCompletionSource();

            var response = (AsyncCompletionSource) API.Api.SetResult(activityId, API.AuthorizationInfo.Username, body);
            response.AddProgressCallback((p) =>
             {
                 UnityEngine.Debug.Log("SaveActivityAndContinue progress: " + p);
                 if (!result.IsCompleted && !result.IsCanceled)
                 {
                     result.SetProgress(p);
                 }
             });

            response
                .Then(() =>
                {
                    NotifyLoading(false);
                    result.SetCompleted();
                })
                .Catch(e => {
                    result.SetException(e);
                });

            return result;
        }

        public IAsyncOperation Continue(string activityId, bool completed)
        {
            NotifyLoading(true);
            return API.Api.SetCompletion(activityId, API.AuthorizationInfo.Username, completed)
                .Then(() =>
                {
                    backupActivity = GetActivity(CurrentActivityId);
                    string activityType = backupActivity.Type;
                    if (activityType.Equals("gameplay", StringComparison.InvariantCultureIgnoreCase)
                    && backupActivity.Details != null && backupActivity.Details.ContainsKey("backup") && (bool)backupActivity.Details["backup"])
                    {
                        string traces = SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
                        Instantiate(Resources.Load("SimvaBackupPopup"));
                        backupOperation = SaveActivity(CurrentActivityId, traces, true);
                        backupOperation.Then(() =>
                        {
                            afterBackup = true;
                        });
                    }

                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    return result;
                })
                .Finally(() =>
                {
                    NotifyLoading(false);
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                });
        }

        public Activity GetActivity(string activityId)
        {
            if (schedule != null)
            {
                return Schedule.Activities[activityId];
            }
            return null;
        }

        public IEnumerator LaunchActivity(string activityId)
        {
            if (activityId == null)
            {
                if (backupOperation != null && !backupOperation.IsCompletedSuccessfully)
                {
                    Game.Instance.AbortQuit();
                    DisableAutoSave();
                    Game.Instance.RunTarget("Simva.Backup", null, false);
                }
                else
                {
                    DisableAutoSave();
                    Game.Instance.RunTarget("Simva.End", null, false);
                    schedule = null;
                }
            }
            else
            {
                Activity activity = GetActivity(activityId);

                if (activity != null)
                {
                    Game.Instance.AbortQuit();
                    Debug.Log("[SIMVA] Schedule: " + activity.Type + ". Name: " + activity.Name + " activityId " + activityId);
                    switch (activity.Type)
                    {
                        case "limesurvey":
                            Debug.Log("[SIMVA] Starting Survey...");
                            DisableAutoSave();
                            Game.Instance.RunTarget("Simva.Survey", null, false);
                            break;
                        case "gameplay":
                        default:
                            var trackerConfig = new TrackerConfig();

                            trackerConfig.setStorageType(TrackerConfig.StorageType.LOCAL);
                            trackerConfig.setTraceFormat(TrackerConfig.TraceFormat.XAPI);
                            trackerConfig.setRawCopy(true);
                            trackerConfig.setDebug(true);

                            if (ActivityHasDetails(activity, "realtime", "trace_storage"))
                            {
                                // Realtime
                                trackerConfig.setStorageType(TrackerConfig.StorageType.NET);
                                trackerConfig.setHost(simvaController.SimvaConf.URL);
                                trackerConfig.setBasePath("");
                                trackerConfig.setLoginEndpoint("/users/login");
                                trackerConfig.setStartEndpoint("/activities/{0}/result");
                                trackerConfig.setTrackEndpoint("/activities/{0}/result");
                                trackerConfig.setTrackingCode(activityId);
                                trackerConfig.setUseBearerOnTrackEndpoint(true);
                                Debug.Log("TrackingCode: " + activity.Id + " settings " + trackerConfig.getTrackingCode());
                            }

                            if (ActivityHasDetails(activity, "backup"))
                            {
                                // Local
                                trackerConfig.setRawCopy(true);
                            }

                            if (ActivityHasDetails(activity, "realtime", "trace_storage", "backup"))
                            {
                                SimvaBridge = new SimvaBridge(API.ApiClient);
                                Debug.Log("[SIMVA] Starting tracker...");
                                yield return StartCoroutine(AnalyticsExtension.Instance.StartTracker(trackerConfig, auth.Username + "_" + activityId + "_backup.log", SimvaBridge));
                            }

                            Debug.Log("[SIMVA] Starting Gameplay...");
                            RestoreAutoSave();
                            Game.Instance.RunTarget(savedGameTarget, this);
                            if(Game.Instance.GameState.CheckFlag("DisclaimerEnabled") == FlagCondition.FLAG_ACTIVE)
                            {
                                Game.Instance.GameState.SetFlag("SeeingDisclaimer", FlagCondition.FLAG_ACTIVE);
                            }
                            break;
                    }
                }
            }
        }

        private bool ActivityHasDetails(Activity activity, params string[] details)
        {
            if (activity.Details == null)
            {
                return false;
            }

            return details.Any(d => IsTrue(activity.Details, d));
        }

        private static bool IsTrue(Dictionary<string, object> details, string key)
        {
            return details.ContainsKey(key) && details[key] is bool && (bool)details[key];
        }

        // NOTIFIERS

        public void AddResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                responseListeners -= manager.Notify;
                // Then we add it
                responseListeners += manager.Notify;
            }
        }

        public void RemoveResponseManager(SimvaResponseManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                responseListeners -= manager.Notify;
            }
        }

        public void AddLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // To make sure we only have one instance of a notify per manager
                // We first remove (as it is ignored if not present)
                loadingListeners -= manager.IsLoading;
                // Then we add it
                loadingListeners += manager.IsLoading;
            }
        }

        public void RemoveLoadingManager(SimvaLoadingManager manager)
        {
            if (manager)
            {
                // If a delegate is not present the method gets ignored
                loadingListeners -= manager.IsLoading;
            }
        }

        public void NotifyManagers(string message)
        {
            responseListeners?.Invoke(message);
        }

        public void NotifyLoading(bool state)
        {
            loadingListeners?.Invoke(state);
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            return InteractuableResult.IGNORES;
        }

        private static bool HasLoginInfo()
        {
            return OpenIdUtility.HasLoginInfo();
        }

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }

        internal IEnumerator AsyncCoroutine(IEnumerator coroutine, IAsyncCompletionSource op)
        {
            yield return coroutine;
            op.SetCompleted();
        }
    }
}
