using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System.Linq;
using UnityEngine.EventSystems;
using System;
using UnityFx.Async.Promises;
using UnityFx.Async;
using UnityEngine.UI;

namespace uAdventure.Runner
{
    public enum GUIState
    {
        GAME_SELECTION, LOADING_GAME, NOTHING, TALK_PLAYER, TALK_CHARACTER, OPTIONS_MENU, ANSWERS_MENU,
        BOOK
    }

    public class Game : Singleton<Game>, IPointerClickHandler
    {

        //#################################################################
        //########################### SINGLETON ###########################
        //#################################################################
        public static string GameToLoad { get; set; }
        //#####################################################################
        //########################### MONOBEHAVIOUR ###########################
        //#####################################################################
        #region Monobehaviour

        public bool useSystemIO = true, forceScene = false, editor_mode = true, actionCanceled = false;
        public string gamePath = "", gameName = "", scene_name = "";
        public static readonly Color NoColor = new Color(-1, -1, -1, -1);

        // Execution
        private bool waitingRunTarget = false, waitingTransition = false, waitingTargetDestroy = false;
        private List<EffectHolderNode> background;
        private Stack<KeyValuePair<Interactuable, ExecutionEvent>> executeStack;
        private IRunnerChapterTarget runnerTarget;
        private GameState game_state;
        private uAdventureRaycaster uAdventureRaycaster;
        private TransitionManager TransitionManager
        {
            get
            {
                var camera = FindObjectOfType<Camera>();
                return camera ? camera.GetComponent<TransitionManager>() : null;
            }
        }

        // GUI
        public GameObject Blur_Prefab;
        private GUISkin skin;
        private GameObject blur;
        private GUIState guistate = GUIState.NOTHING;
        private List<int> order;
        private ConversationNodeHolder guioptions;
        private float elapsedTime;
        private bool doTimeOut;
        private BookDrawer bookDrawer;
        private List<GameExtension> gameExtensions;
        //private bool started;
        private int pulsing = 0;
        private bool wasShowingInventory = false;
        private int loadedChapter = -1;
        private int previousTouchCount;

        public delegate void TargetChangedDelegate(IChapterTarget newTarget);

        public TargetChangedDelegate OnTargetChanged;

        public delegate void ElementInteractedDelegate(bool finished, Element element, Core.Action action);

        public ElementInteractedDelegate OnElementInteracted;

        public delegate void ActionCanceledDelegate();

        public ActionCanceledDelegate OnActionCanceled;

        /*public delegate void ShowTextDelegate(bool finished, ConversationLine line, string text, int x, int y, Color textColor, Color textOutlineColor, Color baseColor, Color outlineColor, string id);

        public ShowTextDelegate OnShowText;*/

        public delegate void ExecutionEvent(object interactuable);

        public GUISkin Skin
        {
            get { return skin; }
        }

        public GameState GameState
        {
            get { return game_state; }
        }

        public IRunnerChapterTarget CurrentTargetRunner
        {
            get { return runnerTarget; }
        }

        public ResourceManager ResourceManager { get; set; }

        public string GameName
        {
            get { return gameName; }
        }

        public string SelectedGame
        {
            get { return gameName; }
        }

        public string SelectedPath
        {
            get { return gamePath; }
        }

