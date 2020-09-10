using System;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class AbstractEditorComponent : LayoutWindow, EditorComponent
    {
        public static EditorComponentAttribute GetAttribute(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(EditorComponentAttribute), true);
            if (attrs.Length > 0)
            {
                var componentAttr = attrs[0] as EditorComponentAttribute;
                return componentAttr;
            }
            return null;
        }

        public static void RegisterComponent(EditorComponent component)
        {
            var attr = GetAttribute(component.GetType());
            if (attr != null)
            {
                foreach (var t in attr.Types)
                {
                    uAdventureWindowMain.Instance.RegisterComponent(t, component);
                }
            }
        }

        public EditorComponentAttribute Attribute {
            get
            {
                return GetAttribute(GetType());
            }
        }

        protected AbstractEditorComponent(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            RegisterComponent(this);
        }

        public bool Collapsed { get; set; }

        public DataControl Target { get; set; }

        public virtual void OnDrawingGizmos() { }

        public virtual void OnDrawingGizmosSelected() { }

        public virtual void OnPostRender() { }

        public virtual void OnPreRender() { }

        public virtual void OnRender() { }

        void EditorComponent.DrawInspector() { Draw(-1); }

        public virtual bool Update() { return false; }
    }

}
