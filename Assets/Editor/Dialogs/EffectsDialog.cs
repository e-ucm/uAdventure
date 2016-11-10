using UnityEngine;
using System.Collections;
using UnityEditor;

public class EffectsDialog : EditorWindow
{

    string prefabName;

    void OnGUI()
    {
        prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);

        if (GUILayout.Button("Save Prefab"))
        {
            OnClickSavePrefab();
            GUIUtility.ExitGUI();
        }
    }

    void OnClickSavePrefab()
    {
        prefabName = prefabName.Trim();

        if (string.IsNullOrEmpty(prefabName))
        {
            EditorUtility.DisplayDialog("Unable to save prefab", "Please specify a valid prefab name.", "Close");
            return;
        }

        // You may also want to check for illegal characters :)

        // Save your prefab

        Close();
    }

}