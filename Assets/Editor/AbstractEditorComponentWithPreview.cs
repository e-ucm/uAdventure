using UnityEngine;

namespace uAdventure.Editor
{
    public class AbstractEditorComponentWithPreview : PreviewLayoutWindow, EditorComponent
    {
        public AbstractEditorComponentWithPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
        }

        public DataControl Target { get; set; }

        public bool Collapsed { get; set; }

        public virtual void OnDrawingGizmos() {}

        public virtual void OnDrawingGizmosSelected() {}

        public virtual void OnPostRender() {}

        public virtual void OnPreRender() {}

        public virtual void OnRender(Rect viewport) {}

        void EditorComponent.DrawInspector(){}

        public virtual bool Update() { return false; }
    }
}