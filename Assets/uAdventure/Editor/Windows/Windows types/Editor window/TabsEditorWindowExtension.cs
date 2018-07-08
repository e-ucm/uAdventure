using System;
using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class TabsEditorWindowExtension : DataControlListEditorWindowExtension
    {

        protected List<KeyValuePair<string, Enum>> Tabs;
        protected Dictionary<Enum, LayoutWindow> Childs;

        protected Enum OpenedWindow;
        protected Enum DefaultOpenedWindow;

        protected TabsEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            Tabs = new List<KeyValuePair<string, Enum>>();
            Childs = new Dictionary<Enum, LayoutWindow>();
        }

        public override void Draw(int aID)
        {
            if (dataControlList.index < 0 || Tabs.Count == 0)
            {
                OnDrawMainView(aID);
            }
            else
            {
                // Tabs menu
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                OpenedWindow = Tabs[GUILayout.Toolbar(Tabs.FindIndex(t => Enum.Equals(t.Value, OpenedWindow)), Tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // Display Window
                var window = Childs[OpenedWindow];
                window.Rect = this.Rect;
                window.Draw(aID);
            }
        }        

        protected void AddTab(string name, Enum identifier, LayoutWindow window)
        {
            Tabs.Add(new KeyValuePair<string, Enum>(name, identifier));
            window.OnRequestRepaint = () => OnRequestRepaint();
            window.BeginWindows = () => BeginWindows();
            window.EndWindows = () => EndWindows();
            Childs.Add(identifier, window);
        }

        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.index = -1;
            OpenedWindow = DefaultOpenedWindow ?? Tabs[0].Value;
        }

        protected abstract void OnDrawMainView(int aID);
    }
}

