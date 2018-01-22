using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;

namespace uAdventure.Runner
{
    public enum guiState
    {
        GAME_SELECTION, LOADING_GAME, NOTHING, TALK_PLAYER, TALK_CHARACTER, OPTIONS_MENU, ANSWERS_MENU
    }

    public class Game : MonoBehaviour
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

        static string gameToLoad = "";
        public static string GameToLoad
        {
            get { return gameToLoad; }
            set { gameToLoad = value; }
        }
        #endregion Singleton
        //#####################################################################
        //########################### MONOBEHAVIOUR ###########################
        //#####################################################################
        #region Monobehaviour

        public bool useSystemIO = true, forceScene = false, editor_mode = true;
        private GUISkin style;
        public string gamePath = "", gameName = "", scene_name = "";
        public GameObject Blur_Prefab;
        MenuMB menu;
        Interactuable next_interaction = null;
        IRunnerChapterTarget runnerTarget;
        GameState game_state;

        public GUISkin Style
        {
            get { return style; }
        }

        public GameState GameState
        {
            get { return game_state; }
        }

        public ResourceManager ResourceManager { get; private set; }

        public string getGameName()
        {
            return gameName;
        }

        public string getSelectedGame()
        { 
            return gameName;
        }
        public string getSelectedPath()
        {
            return gamePath;
        }

        void Awake()
        {
            Game.instance = this;
            //Load tracker data
            SimpleJSON.JSONNode hostfile = new SimpleJSON.JSONClass();

            bool loaded = false;

#if UNITY_WEBPLAYER || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IPHONE
#else
            if (useSystemIO)
            {
                if (!System.IO.File.Exists("host.cfg"))
                {
                    hostfile.Add("host", new SimpleJSON.JSONData("http://192.168.175.117:3000/api/proxy/gleaner/collector/"));
                    hostfile.Add("trackingCode", new SimpleJSON.JSONData("57d81d5585b094006eab04d6ndecvjlvjss8aor"));
                    System.IO.File.WriteAllText("host.cfg", hostfile.ToString());
                }
                else
                    hostfile = SimpleJSON.JSON.Parse(System.IO.File.ReadAllText("host.cfg"));
                loaded = true;
            }
#endif
            try
            {
                if (loaded)
                {
                    Tracker.T.host = hostfile["host"];
                    Tracker.T.trackingCode = hostfile["trackingCode"];
                    //End tracker data loading
                }
            }
            catch { }

            style = Resources.Load("basic") as GUISkin;
            optionlabel = new GUIStyle(style.label);

            if (Game.GameToLoad != "")
            {
                gameName = Game.GameToLoad;
                gamePath = ResourceManager.getCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar;
                useSystemIO = true;
            }

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

            // TODO incidences are unused, why?
            //List<Incidence> incidences = new List<Incidence>();

            AdventureData data = new AdventureData();
            AdventureHandler adventure = new AdventureHandler(data, ResourceManager);
            adventure.Parse("descriptor.xml");
            game_state = new GameState(data);
            CompletableController.Instance.setCompletables(GameState.getCompletables());
        }

