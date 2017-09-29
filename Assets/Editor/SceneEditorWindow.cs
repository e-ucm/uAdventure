using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class SceneEditorWindow : PreviewLayoutWindow
    {
        protected SceneEditor sceneEditor;

        private static Dictionary<Type, List<EditorComponent>> knownComponents;

        public SceneEditorWindow(Rect rect, GUIContent content, GUIStyle style, SceneEditor sceneEditor, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            this.sceneEditor = sceneEditor;
            this.sceneEditor.onSelectElement += (s) =>
            {
                selectedElement = s;
            };

            LoadComponents();

            sceneEditor.Components = knownComponents;
        }

        private DataControl selectedElement;

        protected override void DrawPreview(Rect rect)
        {
            sceneEditor.Draw(rect);
        }

        protected override bool HasToDrawPreviewInspector()
        {
            // We only draw the inspector if we have an element selected and we have components to draw
            return selectedElement != null && knownComponents.ContainsKey(selectedElement.GetType());
        }

        protected override void DrawPreviewInspector()
        {
            var referencedElement = selectedElement;

            // If its a reference
            if (selectedElement is ElementReferenceDataControl)
            {
                // First we draw the special components for the element reference
                var elemRef = referencedElement as ElementReferenceDataControl;
                foreach (var component in knownComponents[referencedElement.GetType()])
                {
                    var oldTarget = component.Target;
                    component.Target = referencedElement;
                    component.Collapsed = EditorGUILayout.BeginToggleGroup(component.GetType().ToString(), component.Collapsed);
                    component.DrawInspector();
                    EditorGUILayout.EndToggleGroup();
                    component.Target = oldTarget;
                }

                // And then we set it up to be able to draw the referenced element components
                referencedElement = elemRef.getReferencedElementDataControl();
            }

            // Component drawing
            foreach (var component in knownComponents[referencedElement.GetType()])
            {
                var oldTarget = component.Target;
                component.Target = referencedElement;
                component.DrawInspector();
                component.Target = oldTarget;
            }
        }

        // ##################### AUX FUNCTIONS #######################

        private void LoadComponents()
        {
            if (knownComponents == null)
            {
                knownComponents = new Dictionary<Type, List<EditorComponent>>();
                // Load from assembly
            }
        }
    }
}