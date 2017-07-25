using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChapterNewNameInputPopup : BaseInputPopup
    {
        void OnGUI()
        {
            EditorGUILayout.LabelField(TC.get("Operation.AddChapterMessage"), EditorStyles.wordWrappedLabel);

            GUILayout.Space(30);

            textContent = GUILayout.TextField(textContent);

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            // Disable button ok if name is not valid
            if (!Controller.Instance.isElementIdValid(textContent, false))
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button("Ok"))
            {
                reference.OnDialogOk(textContent, this);
                this.Close();
            }
            GUI.enabled = true;
            if (GUILayout.Button(TC.get("GeneralText.Cancel")))
            {
                reference.OnDialogCanceled(this);
                this.Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}