using UnityEngine;

namespace uAdventure.Editor
{
    public class ComponentBasedEditorWindow<T> : PreviewLayoutWindow where T : ComponentBasedEditor<T>
    {
        protected T componentBasedEditor;

        private DataControl selectedElement;

        public ComponentBasedEditorWindow(Rect rect, GUIContent content, GUIStyle style, T componentBasedEditor, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            this.componentBasedEditor = componentBasedEditor;
            this.componentBasedEditor.onSelectElement += (s) =>
            {
                selectedElement = s;
            };
        }

        public override void DrawPreview(Rect rect)
        {
            componentBasedEditor.Draw(rect);
        }

        protected override bool HasToDrawPreviewInspector()
        {
            var any = false;
            // We only draw the inspector if we have an element selected and we have components to draw
            if (selectedElement != null)
            {
                componentBasedEditor.DoCallForWholeElement(selectedElement, _ => any = true);
            }

            return selectedElement != null && any;
        }

        protected override void DrawPreviewInspector()
        {
            if (componentBasedEditor.SelectedElement == null)
            {
                return;
            }

            componentBasedEditor.OnInspectorGUI();
        }
    }
}
