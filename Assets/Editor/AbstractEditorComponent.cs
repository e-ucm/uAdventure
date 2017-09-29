using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class AbstractEditorComponent : LayoutWindow, EditorComponent
    {
        public AbstractEditorComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
        }
        public bool Collapsed { get; set; }

        public DataControl Target { get; set; }

        public virtual void OnDrawingGizmos() { }

        public virtual void OnDrawingGizmosSelected() { }

        public virtual void OnPostRender() { }

        public virtual void OnPreRender() { }

        public virtual void OnRender(Rect viewport) { }

        void EditorComponent.DrawInspector() { Draw(-1); }

        public virtual bool Update() { return false; }
        
    }

}
