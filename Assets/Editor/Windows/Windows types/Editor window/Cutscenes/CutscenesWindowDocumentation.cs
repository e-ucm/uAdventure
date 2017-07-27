using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class CutscenesWindowDocumentation : LayoutWindow
    {
        private CutsceneDataControl current;

        public CutscenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
        }


        public override void Draw(int aID)
        {
            current = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];
            
            EditorGUI.BeginChangeCheck();
            var sceneclass = EditorGUILayout.TextField(new GUIContent("xAPI Class"), current.getXApiClass());
            if (EditorGUI.EndChangeCheck())
                current.setXApiClass(sceneclass);
            
            EditorGUI.BeginChangeCheck();
            var scenetype = EditorGUILayout.TextField(new GUIContent("xAPI Type"), current.getXApiType());
            if (EditorGUI.EndChangeCheck())
                current.setXApiType(scenetype);

            EditorGUI.BeginChangeCheck();
            var nameOfCutscene = EditorGUILayout.TextField(TC.get("Cutscene.Name"), current.getName());
            if (EditorGUI.EndChangeCheck())
                current.setName(nameOfCutscene);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel(TC.get("Cutscene.Documentation"));
            var description = EditorGUILayout.TextArea(current.getDocumentation(), GUILayout.ExpandHeight(true));
            if(EditorGUI.EndChangeCheck())
                current.setDocumentation(description);

        }
    }
}