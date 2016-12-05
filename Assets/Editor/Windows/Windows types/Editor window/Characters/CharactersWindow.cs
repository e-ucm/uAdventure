using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class CharactersWindow : ReorderableListEditorWindowExtension
    {
        private enum CharactersWindowType { Action, Appearance, DialogConfiguration, Documentation }

        private static CharactersWindowType openedWindow = CharactersWindowType.DialogConfiguration;
        private static CharactersWindowActions charactersWindowActions;
        private static CharactersWindowAppearance charactersWindowAppearance;
        private static CharactersWindowDialogConfiguration charactersWindowDialogConfiguration;
        private static CharactersWindowDocumentation charactersWindowDocumentation;

        private static float windowWidth, windowHeight;
        private static Rect thisRect;

        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public CharactersWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/npcs", typeof(Texture2D));
            c.text = TC.get("Element.Name27");
            ButtonContent = c;

            charactersWindowActions = new CharactersWindowActions(aStartPos, new GUIContent(TC.get("NPC.ActionsPanelTitle")), "Window");
            charactersWindowAppearance = new CharactersWindowAppearance(aStartPos, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            charactersWindowDialogConfiguration = new CharactersWindowDialogConfiguration(aStartPos, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            charactersWindowDocumentation = new CharactersWindowDocumentation(aStartPos, new GUIContent(TC.get("NPC.Documentation")), "Window");

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

                if (openedWindow == CharactersWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("NPC.LookPanelTitle")))
                {
                    OnWindowTypeChanged(CharactersWindowType.Appearance);
                }
                if (openedWindow == CharactersWindowType.Appearance)
                    GUI.skin = defaultSkin;


                if (openedWindow == CharactersWindowType.Documentation)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("NPC.Documentation")))
                {
                    OnWindowTypeChanged(CharactersWindowType.Documentation);
                }
                if (openedWindow == CharactersWindowType.Documentation)
                    GUI.skin = defaultSkin;

                if (openedWindow == CharactersWindowType.DialogConfiguration)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("NPC.DialogPanelTitle")))
                {
                    OnWindowTypeChanged(CharactersWindowType.DialogConfiguration);
                }
                if (openedWindow == CharactersWindowType.DialogConfiguration)
                    GUI.skin = defaultSkin;

                if (openedWindow == CharactersWindowType.Action)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("NPC.ActionsPanelTitle")))
                {
                    OnWindowTypeChanged(CharactersWindowType.Action);
                }
                if (openedWindow == CharactersWindowType.Action)
                    GUI.skin = defaultSkin;
                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case CharactersWindowType.Appearance:
                        charactersWindowAppearance.Draw(aID);
                        break;
                    case CharactersWindowType.Action:
                        charactersWindowActions.Draw(aID);
                        break;
                    case CharactersWindowType.DialogConfiguration:
                        charactersWindowDialogConfiguration.Draw(aID);
                        break;
                    case CharactersWindowType.Documentation:
                        charactersWindowDocumentation.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
                    {
                        ShowItemWindowView(i);
                    }

                    GUILayout.EndHorizontal();

                }
            }
        }

        void OnWindowTypeChanged(CharactersWindowType type_)
        {
            openedWindow = type_;
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            isConcreteItemVisible = false;
            GameRources.GetInstance().selectedCharacterIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            isConcreteItemVisible = true;
            GameRources.GetInstance().selectedCharacterIndex = o;

            charactersWindowActions = new CharactersWindowActions(thisRect, new GUIContent(TC.get("NPC.ActionsPanelTitle")), "Window");
            charactersWindowAppearance = new CharactersWindowAppearance(thisRect, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            charactersWindowDialogConfiguration = new CharactersWindowDialogConfiguration(thisRect, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            charactersWindowDocumentation = new CharactersWindowDocumentation(thisRect, new GUIContent(TC.get("NPC.Documentation")), "Window");
        }
        
        ///////////////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
            Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters()[index].setId(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.getInstance()
                           .getCharapterList()
                           .getSelectedChapterDataControl()
                           .getNPCsList()
                           .duplicateElement(
                               Controller.getInstance()
                                   .getCharapterList()
                                   .getSelectedChapterDataControl()
                                   .getNPCsList()
                                   .getNPCs()[r.index]);
            }
            else
            {
                Controller.getInstance().getSelectedChapterDataControl().getNPCsList().addElement(Controller.NPC, "newCharacter");
            }

        }

        protected override void OnAddOption(ReorderableList r, string option){}

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterDataControl()
                              .getScenesList()
                              .deleteElement(
                                  Controller.getInstance()
                                      .getCharapterList()
                                      .getSelectedChapterDataControl()
                                      .getNPCsList()
                                      .getNPCs()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
            List<NPC> previousList = Controller.getInstance()
                              .getCharapterList()
                              .getSelectedChapterData()
                              .getCharacters();

            List<NPC> reordered = new List<NPC>();
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
            Elements = Controller.getInstance().getCharapterList().getSelectedChapterData().getCharacters().ConvertAll(s => s.getId());
        }
    }
}