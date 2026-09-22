using System;
using uAdventure.Runner;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityFx.Async.Promises;
using uAdventure.Analytics;
using Simva;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xasu;
using Xasu.Auth.Protocols.OAuth2;
using Xasu.Auth.Protocols;
using Xasu.Config;
using Xasu.Requests;
using UnityFx.Async;

namespace uAdventure.Simva
{
    public class SimvaExtension : GameExtension, Interactuable, ISimvaBridge
    {
        private string savedGameTarget;
        private bool wasAutoSave;
        private bool firstTimeDisabling = true;
        private OAuth2Token auth;
        private bool hasStartedGameplay;

        private global::Simva.SimvaPlugin Plugin => GetHiddenPlugin();

        public IHttpRequestHandler RequestHandler
        {
            get
            {
                var plugin = Plugin;
                if (plugin == null)
                {
                    return new UnityRequestHandler();
                }
                return plugin.RequestHandler ?? (plugin.RequestHandler = new UnityRequestHandler());
            }
            set
            {
                var plugin = Plugin;
                if (plugin != null)
                {
                    plugin.RequestHandler = value;
                }
            }
        }

        public void ApplySettings(SimvaPluginSettings settings)
        {
            if (settings == null)
            {
                return;
            }

            var plugin = Plugin;
            if (plugin == null)
            {
                return;
            }

            plugin.SaveAuthUntilCompleted = settings.SaveAuthUntilCompleted;
            plugin.RunGameIfSimvaIsNotConfigured = settings.RunGameIfSimvaIsNotConfigured;
            plugin.ContinueOnQuit = settings.ContinueOnQuit;
            plugin.EnableLoginDemoButton = settings.EnableLoginDemoButton;
            plugin.LanguageByDefault = settings.LanguageByDefault;
            plugin.SaveDisclaimerAccepted = settings.SaveDisclaimerAccepted;
            plugin.BasicScormXAPIDataManagementByGame = settings.BasicScormXAPIDataManagementByGame;
            plugin.EnableDebugLogging = settings.EnableDebugLogging;
        }

