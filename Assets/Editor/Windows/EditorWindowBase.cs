using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.QR;
using System;

namespace uAdventure.Editor
{
    public class EditorWindowBase : EditorWindow
    {
        /* -----------------------
         * WINDOW CONSTS
         * ----------------------*/

        public const float LEFT_MENU_WIDTH = 250;
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


        private static WindowMenuContainer fileMenu,
            editMenu,
            adventureMenu,
            chaptersMenu,
            runMenu,
            configurationMenu,
            aboutMenu;

        private EditorWindowType openedWindow = EditorWindowType.Chapter;

        private LayoutWindow m_Window = null;
        private ChapterWindow chapterWindow;
        //private static AdaptationProfileWindow adapatationProfileWindow;

        private Vector2 scrollPosition;

        private static Texture2D redoTexture = null;
        private static Texture2D undoTexture = null;
        
        private static Texture2D adaptationTexture = null;
        
        private static GUIContent leftMenuContentAdaptation;

        private List<EditorWindowExtension> extensions;
        private EditorWindowExtension extensionSelected;

        private static Rect zeroRect;
        private Rect windowArea;

        [MenuItem("eAdventure/Open eAdventure editor")]
        public static void Init()
        {
            if (!Controller.getInstance().Initialized())
            {
                Controller.resetInstance();
                Language.Initialize();
                Controller.getInstance().init();
            }


            thisWindowReference = EditorWindow.GetWindow<EditorWindowBase>();

            redoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/redo", typeof(Texture2D));
            undoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/undo", typeof(Texture2D));
            
            adaptationTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/adaptationProfiles", typeof(Texture2D));

            thisWindowReference.InitGUI();
            thisWindowReference.Show();

            fileMenu = new FileMenu();
            editMenu = new EditMenu();
            adventureMenu = new AdventureMenu();
            chaptersMenu = new ChaptersMenu();
            runMenu = new RunMenu();
            configurationMenu = new ConfigurationMenu();
            aboutMenu = new AboutMenu();
        }

        public void InitGUI()
        {
            zeroRect = new Rect(0, 0, 0, 0);

            chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");

            // Extensions of the editor
            extensions = EditorWindowBaseExtensionFactory.Instance.CreateAllExistingExtensions(zeroRect, "Window");
            /*extensions.Add(new ScenesWindow(windowRect, new GUIContent(TC.get("Element.Name1")), "Window"));
            extensions.Add(new CutscenesWindow(windowRect, new GUIContent(TC.get("Element.Name9")), "Window"));
            extensions.Add(new BooksWindow(windowRect, new GUIContent(TC.get("Element.Name11")), "Window"));
            extensions.Add(new ItemsWindow(windowRect, new GUIContent(TC.get("Element.Name18")), "Window"));
            extensions.Add(new SetItemsWindow(windowRect, new GUIContent(TC.get("Element.Name59")), "Window"));
            extensions.Add(new PlayerWindow(windowRect, new GUIContent(TC.get("Element.Name26")), "Window"));
            extensions.Add(new CharactersWindow(windowRect, new GUIContent(TC.get("Element.Name27")), "Window"));
            extensions.Add(new ConversationWindow(windowRect, new GUIContent(TC.get("Element.Name31")), "Window"));
            extensions.Add(new AdvencedFeaturesWindow(windowRect, new GUIContent(TC.get("AdvancedFeatures.Title")), "Window"));
            extensions.Add(new AssesmentProfileWindow(windowRect, new GUIContent(Language.GetText("ASSESMENT_PROFILES")), "Window"));
            extensions.Add(new MapSceneWindow(windowRect, new GUIContent("MapSceneWindow"), "Window"));
            extensions.Add(new GeoElementWindow(windowRect, new GUIContent("GeoElements"), "Window"));
            extensions.Add(new QRCodeEditorWindow(windowRect, new GUIContent("QR Codes"), "Window"));*/

            var ops = new GUILayoutOption[] {
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true)
                };
            foreach (var e in extensions)
            {
                e.Options = ops;
                e.OnRequestMainView += (thisWindowReference as EditorWindowBase).RequestMainView;
                e.OnRequestRepaint += thisWindowReference.Repaint;
            }
            //adapatationProfileWindow = new AdaptationProfileWindow(windowRect,
            //    new GUIContent(Language.GetText("ADAPTATION_PROFILES")), "Window");
            
        }

        public static void RefreshChapter()
        {
            thisWindowReference.chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");
            thisWindowReference.openedWindow = EditorWindowType.Chapter;
        }

        void OnGUI()
        {
            /**
            UPPER MENU
            */
            EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(TOP_MENU_HEIGHT));
            if (GUILayout.Button(TC.get("MenuFile.Title"),"toolbarButton"))
            {
                fileMenu.menu.ShowAsContext();
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
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            var leftMenuRect = EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_MENU_WIDTH), GUILayout.ExpandHeight(true));

            //EditorGUILayout.BeginHorizontal(EditorGUILayout.MaxWidth(25), EditorGUILayout.MaxHeight(25));
            //if (EditorGUILayout.Button(undoTexture, EditorGUILayout.MaxWidth(25), EditorGUILayout.MaxHeight(25)))
            //{
            //    UndoAction();
            //}

            //EditorGUILayout.Space(5);

            //if (EditorGUILayout.Button(redoTexture, EditorGUILayout.MaxWidth(25), EditorGUILayout.MaxHeight(25)))
            //{
            //    RedoAction();
            //}
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.Space(25);

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
            /*if (windowArea != null)
            {
                windowArea = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                Debug.Log(Event.current.type + " " + windowArea);
            }*/
            //GUI.BeginGroup(windowArea);
            GUILayout.Label("Here should be windows");
            BeginWindows();
            //extensionSelected.OnGUI();
            
            switch (openedWindow)
            {
                case EditorWindowType.Chapter:
                    m_Window = chapterWindow;
                    break;
                default:
                    if (extensionSelected != null)
                        m_Window = extensionSelected;
                    break;
            }

            if (m_Window != null)
            {
                //leftMenuRect
                if(Event.current.type == EventType.repaint)
                {
                    m_Window.Rect = windowArea;
                    extensions.ForEach(e => e.Rect = windowArea);
                }

                m_Window.OnGUI();
            }
            EndWindows();
            //GUI.EndGroup();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        void OnWindowTypeChanged(EditorWindowType type_)
        {
            openedWindow = type_;
        }

        void RedoAction()
        {
            Debug.Log("redo clicked");
            Controller.getInstance().redoTool();
        }

        void UndoAction()
        {
            Debug.Log("undo clicked");
            Controller.getInstance().undoTool();
        }

        // Request

        void RequestMainView(EditorWindowExtension who)
        {
            extensionSelected = who;
            OnWindowTypeChanged(EditorWindowType.Extension);
            extensions.ForEach(e => e.Selected = e == who);
        }

        internal static void LanguageChanged()
        {
            thisWindowReference.InitGUI();
        }
    }
}