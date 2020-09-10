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

        public override void OnAfterGameLoad()
        {
            if (!IsEnabled)
            {
                Debug.Log("No study was set for Simva");
                return;
            }

            if (IsActive)
            {
                // No need to restart
                return;
            }

            AnalyticsExtension.Instance.AutoStart = false;

            //if(PlayerPrefs.GetString())
            Game.Instance.GameState.Data.getChapters()[0].getObjects<SimvaScene>().AddRange(new SimvaScene[]
            {
                new LoginScene(),
                new SurveyScene(),
                new EndScene()
            });
            Game.Instance.GameState.CurrentTarget = "Simva.Login";
        }

        public override void OnBeforeGameSave()
        {
        }

        public override bool OnGameFinished()
        {
            if (IsActive)
            {
                Activity activity = GetActivity(CurrentActivityId);
                string activityType = activity.Type;
                if (activityType.Equals("gameplay", StringComparison.InvariantCultureIgnoreCase) 
                    && activity.Details != null && activity.Details.ContainsKey("backup") && (bool)activity.Details["backup"])
                {
                    string traces = SimvaBridge.Load(((TrackerAssetSettings)TrackerAsset.Instance.Settings).BackupFile);
                    SaveActivityAndContinue(CurrentActivityId, traces, true);
                }
                else
                {
                    Continue(CurrentActivityId, true);
                }

                return false;
            }
            else
            {
                // The application can be closed
                return true;
            }
        }

        public override void OnGameReady()
        {
        }

        public override void Restart()
        {
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
                    LaunchActivity(schedule.Next);
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

        public IAsyncOperation<Schedule> UpdateSchedule()
        {
            var webRequest = simvaController.Api.GetSchedule(simvaController.SimvaConf.Study);
            webRequest.Then(result =>
            {
                this.schedule = result;
            });
            return webRequest;
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
                    LaunchActivity(schedule.Next);
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

        public void LaunchActivity(string activityId)
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
                    switch (activity.Type)
                    {
                        case "limesurvey":
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
                            }
                            Debug.Log("TrackingCode: " + activity.Id + " settings " + trackerConfig.getTrackingCode());

                            if (ActivityHasDetails(activity, "backup"))
                            {
                                // Local
                                trackerConfig.setRawCopy(true);
                            }
                            SimvaBridge = new SimvaBridge(API.ApiClient);
                            AnalyticsExtension.Instance.StartTracker(trackerConfig, SimvaBridge);


                            Game.Instance.RunTarget(Game.Instance.GameState.InitialChapterTarget.getId());
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

        public bool canBeInteracted()
        {
            return false;
        }

        public void setInteractuable(bool state)
        {
        }
    }
}
