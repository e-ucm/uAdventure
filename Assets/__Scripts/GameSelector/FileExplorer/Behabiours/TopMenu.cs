using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TopMenu : MonoBehaviour {

	Text currentpath;
	Button addgame;
	PathShower shower;

	void Start () {
		currentpath = this.transform.FindChild ("Path").GetComponent<Text> ();
		addgame = this.transform.FindChild ("Panel").FindChild ("AddGame").GetComponent<Button> ();
		shower = GameObject.FindObjectOfType<PathShower> ();
	}
	
	// Update is called once per frame
	void Update () {
		currentpath.text = shower.Path;
	}
}
