using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(50, typeof(NPCDataControl))]
    public class CharactersWindow : PreviewDataControlExtension
    {
        private enum CharactersWindowType { Action, Appearance, DialogConfiguration, Documentation }
        
        private static CharactersWindowActions charactersWindowActions;
        private static CharactersWindowAppearance charactersWindowAppearance;
        private static CharactersWindowDialogConfiguration charactersWindowDialogConfiguration;
        private static CharactersWindowDocumentation charactersWindowDocumentation;

        public CharactersWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name27")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/npcs"),
                text = "Element.Name27"
            };

            charactersWindowActions = new CharactersWindowActions(aStartPos, new GUIContent(TC.get("NPC.ActionsPanelTitle")), "Window");
            charactersWindowAppearance = new CharactersWindowAppearance(aStartPos, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            charactersWindowDialogConfiguration = new CharactersWindowDialogConfiguration(aStartPos, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            charactersWindowDocumentation = new CharactersWindowDocumentation(aStartPos, new GUIContent(TC.get("NPC.Documentation")), "Window");

            AddTab(TC.get("NPC.LookPanelTitle"), CharactersWindowType.Appearance, charactersWindowAppearance);
            AddTab(TC.get("NPC.Documentation"), CharactersWindowType.Documentation, charactersWindowDocumentation);
            AddTab(TC.get("NPC.DialogPanelTitle"), CharactersWindowType.DialogConfiguration, charactersWindowDialogConfiguration);
            AddTab(TC.get("NPC.ActionsPanelTitle"), CharactersWindowType.Action, charactersWindowActions);
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            GameRources.GetInstance().selectedCharacterIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            GameRources.GetInstance().selectedCharacterIndex = o;
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
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getNPCsList(),
                sceneList => (sceneList as NPCsListDataControl).getNPCs().Cast<DataControl>().ToList());
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            charactersWindowAppearance.Target = dataControlList.list[index] as DataControl;
            charactersWindowAppearance.DrawPreview(rect);
            charactersWindowAppearance.Target = null;
        }
    }
}