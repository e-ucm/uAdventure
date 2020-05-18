using UnityEditor;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class WelcomeWindow : EditorWindow
    {
        public static WelcomeWindow Instance { get { return (WelcomeWindow)thisWindowReference; } }

        public enum WelcomeWindowType { New, Open, Recent };

        // The position of the window
        private static float windowWidth, windowHeight;
        private static Rect windowRect, logoRect, buttonsRect;
        private static EditorWindow thisWindowReference;
        private static Texture2D logo = null;
        private static WelcomeWindowType openedWindow = WelcomeWindowType.New;

        private LayoutWindow m_Window1 = null;
        private static NewGameWindow newGameWindow;
        private static OpenGameWindow openGameWindow;

        // Add menu item 
        private void OnEnable()
        {
            if(!thisWindowReference)
                thisWindowReference = this;
            else
            {
                DestroyImmediate(this);
                return;
            }

            if (!Language.Initialized)
                Language.Initialize();

            openedWindow = WelcomeWindowType.New;

            logo = Resources.Load<Texture2D>("EAdventureData/img/icons/logo-editor");
        }

        public void OnGUI()
        {
            if(newGameWindow == null || openGameWindow == null)
            {
                newGameWindow = new NewGameWindow(windowRect, new GUIContent("New"), "Window");
                openGameWindow = new OpenGameWindow(windowRect, new GUIContent("Open"), "Window");
            }

            windowWidth = position.width;
            windowHeight = position.height;
            
            logoRect = new Rect(0.01f * windowWidth, 0.01f * windowHeight, windowWidth * 0.98f, windowHeight * 0.25f);
            buttonsRect = new Rect(0.01f * windowWidth, 0.27f * windowHeight, windowWidth * 0.98f, windowHeight * 0.28f);
            windowRect = new Rect(0.01f * windowWidth, 0.32f * windowHeight, 0.98f * windowWidth, 0.67f * windowHeight);

            GUI.DrawTexture(logoRect, logo);

            GUILayout.BeginArea(buttonsRect);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("New"))
            {
                OnWindowTypeChanged(WelcomeWindowType.New);
            }
            if (GUILayout.Button("Open"))
            {
                OnWindowTypeChanged(WelcomeWindowType.Open);
                openGameWindow.OpenFileDialog();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            BeginWindows();

            switch (openedWindow)
            {
                case WelcomeWindowType.New:
                    m_Window1 = newGameWindow;
                    break;
                case WelcomeWindowType.Open:
                    m_Window1 = openGameWindow;
                    break;
            }

            if (m_Window1 != null)
            {
                m_Window1.Rect = windowRect;
                m_Window1.OnGUI();
            }
            EndWindows();
        }


        void OnWindowTypeChanged(WelcomeWindowType type_)
        {
            openedWindow = type_;
        }
    }
}