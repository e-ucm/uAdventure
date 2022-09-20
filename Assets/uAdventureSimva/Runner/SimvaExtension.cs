using System;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityFx.Async.Promises;
using uAdventure.Analytics;
using Simva;
using System.Collections;
using Newtonsoft.Json;
using Xasu.Auth.Protocols.OAuth2;
using Xasu.Auth.Protocols;
using UnityFx.Async;

namespace uAdventure.Simva
{
    public class SimvaExtension : GameExtension, Interactuable, ISimvaBridge
    {
        private string savedGameTarget;
        private bool wasAutoSave;
        private bool firstTimeDisabling = true;
        private OAuth2Token auth;

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

            if (!SimvaManager.Instance.IsEnabled)
            {
                Debug.Log("[SIMVA] Study is not set! Stopping...");
                yield return null;
            }
            else if (SimvaManager.Instance.IsActive)
            {
                Debug.Log("[SIMVA] Simva is already started...");
                // No need to restart
                yield return null;
            }
            else
            {
                SimvaManager.Instance.Bridge = this;
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


        [Priority(10)]
        public override IEnumerator OnGameFinished()
        {
            yield return new WaitWhile(() => Game.Instance.isSomethingRunning());
            if (SimvaManager.Instance.IsActive)
            {
                var readyToClose = false;
                SimvaManager.Instance.OnGameFinished()
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
                var auth = JsonConvert.DeserializeObject<OAuth2Token>(PlayerPrefs.GetString("simva_auth"));
                auth.ClientId = "uadventure";
                SimvaManager.Instance.LoginWithRefreshToken(auth.RefreshToken);
            }
            else if (HasLoginInfo())
            {
                SimvaManager.Instance.ContinueLoginAndSchedule();
            }
            yield return null;
        }

        public override IEnumerator Restart()
        {
            yield return null;
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

        public void StartGameplay()
        {
            Game.Instance.AbortQuit();
            RunScene(savedGameTarget);
        }

        public void RunScene(string name)
        {
            Game.Instance.AbortQuit();
            switch (name)
            {
                case "Simva.Login":
                case "Simva.Survey":
                case "Simva.Finalize":
                case "Simva.End":
                    DisableAutoSave();
                    Game.Instance.RunTarget(name, null, false);
                    break;
                default:
                    RestoreAutoSave();
                    Game.Instance.RunTarget(name, null);
                    break;
            }
        }

        public IAsyncOperation StartTracker(Xasu.Config.TrackerConfig config, IAuthProtocol onlineProtocol, IAuthProtocol backupProtocol)
        {
            var result = new AsyncCompletionSource();
            StartCoroutine(StartTrackerRoutine(config, onlineProtocol, backupProtocol, () => result.SetCompleted()));
            return result;
        }

        private IEnumerator StartTrackerRoutine(Xasu.Config.TrackerConfig config, IAuthProtocol onlineProtocol, IAuthProtocol backupProtocol, Action done)
        {
            yield return StartCoroutine(GetInstance<AnalyticsExtension>().StartTracker(config, onlineProtocol, backupProtocol));
            done();
        }

        public void OnAuthUpdated(OAuth2Token token)
        {
            auth = token;
        }

        public void Demo()
        {
            PreviewManager.Instance.InPreviewMode = true;
            Game.Instance.Restart();
            Game.Instance.GameState.Data.setAutoSave(false);
            Game.Instance.GameState.Data.setSaveOnSuspend(false);
            Game.Instance.RunTarget(Game.Instance.GameState.InitialChapterTarget.getId());
        }
    }
}
