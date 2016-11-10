using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CutscenesWindowDocumentation : LayoutWindow
{
    private string descriptionOfCutscene, nameOfCutscene, descriptionOfCutsceneLast, nameOfCutsceneLast, sceneclass = "", sceneclasslast, scenetype = "", scenetypelast;
    private float windowHeight;
    private Cutscene current;

    public CutscenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        string doc = "", name = "", sclass = "", stype = "";

        if (GameRources.GetInstance().selectedCutsceneIndex >= 0)
        {
            current = Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];

            doc = current.getDocumentation();
            name = current.getName();
            sclass = current.getXApiClass();
            stype = current.getXApiType();
        }

        doc = (doc == null ? "" : doc);
        name = (name == null ? "" : name);
        sclass = (sclass == null ? "" : sclass);
        stype = (stype == null ? "" : stype);

        descriptionOfCutscene = descriptionOfCutsceneLast = doc;
        nameOfCutscene = nameOfCutsceneLast = name;
        sceneclass = sceneclasslast = sclass;
        scenetype = scenetypelast = stype;

        windowHeight = aStartPos.height;
    }


    public override void Draw(int aID)
    {
        sceneclass = EditorGUILayout.TextField(new GUIContent("xAPI Class"), sceneclass);
        if (!sceneclass.Equals(sceneclasslast)) ChangeClass(sceneclass);

        scenetype = EditorGUILayout.TextField(new GUIContent("xAPI Type"), scenetype);
        if (!scenetype.Equals(scenetypelast)) ChangeType(scenetype);

        GUILayout.Label(TC.get("Cutscene.Documentation"));
        descriptionOfCutscene = GUILayout.TextArea(descriptionOfCutscene, GUILayout.MinHeight(0.4f * windowHeight));
        if (!descriptionOfCutscene.Equals(descriptionOfCutsceneLast))
            ChangeDocumentation(descriptionOfCutscene);

        GUILayout.Space(30);

        GUILayout.Label(TC.get("Cutscene.Name"));
        nameOfCutscene = GUILayout.TextField(nameOfCutscene);
        if (!nameOfCutscene.Equals(nameOfCutsceneLast))
            ChangeName(nameOfCutscene);
    }

    private void ChangeClass(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex].setXApiClass(s);
        sceneclasslast = s;
    }

    private void ChangeType(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex].setXApiType(s);
        scenetypelast = s;
    }

    private void ChangeName(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex].setName(s);
        nameOfCutsceneLast = s;
    }

    private void ChangeDocumentation(string s)
    {
        Controller.getInstance().getCharapterList().getSelectedChapterData().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex].setDocumentation(s);
        descriptionOfCutsceneLast = s;
    }
}