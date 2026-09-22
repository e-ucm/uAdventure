using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public abstract class EditorWindowBase : EditorWindow
    {
        /* -----------------------
         * WINDOW CONSTS
         * ----------------------*/

        public const float LEFT_MENU_WIDTH = 200;
        public const float TOP_MENU_HEIGHT = 15;

        /* -----------------------
         * END WINDOW CONSTS
         * ----------------------*/

        public static bool WantsMouseMove = false;


        private static WindowMenuContainer fileMenu,
            editMenu,
            adventureMenu,
            chaptersMenu,
            configurationMenu,
            aboutMenu;

        protected Enum openedWindow;

        private LayoutWindow m_Window = null;

        private Vector2 scrollPosition;
        private EditorWindowExtension extensionSelected;

        private static Rect zeroRect;
        private Rect windowArea;
        [NonSerialized]
        private bool inited = false;

        public static bool Locked { get; set; }

        protected List<EditorWindowExtension> Extensions { get; set; }

        protected void AddExtension(EditorWindowExtension extension)
        {
            if(Extensions == null)
            {
                Extensions = new List<EditorWindowExtension>();
            }

            if (!Extensions.Contains(extension))
            {
                if(extensionSelected == null)
                {
                    extensionSelected = extension;
                }
                Extensions.Add(extension);
            }
        }

        protected Dictionary<GenericMenu, string> Menus { get; set; }

        protected void AddMenu(GenericMenu menu, string title)
        {
            if (Menus == null)
            {
                Menus = new Dictionary<GenericMenu, string>();
            }

            if (!Menus.ContainsKey(menu))
            {
                Menus.Add(menu, title);
            }
        }

        public static void LockWindow()
        {
            Locked = true;
        }

        public static void UnlockWindow()
        {
            Locked = false;
        }

        public void RefreshWindows()
        {
            InitWindows();

            var ops = new GUILayoutOption[]
            {
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true)
            };
            foreach (var e in Extensions)
            {
                e.Options = ops;
                e.OnRequestMainView += RequestMainView;
                e.OnRequestRepaint += Repaint;
                e.EndWindows = EndWindows;
                e.BeginWindows = BeginWindows;
            }

            inited = true;
        }

        protected abstract void InitWindows();

        protected virtual void OnGUI()
        {
            if (Locked)
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

            if (!inited)
            {
                Debug.Log("InitWindows " + inited);
                RefreshWindows();
            }
            /**
            UPPER MENU
            */
            if(Menus != null && Menus.Count > 0)
            {
                EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.Height(TOP_MENU_HEIGHT));
                foreach (var kv in Menus)
                {
                    if (GUILayout.Button(kv.Value, "toolbarButton"))
                    {
                        kv.Key.ShowAsContext();
                    }
                } 
                EditorGUILayout.EndHorizontal();
            }

            /**
            LEFT MENU
            */
            if (Controller.Instance.Loaded)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                EditorGUILayout.BeginVertical(GUILayout.Width(LEFT_MENU_WIDTH), GUILayout.ExpandHeight(true));

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                DrawLeftMenu();

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();

                /**
                WINDOWS
                */

                windowArea = EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                if (extensionSelected != null)
                {
                    m_Window = extensionSelected;
                }

                if (m_Window != null)
                {
                    if (Event.current.type == EventType.Repaint && m_Window.Rect != windowArea)
                    {
                        m_Window.Rect = windowArea;
                        Extensions.ForEach(e => e.Rect = windowArea);
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
                    RefreshWindows();
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
                        RefreshWindows();
                    }
                }
            }
        }

        protected virtual void DrawLeftMenu()
        {
            // Button event scene
            if (Extensions != null)
            {
                Extensions.ForEach(e => e.LayoutDrawLeftPanelContent(null, null));
            }
        }

        public void RequestMainView(EditorWindowExtension who)
        {
            extensionSelected = who;
            Extensions.ForEach(e => e.Selected = e == who);
        }

        internal static void LanguageChanged()
        {
            //OnEnable();
        }
    }
}