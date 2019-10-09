using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NameSaver : MonoBehaviour {
    public Text t;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SaveName()
    {
		string path = Application.persistentDataPath;

		if (!path.EndsWith ("/")) {
			path += "/";
		}

		if(PlayerPrefs.HasKey("username") && PlayerPrefs.GetString("username") != t.text){
			if(System.IO.File.Exists(path + "tracesRaw.csv")){
				System.IO.File.AppendAllText (path + PlayerPrefs.GetString("username") + ".csv.backup", System.IO.File.ReadAllText(path + "tracesRaw.csv"));
				System.IO.File.Delete (path + "tracesRaw.csv");
			}
		}
        PlayerPrefs.SetString("username", t.text);
    }
}
