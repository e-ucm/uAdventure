using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(40, typeof(AtrezzoDataControl))]
    public class SetItemsWindow : PreviewDataControlExtension
    {
        private enum SetItemsWindowType { Appearance, Documentation }
        
        private static SetItemsWindowApperance setItemsWindowApperance;
        private static SetItemsWindowDocumentation setItemsWindowDocumentation;

        public SetItemsWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name59")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/Atrezzo-List-1"),
                text = "Element.Name59"
            };

            setItemsWindowApperance = new SetItemsWindowApperance(aStartPos, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
            setItemsWindowDocumentation = new SetItemsWindowDocumentation(aStartPos, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");

            AddTab(TC.get("Atrezzo.LookPanelTitle"), SetItemsWindowType.Appearance, setItemsWindowApperance);
            AddTab(TC.get("Atrezzo.DocPanelTitle"), SetItemsWindowType.Documentation, setItemsWindowDocumentation);
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            GameRources.GetInstance().selectedSetItemIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
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