using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uAdventure.Runner;

public class PlayerFollower : MonoBehaviour {

    bool follow = false;
    bool init = false;

    Vector3 minpos;
    Vector3 maxpos;

	// Use this for initialization
	void Start () {
        follow = !Game.Instance.GameState.isFirstPerson();
        
    }
	
	// Update is called once per frame
	void Update () {
        if (!init)
        {
            init = true;
            GameObject t = GameObject.Find("Background");

            if (t != null)
            {
                Texture tex = t.GetComponent<MeshRenderer>().material.mainTexture;

                float scale = tex.height / ((float)Screen.height);

                float x = ((tex.width * scale) - Screen.width)/ 2f;

                minpos = t.transform.position - new Vector3(x/10f, 0, 0);
                transform.localPosition = new Vector3(t.transform.position.x, transform.localPosition.y, transform.localPosition.z);
            }
        }

        if (follow)
        {
            Vector3 v = Vector3.Lerp(transform.localPosition, new Vector3(PlayerMB.Instance.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z), 0.05f);

            if (v.x > minpos.x)
                this.transform.localPosition = v;
        }
            
	}
}
