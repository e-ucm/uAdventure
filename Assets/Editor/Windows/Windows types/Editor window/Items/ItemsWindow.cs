using UnityEngine;

using uAdventure.Core;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(30, typeof(Item))]
    public class ItemsWindow : ReorderableListEditorWindowExtension
    {
        private enum ItemsWindowType { Appearance, DescriptionConfig, Actions }

        private static ItemsWindowType openedWindow = ItemsWindowType.Appearance;
        private static ItemsWindowActions itemsWindowActions;
        private static ItemsWindowAppearance itemsWindowAppearance;
        private static ItemsWindowDescription itemsWindowDescription;
        
        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

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
            
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {
            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                /**
                UPPER MENU
                */
                GUILayout.BeginHorizontal();
                if (openedWindow == ItemsWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Item.LookPanelTitle")))
                {
                    OnWindowTypeChanged(ItemsWindowType.Appearance);
                }
                if (openedWindow == ItemsWindowType.Appearance)
                    GUI.skin = defaultSkin;

                if (openedWindow == ItemsWindowType.Actions)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Item.ActionsPanelTitle")))
                {
                    OnWindowTypeChanged(ItemsWindowType.Actions);
                }
                if (openedWindow == ItemsWindowType.Actions)
                    GUI.skin = defaultSkin;

                if (openedWindow == ItemsWindowType.DescriptionConfig)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Item.DocPanelTitle")))
                {
                    OnWindowTypeChanged(ItemsWindowType.DescriptionConfig);
                }
                if (openedWindow == ItemsWindowType.DescriptionConfig)
                    GUI.skin = defaultSkin;

                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case ItemsWindowType.Actions:
                        itemsWindowActions.Rect = Rect;
                        itemsWindowActions.Draw(aID);
                        break;
                    case ItemsWindowType.Appearance:
                        itemsWindowAppearance.Rect = Rect;
                        itemsWindowAppearance.Draw(aID);
                        break;
                    case ItemsWindowType.DescriptionConfig:
                        itemsWindowDescription.Rect = Rect;
                        itemsWindowDescription.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterData().getItems().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.Instance.ChapterList.getSelectedChapterData().getItems()[i].getId(), GUILayout.Width(m_Rect.width * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(m_Rect.width * 0.2f)))
                    {
                        ShowItemWindowView(i);
                    }

                    GUILayout.EndHorizontal();

                }
            }
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            isConcreteItemVisible = false;
            GameRources.GetInstance().selectedItemIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            isConcreteItemVisible = true;
            GameRources.GetInstance().selectedItemIndex = o;

            itemsWindowActions = new ItemsWindowActions(m_Rect, new GUIContent(TC.get("Item.ActionsPanelTitle")), "Window");
            itemsWindowAppearance = new ItemsWindowAppearance(m_Rect, new GUIContent(TC.get("Item.LookPanelTitle")), "Window");
            itemsWindowDescription = new ItemsWindowDescription(m_Rect, new GUIContent(TC.get("Item.DocPanelTitle")), "Window");
        }

        void OnWindowTypeChanged(ItemsWindowType type_)
        {
            openedWindow = type_;
        }
        
        //////////////////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.Instance.ChapterList.getSelectedChapterDataControl().getItemsList().getItems ()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance                           .ChapterList                           .getSelectedChapterDataControl()
                           .getItemsList()
                           .duplicateElement(
                               Controller.Instance                                   .ChapterList                                   .getSelectedChapterDataControl()
                                   .getItemsList()
                                   .getItems()[r.index]);
            }
            else
            {
                Controller.Instance.SelectedChapterDataControl.getItemsList().addElement(Controller.ITEM, "newItem");
            }

        }

        protected override void OnAddOption(ReorderableList r, string option)
        {
            // No options
        }

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.Instance                              .ChapterList                              .getSelectedChapterDataControl()
                              .getItemsList()
                              .deleteElement(
                                  Controller.Instance                                      .ChapterList                                      .getSelectedChapterDataControl()
                                      .getItemsList()
                                      .getItems()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
			var itemsListDataControl = Controller.Instance 				.ChapterList .getSelectedChapterDataControl ().getItemsList ();

			var toPos = r.index;
			var fromPos = itemsListDataControl.getItems ().FindIndex (i => i.getId () == r.list [r.index] as string);

			itemsListDataControl.MoveElement (itemsListDataControl.getItems ()[fromPos], fromPos, toPos);
		}



        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getItemsList ().getItems ().ConvertAll(s => s.getId());
        }
    }
}