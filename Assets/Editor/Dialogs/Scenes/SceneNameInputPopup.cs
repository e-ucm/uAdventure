using UnityEngine;
using System.Collections;
using UnityEditor;

public class SceneNameInputPopup : BaseInputPopup
{

    void OnGUI()
    {
        EditorGUILayout.LabelField(TC.get("Scene.NewQuestion"), EditorStyles.wordWrappedLabel);

        GUILayout.Space(30);

        textContent = GUILayout.TextField(textContent);

        GUILayout.Space(30);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("OK"))
        {
            reference.OnDialogOk(textContent, this);
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
