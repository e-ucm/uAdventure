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
using Xasu.Auth.Protocols.OAuth2;

namespace uAdventure.Simva
{
    public class SimvaExtension : GameExtension, Interactuable
    {
        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;
        private string savedGameTarget;
        private bool wasAutoSave;
        private bool firstTimeDisabling = true;

        private OAuth2Token auth;

        public Schedule Schedule { get; private set; }

        public SimvaApi<IStudentsApi> API { get; private set; }

        public bool IsActive
        {
            get
            {
                return this.API != null && this.auth != null && Schedule != null;
            }
        }

        public string CurrentActivityId
        {
            get
            {
                if (Schedule != null)
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
                Debug.Log("[SIMVA] Disabling tracker autostart...");
                GetInstance<AnalyticsExtension>().AutoStart = false;

                //if(PlayerPrefs.GetString())
                Debug.Log("[SIMVA] Adding scenes...");
                Game.Instance.GameState.Data.getChapters()[0].getObjects<SimvaScene>().AddRange(new SimvaScene[]
                {
                    new LoginScene(),
                    new SurveyScene(),
                    new FinalizeScene(),
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

        private bool afterFinalize;
        public void AfterFinalize()
        {
            afterFinalize = true;
        }


        [Priority(10)]
        public override IEnumerator OnGameFinished()
        {
            if (IsActive)
            {
                var readyToClose = false;
                DisableAutoSave();
                Game.Instance.RunTarget("Simva.Finalize", null, false);
                yield return new WaitUntil(() => afterFinalize);
                Continue(CurrentActivityId, true)
                    .Then(() => readyToClose = true);

                yield return new WaitUntil(() => readyToClose);
            }
            else
            {
                yield return GetInstance<AnalyticsExtension>().OnGameFinished();
            }
        }

        public override IEnumerator OnGameReady()
        {
            if (PlayerPrefs.HasKey("simva_auth"))
            {
                NotifyLoading(true);
                this.auth = JsonConvert.DeserializeObject<OAuth2Token>(PlayerPrefs.GetString("simva_auth"));
                this.auth.ClientId = "uadventure";
                SimvaApi<IStudentsApi>.Login(this.auth.RefreshToken)
                    .Then(simvaController =>
                    {
                        this.API = simvaController;
                        this.API.Authorization.RegisterAuthInfoUpdate(auth => this.auth = auth);
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
            SimvaApi<IStudentsApi>.Login()
                .Then(simvaController =>
                {
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(auth => this.auth = auth);
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

        public void LoginAndSchedule(string token)
        {
            NotifyLoading(true);
            SimvaApi<IStudentsApi>.LoginWithToken(token)
                .Then(simvaController =>
                {
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(auth => this.auth = auth);
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
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(auth => this.auth = auth);
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

        public IAsyncOperation<Schedule> UpdateSchedule()
        {
            var result = new AsyncCompletionSource<Schedule>(); 
                
            API.Api.GetSchedule(API.SimvaConf.Study)
                .Then(schedule =>
                {
                    this.Schedule = schedule;
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

            var response = (AsyncCompletionSource) API.Api.SetResult(activityId, API.Authorization.Agent.name, body);
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
            return API.Api.SetCompletion(activityId, API.Authorization.Agent.name, completed)
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
            if (Schedule != null)
            {
                return Schedule.Activities[activityId];
            }
            return null;
        }

        public IEnumerator LaunchActivity(string activityId)
        {
            if (activityId == null)
            {
                DisableAutoSave();
                Game.Instance.RunTarget("Simva.End", null, false);
                Schedule = null;
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
                            trackerConfig.setDebug(true);
                            trackerConfig.setRawCopy(true);

                            if (ActivityHasDetails(activity, "realtime", "trace_storage"))
                            {
                                // Realtime
                                trackerConfig.setStorageType(TrackerConfig.StorageType.NET);
                                trackerConfig.setHost(API.SimvaConf.URL);
                                trackerConfig.setBasePath("");
                                trackerConfig.setLoginEndpoint("/users/login");
                                trackerConfig.setStartEndpoint(string.Format("/activities/{0}", activityId));
                                trackerConfig.setTrackEndpoint(string.Format("/activities/{0}", activityId));
                                trackerConfig.setTrackingCode(activityId);
                                trackerConfig.setUseBearerOnTrackEndpoint(true);
                                Debug.Log("TrackingCode: " + activity.Id + " settings " + trackerConfig.getTrackingCode());
                            }

                            if (ActivityHasDetails(activity, "backup"))
                            {
                                // Backup
                                trackerConfig.setRawCopy(true);
                                trackerConfig.setBackupEndpoint(string.Format("/activities/{0}/result", activityId));
                                trackerConfig.setBackupFileName(auth.Username + "_" + activityId + "_backup.log");
                            }

                            if (ActivityHasDetails(activity, "realtime", "trace_storage", "backup"))
                            {
                                Debug.Log("[SIMVA] Starting tracker...");
                                yield return StartCoroutine(GetInstance<AnalyticsExtension>().StartTracker(trackerConfig, API.Authorization, API.Authorization));
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
            return false;// OpenIdUtility.HasLoginInfo();
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
