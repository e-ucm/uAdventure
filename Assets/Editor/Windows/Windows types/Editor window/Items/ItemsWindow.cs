using UnityEngine;

using uAdventure.Core;
using UnityEditorInternal;
using System.Collections.Generic;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(30, typeof(Item))]
    public class ItemsWindow : PreviewDataControlExtension 
    {
        private enum ItemsWindowType { Appearance, DescriptionConfig, Actions }

        private static ItemsWindowType openedWindow = ItemsWindowType.Appearance;
        private static ItemsWindowActions itemsWindowActions;
        private static ItemsWindowAppearance itemsWindowAppearance;
        private static ItemsWindowDescription itemsWindowDescription;

        public ItemsWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name18")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/items", typeof(Texture2D));
            c.text = TC.get("Element.Name18");
            this.ButtonContent = c;

            itemsWindowActions = new ItemsWindowActions(aStartPos, new GUIContent(TC.get("Item.ActionsPanelTitle")), "Window");
            itemsWindowAppearance = new ItemsWindowAppearance(aStartPos, new GUIContent(TC.get("Item.LookPanelTitle")), "Window");
            itemsWindowDescription = new ItemsWindowDescription(aStartPos, new GUIContent(TC.get("Item.DocPanelTitle")), "Window");

            AddTab(TC.get("Item.LookPanelTitle"), ItemsWindowType.Appearance, itemsWindowAppearance);
            AddTab(TC.get("Item.ActionsPanelTitle"), ItemsWindowType.Actions, itemsWindowActions);
            AddTab(TC.get("Item.DocPanelTitle"), ItemsWindowType.DescriptionConfig, itemsWindowDescription);
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            GameRources.GetInstance().selectedItemIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            GameRources.GetInstance().selectedItemIndex = o;
        }

        void OnWindowTypeChanged(ItemsWindowType type_)
        {
            openedWindow = type_;
        }
        
        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }


        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getItemsList(),
                sceneList => (sceneList as ItemsListDataControl).getItems().Cast<DataControl>().ToList());
            ShowBaseWindowView();
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            itemsWindowAppearance.Target = dataControlList.list[index] as DataControl;
            itemsWindowAppearance.DrawPreview(rect);
            itemsWindowAppearance.Target = null;
        }
    }
}