using UnityEngine;

using uAdventure.Core;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(90, typeof(AdvancedFeaturesWindow))]
    public class AdvancedFeaturesWindow : DefaultButtonMenuEditorWindowExtension
    {
        private enum AdvancedFeaturesWindowType
        {
            GlobalStates,
            ListOfTimers,
            Macros
        }
        

        private AdvancedFeaturesWindowType openedWindow = AdvancedFeaturesWindowType.ListOfTimers;

        private readonly BaseWindow[] windows;

        private readonly List<KeyValuePair<string, AdvancedFeaturesWindowType>> tabs;

        public AdvancedFeaturesWindow(Rect aStartPos, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("AdvancedFeatures.Title")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent
            {
                text = "AdvancedFeatures.Title",
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/advanced")
            };

            windows = new BaseWindow[3]
            {
                new AdvancedFeaturesWindowGlobalStates(aStartPos,
                    new GUIContent(TC.get("Element.Name55")), "Window"),
                new AdvancedFeaturesWindowListOfTimers(aStartPos,
                    new GUIContent(TC.get("TimersList.Title")), "Window"),
                new AdvancedFeaturesWindowMacros(aStartPos,
                    new GUIContent(TC.get("Element.Name57")), "Window")
            };

            foreach(var window in windows)
            {
                window.BeginWindows = BeginWindows;
                window.EndWindows = EndWindows;
                window.OnRequestRepaint = OnRequestRepaint;
            }
            
            tabs = new List<KeyValuePair<string, AdvancedFeaturesWindowType>>()
            {
                new KeyValuePair<string, AdvancedFeaturesWindowType>(TC.get("TimersList.Title"), AdvancedFeaturesWindowType.ListOfTimers),
                new KeyValuePair<string, AdvancedFeaturesWindowType>(TC.get("Element.Name55"),   AdvancedFeaturesWindowType.GlobalStates),
                new KeyValuePair<string, AdvancedFeaturesWindowType>(TC.get("Element.Name57"),   AdvancedFeaturesWindowType.Macros)
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
            if (windowIndex< 0 || windowIndex >= windows.Length)
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