using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uAdventure.Runner;
using Simva;
using UnityFx.Async.Promises;
using UnityEngine;
using UnityFx.Async;
using UnityEngine.Networking;

namespace uAdventure.Analytics
{
    public class Cmi5Extension : GameExtension
    {
        private Cmi5Helper.ICmi5LaunchConfig Config { get { return Cmi5Launcher.config; } }
        private Dictionary<string, object> LaunchData { get { return launchData; } }
        private Dictionary<string, object> AgentProfile { get { return agentProfile; } }


        private Dictionary<string, object> launchData,agentProfile;



        [Priority(15)]
        public override IEnumerator OnAfterGameLoad()
        {
            Debug.Log("[CMI-5] Starting...");

            if (Config != null)
            {
                Debug.Log("[CMI-5] Launch config detected...");

                TrackerConfig trackerConfig = null;
                var fetchDone = false;

                Debug.Log("[CMI-5] Fetching auth token...");
                Cmi5Helper.FetchAuthToken()
                    .Then(fetch =>
                    {
                        // Process errors
                        if (!string.IsNullOrEmpty(fetch.ErrorCode))
                        {
                            Debug.LogError("[CMI-5] Fetch error (" + fetch.ErrorCode + "): " + fetch.ErrorText);
                        }
                        else
                        {
                            Debug.Log("[CMI-5] Auth token obtained: " + fetch.AuthToken);

                            AnalyticsExtension.Instance.AutoStart = false;

                            trackerConfig = new TrackerConfig();
                            trackerConfig.setStorageType(TrackerConfig.StorageType.NET);
                            trackerConfig.setHost(Config.Endpoint);
                            trackerConfig.setBasePath("");
                            trackerConfig.setLoginEndpoint("/");
                            trackerConfig.setStartEndpoint("/");
                            trackerConfig.setTrackEndpoint("/");
                            trackerConfig.setTrackingCode(fetch.AuthToken);
                            trackerConfig.setUseBearerOnTrackEndpoint(true);
                        }

                        fetchDone = true;
                    })
                    .Catch(ex =>
                    {
                        var apiEx = (ApiException)ex;
                        // Process exceptions
                        Debug.LogError("[CMI-5] Fetch Http error (" + apiEx.ErrorCode + "): " + apiEx.ErrorContent);
                    });

                yield return new WaitUntil(() => fetchDone);

                if(trackerConfig != null)
                {
                    bool failed = false;

                    Debug.Log("[CMI-5] Fetching 'LMS.LaunchData' state document");
                    GetStateDocument("LMS.LaunchData")
                        .Then(launchData =>
                        {
                            Debug.Log("[CMI-5] Fetching 'LMS.LaunchData' done!");
                            this.launchData = launchData;
                        })
                        .Catch(ex =>
                        {
                            Debug.LogError("[CMI-5] Fetching 'LMS.LaunchData' failed: " + ex.ToString());
                            failed = true;
                        });

                    Debug.Log("[CMI-5] Fetching 'cmi5LearnerPreferences' Agent Profile document");
                    GetAgentProfileDocument("cmi5LearnerPreferences")
                        .Then(agentProfile =>
                        {
                            Debug.Log("[CMI-5] Fetching 'cmi5LearnerPreferences' done!");
                            this.agentProfile = agentProfile;
                        })
                        .Catch(ex =>
                        {
                            Debug.LogError("[CMI-5] Fetching 'cmi5LearnerPreferences' failed: " + ex.ToString());
                            failed = true;
                        });

                    yield return new WaitUntil(() => failed || (launchData != null && agentProfile != null));

                    // TODO Restore saved game

                    Debug.LogError("[CMI-5] Starting tracker...");

                    yield return StartCoroutine(AnalyticsExtension.Instance.StartTracker(trackerConfig, null));

                    // TODO 

                    /*
                     * CMI 5 estado según lo dejé:
                     *
                     * Basándome en esta web (https://aicc.github.io/CMI-5_Spec_Current/flows/au-flow.html), 
                     * la clase CMI-5 extensión está lista para llegar a "Send Initialized statement"
                     */
                }
            }
            else
            {
                yield return null;
            }
        }

        public override void OnBeforeGameSave()
        {
            throw new NotImplementedException();
        }

        public override IEnumerator OnGameFinished()
        {
            throw new NotImplementedException();
        }

        public override IEnumerator OnGameReady()
        {
            throw new NotImplementedException();
        }

        public override IEnumerator Restart()
        {
            throw new NotImplementedException();
        }

        // Util

        /// <summary>
        /// contextTemplate -> JSON
        /// launchMode -> "Normal", "Browse", or "Review"
        /// launchParameters -> Any string value
        /// masteryScore -> Decimal value between 0 and 1 (inclusive) with up to 4 decimal places of precision.
        /// moveOn -> string moveOn values as defined in the Course Structure (Section 13.1.4 – AU Metadata)
        /// returnURL -> Any URL. String (Not URL encoded)
        /// entitlementKey -> JSON Object
        /// courseStructure -> string
        /// alternate -> string
        /// </summary>
        /// <param name="stateid"></param>
        /// <returns></returns>
        public AsyncCompletionSource<Dictionary<string, object>> GetStateDocument(string stateid)
        {
            var promise = new AsyncCompletionSource<Dictionary<string, object>>();

            if (Config != null)
            {
                var url = Config.Endpoint + "activities/state?" +
                    "activityId=" + UnityWebRequest.EscapeURL(Config.ActivityId) +
                    "&agent=" + UnityWebRequest.EscapeURL(Config.Actor) +
                    "&stateId=" + UnityWebRequest.EscapeURL(stateid) +
                    "&registration=" + UnityWebRequest.EscapeURL(Config.Registration);

                RequestsUtil.DoRequest<Dictionary<string, object>>(UnityWebRequest.Get(url))
                    .Wrap(promise);
            }
            return promise;
        }

        /// <summary>
        /// languagePreference -> "en-US,fr-FR,fr-BE"
        /// audioPreference -> "on"
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public AsyncCompletionSource<Dictionary<string, object>> GetAgentProfileDocument(string profileId)
        {
            var promise = new AsyncCompletionSource<Dictionary<string, object>>();

            if (Config != null)
            {
                var url = Config.Endpoint + "agents/profile?" +
                    "agent=" + UnityWebRequest.EscapeURL(Config.Actor) +
                    "&profileId=" + UnityWebRequest.EscapeURL(profileId);

                RequestsUtil.DoRequest<Dictionary<string, object>>(UnityWebRequest.Get(url))
                    .Wrap(promise);
            }
            return promise;
        }
    }
}
