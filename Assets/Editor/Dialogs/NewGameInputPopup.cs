using UnityEngine;
using System.Collections;
using UnityEditor;

public class NewGameInputPopup : BaseInputPopup
{
    void OnGUI()
    {
        EditorGUILayout.LabelField(TC.get("NewGame.Question"), EditorStyles.boldLabel);

        GUILayout.Space(30);

        textContent = GUILayout.TextField(textContent);

        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
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
