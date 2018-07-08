using UnityEngine;
using System.Collections;
using System;
using UnityEditor.AnimatedValues;
using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class ButtonMenuEditorWindowExtension : EditorWindowExtension
    {
        protected AnimBool extended;
        public GUIContent ButtonContent { get; set; }
        protected bool UseAnimation { get; set; }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }

            set
            {
                base.Selected = value;
                extended.target = Selected;
            }
        }

        protected ButtonMenuEditorWindowExtension(Rect rect, params GUILayoutOption[] aOptions) : this(rect, null, null, aOptions) { }
        protected ButtonMenuEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected ButtonMenuEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected ButtonMenuEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            UseAnimation = true;
            extended = new AnimBool(false);
        }

        public override void DrawLeftPanelContent(Rect rect, GUIStyle style)
        {
            var bv = new Vector2(ButtonHeight, 0);
            var buttonRect = new Rect(rect.position, bv + new Vector2(0, rect.width));
            var menuRect = new Rect(rect.position + bv, rect.size - bv);

            if(DrawButton(buttonRect, style))
            {
                OnButton();
            }
            DrawMenu(menuRect, style);
        }

        public override void LayoutDrawLeftPanelContent(GUIStyle style, params GUILayoutOption[] options)
        {
            if (LayoutDrawButton(style, options))
            {
                OnButton();
            }

            if (!UseAnimation || EditorGUILayout.BeginFadeGroup(extended.faded))
            {
                LayoutDrawMenu(style, options);
            }

            if (UseAnimation)
            {
                EditorGUILayout.EndFadeGroup();
                OnRequestRepaint();
            }
        }

        void UpdateTotalHeight()
        {
            ContentHeight = buttonHeight + menuHeight;
        }

        // Button part
        protected float buttonHeight;
        public float ButtonHeight { get { return buttonHeight; } set { buttonHeight = value; UpdateTotalHeight(); } }
        public abstract bool DrawButton(Rect rect, GUIStyle style);
        public abstract bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options);
        public virtual bool LayoutDrawButton() { return LayoutDrawButton("Button"); }

        // Menu part
        protected float menuHeight;
        public float MenuHeight { get { return menuHeight; } set { menuHeight = value; UpdateTotalHeight(); } }
        public abstract void DrawMenu(Rect rect, GUIStyle style);
        public abstract void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options);
        public virtual void LayoutDrawMenu() { LayoutDrawMenu(null); }

        // OnButton
        protected abstract void OnButton();
    }
}

