using UnityEngine;
using UnityEditor;

using uAdventure.Core;

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
            var sceneObject = ((Scene)workingScene.getContent());

            //XApi class
            EditorGUI.BeginChangeCheck();
            var newClass = EditorGUILayout.TextField(new GUIContent("xAPI Class"), sceneObject.getXApiClass());
            if (EditorGUI.EndChangeCheck())
                sceneObject.setXApiClass(newClass);

            // Xapi Type
            EditorGUI.BeginChangeCheck();
            var newType = EditorGUILayout.TextField(new GUIContent("xAPI Type"), sceneObject.getXApiType());
            if (EditorGUI.EndChangeCheck())
                sceneObject.setXApiType(newType);


            // Name
            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(TC.get("Scene.Name"), workingScene.getName());
            if (EditorGUI.EndChangeCheck())
                workingScene.setName(newName);

            // Documentation
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField(TC.get("Scene.Documentation"));
            var newDescription = EditorGUILayout.TextArea(workingScene.getDocumentation(), GUILayout.ExpandHeight(true));
            if (EditorGUI.EndChangeCheck())
                workingScene.setDocumentation(newDescription);
        }
    }
}