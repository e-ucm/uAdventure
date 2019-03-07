using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;

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

            var xAPIConfigurable = current as IxAPIConfigurable;

            //XApi class
            EditorGUI.BeginChangeCheck();
            var classes = xAPIConfigurable.getxAPIValidClasses();
            if (!classes.Contains(xAPIConfigurable.getxAPIClass()))
            {
                xAPIConfigurable.setxAPIClass(classes[0]);
            }

            var newClass = classes[EditorGUILayout.Popup("xAPI Class", classes.IndexOf(xAPIConfigurable.getxAPIClass()), classes.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                xAPIConfigurable.setxAPIClass(newClass);
            }

            // Xapi Type
            EditorGUI.BeginChangeCheck();
            var types = xAPIConfigurable.getxAPIValidTypes(xAPIConfigurable.getxAPIClass());
            if (!types.Contains(xAPIConfigurable.getxAPIType()))
            {
                xAPIConfigurable.setxAPIType(types[0]);
            }

            var newType = types[EditorGUILayout.Popup("xAPI type", types.IndexOf(xAPIConfigurable.getxAPIType()), types.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                xAPIConfigurable.setxAPIType(newType);
            }


            EditorGUI.BeginChangeCheck();
            var nameOfCutscene = EditorGUILayout.TextField(TC.get("Cutscene.Name"), current.getName());
            if (EditorGUI.EndChangeCheck())
            {
                current.setName(nameOfCutscene);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel(TC.get("Cutscene.Documentation"));
            var description = EditorGUILayout.TextArea(current.getDocumentation(), GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                current.setDocumentation(description);
            }
        }
    }
}