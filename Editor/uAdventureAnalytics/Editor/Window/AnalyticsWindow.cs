using UnityEngine;
using uAdventure.Core;
using uAdventure.Editor;
using System.Collections.Generic;

namespace uAdventure.Analytics
{
    [EditorWindowExtension(200, typeof(AnalyticsWindow))]
    public class AnalyticsWindow : DefaultButtonMenuEditorWindowExtension
    {
        private enum AnalyticsWindowType
        {
            Completables,
            TrackerConfig
        }


        private AnalyticsWindowType openedWindow = AnalyticsWindowType.Completables;

        private readonly BaseWindow[] windows;

        private readonly List<KeyValuePair<string, AnalyticsWindowType>> tabs;

        public AnalyticsWindow(Rect aStartPos, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Analytics.Title")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/assessmentProfiles"),
                text = "Analytics.Title"
            };

            windows = new BaseWindow[2]
            {
                new CompletablesWindow(aStartPos,
                    new GUIContent(TC.get("Completables")), "Window"),
                new TrackerConfigWindow(aStartPos,
                    new GUIContent(TC.get("TrackerConfig")), "Window")
            };

            foreach (var window in windows)
            {
                window.BeginWindows = BeginWindows;
                window.EndWindows = EndWindows;
                window.OnRequestRepaint = OnRequestRepaint;
            }

            tabs = new List<KeyValuePair<string, AnalyticsWindowType>>()
            {
                new KeyValuePair<string, AnalyticsWindowType>(TC.get("Completables"), AnalyticsWindowType.Completables),
                new KeyValuePair<string, AnalyticsWindowType>(TC.get("TrackerConfig"), AnalyticsWindowType.TrackerConfig)
            };
        }


        public override void Draw(int aID)
        {

            /**
             UPPER MENU
            */
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            openedWindow = tabs[GUILayout.Toolbar(tabs.FindIndex(t => t.Value == openedWindow), tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            var windowIndex = (int)openedWindow;
            if (windowIndex < 0 || windowIndex >= windows.Length)
            {
                return;
            }

            var currentWindow = windows[windowIndex];
            currentWindow.Rect = Rect;
            currentWindow.Draw(aID);
        }

        protected override void OnButton() { }
    }
}