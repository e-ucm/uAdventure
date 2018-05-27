using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(90, typeof(AdvancedFeaturesWindow))]
    public class AdvancedFeaturesWindow : DefaultButtonMenuEditorWindowExtension
    {
        private enum AdvencedFeaturesWindowType
        {
            GlobalStates,
            ListOfTimers,
            Macros
        }
        

        private static AdvencedFeaturesWindowType openedWindow = AdvencedFeaturesWindowType.ListOfTimers;

        private static AdvencedFeaturesWindowGlobalStates advencedFeaturesWindowGlobalStates;
        private static AdvencedFeaturesWindowListOfTimers advencedFeaturesWindowListOfTimers;
        private static AdvencedFeaturesWindowMacros advencedFeaturesWindowMacros;

        private List<KeyValuePair<string, AdvencedFeaturesWindowType>> tabs;

        public AdvancedFeaturesWindow(Rect aStartPos, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("AdvancedFeatures.Title")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/advanced", typeof(Texture2D));
            c.text = TC.get("AdvancedFeatures.Title");
            ButtonContent = c;

            advencedFeaturesWindowGlobalStates = new AdvencedFeaturesWindowGlobalStates(aStartPos,
                new GUIContent(TC.get("Element.Name55")), "Window");
            advencedFeaturesWindowListOfTimers = new AdvencedFeaturesWindowListOfTimers(aStartPos,
                new GUIContent(TC.get("TimersList.Title")), "Window");
            advencedFeaturesWindowMacros = new AdvencedFeaturesWindowMacros(aStartPos,
                new GUIContent(TC.get("Element.Name57")), "Window");


            tabs = new List<KeyValuePair<string, AdvencedFeaturesWindowType>>()
                {
                    new KeyValuePair<string, AdvencedFeaturesWindowType>(TC.get("TimersList.Title"),      AdvencedFeaturesWindowType.ListOfTimers),
                    new KeyValuePair<string, AdvencedFeaturesWindowType>(TC.get("Element.Name55"),       AdvencedFeaturesWindowType.GlobalStates),
                    new KeyValuePair<string, AdvencedFeaturesWindowType>(TC.get("Element.Name57"),  AdvencedFeaturesWindowType.Macros)
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

            switch (openedWindow)
            {
                case AdvencedFeaturesWindowType.GlobalStates:
                    advencedFeaturesWindowGlobalStates.Rect = Rect;
                    advencedFeaturesWindowGlobalStates.Draw(aID);
                    break;
                case AdvencedFeaturesWindowType.ListOfTimers:
                    advencedFeaturesWindowListOfTimers.Rect = Rect;
                    advencedFeaturesWindowListOfTimers.Draw(aID);
                    break;
                case AdvencedFeaturesWindowType.Macros:
                    advencedFeaturesWindowMacros.Rect = Rect;
                    advencedFeaturesWindowMacros.Draw(aID);
                    break;
            }
        }


        void OnWindowTypeChanged(AdvencedFeaturesWindowType type_)
        {
            openedWindow = type_;
        }

        protected override void OnButton()
        {
        }
    }
}