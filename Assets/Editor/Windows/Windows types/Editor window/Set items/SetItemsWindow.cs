using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class SetItemsWindow : ReorderableListEditorWindowExtension
    {
        private enum SetItemsWindowType { Appearance, Documentation }

        private static SetItemsWindowType openedWindow = SetItemsWindowType.Appearance;
        private static SetItemsWindowApperance setItemsWindowApperance;
        private static SetItemsWindowDocumentation setItemsWindowDocumentation;

        private static float windowWidth, windowHeight;
        private static Rect thisRect;

        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public SetItemsWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/Atrezzo-List-1", typeof(Texture2D));
            c.text = TC.get("Element.Name59");
            ButtonContent = c;

            setItemsWindowApperance = new SetItemsWindowApperance(aStartPos, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
            setItemsWindowDocumentation = new SetItemsWindowDocumentation(aStartPos, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");

            windowWidth = aStartPos.width;
            windowHeight = aStartPos.height;

            thisRect = aStartPos;
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
                        setItemsWindowApperance.Draw(aID);
                        break;
                    case SetItemsWindowType.Documentation:
                        setItemsWindowDocumentation.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
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

            setItemsWindowApperance = new SetItemsWindowApperance(thisRect, new GUIContent(TC.get("Atrezzo.LookPanelTitle")), "Window");
            setItemsWindowDocumentation = new SetItemsWindowDocumentation(thisRect, new GUIContent(TC.get("Atrezzo.DocPanelTitle")), "Window");
        }

        /////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo()[index].setId(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.getInstance()
                           .getCharapterList()
                           .getSelectedChapterDataControl()
                           .getAtrezzoList()
                           .duplicateElement(
                               Controller.getInstance()
                                   .getCharapterList()
                                   .getSelectedChapterDataControl()
                                   .getAtrezzoList()
                                   .getAtrezzoList()[r.index]);
            }
            else
            {
                Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().addElement(Controller.ATREZZO, "newAtrezzo");
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
                Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterDataControl()
                              .getAtrezzoList()
                              .deleteElement(
                                  Controller.getInstance()
                                      .getCharapterList()
                                      .getSelectedChapterDataControl()
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
            List<Atrezzo> previousList = Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterData()
                              .getAtrezzo();

            List<Atrezzo> reordered = new List<Atrezzo>();
            foreach (string name in r.list)
                reordered.Add(previousList.Find(s => s.getId() == name));


            previousList.Clear();
            previousList.AddRange(reordered);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
            Elements = Controller.getInstance().getCharapterList().getSelectedChapterData().getAtrezzo().ConvertAll(s => s.getId());
        }
    }
}