using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Linq;
using uAdventure.Analytics;
using uAdventure.Geo;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace uAdventure.Editor
{
    public class EditorWindowBase : EditorWindow
    {
        /* -----------------------
         * WINDOW CONSTS
         * ----------------------*/

        public const float LEFT_MENU_WIDTH = 200;
        public const float TOP_MENU_HEIGHT = 15;

        /* -----------------------
         * END WINDOW CONSTS
         * ----------------------*/

        public enum EditorMenuItem
        {
            File,
            Edit,
            Adventure,
            Chapters,
            Run,
            Configuration,
            About
        };

        public enum EditorWindowType
        {
            Chapter,
            AdaptationProfiles,
            Extension
        };

        // The position of the window
        private static EditorWindowBase thisWindowReference;
        public static bool WantsMouseMove = false;


        private static WindowMenuContainer fileMenu,
            editMenu,
            adventureMenu,
            chaptersMenu,
            configurationMenu,
            aboutMenu;

        private EditorWindowType openedWindow = EditorWindowType.Chapter;

        private LayoutWindow m_Window = null;
        private ChapterWindow chapterWindow;
        private GameObject debugGUIHolder = null;

        private Vector2 scrollPosition;

        private static Texture2D redoTexture = null;
        private static Texture2D undoTexture = null;
        
        private static Texture2D adaptationTexture = null;

        private List<EditorWindowExtension> extensions;
        private EditorWindowExtension extensionSelected;

        private static Rect zeroRect;
        private Rect windowArea;

        private static bool locked;

        private void Return(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {

                if (debugGUIHolder)
                {
                    DestroyImmediate(debugGUIHolder);
                }

                FocusWindowIfItsOpen(GetType());
            }
        }

        public static bool Locked
        {
            get { return locked; }
        }

        public static void LockWindow()
        {
            locked = true;
        }

        public static void UnlockWindow()
        {
            locked = false;
        }

        public void OnEnable()
        {
            if (!thisWindowReference)
            {
                thisWindowReference = this;
                EditorApplication.playModeStateChanged += Return;
            }
            else
            {
                DestroyImmediate(thisWindowReference);
                return;
            }

            if (!Language.Initialized)
                Language.Initialize();

            if (!Controller.Instance.Initialized)
			{
				Controller.ResetInstance();
				Controller.Instance.Init();
			}

            var initialScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/uAdventure/Scenes/_Scene1.unity");

            if (initialScene)
            {
                EditorSceneManager.playModeStartScene = initialScene;
            }
            else
            {
                var title = "EditorWindow.MainSceneNotFound.Title".Traslate();
                var body = "EditorWindow.MainSceneNotFound.Body".Traslate();
                var ok = "GeneralText.OK".Traslate();
                EditorUtility.DisplayDialog(title, body, ok);
            }


            if (!redoTexture)
            {
                redoTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/redo");
            }
            if (!undoTexture)
            {
                undoTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/undo");
            }
            if (!adaptationTexture)
            {
                adaptationTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/adaptationProfiles");
            }
			
			fileMenu = new FileMenu();
			editMenu = new EditMenu();
			adventureMenu = new AdventureMenu();
			chaptersMenu = new ChaptersMenu();
			configurationMenu = new ConfigurationMenu();
			aboutMenu = new AboutMenu();

            var g = GeoController.Instance;
            var a = AnalyticsController.Instance;
        }

        protected void OnDestroy()
        {
            if(thisWindowReference == this)
            {
                EditorApplication.playModeStateChanged -= Return;
            }

            if (debugGUIHolder)
            {
                DestroyImmediate(debugGUIHolder);
            }
        }

        public static void RefreshLanguage(){
			thisWindowReference.OnEnable ();
		}

        private static Dictionary<Type, List<EditorComponent>> knownComponents;
        public static Dictionary<Type, List<EditorComponent>> Components
        {
            get { return knownComponents; }
        }

        public static void RegisterComponent<T>(EditorComponent component) { RegisterComponent(typeof(T), component); }

        public static void RegisterComponent(Type t, EditorComponent component)
        {
            if (knownComponents == null)
            {
                knownComponents = new Dictionary<Type, List<EditorComponent>>();
            }

            if (!knownComponents.ContainsKey(t))
            {
                knownComponents.Add(t, new List<EditorComponent>());
            }

            if(component is BaseWindow)
            {
                (component as BaseWindow).OnRequestRepaint = () => thisWindowReference.Repaint();
            }

            // if there's a component of the same tipe already registered we ignore it
            if (knownComponents[t].Any(c => c.GetType() == component.GetType()))
            {
                return;
            }

            knownComponents[t].Add(component);
            knownComponents[t].Sort((c1, c2) => CompareComponents(c1, c2));
        }

        private static int CompareComponents(EditorComponent c1, EditorComponent c2)
        {
            var c1Attr = c1.GetType().GetCustomAttributes(typeof(EditorComponentAttribute), true)[0] as EditorComponentAttribute;
            var c2Attr = c2.GetType().GetCustomAttributes(typeof(EditorComponentAttribute), true)[0] as EditorComponentAttribute;

            return c1Attr.Order.CompareTo(c2Attr.Order);
        } 

        public static void RefreshChapter()
        {
            thisWindowReference.chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");
            thisWindowReference.openedWindow = EditorWindowType.Chapter;
        }

        public static void RefreshWindows()
        {
            thisWindowReference.chapterWindow = null;
            thisWindowReference.InitWindows();
        }

        public static void SelectElement(List<Searchable> path)
        {
            var extension = EditorWindowBaseExtensionFactory.Instance.GetExistingExtensionFor(path.First().GetType(), 
                thisWindowReference.extensions);

            
            if (extension != null)
            {
                thisWindowReference.RequestMainView(extension);
                extension.SelectElement(path);
            }
        }

        void InitWindows()
        {
			if (chapterWindow == null)
            {
				zeroRect = new Rect(0, 0, 0, 0);    
				chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");
                thisWindowReference.openedWindow = EditorWindowType.Chapter;

                // Extensions of the editor
                extensions = EditorWindowBaseExtensionFactory.Instance.CreateAllExistingExtensions(zeroRect, "Window");

				var ops = new GUILayoutOption[] 
                {
					GUILayout.ExpandWidth(true),
					GUILayout.ExpandHeight(true)
				};
				foreach (var e in extensions)
				{
					e.Options = ops;
					e.OnRequestMainView += thisWindowReference.RequestMainView;
					e.OnRequestRepaint += Repaint;
                    e.EndWindows = EndWindows;
                    e.BeginWindows = BeginWindows;
                }   
			}
		}

        protected void OnGUI()
        {
            if (locked)
            {
                GUI.depth = Int32.MaxValue;
                if (Event.current.type != EventType.Layout && Event.current.type != EventType.Repaint)
                {
                    Event.current.Use();
                }
            }
            else
            {
                GUI.depth = 0;
            }

            this.wantsMouseMove = WantsMouseMove;

            InitWindows ();
            /**
            UPPER MENU
            */
            EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(TOP_MENU_HEIGHT));
            if (GUILayout.Button(TC.get("MenuFile.Title"),"toolbarButton"))
            {
                fileMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("MenuEdit.Title"), "toolbarButton"))
            {
                editMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("MenuAdventure.Title"), "toolbarButton"))
            {
                adventureMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("MenuChapters.Title"), "toolbarButton"))
            {
                chaptersMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("MenuConfiguration.Title"), "toolbarButton"))
            {
                configurationMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("About"), "toolbarButton"))
            {
                aboutMenu.menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            /**
            LEFT MENU
            */
            if (Controller.Instance.Loaded)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_MENU_WIDTH), GUILayout.ExpandHeight(true));

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                // Button event chapter
                if (GUILayout.Button(TC.get("Element.Name0")))
                {
                    OnWindowTypeChanged(EditorWindowType.Chapter);
                }

                // Button event scene
                extensions.ForEach(e => e.LayoutDrawLeftPanelContent(null, null));

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                /**
                WINDOWS
                */

                windowArea = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                switch (openedWindow)
                {
                    case EditorWindowType.Chapter:
                        m_Window = chapterWindow;
                        break;
                    default:
                        if (extensionSelected != null)
                        {
                            m_Window = extensionSelected;
                        }
                        break;
                }

                if (m_Window != null)
                {
                    if (Event.current.type == EventType.Repaint && m_Window.Rect != windowArea)
                    {
                        m_Window.Rect = windowArea;
                        extensions.ForEach(e => e.Rect = windowArea);
                    }
                    m_Window.OnGUI();
                }
 
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();


                var unclippedDrawReceiver = m_Window as IUnclippedDrawReceiver;
                if (unclippedDrawReceiver != null)
                {
                    unclippedDrawReceiver.UnclippedDraw(windowArea);
                }
            }
            else
            {
                GUILayout.Label("EditorWindow.NotLoaded".Traslate());
                if (Controller.Instance.HasError)
                {
                    EditorGUILayout.TextArea(Controller.Instance.Error);
                }

                if (GUILayout.Button("EditorWindow.OpenWelcome".Traslate()))
                {
                    Controller.OpenWelcomeWindow();
                }
                if (GUILayout.Button("EditorWindow.Reload".Traslate()))
                {
                    Controller.ResetInstance();
                    Controller.Instance.Init();
                    EditorWindowBase.RefreshWindows();
                }
                if (GUILayout.Button("GeneralText.New".Traslate()))
                {
                    var title = "EditorWindow.CreateNew.Title".Traslate();
                    var body = "EditorWindow.CreateNew.Body".Traslate();
                    var yes = "GeneralText.Yes".Traslate();
                    var no = "GeneralText.No".Traslate();

                    if (EditorUtility.DisplayDialog(title, body, yes, no))
                    {
                        Controller.Instance.NewAdventure(Controller.FILE_ADVENTURE_1STPERSON_PLAYER);
                        Controller.OpenEditorWindow();
                        EditorWindowBase.RefreshWindows();
                    }
                }
            }
        }

        void OnWindowTypeChanged(EditorWindowType type_)
        {
            openedWindow = type_;
        }

        void RequestMainView(EditorWindowExtension who)
        {
            extensionSelected = who;
            OnWindowTypeChanged(EditorWindowType.Extension);
            extensions.ForEach(e => e.Selected = e == who);
        }

        internal static void LanguageChanged()
        {
            if (thisWindowReference)
            {
                thisWindowReference.OnEnable();
            }
        }
    }
}