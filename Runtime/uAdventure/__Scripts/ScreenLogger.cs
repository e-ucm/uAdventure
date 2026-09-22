using UnityEngine;
using System.Collections.Generic;

namespace uAdventure.Runner
{
    public class ScreenLogger : MonoBehaviour
    {

        private List<string> logs = new List<string>();

        void OnEnable()
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        void OnDisable()
        {
            // Remove callback when object goes out of scope
            Application.logMessageReceived -= Application_logMessageReceived;
        }

        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            logs.Add(type + " : " + condition + " at " + stackTrace);
        }

        Vector2 sc;
        bool showLog = false;
        void OnGUI()
        {
            if (GUILayout.Button("Show/Hide log", GUILayout.Width(Screen.width), GUILayout.Height(50))) showLog = !showLog;
            if (showLog)
            {

                sc = GUILayout.BeginScrollView(sc, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
                GUILayout.BeginVertical();
                logs.ForEach(l => GUILayout.Label(l));
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            }
        }

    }

}