using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class TabsEditorWindowExtension : DataControlListEditorWindowExtension
    {

        protected List<KeyValuePair<string, Enum>> Tabs;
        protected Dictionary<Enum, LayoutWindow> Childs;

        protected Enum OpenedWindow;
        protected Enum DefaultOpenedWindow;

        public TabsEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            Tabs = new List<KeyValuePair<string, Enum>>();
            Childs = new Dictionary<Enum, LayoutWindow>();
        }

        public override void Draw(int aID)
        {
            if (dataControlList.index < 0)
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

        private Dictionary<int, Rect> rects;
        

        public override void OnDrawMoreWindows()
        {
            if (OpenedWindow != null)
            {
                // Display Window
                var window = Childs[OpenedWindow];
                window.Rect = this.Rect;
                window.OnDrawMoreWindows();
            }
        }

        protected void AddTab(string name, Enum identifier, LayoutWindow window)
        {
            Tabs.Add(new KeyValuePair<string, Enum>(name, identifier));
            Childs.Add(identifier, window);
        }

        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.index = -1;
            OpenedWindow = DefaultOpenedWindow != null ? DefaultOpenedWindow : Tabs[0].Value;
        }

        protected override void OnSelect(ReorderableList r) { }

        protected abstract void OnDrawMainView(int aID);
    }
}

