using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

public class ScenesWindowExits : LayoutWindow, DialogReceiverInterface
{
    private Texture2D backgroundPreviewTex = null;

    private Texture2D addTexture = null;
    private Texture2D moveUp, moveDown = null;
    private Texture2D clearImg = null;
    private Texture2D duplicateImg = null;

    private string backgroundPath = "";

    private static float windowWidth, windowHeight;
    private static Rect tableRect;
    private static Rect previewRect;
    private static Rect infoPreviewRect;
    private Rect rightPanelRect;

    private static Vector2 scrollPosition;

    private static GUISkin selectedAreaSkin;
    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;

    private string[] transitionTypes;
    private int selectedTransitionType, selectedTransitionTypeLast;
    private string transitionTimeString, transitionTimeStringLast;
    private int transitionTimeInt;

    private int selectedExit;

    public ScenesWindowExits(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
        params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {

        clearImg = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTexture = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        moveUp = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof (Texture2D));
        moveDown = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof (Texture2D));
        duplicateImg = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;


        transitionTypes = new string[]
        { TC.get("Exit.NoTransition"), TC.get("Exit.TopToBottom"), TC.get("Exit.BottomToTop"), TC.get("Exit.LeftToRight"), TC.get("Exit.RightToLeft"), TC.get("Exit.FadeIn") };

        transitionTimeInt = 0;
        transitionTimeString = transitionTimeStringLast = transitionTimeInt.ToString();

        if (GameRources.GetInstance().selectedSceneIndex >= 0)
        {

            backgroundPath =
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getPreviewBackground();
        }



        if (backgroundPath != null && !backgroundPath.Equals(""))
            backgroundPreviewTex = AssetsController.getImage(backgroundPath).texture;

