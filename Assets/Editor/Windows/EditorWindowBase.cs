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

        public void OnEnable()
		{
			if (!thisWindowReference)
				thisWindowReference = this;
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

			if(!redoTexture)
				redoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/redo", typeof(Texture2D));
			if(!undoTexture)
				undoTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/undo", typeof(Texture2D));
			if(!adaptationTexture)
				adaptationTexture = (Texture2D)Resources.Load("EAdventureData/img/icons/adaptationProfiles", typeof(Texture2D));
			
			fileMenu = new FileMenu();
			editMenu = new EditMenu();
			adventureMenu = new AdventureMenu();
			chaptersMenu = new ChaptersMenu();
			runMenu = new RunMenu();
			configurationMenu = new ConfigurationMenu();
			aboutMenu = new AboutMenu();
        }

		public static void RefreshLanguage(){
			thisWindowReference.OnEnable ();
		}

        public static void RefreshChapter()
        {
            thisWindowReference.chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");
            thisWindowReference.openedWindow = EditorWindowType.Chapter;
        }

		void InitWindows(){
			if (chapterWindow == null) {
				zeroRect = new Rect(0, 0, 0, 0);    
				chapterWindow = new ChapterWindow(zeroRect, new GUIContent(TC.get("Element.Name0")), "Window");

				// Extensions of the editor
				extensions = EditorWindowBaseExtensionFactory.Instance.CreateAllExistingExtensions(zeroRect, "Window");

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
			}
		}

        void OnGUI()
		{
			InitWindows ();
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
            if (Controller.Instance.Loaded)
            {
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
                    m_Window.OnGUI();
                    if (Event.current.type == EventType.repaint)
                    {
                        if (m_Window.Rect != windowArea)
                        {
                            // We first draw it with the old size to make the window respond the size change
                            m_Window.OnGUI();
                            m_Window.Rect = windowArea;
                        }
                        extensions.ForEach(e => e.Rect = windowArea);
                    }

                    m_Window.OnGUI();
                    m_Window.OnDrawMoreWindows();
                }
                EndWindows();
            }
            else
            {
                GUILayout.Label("Adventure not loaded.");
                if (GUILayout.Button("Open Welcome Window"))
                {
                    Controller.OpenWelcomeWindow();
                }
                if (GUILayout.Button("Reload"))
                {
                    Controller.ResetInstance();
                    Controller.Instance.Init();
                }
                if (GUILayout.Button("New"))
                {
                    Controller.Instance.NewAdventure(0);
                }
            }
            
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
            Controller.Instance.redoTool();
        }

        void UndoAction()
        {
            Debug.Log("undo clicked");
            Controller.Instance.undoTool();
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
			if(thisWindowReference)
				thisWindowReference.OnEnable();
        }
    }
}