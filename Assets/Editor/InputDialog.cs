using UnityEngine;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class InputDialog : BaseChooseObjectPopup
    {
        private bool okActive = true;
        private string message;

        public void Init(DialogReceiverInterface e, string message, string[] elements)
        {
            this.message = message;
            this.elements = elements;
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
            EditorWindow.FocusWindowIfItsOpen<InputDialog>();
            EditorGUILayout.LabelField(message, EditorStyles.boldLabel);

            GUILayout.Space(20);

            selectedElementID = elements[EditorGUILayout.Popup(Array.IndexOf(elements, selectedElementID), elements)];

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (!okActive)
                GUI.enabled = false;
            if (GUILayout.Button("OK"))
            {
                reference.OnDialogOk(selectedElementID, this);
                this.Close();
            }
            GUI.enabled = true;

            if (GUILayout.Button(TC.get("GeneralText.Cancel")))
            {
                reference.OnDialogCanceled();
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}