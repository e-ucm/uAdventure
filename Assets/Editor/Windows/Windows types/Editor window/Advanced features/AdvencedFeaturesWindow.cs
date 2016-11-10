using UnityEngine;
using System.Collections;

public class AdvencedFeaturesWindow : LayoutWindow
{
    private enum AdvencedFeaturesWindowType
    {
        GlobalStates,
        ListOfTimers,
        Macros
    }

    private static GUISkin selectedButtonSkin;
    private static GUISkin defaultSkin;

    private static AdvencedFeaturesWindowType openedWindow = AdvencedFeaturesWindowType.ListOfTimers;

    private static AdvencedFeaturesWindowGlobalStates advencedFeaturesWindowGlobalStates;
    private static AdvencedFeaturesWindowListOfTimers advencedFeaturesWindowListOfTimers;
    private static AdvencedFeaturesWindowMacros advencedFeaturesWindowMacros;

    public AdvencedFeaturesWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
        params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        advencedFeaturesWindowGlobalStates = new AdvencedFeaturesWindowGlobalStates(aStartPos,
            new GUIContent(TC.get("Element.Name55")), "Window");
        advencedFeaturesWindowListOfTimers = new AdvencedFeaturesWindowListOfTimers(aStartPos,
            new GUIContent(TC.get("TimersList.Title")), "Window");
        advencedFeaturesWindowMacros = new AdvencedFeaturesWindowMacros(aStartPos,
            new GUIContent(TC.get("Element.Name57")), "Window");
        selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
    }


    public override void Draw(int aID)
    {

        /**
            UPPER MENU
            */
        GUILayout.BeginHorizontal();

        if (openedWindow == AdvencedFeaturesWindowType.ListOfTimers)
            GUI.skin = selectedButtonSkin;
        if (GUILayout.Button(TC.get("TimersList.Title")))
        {
            OnWindowTypeChanged(AdvencedFeaturesWindowType.ListOfTimers);
        }
        if (openedWindow == AdvencedFeaturesWindowType.ListOfTimers)
            GUI.skin = defaultSkin;

        if (openedWindow == AdvencedFeaturesWindowType.GlobalStates)
            GUI.skin = selectedButtonSkin;
        if (GUILayout.Button(TC.get("Element.Name55")))
        {
            OnWindowTypeChanged(AdvencedFeaturesWindowType.GlobalStates);
        }
        if (openedWindow == AdvencedFeaturesWindowType.GlobalStates)
            GUI.skin = defaultSkin;

        if (openedWindow == AdvencedFeaturesWindowType.Macros)
            GUI.skin = selectedButtonSkin;
        if (GUILayout.Button(TC.get("Element.Name57")))
        {
            OnWindowTypeChanged(AdvencedFeaturesWindowType.Macros);
        }
        if (openedWindow == AdvencedFeaturesWindowType.Macros)
            GUI.skin = defaultSkin;
        GUILayout.EndHorizontal();

        switch (openedWindow)
        {
            case AdvencedFeaturesWindowType.GlobalStates:
                advencedFeaturesWindowGlobalStates.Draw(aID);
                break;
            case AdvencedFeaturesWindowType.ListOfTimers:
                advencedFeaturesWindowListOfTimers.Draw(aID);
                break;
            case AdvencedFeaturesWindowType.Macros:
                advencedFeaturesWindowMacros.Draw(aID);
                break;
        }
    }


    void OnWindowTypeChanged(AdvencedFeaturesWindowType type_)
    {
        openedWindow = type_;
    }
}