using System;
using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class TabsEditorWindowExtension : DataControlListEditorWindowExtension
    {

        protected TabsManager tabsManager;

        public List<KeyValuePair<string, Enum>> Tabs { get { return tabsManager.Tabs; } set { tabsManager.Tabs = value; } }
        public Dictionary<Enum, LayoutWindow> Childs { get { return tabsManager.Childs; } set { tabsManager.Childs = value; } }

        public Enum OpenedWindow { get { return tabsManager.OpenedWindow; } set { tabsManager.OpenedWindow = value; } }
        public Enum DefaultOpenedWindow { get { return tabsManager.DefaultOpenedWindow; } set { tabsManager.DefaultOpenedWindow = value; } }

        protected TabsEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            tabsManager = new TabsManager(this);
        }

        public override void Draw(int aID)
        {
            if (dataControlList.index < 0 || !tabsManager.Draw(aID))
            {
                OnDrawMainView(aID);
            }
        }        

        protected void AddTab(string name, Enum identifier, LayoutWindow window)
        {
            tabsManager.AddTab(name, identifier, window);
        }

        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.index = -1;
            tabsManager.Reset();
        }

        protected abstract void OnDrawMainView(int aID);
    }
}

