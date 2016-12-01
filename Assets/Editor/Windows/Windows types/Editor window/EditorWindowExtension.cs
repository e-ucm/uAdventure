using UnityEngine;
using System.Collections;

namespace uAdventure.Editor
{
    public abstract class EditorWindowExtension : LayoutWindow
    {
        public EditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
        }

        public bool Selected { get; set; }
        public float ButtonHeight { get; set; }
        public abstract bool DrawButton(Rect rect, GUIStyle aStyle);
        public abstract bool LayoutDrawButton(GUIStyle aStyle, params GUILayoutOption[] aOptions);
        public bool LayoutDrawButton() { return LayoutDrawButton("Button"); }
        public float MenuHeight { get; set; }
        public abstract void DrawMenu(Rect rect, GUIStyle aStyle);
        public abstract void LayoutDrawMenu(GUIStyle aStyle, params GUILayoutOption[] aOptions);
        public void LayoutDrawMenu() { LayoutDrawMenu(null); }
    }

}

