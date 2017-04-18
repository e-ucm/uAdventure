using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System.IO;
using System.Windows.Forms;

namespace uAdventure.Editor
{
    public class NewGameWindow : LayoutWindow//, DialogReceiverInterface
    {
        public enum GameType
        {
            FPS = 0,
            TPS = 1
        };

        private System.Windows.Forms.SaveFileDialog sfd;
        private string selectedGameProjectPath = "";
        
        public NewGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            newGameButtonFPSImage =
                (Texture2D)Resources.Load("EAdventureData/img/newAdventureTransparentMode65", typeof(Texture2D));
            newGameButtonTPSImage =
                (Texture2D)Resources.Load("EAdventureData/img/newAdventureNormalMode65", typeof(Texture2D));
            newGameScreenFPSImage =
                (Texture2D)Resources.Load("EAdventureData/help/common_img/fireProtocol", typeof(Texture2D));
            newGameScreenTPSImage = (Texture2D)Resources.Load("EAdventureData/help/common_img/1492", typeof(Texture2D));

            sfd = new System.Windows.Forms.SaveFileDialog();
        }

        public Vector2 scrollPositionButtons;
        public Vector2 scrollPositionInfo;
        private Texture2D newGameButtonFPSImage = null;
        private Texture2D newGameButtonTPSImage = null;
        private Texture2D newGameScreenFPSImage = null;
        private Texture2D newGameScreenTPSImage = null;
        private Rect screenRect;
        private Rect bottomButtonRect;

        private string infoFPS =
            "You have selected to create a new adventure in 1st person mode (player is not shown). \nThere is no avatar for the player.\nThe player explores the game himself, and transitions between scenes are instantaneous.\n Usually these games are designed using photos to configure the scenes.\nHence the playerinteracts in first person with a very real-looking world, in which you can turn or go back in the scene bi clicking left, right, down, etc.";

        private string infoTPS =
            "You have selected to create a new adventure in 3rd person mode (player is visible).\n The player is represented by an avatar, which is drawn onto the game all the time.\nIt needs some time to go from place to place, and when he speaks the text is displayed just over his head.";

        public static GameType selectedGameType;

        private string newGameName;

        public override void Draw(int aID)
        {
            var windowWidth = Rect.width;
            var windowHeight = Rect.height;

            screenRect = new Rect(0.01f * windowWidth, 0.5f * windowHeight, 0.98f * windowWidth, 0.4f * windowHeight);
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
                    //GUILayout.Label(infoFPS);
                    GUILayout.Label(infoFPS);
                    GUILayout.Box(newGameScreenFPSImage);
                    //GUI.DrawTexture(screenRect, newGameScreenFPSImage);
                    // GUI.DrawTexture(newGameScreenFPSImage);
                }
                else
                {
                    GUILayout.Label(infoTPS);
                    GUILayout.Box(newGameScreenTPSImage);
                    //GUI.DrawTexture(screenRect, newGameScreenTPSImage);
                    //GUILayout.Box(newGameScreenTPSImage, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    // GUI.DrawTexture(newGameScreenTPSImage);
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(bottomButtonRect);
            GUILayout.BeginHorizontal();

            //if (GUILayout.Button(TC.get("GeneralText.New")))
            if (GUILayout.Button("New"))
            {
                //NewGameInputPopup window = (NewGameInputPopup) ScriptableObject.CreateInstance(typeof (NewGameInputPopup));
                //window.Init(this, "Game");
                startNewGame();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void startNewGame()
        {

            int type = -1;
            switch (selectedGameType)
            {
                default:
                case GameType.FPS: type = Controller.FILE_ADVENTURE_1STPERSON_PLAYER; break;
                case GameType.TPS: type = Controller.FILE_ADVENTURE_3RDPERSON_PLAYER; break;
            }

            if (EditorUtility.DisplayDialog("Seguro?", "Crear una nueva aventura borrará todos los archivos antiguos sin vuelta atrás. Seguro que deseas continuar?", "Sí", "Cancelar"))
            {
                Controller.Instance.NewAdventure(type);
                Controller.OpenEditorWindow();
            }
        }

        private void old_startNewGame()
        {
            int type = -1;
            switch (selectedGameType)
            {
                default:
                case GameType.FPS: type = Controller.FILE_ADVENTURE_1STPERSON_PLAYER; break;
                case GameType.TPS: type = Controller.FILE_ADVENTURE_3RDPERSON_PLAYER; break;
            }
            
            Stream myStream = null;
            sfd.InitialDirectory = "c:\\";
            sfd.Filter = "ead files (*.ead) | *.ead |eap files (*.eap) | *.eap | All files(*.*) | *.* ";
            sfd.FilterIndex = 2;
            sfd.RestoreDirectory = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                if ((myStream = sfd.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        // Insert code to read the stream here.
                        selectedGameProjectPath = sfd.FileName;
                        if(GameRources.CreateGameProject(selectedGameProjectPath, type))
                        {
                            GameRources.LoadGameProject(selectedGameProjectPath);
                            Controller.OpenEditorWindow();
                        }
                    }
                    myStream.Dispose();
                }

            }
        }
    }
}