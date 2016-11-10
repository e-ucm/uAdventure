using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Bubble : MonoBehaviour {

	private enum BubbleState { SHOWING, DESTROYING, NOTHING };

	BubbleState state = BubbleState.NOTHING;

	BubbleData data;
	public BubbleData Data {
		get { return data; }
		set { data = value; }
	}

	Transform text;
	RectTransform bubblePos;
	// Use this for initialization
	void Start () {
		//resize ();
		text = this.transform.FindChild ("Text");
		text.GetComponent<Text> ().text = data.Line;
		bubblePos = this.GetComponent<RectTransform> ();

		/*float guiscale = Screen.height/600f;

		text.GetComponent<Text>().fontSize = Mathf.RoundToInt(guiscale * 20);*/

		this.bubblePos.anchoredPosition = data.origin;
		this.moveTo (data.destiny);
		this.state = BubbleState.SHOWING;
	}

	//######################################################################
	//############################## MOVEMENT ##############################
	//######################################################################
	private Vector2 finalPosition;
	private float distance;
	public float easing = 0.1f;
	public bool _____________________________;
	// fields set dynamically
	public float camZ; // The desired Z pos of the camera
	void Awake() {
		camZ = this.transform.position.z;
	}


	private float destroytime = 0.2f;
	private float currenttime = 0f;
	void FixedUpdate () {
		float completed = 0f;
		switch (state) {
		case BubbleState.NOTHING:
			break;
		case BubbleState.DESTROYING:
			currenttime += Time.deltaTime;
			completed = 1f - currenttime / 0.2f;

			setAlpha (completed);
			setScale (completed);

			if (completed <= 0) {
				GameObject.Destroy (this.gameObject);
			}
			break;
		case BubbleState.SHOWING:
			Vector3 destination;
			if (finalPosition == null) {
				destination = Vector3.zero;
			} else {
				destination = finalPosition;
			}

			destination = Vector3.Lerp (this.bubblePos.anchoredPosition, destination, easing);
			destination.z = camZ;


			completed = 1f - (Vector2.Distance (destination, finalPosition) / distance);

			if (float.IsNaN (completed))
				completed = 1f;
			
			setAlpha (completed);
			setScale (completed);

			this.bubblePos.anchoredPosition = destination;

			if (completed >= 1f) {
				this.state = BubbleState.NOTHING;
			}
			break;
		}
	}

	public void moveTo(Vector2 position){
		this.distance = Vector2.Distance (this.bubblePos.anchoredPosition, position);
		this.finalPosition = position;
	}

	public void destroy(){
		this.state = BubbleState.DESTROYING;
	}

	float min_alpha = 0f, max_alpha = 1f;
	public void setAlpha(float percent){
		float alpha = (max_alpha - min_alpha) * percent + min_alpha;
		this.GetComponent<Image> ().color = new Color (data.BaseColor.r, data.BaseColor.g, data.BaseColor.b, alpha);
		this.GetComponent<Outline> ().effectColor = new Color (data.OutlineColor.r, data.OutlineColor.g, data.OutlineColor.b, alpha);

		text.GetComponent<Text> ().color = new Color (data.TextColor.r, data.TextColor.g, data.TextColor.b, alpha);
		text.GetComponent<Outline> ().effectColor = new Color (data.TextOutlineColor.r, data.TextOutlineColor.g, data.TextOutlineColor.b, alpha);

	}

	float min_scale = 0.7f, max_scale = 1f;
	public void setScale(float percent){
		float scale = (max_scale - min_scale) * percent + min_scale;
		this.transform.localScale = new Vector3 (scale, scale, 1);
	}

	private float width = 200f;
	public void resize(){
		float newwidth = (Screen.width / 600f) * width;

		this.bubblePos.sizeDelta = new Vector2 (newwidth, 0);
	}
}
