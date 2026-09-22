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

        public ScenesWindowDocumentation(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

        }

        public override void Draw(int aID)
        {
            var workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];

            var xAPIConfigurable = workingScene as IxAPIConfigurable;

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

            // Name
            EditorGUI.BeginChangeCheck();
            var allowsSaving = EditorGUILayout.Toggle(TC.get("Scene.AllowsSaving"), workingScene.allowsSavingGame());
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setAllowsSavingGame(allowsSaving);
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