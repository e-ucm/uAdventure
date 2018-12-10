using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public abstract class EditorWindowExtension : LayoutWindow
    {
        protected EditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        protected EditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected EditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected EditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options) {}

        // Request main view
        public delegate void RequestMainView(EditorWindowExtension extension);
        public RequestMainView OnRequestMainView;

        public virtual bool Selected { get; set; }
        public float ContentHeight { get; set; }
        public abstract void DrawLeftPanelContent(Rect rect, GUIStyle style);
        public abstract void LayoutDrawLeftPanelContent(GUIStyle style, params GUILayoutOption[] options);
        public virtual void SelectElement(List<Searchable> path) { }
    }

}

