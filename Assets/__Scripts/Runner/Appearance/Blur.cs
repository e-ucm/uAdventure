using UnityEngine;
using System.Collections;

public class Blur : MonoBehaviour {
	public Material AfterBlurMaterial;

	private enum BlurState { NONE, GENERATING, GENERATED, SHOWING };
	private BlurState state;

	Texture2D generated;
	RenderTexture cameratex;

	Material blurred;
	void Start () {
		//generated = new Texture2D (1, 1);
		blurred = this.GetComponent<Renderer> ().material;
		//cameratex = new RenderTexture (Screen.width, Screen.height, 24);
		//blurred.SetTexture ("_BumpMap", generated);
	}

	void Update () {
		/*if (state == BlurState.NONE) {
			Camera.main.targetTexture = cameratex;
			state = BlurState.GENERATING;
		} else if (state == BlurState.GENERATING) {
			generated = cameratex.
			/*Texture tmp = blurred.GetTexture ("_BumpMap");
			float size = blurred.GetFloat ("_Size");
			generated = tmp as Texture2D;
			if (generated != null)
				state = BlurState.GENERATED;


		} else if (state == BlurState.GENERATED) {
			this.GetComponent<Renderer> ().material = AfterBlurMaterial;
			AfterBlurMaterial.mainTexture = generated;
			state = BlurState.SHOWING;
		} else if (state == BlurState.SHOWING) {


		}*/
	}
}
