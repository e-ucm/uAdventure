using UnityEngine;

namespace uAdventure.Editor
{
    public class AbstractEditorComponentWithPreview : PreviewLayoutWindow, EditorComponent
    {
        public EditorComponentAttribute Attribute
        {
            get
            {
                var attrs = GetType().GetCustomAttributes(typeof(EditorComponentAttribute), true);
                if (attrs.Length > 0)
                {
                    var componentAttr = attrs[0] as EditorComponentAttribute;
                    return componentAttr;
                }
                return null;
            }
        }

        public AbstractEditorComponentWithPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            var attr = Attribute;
            if (attr != null)
            {
                EditorWindowBase.RegisterComponent(attr.Type, this);
            }
        }

        public DataControl Target { get; set; }

        public bool Collapsed { get; set; }

        public virtual void OnDrawingGizmos() {}

        public virtual void OnDrawingGizmosSelected() {}

        public virtual void OnPostRender() {}

        public virtual void OnPreRender() {}

        public virtual void OnRender(Rect viewport) {}

        void EditorComponent.DrawInspector(){ DrawInspector();  }

        public virtual bool Update() { return false; }

        protected Rect GetViewportRect(Rect rect, Rect viewport)
        {
            var myPos = SceneEditor.Current.Matrix.MultiplyPoint(rect.position);
            var mySize = SceneEditor.Current.Matrix.MultiplyVector(rect.size);
            return new Rect(myPos, mySize).AdjustToViewport(800, 600, viewport);
        }
    }
}