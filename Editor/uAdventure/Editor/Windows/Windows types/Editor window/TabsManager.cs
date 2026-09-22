using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace uAdventure.Editor
{
    public class TabsManager
    {
        protected readonly LayoutWindow parent;

        public List<KeyValuePair<string, Enum>> Tabs { get; set; }
        public Dictionary<Enum, LayoutWindow> Childs { get; set; }

        public Enum OpenedWindow { get; set; }
        public Enum DefaultOpenedWindow { get; set; }

        public TabsManager(LayoutWindow parent)
        {
            this.parent = parent;
            Tabs = new List<KeyValuePair<string, Enum>>();
            Childs = new Dictionary<Enum, LayoutWindow>();
        }

        public bool Draw(int aID)
        {
            if (Tabs.Count == 0)
            {
                return false;
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
                window.Rect = parent.Rect;
                window.Draw(aID);
                return true;
            }
        }

        public void AddTab(string name, Enum identifier, LayoutWindow window)
        {
            Tabs.Add(new KeyValuePair<string, Enum>(name, identifier));
            window.OnRequestRepaint = () => parent.OnRequestRepaint();
            window.BeginWindows = () => parent.BeginWindows();
            window.EndWindows = () => parent.EndWindows();
            Childs.Add(identifier, window);
        }

        public void Reset()
        {
            OpenedWindow = DefaultOpenedWindow ?? (Tabs.Count > 0 ? Tabs[0].Value : null);
        }
    }
}
