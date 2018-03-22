using UnityEngine;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class InputDialog : BaseInputPopup
    {
        private object token;
        private string value;
        private string message;

        public void Init(DialogReceiverInterface e, object token, string title, string message, string defaultValue = null)
        {
            this.message = message;
            this.value = defaultValue;
            this.titleContent = new GUIContent(title);
            this.token = token;
            this.Init(e);
        }

        public override void Init(DialogReceiverInterface e)
        {
            base.Init(e);
        }

        void OnGUI()
        {
            EditorWindow.FocusWindowIfItsOpen<InputDialog>();

            // Message
            EditorGUILayout.LabelField(message, EditorStyles.boldLabel);
            GUILayout.Space(20);

            // Input field
            value = EditorGUILayout.TextField(value);
            
            // Bottom buttons
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                if (string.IsNullOrEmpty(value))
                    GUI.enabled = false;

                if (GUILayout.Button(TC.get("GeneralText.OK")))
                {
                    reference.OnDialogOk(value, token ?? this);
                    this.Close();
                }

                GUI.enabled = true;
                if (GUILayout.Button(TC.get("GeneralText.Cancel")))
                {
                    reference.OnDialogCanceled();
                    this.Close();
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    public class ChooseObjectDialog : BaseChooseObjectPopup
    {
        private bool okActive = true;
        private string message;
        private object token;

        public void Init(DialogReceiverInterface e, object token, string title, string message, string[] elements)
        {
            this.titleContent = new GUIContent(title);
            this.message = message;
            this.elements = elements;
            this.token = token;
            this.Init(e);
        }

        public override void Init(DialogReceiverInterface e)
        {
            if (elements == null)
            {
                elements = new string[1];
                elements[0] = "None";
                okActive = false;
            }

            selectedElementID = elements[0];

            base.Init(e);
        }

        void OnGUI()
        {
            EditorWindow.FocusWindowIfItsOpen<ChooseObjectDialog>();

            // Message
            EditorGUILayout.LabelField(message, EditorStyles.boldLabel);
            GUILayout.Space(20);

            // Input field
            selectedElementID = elements[EditorGUILayout.Popup(Array.IndexOf(elements, selectedElementID), elements)];

            // Bottom buttons
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            {
                if (!okActive)
                    GUI.enabled = false;

                if (GUILayout.Button(TC.get("GeneralText.OK")))
                {
                    reference.OnDialogOk(selectedElementID, token ?? this);
                    this.Close();
                }

                GUI.enabled = true;
                if (GUILayout.Button(TC.get("GeneralText.Cancel")))
                {
                    reference.OnDialogCanceled();
                    this.Close();
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}