using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ConfirmationDialog : EditorWindow
    {
        protected DialogReceiverInterface reference;
        protected string question;
        protected GUIStyle style;

        public virtual void Init(DialogReceiverInterface e, string startTextContent)
        {
            ConfirmationDialog window = this;
            reference = e;
            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;

            question = startTextContent;
            window.position = new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 500, 100);
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Space(15);
            EditorGUILayout.LabelField(question, EditorStyles.boldLabel);
            GUILayout.Space(15);
            EditorGUILayout.LabelField("GeneralText.ConfirmationQuestion", EditorStyles.wordWrappedLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                reference.OnDialogOk("", this);
                this.Close();
            }
            if (GUILayout.Button(TC.get("GeneralText.Cancel")))
            {
                reference.OnDialogCanceled();
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}