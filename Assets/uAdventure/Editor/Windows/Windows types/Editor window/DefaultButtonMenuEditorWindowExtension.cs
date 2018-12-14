using UnityEngine;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class DefaultButtonMenuEditorWindowExtension : ButtonMenuEditorWindowExtension
    {
        protected string buttonStyle;

        protected DefaultButtonMenuEditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected DefaultButtonMenuEditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {
            UseAnimation = false;
        }
        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            var buttonKey = ButtonContent.text; 
            ButtonContent.text = "  " + buttonKey.Traslate();
            var currentSkin = GUI.skin;

            if (style == null)
            {
                if (!string.IsNullOrEmpty(buttonStyle))
                {
                    GUI.skin = uASkin;
                    style = buttonStyle;
                }
                else
                {
                    style = "Button";
                }
            }

            var buttonPressed = GUI.Button(rect, ButtonContent, style);
            GUI.skin = currentSkin;
            if (buttonPressed)
            {
                OnRequestMainView(this);
            }

            ButtonContent.text = buttonKey;
            return buttonPressed;
        }

        public override bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options)
        {
            var buttonKey = ButtonContent.text;
            ButtonContent.text = "  " + buttonKey.Traslate();
            var currentSkin = GUI.skin;

            if (style == null)
            {
                if (!string.IsNullOrEmpty(buttonStyle))
                {
                    GUI.skin = uASkin;
                    style = buttonStyle;
                }
                else
                {
                    style = "Button";
                }
            }

            var buttonPressed = GUILayout.Button(ButtonContent, style, options);
            GUI.skin = currentSkin;
            if (buttonPressed)
            {
                OnRequestMainView(this);
            }

            ButtonContent.text = buttonKey;
            return buttonPressed;
        }

        public override void DrawMenu(Rect rect, GUIStyle style) { }
        public override void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options){ }
    }
}

