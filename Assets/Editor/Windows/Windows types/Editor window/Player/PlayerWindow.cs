using UnityEngine;

using uAdventure.Core;
using System;

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

        private static PlayerWindowType openedWindow = PlayerWindowType.DialogConfiguration;
        private static PlayerWindowAppearance playerWindowAppearance;
        private static PlayerWindowDialogConfiguration playerWindowDialogConfiguration;
        private static PlayerWindowDocumentation playerWindowDocumentation;

        private Player playerRef = null;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;

        public PlayerWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name26")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/player", typeof(Texture2D));
            c.text = TC.get("Element.Name26");
            ButtonContent = c;

            playerWindowAppearance = new PlayerWindowAppearance(aStartPos,
                new GUIContent(TC.get("NPC.LookPanelTitle")), "Window");
            playerWindowDialogConfiguration = new PlayerWindowDialogConfiguration(aStartPos,
                new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window");
            playerWindowDocumentation = new PlayerWindowDocumentation(aStartPos,
                new GUIContent(TC.get("NPC.Documentation")), "Window");
            selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
        }


        public override void Draw(int aID)
        {

            GUILayout.BeginHorizontal();
            if (Controller.Instance.playerMode() == Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
            {
                if (openedWindow == PlayerWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("NPC.LookPanelTitle")))
                {
                    OnWindowTypeChanged(PlayerWindowType.Appearance);
                }
                if (openedWindow == PlayerWindowType.Appearance)
                    GUI.skin = defaultSkin;
            }

            if (openedWindow == PlayerWindowType.DialogConfiguration)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("NPC.DialogPanelTitle")))
            {
                OnWindowTypeChanged(PlayerWindowType.DialogConfiguration);
            }
            if (openedWindow == PlayerWindowType.DialogConfiguration)
                GUI.skin = defaultSkin;


            if (openedWindow == PlayerWindowType.Documentation)
                GUI.skin = selectedButtonSkin;
            if (GUILayout.Button(TC.get("NPC.Documentation")))
            {
                OnWindowTypeChanged(PlayerWindowType.Documentation);
            }
            if (openedWindow == PlayerWindowType.Documentation)
                GUI.skin = defaultSkin;
            GUILayout.EndHorizontal();

            switch (openedWindow)
            {
                case PlayerWindowType.Appearance:
                    playerWindowAppearance.Rect = Rect;
                    playerWindowAppearance.Draw(aID);
                    break;
                case PlayerWindowType.DialogConfiguration:
                    playerWindowDialogConfiguration.Rect = Rect;
                    playerWindowDialogConfiguration.Draw(aID);
                    break;
                case PlayerWindowType.Documentation:
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
            OnWindowTypeChanged(PlayerWindowType.Appearance);
        }
    }
}