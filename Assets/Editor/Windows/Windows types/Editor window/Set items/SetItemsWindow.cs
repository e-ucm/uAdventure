using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(40, typeof(Atrezzo))]
    public class SetItemsWindow : ReorderableListEditorWindowExtension
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
            
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                /**
                 UPPER MENU
                */
                GUILayout.BeginHorizontal();
                if (openedWindow == SetItemsWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Atrezzo.LookPanelTitle")))
                {
                    OnWindowTypeChanged(SetItemsWindowType.Appearance);
                }
                if (openedWindow == SetItemsWindowType.Appearance)
                    GUI.skin = defaultSkin;

                if (openedWindow == SetItemsWindowType.Documentation)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Atrezzo.DocPanelTitle")))
                {
                    OnWindowTypeChanged(SetItemsWindowType.Documentation);
                }
                if (openedWindow == SetItemsWindowType.Documentation)
                    GUI.skin = defaultSkin;
                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case SetItemsWindowType.Appearance:
                        setItemsWindowApperance.Rect = Rect;
                        setItemsWindowApperance.Draw(aID);
                        break;
                    case SetItemsWindowType.Documentation:
                        setItemsWindowApperance.Rect = Rect;
                        setItemsWindowDocumentation.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterData().getAtrezzo().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.Instance.ChapterList.getSelectedChapterData().getAtrezzo()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
                    {
                        ShowItemWindowView(i);
                    }

                    GUILayout.EndHorizontal();
                }
            }
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

            setItemsWindowApperance = new SetItemsWindowApperance(m_Rect, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
            setItemsWindowDocumentation = new SetItemsWindowDocumentation(m_Rect, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");
        }

        /////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.Instance.ChapterList.getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList ()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance                           .ChapterList                           .getSelectedChapterDataControl()
                           .getAtrezzoList()
                           .duplicateElement(
                               Controller.Instance                                   .ChapterList                                   .getSelectedChapterDataControl()
                                   .getAtrezzoList()
                                   .getAtrezzoList()[r.index]);
            }
            else
            {
                Controller.Instance.SelectedChapterDataControl.getAtrezzoList().addElement(Controller.ATREZZO, "newAtrezzo");
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
                              .getAtrezzoList()
                              .deleteElement(
                                  Controller.Instance                                      .ChapterList                                      .getSelectedChapterDataControl()
                                      .getAtrezzoList()
                                      .getAtrezzoList()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
			var dataControlList = Controller.Instance 				.ChapterList .getSelectedChapterDataControl ().getAtrezzoList ();

			var toPos = r.index;
			var fromPos = dataControlList.getAtrezzoList ().FindIndex (i => i.getId () == r.list [r.index] as string);

			dataControlList.MoveElement (dataControlList.getAtrezzoList ()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getAtrezzoList ().getAtrezzoList().ConvertAll(s => s.getId());
        }
    }
}