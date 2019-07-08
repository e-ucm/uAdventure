using UnityEngine;

namespace uAdventure.Editor
{
    public class AbstractEditorComponentWithPreview : PreviewLayoutWindow, EditorComponent
    {
        public EditorComponentAttribute Attribute
        {
            get { return AbstractEditorComponent.GetAttribute(GetType()); }
        }

        public AbstractEditorComponentWithPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            AbstractEditorComponent.RegisterComponent(this);
        }

        private DataControl target;
        public DataControl Target { get { return target; } set
            {
                if (value != target)
                {
                    target = value;
                    OnTargetChanged();
                }
            }
        }

        public bool Collapsed { get; set; }

        protected virtual void OnTargetChanged() { }

        public virtual void OnDrawingGizmos() {}

        public virtual void OnDrawingGizmosSelected() {}

        public virtual void OnPostRender() {}

        public virtual void OnPreRender() {}

        public virtual void OnRender() {}

        void EditorComponent.DrawInspector(){ DrawInspector();  }

        public virtual bool Update() { return false; }
    }
}