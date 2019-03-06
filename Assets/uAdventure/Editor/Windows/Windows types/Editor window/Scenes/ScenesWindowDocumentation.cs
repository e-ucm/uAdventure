using UnityEngine;
using UnityEditor;

using uAdventure.Core;
using System.Collections.Generic;
using System;
using System.Linq;

namespace uAdventure.Editor
{

    public class ScenesWindowDocumentation : LayoutWindow
    {

        private readonly Dictionary<string, List<string>> xApiOptions;

        public ScenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor, params GUILayoutOption[] aOptions)
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
            var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];
            var sceneObject = ((Scene)workingScene.getContent());

            //XApi class
            EditorGUI.BeginChangeCheck();
            List<string> keys = xApiOptions.Keys.ToList();
            if (!keys.Contains(sceneObject.getXApiClass()))
            {
                sceneObject.setXApiClass(keys[0]);
            }

            var newClass = keys[EditorGUILayout.Popup("xAPI Class", keys.IndexOf(sceneObject.getXApiClass()), keys.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                sceneObject.setXApiClass(newClass);
            }

            // Xapi Type
            EditorGUI.BeginChangeCheck();
            List<string> types = xApiOptions[sceneObject.getXApiClass()];
            if (!types.Contains(sceneObject.getXApiType()))
            {
                sceneObject.setXApiType(types[0]);
            }

            var newType = types[EditorGUILayout.Popup("xAPI type", types.IndexOf(sceneObject.getXApiType()), types.ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                sceneObject.setXApiType(newType);
            }

            // Name
            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(TC.get("Scene.Name"), workingScene.getName());
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setName(newName);
            }

            // Documentation
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(TC.get("Scene.Documentation"));
            var newDescription = EditorGUILayout.TextArea(workingScene.getDocumentation(), GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setDocumentation(newDescription);
            }
        }
    }
}