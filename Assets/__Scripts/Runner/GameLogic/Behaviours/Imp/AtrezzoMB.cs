using UnityEngine;
using System.Collections;

public class AtrezzoMB : Representable {
	
	void Start () {
		base.Start ();
		base.setTexture(Atrezzo.RESOURCE_TYPE_IMAGE);
		base.Positionate ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
