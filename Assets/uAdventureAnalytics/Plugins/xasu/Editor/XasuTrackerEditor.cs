using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xasu.Editor
{

    [CustomEditor(typeof(XasuTracker))]
    public class XasuTrackerEditor : UnityEditor.Editor
    {
        public override bool HasPreviewGUI()
        {
            return Application.isPlaying && XasuTracker.Instance != null && XasuTracker.Instance.Status != null;
        }

        private Vector2 scroll;
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            // base.OnPreviewGUI(r, background);
            GUI.Label(r, Newtonsoft.Json.JsonConvert.SerializeObject(XasuTracker.Instance.Status, Newtonsoft.Json.Formatting.Indented));
            //GUILayout.BeginArea(r, background);
            /*scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            */
           // string text = "Tracker status not available!";
            //if(Application.isPlaying && XasuTracker.Instance != null && XasuTracker.Instance.Status != null)
            //{
            //    text = Newtonsoft.Json.JsonConvert.SerializeObject(XasuTracker.Instance.Status);
            //}
            //GUILayout.Label(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            //EditorGUILayout.EndScrollView();
            //GUILayout.EndArea();
        }
    }
}

