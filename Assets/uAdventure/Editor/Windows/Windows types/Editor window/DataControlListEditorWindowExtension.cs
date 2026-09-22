using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class DataControlListEditorWindowExtension : ButtonMenuEditorWindowExtension
    {
        protected DataControlList dataControlList;

        protected DataControlListEditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        protected DataControlListEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected DataControlListEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected DataControlListEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {

            dataControlList = new DataControlList()
            {
                RequestRepaint = Repaint,
                footerHeight = 25,
                elementHeight = 25,
                headerHeight = 0,
                Columns = new List<ColumnList.Column>() { new ColumnList.Column() { Text = ""/*, SizeOptions = GUILayout.ExpandWidth(true)*/ } },
                drawElementCallback = OnDrawElement,
                onSelectCallback = OnSelect
            };
            
            dataControlList.onRemoveCallback = (list) => 
            {
                list.index = -1;
                OnSelect(list);
            };

        }
        
        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            var buttonKey = ButtonContent.text;
            ButtonContent.text = buttonKey.Traslate();

            if (style == null)
            {
                style = "Button";
            }

            var buttonPressed = GUI.Button(rect, ButtonContent, style);
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
            ButtonContent.text = buttonKey.Traslate();

            if (style == null)
            {
                style = "Button";
            }

            var buttonPressed = GUILayout.Button(ButtonContent, style, options);
            if (buttonPressed)
            {
                OnRequestMainView(this);
            }

            ButtonContent.text = buttonKey;
            return buttonPressed;
        }

        protected override void OnButton()
        {
            dataControlList.index = -1;
        }

        public override void DrawMenu(Rect rect, GUIStyle style)
        {
            dataControlList.DoList(rect.height);
        }


        public override void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options)
        {
            try
            {
                dataControlList.DoList(Math.Max(1, dataControlList.list.Count) * 25 + 30);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        protected virtual void OnDrawElement(Rect cellRect, int index, bool isActive, bool isFocused)
        {
            var element = dataControlList.list[index] as DataControl;
            var content = element.getContent() as HasId;
            if (content == null)
                return;

            if (element.canBeRenamed() && isActive)
            {
                EditorGUI.BeginChangeCheck();
                var newName = EditorGUI.DelayedTextField(cellRect, content.getId());
                if (EditorGUI.EndChangeCheck()) element.renameElement(newName);
            }
            else
            {
                EditorGUI.LabelField(cellRect, content.getId());
            }
        }
        protected abstract void OnSelect(ReorderableList r);
    }
}
