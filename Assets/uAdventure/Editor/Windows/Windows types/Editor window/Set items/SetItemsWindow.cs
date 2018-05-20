using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(40, typeof(Atrezzo))]
    public class SetItemsWindow : PreviewDataControlExtension
    {
        private enum SetItemsWindowType { Appearance, Documentation }

        private static SetItemsWindowType openedWindow = SetItemsWindowType.Appearance;
        private static SetItemsWindowApperance setItemsWindowApperance;
        private static SetItemsWindowDocumentation setItemsWindowDocumentation;
        
        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public SetItemsWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name59")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/Atrezzo-List-1", typeof(Texture2D));
            c.text = TC.get("Element.Name59");
            ButtonContent = c;

            setItemsWindowApperance = new SetItemsWindowApperance(aStartPos, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
            setItemsWindowDocumentation = new SetItemsWindowDocumentation(aStartPos, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");
            
            selectedButtonSkin = Resources.Load<GUISkin>("EAdventureData/skin/ButtonSelected");

            AddTab(TC.get("Atrezzo.LookPanelTitle"), SetItemsWindowType.Appearance, setItemsWindowApperance);
            AddTab(TC.get("Atrezzo.DocPanelTitle"), SetItemsWindowType.Documentation, setItemsWindowDocumentation);
        }

        void OnWindowTypeChanged(SetItemsWindowType type_)
        {
            openedWindow = type_;
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            isConcreteItemVisible = false;
            GameRources.GetInstance().selectedSetItemIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            isConcreteItemVisible = true;
            GameRources.GetInstance().selectedSetItemIndex = o;
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnButton()
        {
            base.OnButton();
            ShowBaseWindowView();
            dataControlList.index = -1;
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getAtrezzoList(),
                list => (list as AtrezzoListDataControl).getAtrezzoList().Cast<DataControl>().ToList());
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            setItemsWindowApperance.Target = dataControlList.list[index] as DataControl;
            setItemsWindowApperance.DrawPreview(rect);
            setItemsWindowApperance.Target = null;
        }
    }
}