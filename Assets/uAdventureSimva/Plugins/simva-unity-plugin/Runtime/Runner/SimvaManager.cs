using System;
using UnityEngine;
using UnityFx.Async;
using UnityFx.Async.Promises;
using SimvaPlugin;
using Simva.Api;
using Simva.Model;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using Xasu.Auth.Protocols.OAuth2;

namespace Simva
{
    public class SimvaManager : MonoBehaviour
    {
        private static SimvaManager instance;

        public static SimvaManager Instance 
        { 
            get 
            { 
                if(instance == null)
                {
                    instance = new GameObject("SimvaManager", typeof(SimvaManager)).GetComponent<SimvaManager>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance; 
            } 
        }

        public delegate void LoadingDelegate(bool loading);
        public delegate void ResponseDelegate(string message);

        private LoadingDelegate loadingListeners;
        private ResponseDelegate responseListeners;
        private OAuth2Token auth;
        public ISimvaBridge Bridge { get; set; }

        public bool Finalized { get; protected set; }

        public Schedule Schedule { get; private set; }

        public SimvaApi<IStudentsApi> API { get; private set; }

        public bool IsActive
        {
            get
            {
                return this.API != null && this.auth != null && Schedule != null && Bridge != null;
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

        protected SimvaManager()
        {

        }

        public void AfterFinalize()
        {
            Finalized = true;
        }


        public IAsyncOperation InitUser()
        {
            return LoginAndSchedule();
        }

        public void Demo()
        {
            Bridge.Demo();
        }

        public IAsyncOperation LoginWithRefreshToken(string refreshToken)
        {
            NotifyLoading(true);
            return SimvaApi<IStudentsApi>.Login(refreshToken)
                    .Then(simvaController =>
                    {
                        this.API = simvaController;
                        this.API.Authorization.RegisterAuthInfoUpdate(OnAuthInfoUpdate);
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

        public IAsyncOperation LoginAndSchedule()
        {
            NotifyLoading(true);
            return SimvaApi<IStudentsApi>.Login()
                .Then(simvaController =>
                {
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(OnAuthInfoUpdate);
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

        public IAsyncOperation LoginAndSchedule(string token)
        {
            NotifyLoading(true);
            return SimvaApi<IStudentsApi>.LoginWithToken(token)
                .Then(simvaController =>
                {
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(OnAuthInfoUpdate);
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

        public IAsyncOperation ContinueLoginAndSchedule()
        {
            NotifyLoading(true);
            return SimvaApi<IStudentsApi>.ContinueLogin()
                .Then(simvaController =>
                {
                    this.API = simvaController;
                    this.API.Authorization.RegisterAuthInfoUpdate(OnAuthInfoUpdate);
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



        private IAsyncOperation<Schedule> UpdateSchedule()
        {
            var result = new AsyncCompletionSource<Schedule>();

            API.Api.GetSchedule(API.SimvaConf.Study)
                .Then(schedule =>
                {
                    this.Schedule = schedule;
                    foreach (var a in schedule.Activities)
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

            var response = (AsyncCompletionSource)API.Api.SetResult(activityId, API.Authorization.Agent.name, body);
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

        public IAsyncOperation ContinueSurvey()
        {
            NotifyLoading(true);
            return API.Api.GetCompletion(CurrentActivityId, API.Authorization.Agent.name)
                .Then(result =>
                {
                    if (result[API.Authorization.Agent.name])
                    {
                        return UpdateSchedule();
                    }
                    else
                    {
                        NotifyManagers("Survey not completed");
                        NotifyLoading(false);
                        var nullresult = new AsyncCompletionSource<Schedule>();
                        nullresult.SetResult(null);
                        return nullresult;
                    }
                })
                .Then(schedule =>
                {
                    var result = new AsyncCompletionSource();
                    if (schedule != null)
                    {
                        StartCoroutine(AsyncCoroutine(LaunchActivity(schedule.Next), result));
                    }
                    else
                    {
                        result.SetException(new Exception("No schedule!"));
                    }
                    return result;
                })
                .Catch(error =>
                {
                    NotifyManagers(error.Message);
                    NotifyLoading(false);
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


        private void OnAuthInfoUpdate(OAuth2Token token)
        {
            this.auth = token;
            Bridge.OnAuthUpdated(token);
        }


        private IEnumerator LaunchActivity(string activityId)
        {
            if (activityId == null)
            {
                Bridge.RunScene("Simva.End");
                PlayerPrefs.DeleteKey("simva_auth");
                Schedule = null;
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
                            Bridge.RunScene("Simva.Survey");
                            break;
                        case "gameplay":
                        default:
                            var xasuTrackerConfig = new Xasu.Config.TrackerConfig();

                            xasuTrackerConfig.Simva = true;
                            xasuTrackerConfig.Offline = true;
                            xasuTrackerConfig.TraceFormat = Xasu.Config.TraceFormats.XAPI;

                            xasuTrackerConfig.FlushInterval = 3;
                            xasuTrackerConfig.BatchSize = 256;

                            if (ActivityHasDetails(activity, "realtime", "trace_storage"))
                            {
                                xasuTrackerConfig.Online = true;
                                xasuTrackerConfig.Fallback = true;
                                xasuTrackerConfig.LRSEndpoint = API.SimvaConf.URL + string.Format("/activities/{0}", activityId);
                            }

                            if (ActivityHasDetails(activity, "backup"))
                            {
                                // Backup
                                xasuTrackerConfig.Backup = true;
                                xasuTrackerConfig.BackupEndpoint = API.SimvaConf.URL + string.Format("/activities/{0}/result", activityId);
                                xasuTrackerConfig.BackupFileName = auth.Username + "_" + activityId + "_backup.log";
                            }

                            if (ActivityHasDetails(activity, "realtime", "trace_storage", "backup"))
                            {
                                Debug.Log("[SIMVA] Starting tracker...");
                                var trackerStarted = false;
                                Bridge.StartTracker(xasuTrackerConfig, API.Authorization, API.Authorization)
                                    .Then(() => trackerStarted = true);

                                yield return new WaitUntil(() => trackerStarted);
                            }

                            Debug.Log("[SIMVA] Starting Gameplay...");
                            Bridge.StartGameplay();
                            break;
                    }
                }
            }
        }

        public IAsyncOperation OnGameFinished()
        {
            var result = new AsyncCompletionSource();
            try
            {
                if (IsActive)
                {
                    Bridge.RunScene("Simva.Finalize");
                    StartCoroutine(OnGameFinishedRoutine(() => result.SetCompleted()));
                }
                else
                {
                    result.SetCompleted();
                }
            }
            catch(Exception ex)
            {
                result.SetException(ex);
            }
            return result;
        }

        private IEnumerator OnGameFinishedRoutine(System.Action done)
        {
            var readyToClose = false;
            SimvaManager.Instance.Finalized = false;
            yield return new WaitUntil(() => SimvaManager.Instance.Finalized);
            Continue(SimvaManager.Instance.CurrentActivityId, true)
                .Then(() => readyToClose = true);
            yield return new WaitUntil(() => readyToClose);
            done();
        }

        #region Private

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

        internal IEnumerator AsyncCoroutine(IEnumerator coroutine, IAsyncCompletionSource op)
        {
            yield return coroutine;
            op.SetCompleted();
        }

        #endregion

        #region Notifiers
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

        #endregion
    }
}
