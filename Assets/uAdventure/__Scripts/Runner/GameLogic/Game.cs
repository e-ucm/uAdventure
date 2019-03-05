﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;
using RAGE.Analytics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using AssetPackage;

namespace uAdventure.Runner
{
    public enum GUIState
    {
        GAME_SELECTION, LOADING_GAME, NOTHING, TALK_PLAYER, TALK_CHARACTER, OPTIONS_MENU, ANSWERS_MENU,
        BOOK
    }

    public class Game : MonoBehaviour, IPointerClickHandler
    {

        //#################################################################
        //########################### SINGLETON ###########################
        //#################################################################
        #region Singleton
        static Game instance;

        public static Game Instance
        {
            get { return instance; }
        }
        
        public static string GameToLoad { get; set; }
        #endregion Singleton
        //#####################################################################
        //########################### MONOBEHAVIOUR ###########################
        //#####################################################################
        #region Monobehaviour

        public bool useSystemIO = true, forceScene = false, editor_mode = true;
        public string gamePath = "", gameName = "", scene_name = "";

        // Execution
        private bool waitingRunTarget = false;
        private Stack<KeyValuePair<Interactuable, ExecutionEvent>> executeStack;
        private IRunnerChapterTarget runnerTarget;
        private GameState game_state;
        private uAdventureRaycaster uAdventureRaycaster;

        // GUI
        public GameObject Blur_Prefab;
        private GUISkin skin;
        private GameObject blur;
        private GUIState guistate = GUIState.NOTHING;
        private List<int> order;
        private ConversationNodeHolder guioptions;
        private float elapsedTime;
        private bool doTimeOut;
        private Book openedBook;
        private BookDrawer bookDrawer;

        public delegate void ExecutionEvent(object interactuable);

        public GUISkin Skin
        {
            get { return skin; }
        }

        public GameState GameState
        {
            get { return game_state; }
        }

        public ResourceManager ResourceManager { get; private set; }

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
            Game.instance = this;
            executeStack = new Stack<KeyValuePair<Interactuable, ExecutionEvent>>();
            LoadTrackerSettings();

            skin = Resources.Load("basic") as GUISkin;

            if (!string.IsNullOrEmpty(gamePath))
            {
                ResourceManager = ResourceManagerFactory.CreateExternal(gamePath + gameName);
            }
            else
            {
                if (!string.IsNullOrEmpty(gameName))
                {
                    ResourceManager = ResourceManagerFactory.CreateLocal(gameName, useSystemIO ? ResourceManager.LoadingType.SYSTEM_IO : ResourceManager.LoadingType.RESOURCES_LOAD);
                }
                else
                {
                    ResourceManager = ResourceManagerFactory.CreateLocal("CurrentGame/", useSystemIO ? ResourceManager.LoadingType.SYSTEM_IO : ResourceManager.LoadingType.RESOURCES_LOAD);
                }
            }

            if (Game.GameToLoad != "")
            {
                gameName = Game.GameToLoad;
                gamePath = ResourceManager.getCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar;
                useSystemIO = true;
            }

            AdventureData data = new AdventureData();
            var incidences = new List<Incidence>();
            AdventureHandler adventure = new AdventureHandler(data, ResourceManager, incidences);
            adventure.Parse("descriptor.xml");
            game_state = new GameState(data);
            CompletablesController.Instance.SetCompletables(GameState.GetCompletables());

            bookDrawer = new BookDrawer(ResourceManager);
        }


        protected void Start()
        {
            if (!forceScene)
            {
                RunTarget(GameState.InitialChapterTarget.getId());
            }
            else
            {
                RunTarget(scene_name);
            }

            uAdventureRaycaster = FindObjectOfType<uAdventureRaycaster>();
            if (!uAdventureRaycaster)
            {
                Debug.LogError("No uAdventure raycaster was found in the scene!");
            }

            // When clicks are out, i capture them
            uAdventureRaycaster.Base = this.gameObject;

            TimerController.Instance.Timers = GameState.GetTimers();
            TimerController.Instance.Run();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            MenuMB.Instance.hide();
            if (!waitingRunTarget && executeStack.Count > 0 && guistate != GUIState.ANSWERS_MENU)
            {
                Interacted();
            }
        }

