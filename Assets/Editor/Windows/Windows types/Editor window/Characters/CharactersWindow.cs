using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(50, typeof(NPC))]
    public class CharactersWindow : ReorderableListEditorWindowExtension
    {
        private enum CharactersWindowType { Action, Appearance, DialogConfiguration, Documentation }

        private static CharactersWindowType openedWindow = CharactersWindowType.DialogConfiguration;
        private static CharactersWindowActions charactersWindowActions;
        private static CharactersWindowAppearance charactersWindowAppearance;
        private static CharactersWindowDialogConfiguration charactersWindowDialogConfiguration;
        private static CharactersWindowDocumentation charactersWindowDocumentation;

        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public CharactersWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name27")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/npcs", typeof(Texture2D));
            c.text = TC.get("Element.Name27");
            ButtonContent = c;

            charactersWindowActions = new CharactersWindowActions(aStartPos, new GUIContent(TC.get("NPC.ActionsPanelTitle")), "Window");
            charactersWindowAppearance = new CharactersWindowAppearance(aStartPos, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            charactersWindowDialogConfiguration = new CharactersWindowDialogConfiguration(aStartPos, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            charactersWindowDocumentation = new CharactersWindowDocumentation(aStartPos, new GUIContent(TC.get("NPC.Documentation")), "Window");
            
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
                List<KeyValuePair<string, CharactersWindowType>> tabs = new List<KeyValuePair<string, CharactersWindowType>>()
                {
                    new KeyValuePair<string, CharactersWindowType>(TC.get("NPC.LookPanelTitle"), CharactersWindowType.Appearance),
                    new KeyValuePair<string, CharactersWindowType>(TC.get("NPC.Documentation"), CharactersWindowType.Documentation),
                    new KeyValuePair<string, CharactersWindowType>(TC.get("NPC.DialogPanelTitle"), CharactersWindowType.DialogConfiguration),
                    new KeyValuePair<string, CharactersWindowType>(TC.get("NPC.ActionsPanelTitle"), CharactersWindowType.Action),
                };

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                openedWindow = tabs[GUILayout.Toolbar(tabs.FindIndex(t => t.Value == openedWindow), tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case CharactersWindowType.Appearance:
                        charactersWindowAppearance.Rect = Rect;
                        charactersWindowAppearance.Draw(aID);
                        break;
                    case CharactersWindowType.Action:
                        charactersWindowActions.Rect = Rect;
                        charactersWindowActions.Draw(aID);
                        break;
                    case CharactersWindowType.DialogConfiguration:
                        charactersWindowDialogConfiguration.Rect = Rect;
                        charactersWindowDialogConfiguration.Draw(aID);
                        break;
                    case CharactersWindowType.Documentation:
                        charactersWindowDocumentation.Rect = Rect;
                        charactersWindowDocumentation.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterData().getCharacters().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.Instance.ChapterList.getSelectedChapterData().getCharacters()[i].getId(), GUILayout.Width(m_Rect.width * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(m_Rect.width * 0.2f)))
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

            charactersWindowActions = new CharactersWindowActions(m_Rect, new GUIContent(TC.get("NPC.ActionsPanelTitle")), "Window");
            charactersWindowAppearance = new CharactersWindowAppearance(m_Rect, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            charactersWindowDialogConfiguration = new CharactersWindowDialogConfiguration(m_Rect, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            charactersWindowDocumentation = new CharactersWindowDocumentation(m_Rect, new GUIContent(TC.get("NPC.Documentation")), "Window");
        }
        
        ///////////////////////////////

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.Instance.ChapterList.getSelectedChapterDataControl().getNPCsList().getNPCs ()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance                           .ChapterList                           .getSelectedChapterDataControl()
                           .getNPCsList()
                           .duplicateElement(
                               Controller.Instance                                   .ChapterList                                   .getSelectedChapterDataControl()
                                   .getNPCsList()
                                   .getNPCs()[r.index]);
            }
            else
            {
                Controller.Instance.SelectedChapterDataControl.getNPCsList().addElement(Controller.NPC, "newCharacter");
            }

        }

        protected override void OnAddOption(ReorderableList r, string option){}

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.Instance                              .ChapterList                              .getSelectedChapterDataControl()
							  .getNPCsList()
                              .deleteElement(
                                  Controller.Instance                                      .ChapterList                                      .getSelectedChapterDataControl()
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
			var dataControlList = Controller.Instance 				.ChapterList .getSelectedChapterDataControl ().getNPCsList ();

			var toPos = r.index;
			var fromPos = dataControlList.getNPCs ().FindIndex (i => i.getId () == r.list [r.index] as string);

			dataControlList.MoveElement (dataControlList.getNPCs ()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getNPCsList ().getNPCs().ConvertAll(s => s.getId());
        }
    }
}