using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChangerMB : MonoBehaviour {
	
	void Start ()
    {
    }

	void Update () {
		
	}

	public void ChangeScene(string name){
		SceneManager.LoadScene (name);
	}
}
