using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

    public static void LoadOrCreateGameProject(string selectedGameProjectPath)
    {
        List<Incidence> list = new List<Incidence>(), adventureList = new List<Incidence>();
        Controller.getInstance().init(selectedGameProjectPath);
    }

}