        protected void Update()
        {
            if (waitingRunTarget && runnerTarget.IsReady)
            {
                waitingRunTarget = false;
                uAdventureRaycaster.Instance.Override = null;
                Interacted();
            }

            if (doTimeOut)
            {
                elapsedTime += Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(1))
            {
                MenuMB.Instance.hide();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    GUIManager.Instance.ShowConfigMenu();
                }
                else if (!isSomethingRunning())
                {
                    Time.timeScale = 0;
                    GUIManager.Instance.ShowConfigMenu();
                }
            }
        }

        public void LoadGame()
        {
            GameState.RestoreFrom("save");
            RunTarget(GameState.CurrentTarget);
        }

        public void SaveGame()
        {
            GameState.SerializeTo("save");
        }

        public bool Execute(Interactuable interactuable, ExecutionEvent callback = null)
        {
            // In case any menu is shown, we hide it
            MenuMB.Instance.hide(true);
            // Then we execute anything
            if (executeStack.Count == 0 || executeStack.Peek().Key != interactuable)
            {
                Debug.Log("Pushed " + interactuable.ToString());
                executeStack.Push(new KeyValuePair<Interactuable, ExecutionEvent>(interactuable, callback));
            }
            while(executeStack.Count > 0)
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
                    Debug.Log("Interacted execution exception: " + ex.Message);
                }

                if (requiresMore)
                {
                    uAdventureRaycaster.Instance.Override = this.gameObject;
                    return true;
                }
                else
                {
                    Debug.Log("Execution finished " + toExecute.ToString());
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
                    try
                    {
                        if (toExecute.Value != null)
                        {
                            toExecute.Value(toExecute.Key);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Log("Execution OnFinished execution exception: " + ex.Message);
                    }
                }
            }
            uAdventureRaycaster.Instance.Override = null;
            return false;
        }

        private bool Interacted()
        {
            if(guistate != GUIState.BOOK)
            {
                guistate = GUIState.NOTHING;
                GUIManager.Instance.DestroyBubbles();
            }
            if (executeStack.Count > 0)
            {
                return Execute(executeStack.Peek().Key);
            }
            return false;
        }

        public bool isSomethingRunning()
        {
            return executeStack.Count > 0;
        }

        public Interactuable getNextInteraction()
        {
            return executeStack.Count > 0 ? executeStack.Peek().Key : null;
        }

        #endregion Monobehaviour
        //################################################################
        //########################### TRACKING ###########################
        //################################################################
        #region Tracking

        private void LoadTrackerSettings()
        {
            //Load tracker data
            SimpleJSON.JSONNode hostfile = new SimpleJSON.JSONClass();
            bool loaded = false;

            if (!Application.isMobilePlatform && Application.platform != RuntimePlatform.WebGLPlayer && useSystemIO)
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

        private void trackSceneChange(IChapterTarget target)
        {
            if (!string.IsNullOrEmpty(target.getXApiClass()) && target.getXApiClass() == "accesible")
            {
                TrackerAsset.Instance.Accessible.Accessed(target.getId(), ExParsers.ParseEnum<AccessibleTracker.Accessible>(target.getXApiType()));
                TrackerAsset.Instance.Flush();
            }

            CompletablesController.Instance.TargetChanged(target);
        }

        #endregion Tracking

        //#################################################################
        //########################### RENDERING ###########################
        //#################################################################
        #region Rendering


        public IRunnerChapterTarget RunTarget(string scene_id, Interactuable notifyObject, bool trace = true)
        {
            return RunTarget(scene_id, 0, 0, notifyObject, trace);
        }

        public IRunnerChapterTarget RunTarget(string scene_id, int transition_time = 0, int transition_type = 0, Interactuable notifyObject = null, bool trace = true)
        {
            Debug.Log("Run target: " + scene_id);
            GUIManager.Instance.ShowHand(false);
            MenuMB.Instance.hide(true);

            IChapterTarget target = GameState.GetChapterTarget(scene_id);

            if (scene_id == GameState.CurrentTarget)
            {
                // TODO TRANSITIONS
                runnerTarget.RenderScene();
            }
            else
            {
                if (runnerTarget != null)
                {
                    runnerTarget.Destroy(transition_time / 1000f);
                }

                // Here we connect with the IChapterTargetFactory and create an IRunnerChapterTarget

                runnerTarget = RunnerChapterTargetFactory.Instance.Instantiate(target);
                runnerTarget.Data = target;
                GameState.CurrentTarget = target.getId();
            }

            if (!trace)
            {
                trackSceneChange(target);
            }

            waitingRunTarget = true;
            if(notifyObject != null)
            {
                executeStack.Push(new KeyValuePair<Interactuable, ExecutionEvent>(notifyObject, null));
            }
            uAdventureRaycaster.Instance.Override = this.gameObject;
            
            return runnerTarget;
        }

        public void reRenderScene()
        {
            if (runnerTarget != null)
            {
                runnerTarget.RenderScene();
            }
        }

        public void SwitchToLastTarget()
        {
            GeneralScene scene = GameState.GetLastScene();

            if (scene != null)
            {
                RunTarget(scene.getId());
            }
        }

        #endregion Rendering
        //#################################################################
        //#################################################################
        //#################################################################
        #region Misc
        public void showActions(List<Action> actions, Vector2 position, IActionReceiver actionReceiver = null)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            MenuMB.Instance.transform.position = new Vector3(pos.x, pos.y, pos.z) + mouseRay.direction.normalized * 10;
            MenuMB.Instance.transform.rotation = Camera.main.transform.rotation;
            MenuMB.Instance.setActions(actions, actionReceiver);
            MenuMB.Instance.show();
            // TODO why isnt position used?
            //this.clicked_on = position;
        }

        public void showOptions(ConversationNodeHolder options)
        {
            var optionsNode = options.getNode() as OptionConversationNode;
            if (optionsNode != null)
            {
                // Disable the UI interactivity
                uAdventureRaycaster.Instance.enabled = false;
                InventoryManager.Instance.Show = false;

                // Enable blurred background
                blur = GameObject.Instantiate(Blur_Prefab);
                blur.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 1);

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
            }
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
            GUIManager.Instance.Talk(text, character);
        }

        public void ShowBook(string bookId)
        {
            guistate = GUIState.BOOK;
            bookDrawer.Book = GameState.FindElement<Book>(bookId);
            bookDrawer.RefreshResources();
        }

        public bool ShowingBook
        {
            get
            {
                return bookDrawer.Book != null;
            }
        }

        protected void OnGUI()
        {

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
                        skin.box.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.button.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.label.fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.GetStyle("optionLabel").fontSize = Mathf.RoundToInt(guiscale * 36);
                        skin.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);
                        skin.GetStyle("emptyProgressBar").fontSize = Mathf.RoundToInt(guiscale * 20);

                        using (new GUILayout.AreaScope(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f)))
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                OptionConversationNode options = (OptionConversationNode)guioptions.getNode();

                                if (options.isKeepShowing())
                                {
                                    var text = GUIManager.Instance.Last;
                                    if (text[0] == '#')
                                    {
                                        text = text.Remove(0, Mathf.Max(0, text.IndexOf(' ') + 1));
                                    }

                                    var textContent = new GUIContent(text);
                                    var textRect = GUILayoutUtility.GetRect(textContent, "optionLabel");

                                    GUIUtil.DrawTextBorder(textRect, textContent, Color.black, "optionLabel");
                                    GUIUtil.DrawText(textRect, textContent, ((GUIStyle)"optionLabel").normal.textColor, "optionLabel");
                                }
                                foreach (var i in order)
                                {
                                    ConversationLine ono = options.getLine(i);
                                    if (ConditionChecker.check(options.getLineConditions(i)) && GUILayout.Button(ono.getText()))
                                    {
                                        OptionSelected(i);
                                    }
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
                        }
                    }
                    break;
                default: break;
            }
        }

        private void OptionSelected(int i)
        {
            doTimeOut = false;
            GameObject.Destroy(blur);
            guioptions.clicked(i);
            uAdventureRaycaster.Instance.enabled = true;
            InventoryManager.Instance.Show = true;
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

        #endregion Misc

    }
}