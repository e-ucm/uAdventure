using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GameRources
    {

        private static GameRources instance = null;

        public int selectedSceneIndex = -1;
        public int selectedCutsceneIndex = -1;
        public int selectedBookIndex = -1;
        public int selectedItemIndex = -1;
        public int selectedCharacterIndex = -1;
        public int selectedSetItemIndex = -1;
        public int selectedConversationIndex = -1;

        public static GameRources GetInstance()
        {
            if (instance == null)
            {
                instance = new GameRources();
            }

            return instance;
        }

        public void Reset()
        {
            selectedSceneIndex = -1;
            selectedCutsceneIndex = -1;
            selectedBookIndex = -1;
            selectedItemIndex = -1;
            selectedCharacterIndex = -1;
            selectedSetItemIndex = -1;
            selectedConversationIndex = -1;
            instance = null;
        }

        public static bool NewGame(int type)
        {
            return Controller.Instance.NewAdventure(type);
        }

        public static bool CreateGameProject(string selectedGameProjectPath, int type)
        {
            return Controller.Instance.newFile(selectedGameProjectPath, type);
        }

        public static void LoadGameProject(string selectedGameProjectPath)
        {
            Controller.Instance.Init(selectedGameProjectPath);
        }

    }
}