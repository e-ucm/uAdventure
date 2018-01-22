using UnityEngine;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(60, typeof(Player))]
    public class PlayerWindow : DefaultButtonMenuEditorWindowExtension
    {
        private enum PlayerWindowType
        {
            Appearance,
            DialogConfiguration,
            Documentation
        }

        private static PlayerWindowType openedWindow;
        private static CharactersWindowAppearance playerWindowAppearance;
        private static CharactersWindowDialogConfiguration playerWindowDialogConfiguration;
        private static PlayerWindowDocumentation playerWindowDocumentation;

        private Player playerRef = null;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        private List<KeyValuePair<string, PlayerWindowType>> tabs;

        public PlayerWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name26")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/player", typeof(Texture2D));
            c.text = TC.get("Element.Name26");
            ButtonContent = c;

            playerWindowAppearance = new CharactersWindowAppearance(aStartPos, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            playerWindowAppearance.BeginWindows = () => BeginWindows();
            playerWindowAppearance.EndWindows = () => EndWindows();
            playerWindowDialogConfiguration = new CharactersWindowDialogConfiguration(aStartPos, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            playerWindowDialogConfiguration.BeginWindows = () => BeginWindows();
            playerWindowDialogConfiguration.EndWindows = () => EndWindows();
            playerWindowDocumentation = new PlayerWindowDocumentation(aStartPos, new GUIContent(TC.get("NPC.Documentation")), "Window");
            playerWindowDocumentation.BeginWindows = () => BeginWindows();
            playerWindowDocumentation.EndWindows = () => EndWindows();
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));

            openedWindow = PlayerWindowType.DialogConfiguration;

            tabs = new List<KeyValuePair<string, PlayerWindowType>>();
            if (Controller.Instance.playerMode() == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
            {
                openedWindow = PlayerWindowType.Appearance;
                tabs.Add(new KeyValuePair<string, PlayerWindowType>(TC.get("NPC.LookPanelTitle"), PlayerWindowType.Appearance));
            }
            tabs.Add(new KeyValuePair<string, PlayerWindowType>(TC.get("NPC.DialogPanelTitle"), PlayerWindowType.DialogConfiguration));
            tabs.Add(new KeyValuePair<string, PlayerWindowType>(TC.get("NPC.Documentation"), PlayerWindowType.Documentation));
            
        }


        public override void Draw(int aID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            openedWindow = tabs[GUILayout.Toolbar(tabs.FindIndex(t => t.Value == openedWindow), tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            switch (openedWindow)
            {
                case PlayerWindowType.Appearance:
                    playerWindowAppearance.Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
                    playerWindowAppearance.Rect = Rect;
                    playerWindowAppearance.Draw(aID);
                    break;
                case PlayerWindowType.DialogConfiguration:
                    playerWindowDialogConfiguration.Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
                    playerWindowDialogConfiguration.Rect = Rect;
                    playerWindowDialogConfiguration.Draw(aID);
                    break;
                case PlayerWindowType.Documentation:
                    playerWindowDocumentation.Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
                    playerWindowDocumentation.Rect = Rect;
                    playerWindowDocumentation.Draw(aID);
                    break;
            }
        }

        void OnWindowTypeChanged(PlayerWindowType type_)
        {
            openedWindow = type_;
        }

        protected override void OnButton()
        {
        }
    }
}