        void Start()
        {
            if (!forceScene)
                RunTarget(GameState.getInitialChapterTarget().getId());
            else
                RunTarget(scene_name);

            TimerController.Instance.Timers = GameState.getTimers();
            TimerController.Instance.Run();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.timeScale == 1)
                {
                    if (next_interaction != null && guistate != guiState.ANSWERS_MENU)
                    {
                        Interacted();
                    }
                    else if (guistate == guiState.NOTHING)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        List<RaycastHit> hits = new List<RaycastHit>(Physics.RaycastAll(ray));
                        //hits.Reverse ();

                        bool no_interaction = true;
                        foreach (RaycastHit hit in hits)
                        {
                            Interactuable interacted = hit.transform.GetComponent<Interactuable>();
                            if (interacted != null && InteractWith(interacted))
                            {
                                trackInteraction(interacted);
                                no_interaction = false;
                                break;
                            }
                        }

                        if (no_interaction)
                            runnerTarget.Interacted();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                MenuMB.Instance.hide();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    GUIManager.Instance.showConfigMenu();
                }
                else
                {
                    Time.timeScale = 0;
                    GUIManager.Instance.showConfigMenu();
                }
            }
        }

        private bool InteractWith(Interactuable interacted)
        {
            bool exit = false;
            next_interaction = null;
            switch (interacted.Interacted())
            {
                case InteractuableResult.DOES_SOMETHING:
                    exit = true;
                    break;
                case InteractuableResult.REQUIRES_MORE_INTERACTION:
                    exit = true;
                    next_interaction = interacted;
                    break;
                case InteractuableResult.IGNORES:
                default:
                    break;
            }
            return exit;
        }

        public bool Execute(Interactuable interactuable)
        {
            MenuMB.Instance.hide(true);
            if (interactuable.Interacted() == InteractuableResult.REQUIRES_MORE_INTERACTION)
            {
                this.next_interaction = interactuable;
                return true;
            }
            return false;
        }

        private bool Interacted()
        {
            guistate = guiState.NOTHING;
            GUIManager.Instance.destroyBubbles();
            if (this.next_interaction != null)
            {
                Interactuable tmp = next_interaction;
                next_interaction = null;
                return Execute(tmp);
            }
            return false;
        }

        public bool isSomethingRunning()
        {
            return next_interaction != null;
        }

        public Interactuable getNextInteraction()
        {
            return next_interaction;
        }

        #endregion Monobehaviour
        //################################################################
        //########################### TRACKING ###########################
        //################################################################
        #region Tracking

        IChapterTarget alternative;

        // TODO ?????
        //List<GeneralScene> finalprogress = new List<GeneralScene>();

        public IChapterTarget getAlternativeTarget()
        {
            return alternative;
        }

        private void trackSceneChange(IChapterTarget target)
        {
            alternative = null;

            if (!string.IsNullOrEmpty(target.getXApiClass()))
            {
                if (target.getXApiClass() == "accesible")
                {
                    Tracker.T.accessible.Accessed(target.getId(), ParseEnum<AccessibleTracker.Accessible>(target.getXApiType()));
                }
                else if (target.getXApiClass() == "alternative")
                {
                    alternative = target;
                }
            }

            CompletableController.Instance.targetChanged(target);

            Tracker.T.RequestFlush();
        }

        private void trackInteraction(Interactuable with)
        {
            switch (with.GetType().ToString())
            {
                case "ActiveAreaMB": Tracker.T.trackedGameObject.Interacted(((ActiveAreaMB)with).aaData.getId(), GameObjectTracker.TrackedGameObject.Npc); break;
                case "CharacterMB": Tracker.T.trackedGameObject.Interacted(((Representable)with).Element.getId(), GameObjectTracker.TrackedGameObject.Npc); break;
                case "ObjectMB": Tracker.T.trackedGameObject.Interacted(((Representable)with).Element.getId(), GameObjectTracker.TrackedGameObject.Item); break;
            }

            Tracker.T.RequestFlush();
        }

        #endregion Tracking
        //#################################################################
        //########################### RENDERING ###########################
        //#################################################################
        #region Rendering
        public static T ParseEnum<T>(string value)
        {
            return (T)System.Enum.Parse(typeof(T), value, true);
        }

        public IRunnerChapterTarget RunTarget(string scene_id, int transition_time = 0, int transition_type = 0)
        {
            MenuMB.Instance.hide(true);
            if (runnerTarget != null)
            {
                runnerTarget.Destroy(transition_time / 1000f);
            }

            // Here we connect with the IChapterTargetFactory and create an IRunnerChapterTarget
            IChapterTarget target = GameState.getChapterTarget(scene_id);

            runnerTarget = RunnerChapterTargetFactory.Instance.Instantiate(target);

            runnerTarget.Data = target;

            trackSceneChange(target);
            
            GameState.CurrentTarget = target.getId();

            return runnerTarget;
        }

        public void reRenderScene()
        {
            if (runnerTarget != null)
                runnerTarget.RenderScene();
        }

        public void SwitchToLastTarget()
        {
            GeneralScene scene = GameState.getLastScene();

            if (scene != null)
                RunTarget(scene.getId());
        }
        #endregion Rendering
        //#################################################################
        //#################################################################
        //#################################################################
        #region Misc
        public void showActions(List<Action> actions, Vector2 position/*, SceneElement shower = null*/)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            MenuMB.Instance.transform.position = new Vector3(pos.x, pos.y, pos.z) + mouseRay.direction.normalized * 10;
            MenuMB.Instance.transform.rotation = Camera.main.transform.rotation;
            MenuMB.Instance.setActions(actions);
            MenuMB.Instance.show();
            // TODO why isnt position used?
            //this.clicked_on = position;
        }

        GameObject blur;
        public void showOptions(ConversationNodeHolder options)
        {
            if (options.getNode().getType() == ConversationNodeViewEnum.OPTION)
            {
                blur = GameObject.Instantiate(Blur_Prefab);
                blur.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + 1);
                this.guioptions = options;
                this.guistate = guiState.ANSWERS_MENU;
            }
        }
        
        public void talk(string text, string character)
        {
            GUIManager.Instance.Talk(text, character);
        }
        
        private guiState guistate = guiState.NOTHING;
        private List<Action> guiactions;
        private ConversationNodeHolder guioptions;

        GUIStyle optionlabel;

        void OnGUI()
        {
            float guiscale = Screen.width / 800f;
            style.box.fontSize = Mathf.RoundToInt(guiscale * 20);
            style.button.fontSize = Mathf.RoundToInt(guiscale * 20);
            style.label.fontSize = Mathf.RoundToInt(guiscale * 20);
            optionlabel.fontSize = Mathf.RoundToInt(guiscale * 36);
            style.GetStyle("talk_player").fontSize = Mathf.RoundToInt(guiscale * 20);
            //float rectwith = guiscale * 330;

            switch (guistate)
            {
                case guiState.ANSWERS_MENU:
                    GUILayout.BeginArea(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f));
                    GUILayout.BeginVertical();
                    OptionConversationNode options = (OptionConversationNode)guioptions.getNode();

                    GUILayout.Label(GUIManager.Instance.Last, optionlabel);
                    for (int i = 0; i < options.getLineCount(); i++)
                    {
                        ConversationLine ono = options.getLine(i);
                        if (ConditionChecker.check(options.getLineConditions(i)))
                            if (GUILayout.Button((string)ono.getText(), style.button))
                            {
                                GameObject.Destroy(blur);
                                guioptions.clicked(i);
                                /*Tracker.T ().Choice (GUIManager.Instance.Last, ono.getText ());
                                Tracker.T ().RequestFlush ();*/
                                Interacted();
                            }
                        ;
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndArea();
                    break;
                default: break;
            }
        }
        #endregion Misc
    }
}