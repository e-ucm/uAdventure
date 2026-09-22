using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class DefaultButtonMenuEditorWindowExtension : ButtonMenuEditorWindowExtension
    {
        protected DefaultButtonMenuEditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
            UseAnimation = false;
        }

        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            var previousText = ButtonContent.text;
            ButtonContent.text = ButtonContent.text.Traslate();
            if (style == null)
            {
                style = "Button";
            }
            var r = GUI.Button(rect, ButtonContent, style);
            if (r)
            {
                OnRequestMainView(this);
            }
            ButtonContent.text = previousText;
            return r;
        }

        public override bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options)
        {
            var previousText = ButtonContent.text;
            ButtonContent.text = ButtonContent.text.Traslate();
            if (style == null)
            {
                style = "Button";
            }
            var r = GUILayout.Button(ButtonContent, style, options);
            if (r)
            {
                OnRequestMainView(this);
            }
            ButtonContent.text = previousText;
            return r;
        }

        public override void DrawMenu(Rect rect, GUIStyle style) { }
        public override void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options){ }
    }
}