        protected void Awake()
        {
            if (FindObjectsOfType(typeof(Game)).Length > 1)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                Debug.Log("[START ENGINE] Starting...");
                DontDestroyOnLoad(this.gameObject);
                DontDestroyOnLoad(Camera.main);

                executeStack = new Stack<KeyValuePair<Interactuable, ExecutionEvent>>();
                background = new List<EffectHolderNode>();

                skin = Resources.Load("basic") as GUISkin;

                if (!string.IsNullOrEmpty(gamePath))
                {
                    Debug.Log("[START ENGINE] Creating external resource manager: " + gamePath + gameName);
                    ResourceManager = ResourceManagerFactory.CreateExternal(gamePath + gameName);
                }
                else
                {
                    Debug.Log("[START ENGINE] Creating resource manager: " + gameName);
                    if (!string.IsNullOrEmpty(gameName))
                    {
                        ResourceManager = ResourceManagerFactory.CreateLocal(gameName, useSystemIO ? ResourceManager.LoadingType.SystemIO : ResourceManager.LoadingType.ResourcesLoad);
                    }
                    else
                    {
                        ResourceManager = ResourceManagerFactory.CreateLocal("CurrentGame/", useSystemIO ? ResourceManager.LoadingType.SystemIO : ResourceManager.LoadingType.ResourcesLoad);
                    }
                }

                if (!string.IsNullOrEmpty(Game.GameToLoad))
                {
                    gameName = Game.GameToLoad;
                    gamePath = ResourceManager.getCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar;
                    Debug.Log("[START ENGINE] Loading game from GameToLoad: " + gamePath);
                    useSystemIO = true;
                }

                var incidences = new List<Incidence>();
                var loadScreen = ShowLoading();
                Debug.Log("[START ENGINE] Loading main assets...");
                loadScreen.Text = "Loading main assets...";
                var descriptorPromise = LoadDescriptor();
                descriptorPromise.AddProgressCallback((p) =>
                {
                    Debug.Log("[START ENGINE] Main assets loading progress: " + p);
                    loadScreen.Progress = p * 100f;
                });
                descriptorPromise.Then(adventureData =>
                {
                    Debug.Log("[START ENGINE] Main assets loading done!");
                    game_state = new GameState(adventureData);
                    bookDrawer = new BookDrawer(ResourceManager);
                    gameExtensions = new List<GameExtension>();
                    Debug.Log("[START ENGINE] Creating game extensions...");
                    foreach (var gameExtension in GetAllSubclassOf(typeof(GameExtension)))
                    {
                        Debug.Log("[START ENGINE] Creating " + gameExtension.ToString());
                        gameExtensions.Add(gameObject.AddComponent(gameExtension) as GameExtension);
                    }
                    Debug.Log("[START GAME] Loading Chapter...");
                    loadScreen.Text = "Loading chapter...";
                    var chapterPromise = LoadChapter(1);
                    chapterPromise.ProgressChanged += (s, e) =>
                    {
                        Debug.Log("[START GAME] Chapter loading progress: " + chapterPromise.Progress);
                        loadScreen.Progress = chapterPromise.Progress;

                    };
                    return chapterPromise;
                })
                .Then(() =>
                {
                    Debug.Log("[START GAME] Chapter loading done!");
                    loadScreen.Text = "Starting game...";
                    StartCoroutine(StartGame(loadScreen));
                });
            }
        }

