using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public class ButtonList : ColumnList
    {
        public float buttonWidth = 25;

        private class Defaults
        {
            public float buttonLeftMargin = 4f;
            public float buttonTopMargin = -1f;
        }

        private static Defaults m_defaults;
        private static Defaults defaults
        {
            get
            {
                return m_defaults ?? (m_defaults = new Defaults());
            }
        }

        public class Button
        {
            public GUIContent content;
            public float width = 25f;
            public ButtonEnabledDeletate onButtonEnabledCallback;
            public ButtonPressedDelegate onButtonPressedCallback;
        }

        public List<Button> buttons = new List<Button>();

        public delegate bool ButtonEnabledDeletate(ReorderableList list);
        public delegate void ButtonPressedDelegate(Rect rect, ReorderableList list);

        private void OnDrawFooter(Rect rect)
        {
            // Update footerheight
            if (this.footerHeight == 0 && buttons.Count > 0)
            {
                this.footerHeight = 13f;
                this.Repaint();
            }
            else if (this.footerHeight > 0 && buttons.Count == 0)
            {
                this.footerHeight = 0;
                this.Repaint();
            }

            float xMax = rect.xMax;
            float num = xMax - (buttonWidth + defaults.buttonLeftMargin) * buttons.Count;

            rect = new Rect(num, rect.y, xMax - num, rect.height);

            Rect buttonRect = new Rect(num + defaults.buttonLeftMargin, rect.y - 1f, 22f, 13f);

            if (Event.current.type == EventType.Repaint)
            {
                ScrollableList.defaultBehaviours.footerBackground.Draw(rect, false, false, false, false);
            }

            var buttonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button);
            buttonStyle.margin = new RectOffset(3, 0, 0, 0);
            buttonStyle.padding = new RectOffset(3, 0, 0, 0);
            buttonStyle.normal.background = buttonStyle.onNormal.background = null;
            buttonStyle.active.background = buttonStyle.onActive.background = null;
            buttonStyle.focused.background = buttonStyle.onFocused.background = null;
            buttonStyle.hover.background = buttonStyle.onHover.background = null;

            foreach (var button in buttons)
            {
                // Remove
                using (new EditorGUI.DisabledScope(button.onButtonEnabledCallback != null && !button.onButtonEnabledCallback(reorderableList)))
                {
                    if (GUI.Button(buttonRect, button.content, buttonStyle))
                    {
                        if (button.onButtonPressedCallback != null)
                        {
                            button.onButtonPressedCallback(buttonRect, reorderableList);
                        }
                        if (onChangedCallback != null)
                        {
                            onChangedCallback(reorderableList);
                        }
                    }
                    buttonRect.x += buttonRect.width + defaults.buttonLeftMargin;
                }
            }
        }

        public ButtonList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
        {
            this.drawFooterCallback = OnDrawFooter;
        }

        public ButtonList(IList elements, Type elementType) : base(elements, elementType)
        {
            this.drawFooterCallback = OnDrawFooter;
        }

        public ButtonList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader) : base(serializedObject, elements, draggable, displayHeader, false, false)
        {
            this.drawFooterCallback = OnDrawFooter;
        }

        public ButtonList(IList elements, Type elementType, bool draggable, bool displayHeader) : base(elements, elementType, draggable, displayHeader, false, false)
        {
            this.drawFooterCallback = OnDrawFooter;
        }
    }
}