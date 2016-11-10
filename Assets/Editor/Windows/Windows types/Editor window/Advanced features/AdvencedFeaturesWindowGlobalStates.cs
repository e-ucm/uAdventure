using UnityEngine;
using System.Collections;

public class AdvencedFeaturesWindowGlobalStates : LayoutWindow
{
    private Texture2D addTex = null;
    private Texture2D duplicateTex = null;
    private Texture2D clearTex = null;

    private float windowWidth, windowHeight;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private Vector2 scrollPosition;

    private int selectedGlobalState;

    private Rect globalStatesTableRect, rightPanelRect, descriptionRect, conditionsRect;

    private string globalStateName, globalStateNameLast;
    private string globalStateDocumentation, globalStateDocumentationLast;

    public AdvencedFeaturesWindowGlobalStates(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
        duplicateTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
        selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

        globalStatesTableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, 0.5f * windowHeight);
        rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.5f * windowHeight);
        descriptionRect = new Rect(0f, 0.6f * windowHeight, 0.95f * windowWidth, 0.2f * windowHeight);
        conditionsRect = new Rect(0f, 0.8f * windowHeight, windowWidth, windowHeight * 0.15f);

        selectedGlobalState = -1;
    }

    public override void Draw(int aID)
    {

        GUILayout.BeginArea(globalStatesTableRect);
        GUILayout.Box(TC.get("GlobalStatesList.ID"), GUILayout.Width(0.85f * windowWidth));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates().Count;
            i++)
        {
            if (i != selectedGlobalState)
            {
                GUI.skin = noBackgroundSkin;

                if (
                    GUILayout.Button(
                        Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[
                            i].getId(), GUILayout.Width(0.85f * windowWidth)))
                {
                    OnSelectedGlobalStateChanged(i);
                }
            }
            else
            {
                GUI.skin = selectedAreaSkin;

                globalStateName = GUILayout.TextField(globalStateName, GUILayout.Width(0.85f * windowWidth));
                if (!globalStateName.Equals(globalStateNameLast))
                    OnGlobalStateNameChanged(globalStateName);
            }
            GUI.skin = defaultSkin;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        /*
        * Right panel
        */
        GUILayout.BeginArea(rightPanelRect);
        GUI.skin = noBackgroundSkin;
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl()
                .addElement(Controller.GLOBAL_STATE, "GlobalState" + Random.Range(0, 10000).ToString());
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl()
                .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[selectedGlobalState]);
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl()
                .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[selectedGlobalState], false);
            selectedGlobalState = -1;
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();

        if (selectedGlobalState != -1)
        {
            GUILayout.Space(10);
            GUILayout.BeginArea(descriptionRect);
            GUILayout.Label(TC.get("GlobalState.Documentation"));
            GUILayout.Space(10);
            globalStateDocumentation = GUILayout.TextArea(globalStateDocumentation, GUILayout.MinHeight(0.15f*windowHeight));
            if (!globalStateDocumentation.Equals(globalStateDocumentationLast))
                OnGlobalStateDocumentationChanged(globalStateDocumentation);
            GUILayout.EndArea();

            GUILayout.BeginArea(conditionsRect);
            if (GUILayout.Button(TC.get("GlobalState.Conditions")))
            {
                ConditionEditorWindow window =
                        (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getGlobalStatesListDataControl().getGlobalStates()[selectedGlobalState].getController());
            }
            GUILayout.EndArea();
        }
    }

    void OnSelectedGlobalStateChanged(int i)
    {
        selectedGlobalState = i;

        globalStateName =
            globalStateNameLast =
                Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[
                    selectedGlobalState].getId();

        if (globalStateName == null)
            globalStateName =
                globalStateNameLast = "";

        globalStateDocumentation = globalStateDocumentationLast =
                Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[
                    selectedGlobalState].getDocumentation();

        if (globalStateDocumentation == null)
            globalStateDocumentation =
                globalStateDocumentationLast = "";
    }

    private void OnGlobalStateNameChanged(string val)
    {
        if (Controller.getInstance().isElementIdValid(val, false))
        {
            globalStateNameLast = val;
            Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[selectedGlobalState].setId(val);
        }
    }

    private void OnGlobalStateDocumentationChanged(string val)
    {
        globalStateDocumentationLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStates()[selectedGlobalState].setDocumentation(val);
    }
}
