using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExitsAppearance : BaseInputPopup, DialogReceiverInterface
{
    private int currentExitIndex;
    private string exitText, exitTextLast;
    private string audioPath, audioPathLast;
    private string exitIconPath, exitIconPathLast;

    private Texture2D clearTexture;
    private Texture2D noAudioTexture;
    private Texture2D audioTexture;
    private Texture2D defaultExitTexture;
    private Texture2D exitTexture;


    public void Init(DialogReceiverInterface e, string startTextContent, int exitIndex)
    {
        currentExitIndex = exitIndex;

        exitText = exitTextLast = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
            .getDefaultExitLook()
            .getExitText();

        exitIconPath =
            exitIconPathLast = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
                .getDefaultExitLook()
                .getCursorPath();

        audioPath =
            audioPathLast = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
                .getDefaultExitLook()
                .getSoundPath();

        clearTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        noAudioTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/noAudio", typeof (Texture2D));
        audioTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/audio", typeof (Texture2D));
        defaultExitTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/exit", typeof (Texture2D));

        if(exitIconPath != null && !exitIconPath.Equals(""))
        exitTexture =
            (Texture2D) Resources.Load(exitIconPath.Substring(0, exitIconPath.LastIndexOf(".")), typeof (Texture2D));

        base.Init(e, startTextContent);
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label(TC.get("Exit.ExitText"));

        GUILayout.Space(10);

        exitText = GUILayout.TextField(exitText);
        if (!exitText.Equals(exitTextLast))
            OnChangeExitText(exitText);

        GUILayout.EndHorizontal();


        GUILayout.Space(30);


        GUILayout.BeginHorizontal();
        if (audioPath != null && !audioPath.Equals(""))
        {
            GUILayout.Label(audioTexture);
            GUILayout.Space(5);
            GUILayout.Label(audioPath);
        }
        else
        {
            GUILayout.Label(noAudioTexture);
            GUILayout.Space(5);
            GUILayout.Label(TC.get("Conversations.NoAudio"));
        }
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            MusicFileOpenDialog musicDialog =
                (MusicFileOpenDialog) ScriptableObject.CreateInstance(typeof (MusicFileOpenDialog));
            musicDialog.Init(this, BaseFileOpenDialog.FileType.EXIT_MUSIC);
        }
        if (GUILayout.Button(clearTexture))
        {
            OnChangeExitAudio("");
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (exitIconPath != null && !exitIconPath.Equals(""))
        {
            GUILayout.Label(exitTexture);
        }
        else
        {
            GUILayout.Label(defaultExitTexture);
        }
        if (GUILayout.Button(TC.get("Buttons.Select")))
        {
            ImageFileOpenDialog imageDialog =
                (ImageFileOpenDialog) ScriptableObject.CreateInstance(typeof (ImageFileOpenDialog));
            imageDialog.Init(this, BaseFileOpenDialog.FileType.EXIT_ICON);
        }
        if (GUILayout.Button(clearTexture))
        {
            OnChangeExitText("");
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            reference.OnDialogOk(textContent, this);
            this.Close();
        }
        GUILayout.EndHorizontal();
    }

    // Event called after changing value (clear or set new)
    void OnChangeExitText(string val)
    {
        exitTextLast = val;
        // Udpate controller
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
            .getDefaultExitLook().setExitText(exitText);
    }

    // Event called after changing value (clear or set new)
    void OnChangeExitIcon(string val)
    {
        exitIconPathLast = val;
        exitTexture =
            (Texture2D) Resources.Load(exitIconPath.Substring(0, exitIconPath.LastIndexOf(".")), typeof (Texture2D));
        // Udpate controller
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
            .getDefaultExitLook().setCursorPath(exitIconPath);
    }

    // Event called after changing value (clear or set new)
    void OnChangeExitAudio(string val)
    {
        audioPathLast = val;
        // Udpate controller
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[currentExitIndex]
            .getDefaultExitLook().setSoundPath(audioPath);
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        switch ((BaseFileOpenDialog.FileType) workingObject)
        {
            case BaseFileOpenDialog.FileType.EXIT_ICON:
                exitIconPath = message;
                OnChangeExitIcon(message);
                break;
            case BaseFileOpenDialog.FileType.EXIT_MUSIC:
                audioPath = message;
                OnChangeExitAudio(message);
                break;
            default:
                break;
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
        Debug.Log("Wiadomość nie OK");
    }
}