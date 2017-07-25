using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CutscenesWindowDocumentation : LayoutWindow
    {
        private string descriptionOfCutscene, nameOfCutscene, descriptionOfCutsceneLast, nameOfCutsceneLast, sceneclass = "", sceneclasslast, scenetype = "", scenetypelast;
        private CutsceneDataControl current;

        public CutscenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            string doc = "", name = "", sclass = "", stype = "";

            
            descriptionOfCutscene = descriptionOfCutsceneLast = doc;
            nameOfCutscene = nameOfCutsceneLast = name;
            sceneclass = sceneclasslast = sclass;
            scenetype = scenetypelast = stype;
            
        }


        public override void Draw(int aID)
        {
            current = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];

            sceneclasslast = current.getXApiClass();
            sceneclass = EditorGUILayout.TextField(new GUIContent("xAPI Class"), sceneclasslast);
            if (!sceneclass.Equals(sceneclasslast))
                current.setXApiClass(sceneclass);

            scenetypelast = current.getXApiType();
            scenetype = EditorGUILayout.TextField(new GUIContent("xAPI Type"), scenetypelast);
            if (!scenetype.Equals(scenetypelast))
                current.setXApiType(scenetype);

            GUILayout.Label(TC.get("Cutscene.Documentation"));
            descriptionOfCutsceneLast = current.getDocumentation();
            descriptionOfCutscene = GUILayout.TextArea(descriptionOfCutsceneLast, GUILayout.MinHeight(0.4f * m_Rect.height));
            if (!descriptionOfCutscene.Equals(descriptionOfCutsceneLast))
                current.setDocumentation(descriptionOfCutscene);

            GUILayout.Space(30);

            GUILayout.Label(TC.get("Cutscene.Name"));
            nameOfCutsceneLast = current.getName();
            nameOfCutscene = GUILayout.TextField(nameOfCutsceneLast);
            if (!nameOfCutscene.Equals(nameOfCutsceneLast))
                current.setName(nameOfCutscene);
        }
    }
}