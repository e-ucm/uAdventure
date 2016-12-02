using UnityEngine;
using System.Collections;

namespace uAdventure.Editor
{
    public abstract class EditorWindowExtension : LayoutWindow
    {
        public EditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
        }

        // Request main view
        public delegate void RequestMainView(EditorWindowExtension extension);
        public RequestMainView OnRequestMainView;

        public virtual bool Selected { get; set; }
        public float ContentHeight { get; set; }
        public abstract void DrawLeftPanelContent(Rect rect, GUIStyle aStyle);
        public abstract void LayoutDrawLeftPanelContent(GUIStyle aStyle, params GUILayoutOption[] aOptions);
    }

}

