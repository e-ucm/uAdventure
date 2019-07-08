using System;
using System.Collections;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SceneEditorWindow : ComponentBasedEditorWindow<SceneEditor>
    {

        public SceneEditorWindow(Rect rect, GUIContent content, GUIStyle style, SceneEditor sceneEditor, params GUILayoutOption[] options) 
            : base(rect, content, style, sceneEditor, options)
        {
        }

        public virtual void OnSceneSelected(SceneDataControl scene)
        {
            componentBasedEditor.Scene = scene;
        }
    }
}