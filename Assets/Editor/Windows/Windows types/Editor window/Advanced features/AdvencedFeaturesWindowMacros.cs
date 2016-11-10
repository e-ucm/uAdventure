using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class AdvencedFeaturesWindowMacros : LayoutWindow
{

    private Texture2D addTex = null;
    private Texture2D duplicateTex = null;
    private Texture2D clearTex = null;

    private float windowWidth, windowHeight;

    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;
    private static GUISkin selectedAreaSkin;

    private Vector2 scrollPosition;

    private int selectedMacro;

    private Rect macroTableRect, rightPanelRect, descriptionRect, effectsRect;

    private string macroName, macroNameLast;
    private string macroDocumentation, macroDocumentationLast;

    public AdvencedFeaturesWindowMacros(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        clearTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
        duplicateTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
        selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

        macroTableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, 0.5f * windowHeight);
        rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.5f * windowHeight);
        descriptionRect = new Rect(0f, 0.6f * windowHeight, 0.95f * windowWidth, 0.2f * windowHeight);
        effectsRect = new Rect(0f, 0.8f * windowHeight, windowWidth, windowHeight * 0.15f);

        selectedMacro = -1;
    }

    public override void Draw(int aID)
    {
        GUILayout.BeginArea(macroTableRect);
        GUILayout.Box(TC.get("MacrosList.ID"), GUILayout.Width(0.85f * windowWidth));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros().Count;
            i++)
        {
            if (i != selectedMacro)
            {
                GUI.skin = noBackgroundSkin;

                if (
                    GUILayout.Button(
                        Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[
                            i].getId(), GUILayout.Width(0.85f * windowWidth)))
                {
                    OnSelectedMacroChanged(i);
                }
            }
            else
            {
                GUI.skin = selectedAreaSkin;

                macroName = GUILayout.TextField(macroName, GUILayout.Width(0.85f * windowWidth));
                if (!macroName.Equals(macroNameLast))
                    OnMacroNameChanged(macroName);
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
            Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl()
                .addElement(Controller.MACRO, "Macro" + Random.Range(0, 10000).ToString());
        }
        if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl()
                .duplicateElement(Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[selectedMacro]);
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f * windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl()
                .deleteElement(Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[selectedMacro], false);
            selectedMacro = -1;
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();

        if (selectedMacro != -1)
        {
            GUILayout.Space(10);
            GUILayout.BeginArea(descriptionRect);
            GUILayout.Label(TC.get("Macro.Documentation"));
            GUILayout.Space(10);
            macroDocumentation = GUILayout.TextArea(macroDocumentation, GUILayout.MinHeight(0.15f * windowHeight));
            if (!macroDocumentation.Equals(macroDocumentationLast))
                OnMacroDocumentationChanged(macroDocumentation);
            GUILayout.EndArea();

            GUILayout.BeginArea(effectsRect);
            if (GUILayout.Button(TC.get("Element.Effects")))
            {
                EffectEditorWindow window =
                (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(Controller.getInstance()
                    .getSelectedChapterDataControl()
                    .getMacrosListDataControl().getMacros()[selectedMacro].getController());
            }
            GUILayout.EndArea();
        }
    }

    void OnSelectedMacroChanged(int i)
    {
        selectedMacro = i;

        macroName =
            macroNameLast =
                Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[
                    selectedMacro].getId();

        if (macroName == null)
            macroName =
                macroNameLast = "";

        macroDocumentation = macroDocumentationLast =
                Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[
                    selectedMacro].getDocumentation();

        if (macroDocumentation == null)
            macroDocumentation =
                macroDocumentationLast = "";
    }

    private void OnMacroNameChanged(string val)
    {
        if (Controller.getInstance().isElementIdValid(val, false))
        {
            macroNameLast = val;
            Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[selectedMacro].setId(val);
        }
    }

    private void OnMacroDocumentationChanged(string val)
    {
        macroDocumentationLast = val;
        Controller.getInstance().getSelectedChapterDataControl().getMacrosListDataControl().getMacros()[selectedMacro].setDocumentation(val);
    }

}
