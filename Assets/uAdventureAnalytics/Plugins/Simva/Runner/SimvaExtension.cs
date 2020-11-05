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

namespace uAdventure.Simva
{
    public class SimvaExtension : GameExtension, Interactuable
    {
        private static SimvaExtension instance;

        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;

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

        public string Token
        {
            get
            {
                if (auth != null)
                {
                    return auth.RefreshToken;
                }
                else if (PlayerPrefs.HasKey("username"))
                {
                    return PlayerPrefs.GetString("username");
                }
                else if (PlayerPrefs.HasKey("Simva.Token"))
                {
                    return PlayerPrefs.GetString("Simva.Token");
                }
                else
                {
                    return "";
                }
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

        public override IEnumerator OnAfterGameLoad()
        {
            Debug.Log("[SIMVA] Disabling tracker autostart...");
            AnalyticsExtension.Instance.AutoStart = false;

            Debug.Log("[SIMVA] Starting...");
            if(SimvaConf.Local == null)
            {
                SimvaConf.Local = new SimvaConf();
                yield return StartCoroutine(SimvaConf.Local.LoadAsync());
                Debug.Log("[SIMVA] Conf Loaded...");
            }

            if (!IsEnabled)
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

                //if(PlayerPrefs.GetString())
                Debug.Log("[SIMVA] Adding scenes...");
                Game.Instance.GameState.Data.getChapters()[0].getObjects<SimvaScene>().AddRange(new SimvaScene[]
                {
                new LoginScene(),
                new SurveyScene(),
                new EndScene()
                });
                Debug.Log("[SIMVA] Setting current target to Simva.Login...");
                Game.Instance.GameState.CurrentTarget = "Simva.Login";
            }
        }

        public override IEnumerator OnBeforeGameSave()
        {
            yield return null;
        }

        public override IEnumerator OnGameFinished()
        {
            if (IsActive)
            {
                Activity activity = GetActivity(CurrentActivityId);
                string activityType = activity.Type;
                var readyToClose = false;
                if (activityType.Equals("gameplay", StringComparison.InvariantCultureIgnoreCase) 
                    && activity.Details != null && activity.Details.ContainsKey("backup") && (bool)activity.Details["backup"])
                {
                    string traces = SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
                    SaveActivityAndContinue(CurrentActivityId, traces, true)
                        .Then(() => readyToClose = true);
                }
                else
                {
                    Continue(CurrentActivityId, true)
                        .Then(() => readyToClose = true);
                }

                yield return new WaitUntil(() => readyToClose);
            }
        }

        public override IEnumerator OnGameReady()
        {
            if (HasLoginInfo())
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


        public IAsyncOperation SaveActivityAndContinue(string activityId, string traces, bool completed)
        {
            NotifyLoading(true);

            var body = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(traces))
            {
                body.Add("tofile", true);
                body.Add("result", traces);
            }

            return API.Api.SetResult(activityId, API.AuthorizationInfo.Username, body)
                .Then(() =>
                {
                    NotifyLoading(false);
                    return Continue(activityId, completed);
                })
                .Catch(error =>
                {
                    NotifyLoading(false);
                    NotifyManagers(error.Message);
                });
        }

        public IAsyncOperation Continue(string activityId, bool completed)
        {
            NotifyLoading(true);
            return API.Api.SetCompletion(activityId, API.AuthorizationInfo.Username, completed)
                .Then(() =>
                {
                    return UpdateSchedule();
                })
                .Then(schedule =>
                {
                    NotifyLoading(false);
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
                Game.Instance.RunTarget("Simva.End", null, false);
            }
            else
            {
                Activity activity = GetActivity(activityId);

                if (activity != null)
                {
                    Debug.Log("[SIMVA] Schedule: " + activity.Type + ". Name: " + activity.Name + " activityId " + activityId);
                    switch (activity.Type)
                    {
                        case "limesurvey":
                            Debug.Log("[SIMVA] Starting Survey...");
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

                            if(ActivityHasDetails(activity, "realtime", "trace_storage", "backup"))
                            {
                                SimvaBridge = new SimvaBridge(API.ApiClient);
                                Debug.Log("[SIMVA] Starting tracker...");
                                yield return StartCoroutine(AnalyticsExtension.Instance.StartTracker(trackerConfig, SimvaBridge));
                            }

                            Debug.Log("[SIMVA] Starting Gameplay...");
                            Game.Instance.RunTarget(Game.Instance.GameState.InitialChapterTarget.getId(), this);
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
            if (responseListeners != null)
            {
                responseListeners(message);
            }
        }

        public void NotifyLoading(bool state)
        {
            if (loadingListeners != null)
            {
                loadingListeners(state);
            }
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            // Depending on the target that is ready
            var target = Game.Instance.GameState.CurrentTarget;

            switch (target)
            {
                case "Simva.Login":
                    if (!string.IsNullOrEmpty(Token))
                    {
                        LoginAndSchedule();
                    }
                    break;
                case "Simva.Survey":

                    break;
                case "Simva.End":

                    break;
            }

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
