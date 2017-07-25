using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEngine;
using UnityEditor;

public class FileSelector : MonoBehaviour {

    private static Texture2D clearImg;

	public string Do(Rect rect, string label, string value, string[] filters)
    {
        var r = value;

        GUILayout.Label(label);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(clearImg, GUILayout.MaxWidth(clearImg.width)))
        {
            r = "";
        }

        GUILayout.Box(r, GUILayout.ExpandWidth(true));
        if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(150)))
        {
            var result = EditorUtility.OpenFilePanelWithFilters(TC.get("FilePanel.Title"), r != "" ? r : "Assets/", filters);
            if(result != "")
            {
                r = result;
            }
        }
        GUILayout.EndHorizontal();

        return r;
    }
}
