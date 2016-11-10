using UnityEngine;
using UnityEditor;
using System.Collections;

public class NewGameWindow : LayoutWindow//, DialogReceiverInterface
{
    public enum GameType
    {
        FPS = 0,
        TPS = 1
    };

    public NewGameWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {

        newGameButtonFPSImage =
            (Texture2D) Resources.Load("EAdventureData/img/newAdventureTransparentMode65", typeof (Texture2D));
        newGameButtonTPSImage =
            (Texture2D) Resources.Load("EAdventureData/img/newAdventureNormalMode65", typeof (Texture2D));
        newGameScreenFPSImage =
            (Texture2D) Resources.Load("EAdventureData/help/common_img/fireProtocol", typeof (Texture2D));
        newGameScreenTPSImage = (Texture2D) Resources.Load("EAdventureData/help/common_img/1492", typeof (Texture2D));

        screenRect = new Rect(0.01f*m_Rect.width, 0.5f*m_Rect.height, 0.98f*m_Rect.width, 0.4f*m_Rect.height);
        bottomButtonRect = new Rect(0.8f*m_Rect.width, 0.9f*m_Rect.height, 0.15f*m_Rect.width, 0.1f*m_Rect.height);
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
        GUILayout.BeginHorizontal();
        {
            scrollPositionButtons = GUILayout.BeginScrollView(scrollPositionButtons, GUILayout.Width(m_Rect.width*0.3f),
                GUILayout.Height(0.8f*m_Rect.height));
            if (GUILayout.Button(newGameButtonFPSImage))
            {
                selectedGameType = GameType.FPS;
            }
            if (GUILayout.Button(newGameButtonTPSImage))
            {
                selectedGameType = GameType.TPS;
            }
            GUILayout.EndScrollView();

            scrollPositionInfo = GUILayout.BeginScrollView(scrollPositionInfo, GUILayout.Width(m_Rect.width*0.68f),
                GUILayout.Height(0.8f*m_Rect.height));
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
        //if (GUILayout.Button(TC.get("GeneralText.Cancel")))
        if (GUILayout.Button("Cancel"))
        {
            Debug.Log(TC.get("GeneralText.Cancel") + selectedGameType);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void startNewGame()
    {
        GameRources.LoadOrCreateGameProject(null);
        EditorWindowBase.Init();
        EditorWindowBase window = (EditorWindowBase) EditorWindow.GetWindow(typeof (EditorWindowBase));
        window.Show();
    }

    //public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    //{
    //    if (workingObject is NewGameInputPopup)
    //    {
    //        newGameName = message;
    //        startNewGame();
    //    }
    //}

    //public void OnDialogCanceled(object workingObject = null)
    //{
    //}
}
