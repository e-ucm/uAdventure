using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.QR;

namespace uAdventure.Editor
{
    public class EditorWindowBase : EditorWindow
    {
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
            completables,
            MapScenes,
            GeoElements,
            AdvancedGeo
        };

        // The position of the window
        private static float windowWidth, windowHeight;
        private static EditorWindow thisWindowReference;
        private static Rect buttonMenuRect, leftMenuRect, windowRect;

        private static WindowMenuContainer fileMenu,
            editMenu,
            adventureMenu,
            chaptersMenu,
            runMenu,
            configurationMenu,
            aboutMenu;

        private static EditorWindowType openedWindow = EditorWindowType.Chapter;

        private LayoutWindow m_Window1 = null;
        private static ChapterWindow chapterWindow;
        private static AdvencedFeaturesWindow completablesWindow;
        //private static AdaptationProfileWindow adapatationProfileWindow;

        private static Vector2 scrollPosition;

        private static Texture2D redoTexture = null;
        private static Texture2D undoTexture = null;
        
        private static Texture2D adaptationTexture = null;
        private static Texture2D completableTexture = null;
        
        private static GUIContent leftMenuContentAdaptation;

        private static List<EditorWindowExtension> extensions;
        private static EditorWindowExtension extensionSelected;

        [MenuItem("eAdventure/Open eAdventure editor")]
        public static void Init()
        {
            if (!Controller.getInstance().Initialized())
            {
                Controller.resetInstance();
                Language.Initialize();
                Controller.getInstance().init();
            }

            thisWindowReference = EditorWindow.GetWindow(typeof(EditorWindowBase));
            windowWidth = EditorWindow.focusedWindow.position.width;
            windowHeight = EditorWindow.focusedWindow.position.height;
            buttonMenuRect = new Rect(0.01f * windowWidth, 0.01f * windowHeight, windowWidth * 0.98f, windowHeight * 0.10f);
            leftMenuRect = new Rect(0.01f * windowWidth, 0.12f * windowHeight, windowWidth * 0.14f, windowHeight * 0.87f);
            windowRect = new Rect(0.16f * windowWidth, 0.12f * windowHeight, windowWidth * 0.83f, windowHeight * 0.85f);

            //leftSubMenuSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkin", typeof(GUISkin));
            //leftSubMenuConcreteItemSkin =(GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

            redoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/redo", typeof(Texture2D));
            undoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/undo", typeof(Texture2D));

            //addTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            //deleteImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            //duplicateImg = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));
            
            adaptationTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/adaptationProfiles", typeof(Texture2D));

            thisWindowReference.Show();

            fileMenu = new FileMenu();
            editMenu = new EditMenu();
            adventureMenu = new AdventureMenu();
            chaptersMenu = new ChaptersMenu();
            runMenu = new RunMenu();
            configurationMenu = new ConfigurationMenu();
            aboutMenu = new AboutMenu();

            extensions = new List<EditorWindowExtension>();

            InitGUI();
        }

        public static void InitGUI()
        {

            chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");

            // Extensions of the editor
            extensions = EditorWindowBaseExtensionFactory.Instance.CreateAllExistingExtensions(windowRect, "Window");
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

            foreach (var e in extensions)
            {
                e.OnRequestMainView += (thisWindowReference as EditorWindowBase).RequestMainView;
                e.OnRequestRepaint += thisWindowReference.Repaint;
            }
            //adapatationProfileWindow = new AdaptationProfileWindow(windowRect,
            //    new GUIContent(Language.GetText("ADAPTATION_PROFILES")), "Window");
            
        }

        public static void RefreshChapter()
        {
            chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");
            openedWindow = EditorWindowType.Chapter;
        }

        void OnGUI()
        {
            /**
            UPPER MENU
            */
            GUILayout.BeginArea(buttonMenuRect);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(TC.get("MenuFile.Title")))
            {
                fileMenu.menu.ShowAsContext();
            }
            //if (GUILayout.Button(Language.GetText("GeneralText.Edit")))
            //{
            //    editMenu.menu.ShowAsContext();
            //}
            //if (GUILayout.Button(Language.GetText("ADVENTURE")))
            //{
            //    adventureMenu.menu.ShowAsContext();
            //}
            if (GUILayout.Button(TC.get("MenuChapters.Title")))
            {
                chaptersMenu.menu.ShowAsContext();
            }
            //if (GUILayout.Button(Language.GetText("RUN")))
            //{
            //    runMenu.menu.ShowAsContext();
            //}
            if (GUILayout.Button(TC.get("MenuConfiguration.Title")))
            {
                configurationMenu.menu.ShowAsContext();
            }
            if (GUILayout.Button(TC.get("About")))
            {
                aboutMenu.menu.ShowAsContext();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            /**
            LEFT MENU
            */
            GUILayout.BeginArea(leftMenuRect);
            GUILayout.BeginVertical();

            //GUILayout.BeginHorizontal(GUILayout.MaxWidth(25), GUILayout.MaxHeight(25));
            //if (GUILayout.Button(undoTexture, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
            //{
            //    UndoAction();
            //}

            //GUILayout.Space(5);

            //if (GUILayout.Button(redoTexture, GUILayout.MaxWidth(25), GUILayout.MaxHeight(25)))
            //{
            //    RedoAction();
            //}
            //GUILayout.EndHorizontal();

            //GUILayout.Space(25);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            // Button event chapter
            if (GUILayout.Button(TC.get("Element.Name0")))
            {
                chapterWindow = new ChapterWindow(windowRect, new GUIContent(TC.get("Element.Name0")), "Window");
                OnWindowTypeChanged(EditorWindowType.Chapter);
            }

            // Button event scene
            extensions.ForEach(e => e.LayoutDrawLeftPanelContent(null, null));

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            /**
            WINDOWS
            */
            BeginWindows();
            //extensionSelected.OnGUI();

            switch (openedWindow)
            {
                case EditorWindowType.Chapter:
                    m_Window1 = chapterWindow;
                    break;
                default:
                    if (extensionSelected != null)
                        m_Window1 = extensionSelected;
                    break;
            }

            if (m_Window1 != null)
                m_Window1.OnGUI();
            EndWindows();
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
            OnWindowTypeChanged(EditorWindowType.MapScenes);
            extensions.ForEach(e => e.Selected = e == who);
        }
    }
}