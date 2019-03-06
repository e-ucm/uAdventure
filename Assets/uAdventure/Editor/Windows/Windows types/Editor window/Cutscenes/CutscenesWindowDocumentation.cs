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
        private readonly Dictionary<string, List<string>> xApiOptions;

        public CutscenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            xApiOptions = new Dictionary<string, List<string>>();

            List<string> accessibleOptions = Enum.GetValues(typeof(AccessibleTracker.Accessible))
                                                 .Cast<AccessibleTracker.Accessible>()
                                                 .Select(v => v.ToString().ToLower())
                                                 .ToList();

            xApiOptions.Add("accesible", accessibleOptions);

            List<string> alternativeOptions = Enum.GetValues(typeof(AlternativeTracker.Alternative))
                                                 .Cast<AlternativeTracker.Alternative>()
                                                 .Select(v => v.ToString().ToLower())
                                                 .ToList();

            xApiOptions.Add("alternative", alternativeOptions);
        }


        public override void Draw(int aID)
        {
            current = Controller.Instance.ChapterList.getSelectedChapterDataControl().getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];

            //XApi class
            EditorGUI.BeginChangeCheck();
            List<string> keys = xApiOptions.Keys.ToList();
            if (!keys.Contains(current.getXApiClass()))
            {
                current.setXApiClass(keys[0]);
            }

            var newClass = keys[EditorGUILayout.Popup("xAPI Class", keys.IndexOf(current.getXApiClass()), keys.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                current.setXApiClass(newClass);
            }

            // Xapi Type
            EditorGUI.BeginChangeCheck();
            List<string> types = xApiOptions[current.getXApiClass()];
            if (!types.Contains(current.getXApiType()))
            {
                current.setXApiType(types[0]);
            }

            var newType = types[EditorGUILayout.Popup("xAPI type", types.IndexOf(current.getXApiType()), types.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                current.setXApiType(newType);
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