        [Priority(10)]
        public override IEnumerator OnAfterGameLoad()
        {
            SimvaManager.Instance.Bridge = this;
            var settings = Resources.Load<SimvaPluginSettings>("SimvaPluginSettings");
            if (settings != null)
            {
                ApplySettings(settings);
            }

            var plugin = Plugin;
            Log("[SIMVA] Starting...");
            if (SimvaConf.Local == null)
            {
                SimvaConf.Local = new SimvaConf();
                yield return StartCoroutine(SimvaConf.Local.LoadAsync());
                plugin = Plugin;
                Log("[SIMVA] Conf Loaded...");
            }

            if (PlayerPrefs.HasKey(global::Simva.SimvaPlugin.SIMVA_DISCLAIMER_ACCEPTED) && (plugin == null || !plugin.SaveDisclaimerAccepted))
            {
                PlayerPrefs.DeleteKey(global::Simva.SimvaPlugin.SIMVA_DISCLAIMER_ACCEPTED);
            }

            if (!SimvaManager.Instance.IsEnabled)
            {
                if (plugin == null || plugin.RunGameIfSimvaIsNotConfigured)
                {
                    Log("Simlet is not set! Running the game without Simva...");
                    SimvaManager.Instance.Bridge = this;
                    GetInstance<AnalyticsExtension>().AutoStart = false;
                    if (XasuTracker.Instance.Status.State == TrackerState.Uninitialized)
                    {
                        var config = new Xasu.Config.TrackerConfig
                        {
                            Offline = true,
                            TraceFormat = TraceFormats.XAPI,
                            FileName = "traces.log",
                            HomePage = "https://articoding/"
                        };
                        XasuTracker.Instance.Init(config, RequestHandler)
                            .ContinueWith(t => {
                                if (t.IsFaulted)
                                    LogWarning("Tracker fallback init failed: " + t.Exception);
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    savedGameTarget = Game.Instance.GameState.CurrentTarget;
                }
                else
                {
                    Log("Simlet is not set! Stopping...");
                    if (Application.isEditor)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
                    }
                    else
                    {
                        Application.Quit();
                    }
                }
                yield break;
            }
            else if (SimvaManager.Instance.IsActive)
            {
                Log("[SIMVA] Simva is already started...");
                yield return null;
            }
            else
            {
                SimvaManager.Instance.Bridge = this;
                Log("[SIMVA] Disabling tracker autostart...");
                GetInstance<AnalyticsExtension>().AutoStart = false;

                Log("[SIMVA] Adding scenes...");
                Game.Instance.GameState.Data.getChapters()[0].getObjects<SimvaScene>().AddRange(new SimvaScene[]
                {
                    new LoginScene(),
                    new SurveyScene(),
                    new FinalizeScene(),
                    new ManualScene(),
                    new DeviceLoginScene(),
                    new EndScene()
                });

                if (plugin != null && plugin.ShowLoginOnStartup && plugin.AutoStart)
                {
                    LoadLanguageDictionaries(plugin.LanguageByDefault);
                    string scene = "";
                    if (SimvaConf.Local != null &&
                        !string.IsNullOrEmpty(SimvaConf.Local.AuthProtocol) &&
                        SimvaConf.Local.AuthProtocol.Equals("device", StringComparison.OrdinalIgnoreCase))
                    {
                        scene = "Simva.Device";
                    }
                    else
                    {
                        scene = plugin.EnableLoginDemoButton ? "Simva.Login.Demo" : "Simva.Login";
                    }
                    Log("[SIMVA] Setting current target to " + scene);
                    DisableAutoSave();
                    savedGameTarget = Game.Instance.GameState.CurrentTarget;
                    Game.Instance.GameState.CurrentTarget = scene;
                }

                if (plugin != null && PlayerPrefs.HasKey("simva_auth") && plugin.SaveAuthUntilCompleted)
                {
                    var stored = JsonConvert.DeserializeObject<OAuth2Token>(PlayerPrefs.GetString("simva_auth"));
                    yield return new WaitForFixedUpdate();
                    stored.ClientId = "uadventure";
                    SimvaManager.Instance.LoginWithRefreshToken(stored.RefreshToken);
                }

                if (plugin == null || plugin.ContinueOnQuit)
                {
                    Application.wantsToQuit -= WantsToQuit;
                    Application.wantsToQuit += WantsToQuit;
                }
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
            var plugin = Plugin;
            if (auth != null && (plugin == null || plugin.SaveAuthUntilCompleted))
            {
                PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
            }
        }

        private void OnApplicationPause(bool paused)
        {
            var plugin = Plugin;
            if (paused && auth != null && (plugin == null || plugin.SaveAuthUntilCompleted))
            {
                PlayerPrefs.SetString("simva_auth", JsonConvert.SerializeObject(auth));
            }
        }

        public bool WantsToQuit()
        {
            if (SimvaManager.Instance.IsActive && hasStartedGameplay && !SimvaManager.Instance.Finalized)
            {
                SimvaManager.Instance.OnGameFinished();
                return false;
            }
            else
            {
                PlayerPrefs.DeleteKey("simva_auth");
            }
            return true;
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
            var plugin = Plugin;
            if (PlayerPrefs.HasKey("simva_auth") && (plugin == null || plugin.SaveAuthUntilCompleted))
            {
                var stored = JsonConvert.DeserializeObject<OAuth2Token>(PlayerPrefs.GetString("simva_auth"));
                stored.ClientId = "uadventure";
                SimvaManager.Instance.LoginWithRefreshToken(stored.RefreshToken);
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
            hasStartedGameplay = true;
            Game.Instance.AbortQuit();
            Log("Starting Gameplay");
            RunScene(savedGameTarget);
        }

        public void RunScene(string name)
        {
            Game.Instance.AbortQuit();
            Log("[SIMVA] Running scene: " + name);
            switch (name)
            {
                case "Simva.Login":
                case "Simva.Device":
                case "Simva.Login.Demo":
                case "Simva.Survey":
                case "Simva.Finalize":
                case "Simva.Manual":
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
            Log("Starting Tracker");
            var result = new AsyncCompletionSource();
            StartCoroutine(StartTrackerRoutine(config, onlineProtocol, backupProtocol, () => result.SetCompleted()));
            return result;
        }

        private IEnumerator StartTrackerRoutine(Xasu.Config.TrackerConfig config, IAuthProtocol onlineProtocol, IAuthProtocol backupProtocol, Action done)
        {
            yield return StartCoroutine(GetInstance<AnalyticsExtension>().StartTracker(config, onlineProtocol, backupProtocol));
            done();
        }

        public IAsyncOperation StopTracker()
        {
            var plugin = Plugin;
            if (plugin != null)
            {
                return plugin.StopTracker();
            }
            return null;
        }

        public void OnAuthUpdated(OAuth2Token token)
        {
            auth = token;
            var hidden = GetHiddenPlugin();
            if (hidden != null)
            {
                hidden.OnAuthUpdated(token);
            }
        }

        public void Demo()
        {
            PreviewManager.Instance.InPreviewMode = true;
            Game.Instance.Restart();
            Game.Instance.GameState.Data.setAutoSave(false);
            Game.Instance.GameState.Data.setSaveOnSuspend(false);
            Game.Instance.RunTarget(Game.Instance.GameState.InitialChapterTarget.getId());
        }

        private void LoadLanguageDictionaries(string language)
        {
            var langCode = global::Simva.SimvaLanguageLoader.ExtractLangCode(language);
            if (string.IsNullOrEmpty(langCode))
            {
                return;
            }

            var plugin = Plugin;
            if (plugin == null)
            {
                return;
            }

            var jsonFiles = global::Simva.SimvaLanguageLoader.LoadLanguageJSON(langCode);
            var dictionary = global::Simva.SimvaLanguageLoader.LoadDictionary(jsonFiles);
            plugin.SetLanguageDictionary(dictionary, false);
            plugin.SetLanguageDictionary(dictionary, true);
        }

        private global::Simva.SimvaPlugin GetHiddenPlugin()
        {
            try
            {
                return global::Simva.SimvaPlugin.Instance;
            }
            catch (Exception ex)
            {
                LogWarning("Hidden SimvaPlugin unavailable: " + ex.Message);
                return null;
            }
        }

        internal void Log(string message)
        {
            WriteLog(0, message);
        }

        internal void LogWarning(string message)
        {
            WriteLog(1, message);
        }

        internal void LogError(string message)
        {
            WriteLog(2, message);
        }

        private static bool writingLog;

        private void WriteLog(int level, string message)
        {
            if (writingLog)
            {
                return;
            }
            writingLog = true;
            try
            {
                var plugin = GetHiddenPlugin();
                if (plugin != null && !plugin.EnableDebugLogging)
                {
                    return;
                }
                var text = "[SimvaExtension] " + message;
                if (level == 1)
                {
                    Debug.LogWarning(text);
                }
                else if (level == 2)
                {
                    Debug.LogError(text);
                }
                else
                {
                    Debug.Log(text);
                }
            }
            finally
            {
                writingLog = false;
            }
        }
    }
}