        //TODO: do new skin?
        selectedAreaSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));
        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));

        tableRect = new Rect(0f, 0.1f*windowHeight, 0.9f*windowWidth, windowHeight*0.33f);
        rightPanelRect = new Rect(0.9f*windowWidth, 0.1f*windowHeight, 0.08f*windowWidth, 0.33f*windowHeight);
        infoPreviewRect = new Rect(0f, 0.45f*windowHeight, windowWidth, windowHeight*0.05f);
        previewRect = new Rect(0f, 0.5f*windowHeight, windowWidth, windowHeight*0.45f);

        selectedExit = 0;
    }

    public override void Draw(int aID)
    {
        GUILayout.BeginArea(tableRect);
        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("ExitsList.NextScene"), GUILayout.Width(windowWidth*0.24f));
        GUILayout.Box(TC.get("ExitsList.Transition"), GUILayout.Width(windowWidth*0.14f));
        GUILayout.Box(TC.get("ExitsList.Appearance"), GUILayout.Width(windowWidth*0.34f));
        GUILayout.Box(TC.get("ExitsList.ConditionsAndEffects"), GUILayout.Width(windowWidth*0.14f));
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList().Count;
            i++)
        {
            if (i == selectedExit)
                GUI.skin = selectedAreaSkin;
            else
                GUI.skin = noBackgroundSkin;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[i].getNextSceneId(),
                GUILayout.Width(windowWidth*0.24f)))
            {
                ChangeExitSelection(i);
            }

            // When is selected - show transition menu
            if (selectedExit == i)
            {
                GUILayout.BeginVertical();
                selectedTransitionType = EditorGUILayout.Popup(selectedTransitionType, transitionTypes,
                    GUILayout.Width(windowWidth*0.12f), GUILayout.MaxWidth(windowWidth*0.12f));
                if (selectedTransitionType != selectedTransitionTypeLast)
                    ChangeSelectedTransitionType(selectedTransitionType);
                transitionTimeString = GUILayout.TextField(transitionTimeString, 3, GUILayout.Width(windowWidth*0.12f),
                    GUILayout.MaxWidth(windowWidth*0.12f));
                transitionTimeString = Regex.Replace(transitionTimeString, @"[^0-9 ]", "");
                if (!transitionTimeString.Equals(transitionTimeStringLast))
                    ChangeSelectedTransitionTime(transitionTimeString);
                GUILayout.EndVertical();
            }
            // When is not selected - show normal text
            else
            {
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.Width(windowWidth*0.14f)))
                {
                    ChangeExitSelection(i);
                }
            }

            if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.Width(windowWidth*0.34f)))
            {
                ChangeExitSelection(i);
                ExitsAppearance window =
                    (ExitsAppearance) ScriptableObject.CreateInstance(typeof (ExitsAppearance));
                window.Init(this, "", selectedExit);
            }
            if (selectedExit == i)
            {
                GUILayout.BeginVertical();
                if (GUILayout.Button(TC.get("Exit.EditConditions"), GUILayout.Width(windowWidth*0.14f)))
                {
                    ConditionEditorWindow window =
                        (ConditionEditorWindow) ScriptableObject.CreateInstance(typeof (ConditionEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[i].getConditions());
                }
                if (GUILayout.Button(TC.get("GeneralText.EditEffects"), GUILayout.Width(windowWidth*0.14f)))
                {
                    EffectEditorWindow window =
                        (EffectEditorWindow) ScriptableObject.CreateInstance(typeof (EffectEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[i].getEffects());
                }
                if (GUILayout.Button(TC.get("Exit.EditPostEffects"), GUILayout.Width(windowWidth*0.14f)))
                {
                    EffectEditorWindow window =
                        (EffectEditorWindow) ScriptableObject.CreateInstance(typeof (EffectEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[i].getPostEffects());
                }
                if (GUILayout.Button(TC.get("Exit.EditNotEffects"), GUILayout.Width(windowWidth*0.14f)))
                {
                    EffectEditorWindow window =
                        (EffectEditorWindow) ScriptableObject.CreateInstance(typeof (EffectEditorWindow));
                    window.Init(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                        GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[i].getNotEffects());
                }
                GUILayout.EndVertical();
            }
            else
            {
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.Width(windowWidth*0.14f)))
                {
                    ChangeExitSelection(i);
                }
            }

            GUILayout.EndHorizontal();
            GUI.skin = defaultSkin;
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();



        /*
        * Right panel
        */
        GUILayout.BeginArea(rightPanelRect);
        GUI.skin = noBackgroundSkin;
        if (GUILayout.Button(addTexture, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            ExitNewLinkTo window =
            (ExitNewLinkTo)ScriptableObject.CreateInstance(typeof(ExitNewLinkTo));
            window.Init(this);
        }
        if (GUILayout.Button(duplicateImg, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList()
                .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getExitsList().getExits()[selectedExit]);
        }
        if (GUILayout.Button(moveUp, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList()
                .moveElementUp(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getExitsList().getExits()[selectedExit]);
        }
        if (GUILayout.Button(moveDown, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList()
                .moveElementDown(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getExitsList().getExits()[selectedExit]);
        }
        if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList()
                .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getExitsList().getExits()[selectedExit], false);
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();


        if (backgroundPath != "")
        {

            GUILayout.BeginArea(infoPreviewRect);
            // Show preview dialog
            if (GUILayout.Button(TC.get("DefaultClickAction.ShowDetails")+"/"+TC.get("GeneralText.Edit")))
            {
                ExitsEditor window =
                    (ExitsEditor) ScriptableObject.CreateInstance(typeof (ExitsEditor));
                window.Init(this, Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex], selectedExit);
            }
            GUILayout.EndArea();
            GUI.DrawTexture(previewRect, backgroundPreviewTex, ScaleMode.ScaleToFit);

        }
        else
        {
            GUILayout.BeginArea(infoPreviewRect);
            GUILayout.Button("No background!");
            GUILayout.EndArea();
        }
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        if (workingObject is ExitNewLinkTo)
        {
            Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                GameRources.GetInstance().selectedSceneIndex].getExitsList()
                .addElement(Controller.EXIT, message);
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
        Debug.Log(TC.get("GeneralText.Cancel"));
    }

    // Event called after change of selected exit
    public void ChangeExitSelection(int i)
    {
        selectedExit = i;
        selectedTransitionType =
            selectedTransitionTypeLast =
                Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
                    GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[selectedExit]
                    .getTransitionType();
        transitionTimeInt = Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[selectedExit].getTransitionTime();
        transitionTimeString = transitionTimeStringLast = transitionTimeInt.ToString();
    }

    private void ChangeSelectedTransitionType(int i)
    {
        selectedTransitionTypeLast = i;
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[selectedExit]
            .setTransitionType(i);
    }

    private void ChangeSelectedTransitionTime(string t)
    {
        transitionTimeStringLast = t;
        Controller.getInstance().getSelectedChapterDataControl().getScenesList().getScenes()[
            GameRources.GetInstance().selectedSceneIndex].getExitsList().getExitsList()[selectedExit]
            .setTransitionTime(int.Parse(t));
    }
}