        public static IEnumerable<System.Type> GetAllSubclassOf(System.Type parent)
        {
            foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in a.GetTypes())
                {
                    if (t.IsSubclassOf(parent))
                    {
                        yield return t;
                    }
                }
            }
        }

        private IEnumerator StartGame(LoadingScreen loadScreen)
        {
            Debug.Log("[START GAME] GameState Restart...");
            GameState.Restart();
            //started = true;
            Debug.Log("[START GAME] Game Resuming...");
            if (GameState.Data.isRestoreAfterOpen())
            {
                GameState.OnGameResume();
            }
            Debug.Log("[START GAME] After Game Load...");
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnAfterGameLoad", gameExtensions))
            {
                yield return StartCoroutine(g.OnAfterGameLoad());
            }
            uAdventureRaycaster = FindObjectOfType<uAdventureRaycaster>();
            if (!uAdventureRaycaster)
            {
                Debug.LogError("No uAdventureRaycaster was found in the scene!");
            }
            else
            {
                // When clicks are out, i capture them
                uAdventureRaycaster.Base = this.gameObject;
            }
            if (!TransitionManager)
            {
                Debug.LogError("No TransitionManager was found in the scene!");
            }

            Debug.Log("[START GAME] Running Target...");
            RunTarget(forceScene ? scene_name : GameState.CurrentTarget);
            yield return new WaitUntil(() => !waitingRunTarget);
            Debug.Log("[START GAME] Game Ready...");
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnGameReady", gameExtensions))
            {
                yield return StartCoroutine(g.OnGameReady());
            }
            uAdventureInputModule.LookingForTarget = null;

            TimerController.Instance.Timers = GameState.GetTimers();
            TimerController.Instance.Run();
            Debug.Log("[START GAME] Done! (Waiting for target to be ready)");
            loadScreen.Close();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (MenuMB.Instance)
            {
                MenuMB.Instance.hide();
            }
            if (!waitingRunTarget && !waitingTargetDestroy && !waitingTransition && executeStack.Count > 0 && guistate != GUIState.ANSWERS_MENU)
            {
                Interacted();
            }
        }

        protected void Update()
        {
            if (background.Count > 0)
            {
                foreach(var element in background.ToList())
                {
                    if (!element.execute())
                    {
                        background.Remove(element);
                    }
                }

                if(background.Count == 0 && executeStack.Count == 0)
                {
                    FinalizeExecution();
                }
            }

            if (waitingRunTarget && runnerTarget.IsReady)
            {
                Debug.Log("[UPDATE] Target Ready!");
                waitingRunTarget = false;
                waitingTransition = true;
                System.Action<Transition, Texture> afterTransition = (transition, texture) =>
                {
                    Debug.Log("[UPDATE] Transition done.");
                    waitingTransition = false;
                    if (uAdventureRaycaster.Instance)
                    {
                        uAdventureRaycaster.Instance.Override = null;
                    }
                    Debug.Log("[UPDATE] Continue execution...");
                    Interacted();
                };

                if (TransitionManager)
                {
                    Debug.Log("[UPDATE] Doing transition...");
                    TransitionManager.DoTransition(afterTransition);
                } 
                else
                {
                    afterTransition(null, null);
                }
            }

            if (doTimeOut)
            {
                elapsedTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(1))
            {
                MenuMB.Instance.hide();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || (Input.touchCount != previousTouchCount && Input.touchCount == 4))
            {
                if (!isSomethingRunning())
                {
                    GUIManager.Instance.ShowConfigMenu();
                }
            }
            previousTouchCount = Input.touchCount;
        } 

        public IEnumerator LoadGame()
        {
            Debug.Log("[LOAD GAME] Restoring save...");
            GameState.RestoreFrom("save");
            Debug.Log("[LOAD GAME] After Game Load...");
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnAfterGameLoad", gameExtensions))
            {
                yield return StartCoroutine(g.OnAfterGameLoad());
            }
            Debug.Log("[LOAD GAME] Running target...");
            waitingRunTarget = true;
            RunTarget(GameState.CurrentTarget);
            Debug.Log("[LOAD GAME] Waiting for target to be ready...!");
            yield return new WaitUntil(() => !waitingRunTarget);
            Debug.Log("[LOAD GAME] Game Ready...");
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnGameReady", gameExtensions))
            {
                yield return StartCoroutine(g.OnGameReady());
            }
            uAdventureInputModule.LookingForTarget = null;
            Debug.Log("[LOAD GAME] Done!");
        }

        public void SaveGame()
        {
            if (!GameState.GetChapterTarget(GameState.CurrentTarget).allowsSavingGame())
            {
                Debug.Log("[SAVE GAME] Current scene doesn't allow saving. Cancelling save...");
                return;
            }
            else
            {
                Debug.Log("[SAVE GAME] Before saving game...");
                foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnBeforeGameSave", gameExtensions))
                {
                    g.OnBeforeGameSave();
                }
                Debug.Log("[SAVE GAME] Saving...");
                GameState.SerializeTo("save");
                Debug.Log("[SAVE GAME] Done!");
            }
        }

        public void AutoSave()
        {
            if(/*Application.isEditor || */!GameState.Data.isAutoSave())
            {
                Debug.Log("[AUTO SAVE] Auto save is disabled. Skipping...");
                return;
            }
            
            if (!GameState.GetChapterTarget(GameState.CurrentTarget).allowsSavingGame())
            {
                Debug.Log("[AUTO SAVE] Current scene doesn't allow saving. Cancelling save...");
                return;
            }


            Debug.Log("[AUTO SAVE] Performing auto-save...");
            SaveGame();
        }

        public void OnApplicationPause(bool paused)
        {
            if (!paused) // Ignore restores
            {
                return;
            }

            if (isSomethingRunning())
            {
                Debug.Log("[SAVE ON SUSPEND] Not saved on suspend because something was running.");
                return;
            }

            if (!GameState.GetChapterTarget(GameState.CurrentTarget).allowsSavingGame())
            {
                Debug.Log("[SAVE ON SUSPEND] Current scene doesn't allow saving. Cancelling save...");
                return;
            }

            if (paused && GameState.Data.isSaveOnSuspend())
            {
                Debug.Log("[SAVE ON SUSPEND] Saving game...");
                GameState.OnGameSuspend();
            }

            /*if (!paused && GameState.Data.isRestoreAfterOpen())
            {
                // TODO REPARE RESTORE AFTER OPEN
                GameState.OnGameResume();
                if (started)
                {
                    var gameReadyOrderedExtensions = PriorityAttribute.OrderExtensionsByMethod("OnGameReady", gameExtensions);
                    RunTarget(GameState.CurrentTarget);
                    gameReadyOrderedExtensions.ForEach(g => g.OnGameReady());
                    uAdventureInputModule.LookingForTarget = null;
                }
            }*/

        }

        public void RunInBackground(EffectHolderNode effect)
        {
            background.Add(effect);
        }

        public bool Execute(Interactuable interactuable, ExecutionEvent callback = null)
        {
            // In case any menu is shown, we hide it
            if (MenuMB.Instance)
            {
                MenuMB.Instance.hide(true);
            }

            if (executeStack.Count == 0)
            {
                AutoSave();
            }

            // Then we execute anything
            if (executeStack.Count == 0 || executeStack.Peek().Key != interactuable)
            {
                Debug.Log("Pushed " + interactuable.ToString());
                executeStack.Push(new KeyValuePair<Interactuable, ExecutionEvent>(interactuable, callback));
            }
            while (executeStack.Count > 0)
            {
                var preInteractSize = executeStack.Count;
                var toExecute = executeStack.Peek();
                var requiresMore = false;

                try
                {
                    requiresMore = toExecute.Key.Interacted() == InteractuableResult.REQUIRES_MORE_INTERACTION;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Interacted execution exception: " + ex.Message + ex.StackTrace);
                    Debug.LogException(ex);
                }

                if (requiresMore && !actionCanceled)
                {
                    uAdventureRaycaster.Instance.Override = this.gameObject;
                    return true;
                }
                else
                {
                    Debug.Log("Execution finished " + toExecute.ToString());
                    if (!actionCanceled)
                    {
                        if (preInteractSize != executeStack.Count)
                        {
                            Debug.Log("The size was different");
                            var backupStack = new Stack<KeyValuePair<Interactuable, ExecutionEvent>>();
                            // We backup the new stacked things
                            while (executeStack.Count > preInteractSize)
                            {
                                backupStack.Push(executeStack.Pop());
                            }
                            // Then we remove our entry
                            executeStack.Pop();
                            // Then we reinsert the backuped stuff
                            while (backupStack.Count > 0)
                            {
                                executeStack.Push(backupStack.Pop());
                            }
                        }
                        else
                        {
                            executeStack.Pop();
                        }
                    }

                    try
                    {
                        if (actionCanceled)
                        {
                            while (executeStack.Count > 0)
                            {
                                var removed = executeStack.Pop();
                                if (removed.Value != null)
                                {
                                    removed.Value(removed.Key);
                                }
                            }
                        }
                        else if (toExecute.Value != null)
                        {
                            toExecute.Value(toExecute.Key);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Log("Execution OnFinished execution exception: " + ex.Message);
                        Debug.LogException(ex);
                        if (actionCanceled)
                        {
                            executeStack.Clear();
                        }
                    }
                }
            }
            if(background.Count > 0)
            {
                return true;
            }

            FinalizeExecution();
            return false;
        }

        private void FinalizeExecution()
        {
            if (uAdventureRaycaster.Instance)
            {
                uAdventureRaycaster.Instance.Override = null;
            }
            if (executeStack.Count == 0)
            {
                if (GameState.ChangeAmbitCount > 0)
                {
                    Debug.LogWarning("There are still some opened change ambits! " + GameState.ChangeAmbitCount);
                }
                OnActionCanceled = null;
                AutoSave();
            }
            // In case any bubble is bugged
            GUIManager.Instance.DestroyBubbles();
            actionCanceled = false;
        }

        public void Quit()
        {
            StartCoroutine(QuitCoroutine());
        }
        private IEnumerator QuitCoroutine()
        {
            quitAborted = false;
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("OnGameFinished", gameExtensions))
            {
                yield return StartCoroutine(g.OnGameFinished());
            }
            if (!quitAborted)
            {
                Application.Quit();
            }
        }

        public void AbortQuit()
        {
            quitAborted = true;
        }

        public void ClearAndRestart()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            DestroyImmediate(this.gameObject);
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public IEnumerator Restart()
        {
            GameState.Restart();
            foreach (var g in PriorityAttribute.OrderExtensionsByMethod("Restart", gameExtensions))
            {
                yield return StartCoroutine(g.Restart());
            }
            RunTarget(GameState.CurrentTarget);
            uAdventureInputModule.LookingForTarget = null;
        }

        public bool IsRunningInBackground(EffectHolderNode node)
        {
            return background.Contains(node);
        }

        public bool ContinueEffectExecution()
        {
            if (executeStack.Count > 0)
            {
                return Execute(executeStack.Peek().Key);
            }
            return false;
        }

        private bool Interacted()
        {
            if (pulsing > 0)
            {
                return false;
            }

            if(guistate != GUIState.BOOK)
            {
                guistate = GUIState.NOTHING;
            }
            if (executeStack.Count > 0)
            {
                return Execute(executeStack.Peek().Key);
            }
            return false;
        }

        public void ElementInteracted(bool finished, Element element, Core.Action action)
        {
            if (OnElementInteracted != null)
            {
                OnElementInteracted(finished, element, action);
            }
        }

        public bool isSomethingRunning()
        {
            return executeStack.Count > 0 || waitingRunTarget || waitingTransition || waitingTargetDestroy;
        }

        public Interactuable getNextInteraction()
        {
            return executeStack.Count > 0 ? executeStack.Peek().Key : null;
        }

        #endregion Monobehaviour
        //#################################################################
        //########################### RENDERING ###########################
        //#################################################################
        #region Rendering


        public IRunnerChapterTarget RunTarget(string scene_id, Interactuable notifyObject, bool trace = true)
        {
            return RunTarget(scene_id, 0, 0, notifyObject, trace);
        }

        public IRunnerChapterTarget RunTarget(string scene_id, int transition_time = 0, TransitionType transition_type = 0, Interactuable notifyObject = null, bool trace = true)
        {
            Debug.Log("Run target: " + scene_id);
            if (GUIManager.Instance)
            {
                GUIManager.Instance.ShowHand(false);
            }
            if (MenuMB.Instance)
            {
                MenuMB.Instance.hide(true);
            }

            IChapterTarget target = GameState.GetChapterTarget(scene_id);

            if (TransitionManager != null)
            {
                var transition = new Transition();
                transition.setTime(transition_time);
                transition.setType(transition_type);
                TransitionManager.PrepareTransition(transition);
            }

            if (runnerTarget != null && runnerTarget.Data == target && scene_id == GameState.CurrentTarget)
            {
                runnerTarget.RenderScene();
                waitingRunTarget = true;
            }
            else
            {
                waitingTargetDestroy = true;
                System.Action runTarget = () =>
                {
                    waitingTargetDestroy = false;
                    waitingRunTarget = true;
                    if (trace)
                    {
                        GameState.CurrentTarget = target.getId();
                    }
                    runnerTarget.RenderScene();

                    if (trace && OnTargetChanged != null)
                    {
                        OnTargetChanged(target);
                    }
                };

                var oldTarget = runnerTarget;

                // Here we connect with the IChapterTargetFactory and create an IRunnerChapterTarget
                runnerTarget = RunnerChapterTargetFactory.Instance.Instantiate(target);
                runnerTarget.Data = target;
                Debug.Log("Target gameobject: " + runnerTarget.gameObject);
                DontDestroyOnLoad(runnerTarget.gameObject);
                Debug.Log("Target creado: " + runnerTarget);

                System.Action afterChapterLoad = () =>
                {
                    if (oldTarget != null)
                    {
                        oldTarget.Destroy(0f, runTarget);
                    }
                    else
                    {
                        runTarget();
                    }
                };

                if (loadedChapter != GameState.CurrentChapter)
                {
                    LoadChapter(GameState.CurrentChapter)
                        .Then(() =>
                        {
                            loadedChapter = GameState.CurrentChapter;
                            afterChapterLoad();
                        })
                        .Catch(ex =>
                        {
                            Debug.LogError(ex);
                        });
                }
                else
                {
                    afterChapterLoad();
                }
            }

            if(notifyObject != null)
            {
                executeStack.Push(new KeyValuePair<Interactuable, ExecutionEvent>(notifyObject, null));
            }

            if (uAdventureRaycaster.Instance)
            {
                uAdventureRaycaster.Instance.Override = this.gameObject;
            }

            return runnerTarget;
        }

        private IAsyncOperation<AdventureData> LoadDescriptor()
        {
            var done = new AsyncCompletionSource<AdventureData>();

            done.SetProgress(0);
            Loader.LoadAdventureDataAsync(ResourceManager, new List<Incidence>())
                .Then(adventureData =>
                {
                    var descriptorAssets = new List<string>();
                    done.SetProgress(0.1f);
                    Debug.Log("Setting progress 10");
                    /*foreach (var cursor in adventureData.getCursors())
                    {
                        descriptorAssets.Add(cursor.getPath());
                    }
                    foreach (var button in adventureData.getButtons())
                    {
                        descriptorAssets.Add(button.getPath());
                    }*/
                    var cachePromise = ResourceManager.CacheAssets(descriptorAssets);
                    cachePromise.AddProgressCallback(p => done.SetProgress(p));
                    cachePromise.Then(() =>
                    {
                        Debug.Log("Done Caching 100");
                        done.SetProgress(1f);
                        done.SetResult(adventureData);
                    });
                });

            return done;
        }

        private IAsyncOperation LoadChapter(int currentChapter)
        {
            var assets = new string[0] as IEnumerable<string>;

            /*foreach(var scene in GameState.GetObjects<Scene>())
            {
                assets = assets.Union(scene.getResources().SelectMany(r => r.getAssetValues()));
                foreach(var exit in scene.getExits())
                {
                    // TODO load exit sounds
                    assets = assets.Union(getEffectsAssets(exit.getEffects()));
                    assets = assets.Union(getEffectsAssets(exit.getNotEffects()));
                    assets = assets.Union(getEffectsAssets(exit.getPostEffects()));
                }

                foreach(var activeArea in scene.getActiveAreas())
                {
                    assets = assets.Union(activeArea.getResources().SelectMany(r => r.getAssetValues()));
                    assets = assets.Union(getActionsAssets(activeArea.getActions()));
                }
            }
            foreach (var item in GameState.GetObjects<Item>())
            {
                assets = assets.Union(item.getResources().SelectMany(r => r.getAssetValues()));
                assets = assets.Union(getActionsAssets(item.getActions()));
            }
            foreach (var npc in GameState.GetObjects<NPC>())
            {
                assets = assets.Union(npc.getResources().SelectMany(r => r.getAssetValues()));
                assets = assets.Union(getActionsAssets(npc.getActions()));
            }
            foreach (var atrezzo in GameState.GetObjects<Atrezzo>())
            {
                assets = assets.Union(atrezzo.getResources().SelectMany(r => r.getAssetValues()));
            }
            foreach (var player in GameState.GetObjects<Player>())
            {
                assets = assets.Union(player.getResources().SelectMany(r => r.getAssetValues()));
            }
            foreach (var conversation in GameState.GetObjects<Conversation>())
            {
                assets = assets.Union(
                    conversation.getAllNodes()
                    .SelectMany(n => Enumerable.Range(0, n.getLineCount()).Select(i => n.getLine(i)))
                    .SelectMany(l => l.getResources())
                    .SelectMany(r => r.getAssetValues()));

                assets = assets.Union(
                    conversation.getAllNodes()
                    .SelectMany(n => getEffectsAssets(n.getEffects())));
            }
            foreach (var book in GameState.GetObjects<Book>())
            {
                assets = assets.Union(book.getResources().SelectMany(r => r.getAssetValues()));
                // TODO read book paragraph types
            }
            foreach (var timer in GameState.GetObjects<Timer>())
            {
                assets = assets.Union(getEffectsAssets(timer.getEffects()));
                assets = assets.Union(getEffectsAssets(timer.getPostEffects()));
            }*/

            return ResourceManager.CacheAssets(assets);
        }

        private IEnumerable<string> getActionsAssets(List<Core.Action> lists)
        {
            var assets = lists.SelectMany(a => getEffectsAssets(a.getEffects()));
            assets = assets.Union(lists.SelectMany(a => getEffectsAssets(a.getClickEffects())));
            assets = assets.Union(lists.SelectMany(a => getEffectsAssets(a.getNotEffects())));
            // TODO read action sounds and custom actions

            return assets;
        }

        private IEnumerable<string> getEffectsAssets(Effects effects)
        {
            // TODO
            return new string[0];
        }

        public void SwitchToLastTarget()
        {
            IChapterTarget last = GameState.PreviousChapterTarget;

            if (last != null) 
                RunTarget(last.getId(), 1000, TransitionType.FadeIn);
        }

        #endregion Rendering
        //#################################################################
        //#################################################################
        //#################################################################
        #region Misc
        public void ActionCanceled()
        {
            if (isSomethingRunning())
            {
                this.actionCanceled = true; 
                if(OnActionCanceled != null)
                {
                    Delegate[] delegateList = OnActionCanceled.GetInvocationList();
                    for (int counter = delegateList.Length - 1; counter >= 0; counter--)
                    {
                        ((ActionCanceledDelegate)delegateList[counter])();
                    }
                }
            }
            OnActionCanceled = null;
        }

        public void showActions(List<Core.Action> actions, Vector2 position, IActionReceiver actionReceiver = null)
        {
            if (!MenuMB.Instance)
            {
                return;
            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            MenuMB.Instance.transform.position = new Vector3(pos.x, pos.y, pos.z) + mouseRay.direction.normalized * 10;
            MenuMB.Instance.transform.rotation = Camera.main.transform.rotation;
            MenuMB.Instance.setActions(actions, actionReceiver);
            MenuMB.Instance.show();
            // TODO why isnt position used?
            //this.clicked_on = position;
        }

        public List<int> showOptions(ConversationNodeHolder options)
        {
            var optionsNode = options.getNode() as OptionConversationNode;
            if (optionsNode != null)
            {
                // Disable the UI interactivity
                uAdventureRaycaster.Instance.enabled = false;
                wasShowingInventory = InventoryManager.Instance.Show;
                InventoryManager.Instance.Show = false;

                // Enable blurred background
                blur = GameObject.Instantiate(Blur_Prefab);
                blur.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
                blur.transform.rotation = Camera.main.transform.rotation;

                // Order shuffeling when node is configured for random
                this.order = Enumerable.Range(0, optionsNode.getLineCount()).ToList();
                if (optionsNode.isRandom())
                {
                    this.order.Shuffle();
                }
                this.guioptions = options;
                this.guistate = GUIState.ANSWERS_MENU;

                // Timeout option
                if (optionsNode.Timeout > 0 && ConditionChecker.check(optionsNode.TimeoutConditions))
                {
                    this.doTimeOut = true;
                    this.elapsedTime = 0;
                }
                else
                {
                    this.doTimeOut = false;
                }
                return order;
            }
            return null;
        }

        public void Talk(string text, int x, int y, Color textColor, Color textOutlineColor)
        {
            GUIManager.Instance.Talk(text, x, y, textColor, textOutlineColor);
        }

        public void Talk(string text, int x, int y, Color textColor, Color textOutlineColor, Color baseColor, Color outlineColor)
        {
            GUIManager.Instance.Talk(text, x, y, textColor, textOutlineColor, baseColor, outlineColor);
        }

        public void Talk(string text, string character)
        {
            GUIManager.Instance.Talk(new ConversationLine(character, text));
        }

        public void Talk(ConversationLine line)
        {
            GUIManager.Instance.Talk(line);
        }

        public void ShowBook(string bookId)
        {
            guistate = GUIState.BOOK;
            bookDrawer.Book = GameState.FindElement<Book>(bookId);
            bookDrawer.RefreshResources();
        }

        public void CloseBook()
        {
            guistate = GUIState.NOTHING;
            bookDrawer.Book = null;
        }

        public bool ShowingBook
        {
            get
            {
                return bookDrawer.Book != null;
            }
        }

        private class LoadingScreen : IDisposable
        {
            private bool showing;
            private Texture2D backgroundColor;
            private GUIStyle backgroundStyle;

            public string Text { get; set; }
            public float Progress { get; set; }

            public LoadingScreen()
            {
                backgroundColor = new Texture2D(1, 1);
                backgroundColor.SetPixel(0, 0, new Color(0.6f, 0.3f, 0.1f));
                backgroundStyle = new GUIStyle();
                backgroundStyle.alignment = TextAnchor.MiddleCenter;
                backgroundStyle.normal.background = backgroundColor;
                showing = true;
            }

            public void OnGUI()
            {
                if (!showing)
                {
                    return;
                }
                using (new GUILayout.AreaScope(new Rect(0, 0, Screen.width, Screen.height)))
                using (new GUILayout.VerticalScope(backgroundStyle, GUILayout.ExpandWidth(true)))
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(Text);

                    GUILayout.Space(30);

                    GUILayout.HorizontalSlider(Progress, 0, 100, GUILayout.Width(400));

                    GUILayout.FlexibleSpace();
                }
            }

            public void Close()
            {
                Dispose();
            }

            public void Dispose()
            {
                showing = false;
            }
        }

        private LoadingScreen loadingScreen;

        private LoadingScreen ShowLoading()
        {
            return loadingScreen = new LoadingScreen();
        }

        private List<GUILayoutOption> auxLimitList = new List<GUILayoutOption>();
        private bool quitAborted;

        protected void OnGUI()
        {
            if(loadingScreen != null)
            {
                loadingScreen.OnGUI();
            }


            switch (guistate)
            {
                case GUIState.BOOK:
                    if(ShowingBook)
                    {
                        bookDrawer.Draw(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)));
                    }
                    break;
                case GUIState.ANSWERS_MENU:

                    using(new GUIUtil.SkinScope(skin))
                    {
                        float guiscale = Screen.width / 800f;
                        var buttonImageWidth = (200f / 600f) * Screen.height;
                        skin.box.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.button.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.button.alignment = TextAnchor.MiddleCenter;
                        skin.button.imagePosition = ImagePosition.ImageAbove;
                        skin.button.stretchHeight = false;
                        skin.button.stretchWidth = true;
                        skin.label.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.GetStyle("optionLabel").fontSize = Mathf.RoundToInt(guiscale * 36);
                        skin.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.GetStyle("emptyProgressBar").fontSize = Mathf.RoundToInt(guiscale * 20);
                        OptionConversationNode options = (OptionConversationNode)guioptions.getNode();
                        var areawidth = Screen.width * 0.8f;
                        using (new GUILayout.AreaScope(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, areawidth, Screen.height * 0.8f)))
                        {
                            using (new GUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                            {
                                var restWidth = 1f;
                                var initedHorizontal = false;

                                if (options.isKeepShowing() && GUIManager.Instance.Line != null)
                                {
                                    var text = GUIManager.Instance.Line.getText();
                                    if (text[0] == '#')
                                        text = text.Remove(0, Mathf.Max(0, text.IndexOf(' ') + 1));

                                    var textContent = new GUIContent(text);
                                    var textRect = GUILayoutUtility.GetRect(textContent, "optionLabel");

                                    GUIUtil.DrawTextBorder(textRect, textContent, Color.black, "optionLabel");
                                    GUIUtil.DrawText(textRect, textContent, ((GUIStyle)"optionLabel").normal.textColor, "optionLabel");

                                    var resources = GUIManager.Instance.Line.getResources().Checked().FirstOrDefault();
                                    if (resources != null)
                                    {
                                        var image = resources.existAsset("image") ? Game.Instance.ResourceManager.getImage(resources.getAssetPath("image")) : null;
                                        if (image)
                                        {
                                            GUILayout.BeginHorizontal();
                                            initedHorizontal = true;
                                            restWidth = 0.7f;
                                            var imageRatio = image.width / (float) image.height;
                                            var imageWidth = areawidth * 0.28f;
                                            var imageHeight = Mathf.Min(imageWidth / imageRatio, Screen.height * 0.45f);
                                            using (new GUILayout.VerticalScope(GUILayout.Width(areawidth * 0.3f)))
                                            {
                                                GUILayout.FlexibleSpace();
                                                GUILayout.Box(image, GUILayout.Width(imageWidth), GUILayout.Height(imageHeight));
                                                GUILayout.FlexibleSpace();
                                            }
                                        }
                                    }
                                }

                                using (new GUILayout.VerticalScope(GUILayout.Width(areawidth * restWidth)))
                                {
                                    if (options.Horizontal)
                                    {
                                        GUILayout.FlexibleSpace();
                                    }
                                    var elementsLeft = options.getLineCount();
                                    while (elementsLeft > 0)
                                    {
                                        if (options.Horizontal)
                                        {
                                            GUILayout.BeginHorizontal();
                                            GUILayout.FlexibleSpace();
                                        }
                                        else
                                        {
                                            GUILayout.BeginVertical();
                                        }
                                        var start = options.getLineCount() - elementsLeft;
                                        var amount = options.MaxElemsPerRow > 0 ? options.MaxElemsPerRow : options.getLineCount();
                                        var end = Mathf.Clamp(start + amount, 0, options.getLineCount());
                                        var eachWidth = (areawidth * restWidth / (end - start)) - 20;
                                        for (int i = start; i < end; i++)
                                        {
                                            ConversationLine ono = options.getLine(order[i]);
                                            var content = new GUIContent(ono.getText());
                                            var resources = ono.getResources().Checked().FirstOrDefault();
                                            auxLimitList.Clear();
                                            if(end - start > 1 && options.Horizontal)
                                            {
                                                auxLimitList.Add(GUILayout.Width(eachWidth));
                                            }

                                            if (resources != null && resources.existAsset("image") && !string.IsNullOrEmpty(resources.getAssetPath("image")))
                                            {
                                                var imagePath = resources.getAssetPath("image");
                                                var image = ResourceManager.getImage(imagePath);
                                                if (image)
                                                {
                                                    content.image = image;
                                                    /*if (image.height > buttonImageWidth)
                                                    {*/
                                                        auxLimitList.Add(GUILayout.Height(buttonImageWidth - 20));
                                                    //}
                                                }
                                            }

                                            if (ConditionChecker.check(options.getLineConditions(order[i])) && GUILayout.Button(content, auxLimitList.ToArray()))
                                            {
                                                OptionSelected(order[i]);
                                            }
                                        }
                                        elementsLeft = options.getLineCount() - end;
                                        if (options.Horizontal)
                                        {
                                            GUILayout.FlexibleSpace();
                                            GUILayout.EndHorizontal();
                                        }
                                        else
                                        {
                                            GUILayout.EndVertical();
                                        }

                                        if (doTimeOut)
                                        {
                                            if (Event.current.type == EventType.Repaint && elapsedTime > options.Timeout)
                                            {
                                                OptionSelected(options.getChildCount() - 1);
                                            }

                                            var timeLeft = Mathf.Max(0, options.Timeout - elapsedTime);
                                            var timeLeftText = Mathf.Round(timeLeft * 10) / 10 + " s";
                                            GUILayout.FlexibleSpace();
                                            DrawProgressBar(GUILayoutUtility.GetRect(0, 0, "emptyProgressBar", GUILayout.ExpandWidth(true), GUILayout.Height(50)), timeLeftText, 1 - (elapsedTime / options.Timeout));
                                        }
                                    }

                                    if (initedHorizontal)
                                    {
                                        GUILayout.EndHorizontal();
                                    }

                                    if (options.Horizontal)
                                    {
                                        GUILayout.FlexibleSpace();
                                    }
                                }
                            }
                        }
                    }
                    break;
                default: break;
            }
        }

        private void OptionSelected(int i)
        {
            uAdventureRaycaster.Instance.enabled = true;
            InventoryManager.Instance.Show = wasShowingInventory;
            doTimeOut = false;
            GameObject.Destroy(blur);
            guioptions.clicked(i);
            Interacted();
        }

        private void DrawProgressBar(Rect rect, string text, float progress)
        {
            var pos = rect.position;
            var size = rect.size;

            using (new GUIUtil.SkinScope(skin))
            {
                progress = Mathf.Clamp01(progress);

                // draw the background:
                using (new GUI.GroupScope(new Rect(pos.x, pos.y, size.x, size.y)))
                {
                    var backgroundRect = new Rect(0, 0, size.x, size.y);
                    GUI.Box(backgroundRect, new GUIContent(), "emptyProgressBar");

                    // draw the filled-in part:
                    using(new GUI.GroupScope(new Rect(0, 0, size.x * progress, size.y)))
                    {
                        GUI.Box(new Rect(0, 0, size.x, size.y), new GUIContent(), "fullProgressBar");
                    }

                    GUI.Label(backgroundRect, text, "textProgressBar");
                }
            }
        }

        public void PulseOnTime(EffectHolderNode effect, int time)
        {
            this.StartCoroutine(PulseOnTimeCorrutine(effect, time));
        }

        private IEnumerator PulseOnTimeCorrutine(EffectHolderNode effect, int time)
        {
            pulsing++;
            yield return new WaitForSeconds(time);
            if (effect != null)
            {
                effect.doPulse();
            }
            pulsing--;
            Interacted();
        }

        #endregion Misc

    }
}