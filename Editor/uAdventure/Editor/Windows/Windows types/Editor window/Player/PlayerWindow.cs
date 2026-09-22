using UnityEngine;

using uAdventure.Core;
using System;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    [EditorWindowExtension(60, typeof(PlayerDataControl))]
    public class PlayerWindow : DefaultButtonMenuEditorWindowExtension
    {        
        private readonly List<LayoutWindow> tabs;
        private int openedWindow;

        public PlayerWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name26")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/player"),
                text = "Element.Name26"
            };

            openedWindow = 0;

            tabs = new List<LayoutWindow>();
            if (Controller.Instance.PlayerMode== Controller.FILE_ADVENTURE_3RDPERSON_PLAYER)
            {
                AddTab(new CharactersWindowAppearance(aStartPos, new GUIContent(TC.get("NPC.LookPanelTitle")), "Window") { IsPlayer = true });
            }

            AddTab(new CharactersWindowDialogConfiguration(aStartPos, new GUIContent(TC.get("NPC.DialogPanelTitle")), "Window"));
            AddTab(new PlayerWindowDocumentation(aStartPos, new GUIContent(TC.get("NPC.Documentation")), "Window"));
        }


        public override void Draw(int aID)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            openedWindow = GUILayout.Toolbar(openedWindow, tabs.ConvertAll(t => t.Content.text).ToArray(), GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (openedWindow >= 0)
            {
                if(tabs[openedWindow] is EditorComponent)
                {
                    (tabs[openedWindow] as EditorComponent).Target = Controller.Instance.SelectedChapterDataControl.getPlayer();
                }
                tabs[openedWindow].Rect = Rect;
                tabs[openedWindow].Draw(aID);
            }
        }

        protected override void OnButton()
        {
        }

        private void AddTab(LayoutWindow layoutWindow)
        {
            tabs.Add(layoutWindow);

            layoutWindow.BeginWindows = () => BeginWindows();
            layoutWindow.EndWindows = () => EndWindows();
        }
    }
}