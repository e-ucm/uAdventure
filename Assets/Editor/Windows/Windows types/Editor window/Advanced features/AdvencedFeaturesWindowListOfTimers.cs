using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class AdvencedFeaturesWindowListOfTimers : LayoutWindow
{
    private Texture2D addTex = null;
    private Texture2D duplicateTex = null;
    private Texture2D clearTex = null;

    private static float windowWidth, windowHeight;
    private static Rect timerTableRect, rightPanelRect, settingsTable;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private int selectedTimer;

    private string timerTime, timerTimeLast;
    private string fullTimerDescription = "", fullTimerDescriptionLast = "";
    private string displayName = "", displayNameLast = "";

    private Vector2 scrollPosition;

    private GUIStyle smallFontStyle;

    public AdvencedFeaturesWindowListOfTimers(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
        params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTex = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        duplicateTex = (Texture2D) Resources.Load("EAdventureData/img/icons/duplicateNode", typeof (Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));
        selectedAreaSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));

        timerTableRect = new Rect(0f, 0.1f*windowHeight, 0.9f*windowWidth, 0.2f*windowHeight);
        rightPanelRect = new Rect(0.9f*windowWidth, 0.1f*windowHeight, 0.08f*windowWidth, 0.2f*windowHeight);
        settingsTable = new Rect(0f, 0.3f*windowHeight, windowWidth, windowHeight*0.65f);

        smallFontStyle = new GUIStyle();
        smallFontStyle.fontSize = 8;

        selectedTimer = -1;
    }

    public override void Draw(int aID)
    {
        /*
        * Timer table
        */
        GUILayout.BeginArea(timerTableRect);

        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("TimersList.Timer"), GUILayout.MaxWidth(windowWidth*0.3f));
        GUILayout.Box(TC.get("TimersList.Time"), GUILayout.MaxWidth(windowWidth*0.3f));
        GUILayout.Box(TC.get("TimersList.Display"), GUILayout.MaxWidth(windowWidth*0.3f)); 
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0;
            i < Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers().Count;
            i++)
        {
            if (i == selectedTimer)
                GUI.skin = selectedAreaSkin;
            else
                GUI.skin = noBackgroundSkin;

            GUILayout.BeginHorizontal();

            if (i == selectedTimer)
            {
                if (GUILayout.Button("Timer #" + i, GUILayout.MaxWidth(windowWidth*0.3f)))
                {
                    OnTimerSelectedChange(i);
                }

                timerTime = GUILayout.TextField(timerTime, GUILayout.MaxWidth(windowWidth * 0.3f));
                timerTime = (Regex.Match(timerTime, "^[0-9]{1,4}$").Success ? timerTime : timerTimeLast);
                if (timerTime != timerTimeLast)
                    OnTimerTime(timerTime);
                
                Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[i].setShowTime(GUILayout
                    .Toggle(
                        Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[i]
                            .isShowTime(),
                        "", GUILayout.MaxWidth(windowWidth*0.3f)));
            }
            else
            {
                if (GUILayout.Button("Timer #" + i, GUILayout.MaxWidth(windowWidth*0.3f)))
                {
                    OnTimerSelectedChange(i);
                }
                if (
                    GUILayout.Button(
                        Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[i].getTime()
                            .ToString(), GUILayout.MaxWidth(windowWidth*0.3f)))
                {
                    OnTimerSelectedChange(i);
                }
                if (
                    GUILayout.Button(
                    Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[i].isShowTime().ToString(), GUILayout.MaxWidth(windowWidth*0.3f)))
                {
                    OnTimerSelectedChange(i);
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
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getTimersList().addElement(Controller.TIMER, "");
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList()
                .duplicateElement(
                    Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[selectedTimer]);
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList()
                .deleteElement(
                    Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[selectedTimer],
                    false);
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();

        /*
        *Properties panel
        */

        if (selectedTimer != -1 &&
            Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[selectedTimer] != null)
        {
            GUILayout.BeginArea(settingsTable);

            GUILayout.Label(TC.get("Timer.Documentation"));
            fullTimerDescription = GUILayout.TextArea(fullTimerDescription, GUILayout.MinHeight(0.1f*windowHeight));
            if (fullTimerDescription != fullTimerDescriptionLast)
                OnTimerDocumentationChanged(fullTimerDescription);


            GUILayout.FlexibleSpace();


            GUILayout.Label(TC.get("TimersList.Time"));

            GUILayout.BeginHorizontal();
            if (
                !Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[selectedTimer]
                    .isShowTime())
                GUI.enabled = false;
            GUILayout.Label(TC.get("Timer.DisplayName"));
            displayName = GUILayout.TextField(displayName);
            if (displayName != displayNameLast)
                OnTimerDisplayNameChanged(displayName);

            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].setCountDown(GUILayout.Toggle(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].isCountDown(), TC.get("Timer.CountDown")));

            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].setShowWhenStopped(GUILayout.Toggle(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].isShowWhenStopped(), TC.get("Timer.ShowWhenStopped")));
            GUI.enabled = true;
            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();


            GUILayout.Label(TC.get("Timer.LoopControl"));
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].setMultipleStarts(GUILayout.Toggle(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].isMultipleStarts(), TC.get("Timer.MultipleStarts")));
            GUILayout.Label(TC.get("Timer.MultipleStartsDesc"), smallFontStyle);
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].setRunsInLoop(GUILayout.Toggle(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].isRunsInLoop(), TC.get("Timer.RunsInLoop")));
            GUILayout.Label(
                 TC.get("Timer.RunsInLoopDesc"), smallFontStyle);


            GUILayout.FlexibleSpace();


            GUILayout.Label(TC.get("Timer.InitConditions"));
            if (GUILayout.Button(TC.get("GeneralText.EditInitConditions")))
            {
                ConditionEditorWindow window =
                       (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].getInitConditions());
            }


            GUILayout.FlexibleSpace();


            GUILayout.Label(TC.get("Timer.EndConditions"));
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].setUsesEndCondition(
                    GUILayout.Toggle(Controller.getInstance()
                        .getSelectedChapterDataControl()
                        .getTimersList().getTimers()[selectedTimer].isUsesEndCondition(),
                        TC.get("Timer.UsesEndConditionShort")));
            if (
                !Controller.getInstance().getSelectedChapterDataControl().getTimersList().getTimers()[selectedTimer]
                    .isUsesEndCondition())
                GUI.enabled = false;
            if (GUILayout.Button(TC.get("GeneralText.EditEndConditions")))
            {
                ConditionEditorWindow window =
                       (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].getEndConditions());

            }
            GUI.enabled = true;


            GUILayout.FlexibleSpace();


            GUILayout.BeginHorizontal();
            GUILayout.Label(TC.get("Timer.Effects"), GUILayout.Width(0.45f*windowWidth));
            GUILayout.Label(TC.get("Timer.PostEffects"), GUILayout.Width(0.45f*windowWidth));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(TC.get("GeneralText.EditEffects"), GUILayout.Width(0.45f*windowWidth)))
            {
                EffectEditorWindow window =
                (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].getEffects());
            }
            if (GUILayout.Button(TC.get("GeneralText.EditPostEffects"), GUILayout.Width(0.45f*windowWidth)))
            {
                EffectEditorWindow window =
                (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getTimersList().getTimers()[selectedTimer].getPostEffects());
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

    }

    void OnTimerSelectedChange(int i)
    {
        selectedTimer = i;

        fullTimerDescription = fullTimerDescriptionLast =
            Controller.getInstance()
                .getSelectedChapterDataControl()
                .getTimersList().getTimers()[selectedTimer].getDocumentation();
        if (fullTimerDescription == null)
            fullTimerDescription = fullTimerDescriptionLast = "";


        displayName = displayNameLast = Controller.getInstance()
            .getSelectedChapterDataControl()
            .getTimersList().getTimers()[selectedTimer].getDisplayName();
        if (displayName == null)
            displayName = displayNameLast = "";


        timerTime = timerTimeLast = Controller.getInstance()
            .getSelectedChapterDataControl()
            .getTimersList().getTimers()[selectedTimer].getTime().ToString();
        if (timerTime == null)
            timerTime = timerTime = "0";
    }

    void OnTimerDocumentationChanged(string val)
    {
        fullTimerDescriptionLast = val;
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getTimersList().getTimers()[selectedTimer].setDocumentation(val);
    }

    void OnTimerDisplayNameChanged(string val)
    {
        displayNameLast = val;
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getTimersList().getTimers()[selectedTimer].setDisplayName(val);
    }

    void OnTimerTime(string val)
    {
        timerTimeLast = val;
        Controller.getInstance()
            .getSelectedChapterDataControl()
            .getTimersList().getTimers()[selectedTimer].setTime(long.Parse(val));
    }
}