using UnityEngine;
using UnityEditor;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class NewGameWindow : LayoutWindow//, DialogReceiverInterface
    {
        public enum GameType
        {
            FPS = 0,
            TPS = 1
        };

        public Vector2 scrollPositionButtons;
        public Vector2 scrollPositionInfo;
        private readonly Texture2D newGameButtonFPSImage = null;
        private readonly Texture2D newGameButtonTPSImage = null;
        private readonly Texture2D newGameScreenFPSImage = null;
        private readonly Texture2D newGameScreenTPSImage = null;
        private Rect bottomButtonRect;

        private readonly string infoFPS = "StartDialog.NewAdventure-TransparentMode.Description";
        private readonly string infoTPS = "StartDialog.NewAdventure-VisibleMode.Description";
        
        public static GameType selectedGameType;

        public NewGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            newGameButtonFPSImage = Resources.Load<Texture2D>("EAdventureData/img/icons/newAdventureTransparentMode65");
            newGameButtonTPSImage = Resources.Load<Texture2D>("EAdventureData/img/icons/newAdventureNormalMode65");
            newGameScreenFPSImage = Resources.Load<Texture2D>("EAdventureData/help/common_img/fireProtocol");
            newGameScreenTPSImage = Resources.Load<Texture2D>("EAdventureData/help/common_img/1492");
        }

        public override void Draw(int aID)
        {
            var windowWidth = Rect.width;
            var windowHeight = Rect.height;
            
            bottomButtonRect = new Rect(0.8f * windowWidth, 0.9f * windowHeight, 0.15f * windowWidth, 0.1f * windowHeight);

            GUILayout.BeginHorizontal();
            {
                scrollPositionButtons = GUILayout.BeginScrollView(scrollPositionButtons, GUILayout.Width(windowWidth * 0.3f),
                    GUILayout.Height(0.8f * windowHeight));
                if (GUILayout.Button(newGameButtonFPSImage))
                {
                    selectedGameType = GameType.FPS;
                }
                if (GUILayout.Button(newGameButtonTPSImage))
                {
                    selectedGameType = GameType.TPS;
                }
                GUILayout.EndScrollView();

                scrollPositionInfo = GUILayout.BeginScrollView(scrollPositionInfo, GUILayout.Width(windowWidth * 0.68f),
                    GUILayout.Height(0.8f * windowHeight));
                if (selectedGameType == GameType.FPS)
                {
                    GUILayout.Label(infoFPS.Traslate());
                    GUILayout.Box(newGameScreenFPSImage);
                }
                else
                {
                    GUILayout.Label(infoTPS.Traslate());
                    GUILayout.Box(newGameScreenTPSImage);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(bottomButtonRect);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("New"))
            {
                CreateNewAdventure();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void CreateNewAdventure()
        {
            int type;
            switch (selectedGameType)
            {
                default:
                    Debug.LogError("Wrong adventure type selected");
                    return;
                case GameType.FPS: type = Controller.FILE_ADVENTURE_1STPERSON_PLAYER; break;
                case GameType.TPS: type = Controller.FILE_ADVENTURE_3RDPERSON_PLAYER; break;
            }

            if (EditorUtility.DisplayDialog("Warning", "Creating a new adventure deletes all previous existing files. Do you want to continue?", "Yes", "No"))
            {
                Controller.Instance.NewAdventure(type);
                Controller.OpenEditorWindow();
                WelcomeWindow.Instance.Close();
                uAdventureWindowMain.Instance.RefreshWindows();
            }
        }
    }
}