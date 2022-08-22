using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePlayerPrefs  {

    [UnityEditor.MenuItem("uAdventure/Delete Player Prefferences")]
	public static void DeletePlayerPreferences()
    {
        PlayerPrefs.DeleteAll();
    }
}
