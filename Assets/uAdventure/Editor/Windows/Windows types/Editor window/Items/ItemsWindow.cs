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
        
        private static ItemsWindowAppearance itemsWindowAppearance;

        public ItemsWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name18")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/items"),
                text = "Element.Name18"
            };

            var itemsWindowActions = new ItemsWindowActions(aStartPos, new GUIContent(TC.get("Item.ActionsPanelTitle")), "Window");
            itemsWindowAppearance = new ItemsWindowAppearance(aStartPos, new GUIContent(TC.get("Item.LookPanelTitle")), "Window");
            var itemsWindowDescription = new ItemsWindowDescription(aStartPos, new GUIContent(TC.get("Item.DocPanelTitle")), "Window");

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