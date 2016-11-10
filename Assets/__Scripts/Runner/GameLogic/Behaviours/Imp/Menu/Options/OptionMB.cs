using UnityEngine;
using System.Collections;

public class OptionMB : MonoBehaviour {
    Action a;
    Transform button, line;

    private Vector2 finalPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void setAction(Action action){
        this.a = action;
        this.button.GetComponent<ButtonMB>().Action = action; 
    }


    public void collapse(){

    }

    void OnMouseEnter(){
    }

    void OnMouseExit() {
    }

    bool highlight = false;
    public  bool Highlight{
        set { this.highlight = value; }
        get { return this.highlight; }
    }

    //######################################################################
    //############################## MOVEMENT ##############################
    //######################################################################

    public float easing = 0.05f;
    public bool _____________________________;
    // fields set dynamically
    public float camZ; // The desired Z pos of the camera
    void Awake() {
        this.button = transform.FindChild ("Button");
        this.line = transform.FindChild ("Line");
        camZ = this.transform.position.z;
    }

    Renderer renderer; 
    void FixedUpdate () {
        Vector3 destination;
        Vector3 linedestination;

        // If there is no poi, return to P:[0,0,0]
        if (finalPosition == null) {
            destination = Vector3.zero;
            linedestination = Vector3.zero;
        } else {
            destination = finalPosition;
            linedestination = finalPosition/2f;
        }

        destination = Vector3.Lerp(button.localPosition, destination, easing);
        destination.z = camZ;

        linedestination = Vector3.Lerp(line.localPosition, linedestination, easing);
        linedestination.z = camZ;

        if(destination.y > 0)
            line.eulerAngles = new Vector3 (0f, 0f, 90 + Mathf.Acos (destination.normalized.x) * Mathf.Rad2Deg);
        else
            line.eulerAngles = new Vector3 (0f, 0f, 90 - Mathf.Acos (destination.normalized.x) * Mathf.Rad2Deg);
        
        line.localScale = new Vector3 (0.05f, destination.magnitude, 1);


        if(highlight)
            destination.z = camZ-2;
        else
            destination.z = camZ-1;

        button.localPosition = destination;
        line.localPosition = linedestination;
    }



    public void moveTo(Vector2 position){
        this.finalPosition = position;

    }
}
