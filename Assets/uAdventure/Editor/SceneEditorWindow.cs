﻿using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SceneEditorWindow : PreviewLayoutWindow
    {
        protected SceneEditor sceneEditor;

        public SceneEditorWindow(Rect rect, GUIContent content, GUIStyle style, SceneEditor sceneEditor, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            this.sceneEditor = sceneEditor;
            this.sceneEditor.onSelectElement += (s) =>
            {
                selectedElement = s;
            };
        }

        private DataControl selectedElement;

        public virtual void OnSceneSelected(SceneDataControl scene)
        {
            sceneEditor.Scene = scene;
        }

        public override void DrawPreview(Rect rect)
        {
            sceneEditor.Draw(rect);
        }

        protected override bool HasToDrawPreviewInspector()
        {
            var any = false;
            // We only draw the inspector if we have an element selected and we have components to draw
            if (selectedElement != null)
            {
                sceneEditor.DoCallForWholeElement(selectedElement, _ => any = true);
            }

            return selectedElement != null && any;
        }

        protected override void DrawPreviewInspector()
        {
            if (sceneEditor.SelectedElement == null)
            {
                return;
            }

            sceneEditor.OnInspectorGUI();
        }
    }
}