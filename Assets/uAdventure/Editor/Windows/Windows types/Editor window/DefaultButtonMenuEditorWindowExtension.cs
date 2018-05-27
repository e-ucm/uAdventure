using UnityEngine;
using System.Collections;
using System;

namespace uAdventure.Editor
{
    public abstract class DefaultButtonMenuEditorWindowExtension : ButtonMenuEditorWindowExtension
    {
        public DefaultButtonMenuEditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        public DefaultButtonMenuEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        public DefaultButtonMenuEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        public DefaultButtonMenuEditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
            UseAnimation = false;
        }

        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            if (style == null) style = "Button";
            var r = GUI.Button(rect, ButtonContent, style);
            if (r) OnRequestMainView(this);
            return r;
        }

        public override bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options)
        {
            if (style == null) style = "Button";

            var r = GUILayout.Button(ButtonContent, style, options);
            if (r) OnRequestMainView(this);
            return r;
        }

        public override void DrawMenu(Rect rect, GUIStyle aStyle) { }
        public override void LayoutDrawMenu(GUIStyle aStyle, params GUILayoutOption[] aOptions){ }
    }
}

