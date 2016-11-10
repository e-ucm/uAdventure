using UnityEngine;
using System.Collections;
using UnityEditor;

public class ChapterWindow : LayoutWindow
{
    private string chapterName, descriptionOfGame, chapterNameLast, descriptionOfGameLast;
    private Texture2D clearImg = null;
    private Vector2 scrollPosition = Vector2.zero;
    private int selInitialScene, selInitialSceneLast;
    private string[] selStringsAdapatation, selStringsAssesment, selStringsInitialScene;
    private float windowHeight;

    public ChapterWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        Chapter chapter = Controller.getInstance().getCharapterList().getSelectedChapterData();
        chapterName = chapterNameLast = chapter.getTitle();
        descriptionOfGame = descriptionOfGameLast = chapter.getDescription();
    
        selInitialScene = selInitialSceneLast = 0;

        int amountOfScenes = Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes().Count;
        selStringsInitialScene = new string[amountOfScenes];
        for (int i = 0; i < amountOfScenes; i++)
        {
            selStringsInitialScene[i] = Controller.getInstance().getCharapterList().getSelectedChapterData().getScenes()[i].getId();
            // Set index for selction grid
            if (selStringsInitialScene[i] == Controller.getInstance().getCharapterList().getSelectedChapterDataControl().getInitialScene())
                selInitialScene = i;
        }

        clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));

        windowHeight = aStartPos.height;
    }


    public override void Draw(int aID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label(TC.get("Chapter.Title"));
        chapterName = GUILayout.TextField(chapterName);
        if(!chapterName.Equals(chapterNameLast))
            ChangeTitle(chapterName);

        GUILayout.Space(20);

        GUILayout.Label(TC.get("Chapter.Description"));
        descriptionOfGame = GUILayout.TextArea(descriptionOfGame, GUILayout.MinHeight(0.2f * windowHeight));
        if (!descriptionOfGame.Equals(descriptionOfGameLast))
            ChangeDescription(descriptionOfGame);

        GUILayout.Space(20);

        GUILayout.Label(TC.get("Chapter.InitialScene"));
        selInitialScene = EditorGUILayout.Popup(selInitialScene, selStringsInitialScene);
        if (selInitialScene != selInitialSceneLast)
            ChangeSelectedInitialScene(selInitialScene);
        GUILayout.EndScrollView();
    }

    private void ChangeTitle(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterDataControl().setTitle(s);
        chapterNameLast = s;
    }

    private void ChangeDescription(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterDataControl().setDescription(s);
        descriptionOfGameLast = s;
    }

    private void ChangeSelectedInitialScene(int i)
    {
        selInitialSceneLast = i;
        Controller.getInstance()
            .getCharapterList()
            .getSelectedChapterDataControl()
            .setInitialScene(
                Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[i].getId());
    }
}