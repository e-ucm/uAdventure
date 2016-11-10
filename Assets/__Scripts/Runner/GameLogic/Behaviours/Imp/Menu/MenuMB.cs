using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuMB : MonoBehaviour {
    private enum Quadrant { MIDDLE, TOP, TOP_RIGHT, RIGHT, BOTTOM_RIGHT, BOTTOM, BOTTOM_LEFT, LEFT, TOP_LEFT };
    private enum Fading { FADE_IN, FADE_OUT, NONE};
    Fading fading = Fading.NONE;
    Quadrant quadrant = Quadrant.TOP;

    static MenuMB instance;

    public static MenuMB Instance {
        get{ return instance; }
    }

    public GameObject Option_Prefab;
    private List<Action> actions;

    Renderer renderer;
    float alpha = 0f;
    void Awake(){
        MenuMB.instance = this;
        this.renderer = this.GetComponent<Renderer> ();
        alpha = 0f;
    }

	// Use this for initialization
	void Start () {
        this.actions = new List<Action> ();

        regenerate ();
	}
	
	// Update is called once per frame
	void Update () {

        switch(fading){
        case Fading.FADE_IN:
            alpha += 0.05f;
            colorChilds(new Color (1, 1, 1, alpha));
            if (alpha >= 1)
                fading = Fading.NONE;
            break;
        case Fading.FADE_OUT:
            alpha -= 0.05f;
            colorChilds (new Color (1, 1, 1, alpha));
            if (alpha <= 0) {
                fading = Fading.NONE;
                destroyChilds ();
            }
            break;
        case Fading.NONE:
        default:
            break;
        }
	}

    public void show (bool instant = false){
        if (instant) {
            colorChilds (new Color (1, 1, 1, 0));
        }else{
            colorChilds (new Color (1, 1, 1, 0));
            this.alpha = 0;
            this.fading = Fading.FADE_IN;
        }
    }

    public void hide (bool instant = false){
        if (instant) {
            destroyChilds ();
        } else {
            colorChilds (new Color (1, 1, 1, 1));
            this.alpha = 1;
            this.fading = Fading.FADE_OUT;
        }
    }

    public void destroyChilds(){
        foreach (Transform option in this.transform) {
            GameObject.Destroy (option.gameObject);
        }
    }

    public void setActions(List<Action> actions){
        this.actions = actions;
        this.quadrant = getQuadrant ();
        regenerate ();
    }

    private void regenerate(){
        destroyChilds ();

        float available_angles_min = 0f
            , available_angles_max = 2 * Mathf.PI;

        int complete = 1;

        switch (this.quadrant) {
        case Quadrant.MIDDLE:
            complete = 0;
            break;
        case Quadrant.TOP:
            available_angles_min = 210 * Mathf.Deg2Rad;
            available_angles_max = 330 * Mathf.Deg2Rad;
            break;
        case Quadrant.TOP_RIGHT:
            available_angles_min = 210 * Mathf.Deg2Rad;
            available_angles_max = 240 * Mathf.Deg2Rad;
            break;
        case Quadrant.RIGHT:
            available_angles_min = 120 * Mathf.Deg2Rad;
            available_angles_max = 240 * Mathf.Deg2Rad;
            break;
        case Quadrant.BOTTOM_RIGHT:
            available_angles_min = 120 * Mathf.Deg2Rad;
            available_angles_max = 150 * Mathf.Deg2Rad;
            break;
        case Quadrant.BOTTOM:
            available_angles_min = 30 * Mathf.Deg2Rad;
            available_angles_max = 150 * Mathf.Deg2Rad;
            break;
        case Quadrant.BOTTOM_LEFT:
            available_angles_min = 30 * Mathf.Deg2Rad;
            available_angles_max = 60 * Mathf.Deg2Rad;
            break;
        case Quadrant.LEFT:
            available_angles_min = -60 * Mathf.Deg2Rad;
            available_angles_max = 60 * Mathf.Deg2Rad;
            break;
        case Quadrant.TOP_LEFT:
            available_angles_min = 300 * Mathf.Deg2Rad;
            available_angles_max = 330 * Mathf.Deg2Rad;
            break;
        }



        float angle = (available_angles_max-available_angles_min) / (actions.Count-complete), current_angle = available_angles_min;


        foreach (Action action in actions) {
            GameObject option = GameObject.Instantiate (Option_Prefab);
            option.GetComponent<OptionMB> ().setAction (action);
            option.transform.parent = this.transform;
            option.transform.localPosition = new Vector3 (0, 0, 0);
            option.transform.localScale = new Vector3 (1f, 1f, 1f);
            Vector2 v = new Vector2 (Mathf.Cos (current_angle)/1.5f, Mathf.Sin (current_angle)/1.5f);
            option.GetComponent<OptionMB> ().moveTo (v);
            current_angle += angle;
        }
    }

    private void colorChilds(Color color){
        foreach(Transform t1 in transform){
            foreach(Transform t2 in t1){
                if (t2.GetComponent<Renderer> () != null)
                    t2.GetComponent<Renderer> ().material.color = color;
            }
        }
    }

    private Quadrant getQuadrant(){
        Quadrant x_quad = Quadrant.MIDDLE
            ,   y_quad = Quadrant.MIDDLE;

        if(this.transform.position.x<10){
            x_quad = Quadrant.LEFT;
        }else if(this.transform.position.x>70){
            x_quad = Quadrant.RIGHT;
        }

        if(this.transform.position.y<10){
            y_quad = Quadrant.BOTTOM;
        }else if(this.transform.position.y>50){
            y_quad = Quadrant.TOP;
        }

        return addQuadrants(x_quad,y_quad);
    }

    private Quadrant addQuadrants(Quadrant q1, Quadrant q2){
        Quadrant ret = Quadrant.MIDDLE;
        switch (q1) {
        case Quadrant.MIDDLE:
            switch (q2) {
            case Quadrant.MIDDLE:
                ret = Quadrant.MIDDLE;
                break;
            case Quadrant.TOP:
                ret = Quadrant.TOP;
                break;
            case Quadrant.BOTTOM:
                ret = Quadrant.BOTTOM;
                break;
            default:
                ret = Quadrant.MIDDLE;
                break;
            }
            break;
        case Quadrant.LEFT:
            switch (q2) {
            case Quadrant.MIDDLE:
                ret = Quadrant.LEFT;
                break;
            case Quadrant.TOP:
                ret = Quadrant.TOP_LEFT;
                break;
            case Quadrant.BOTTOM:
                ret = Quadrant.BOTTOM_LEFT;
                break;
            default:
                ret = Quadrant.LEFT;
                break;
            }
            break;
        case Quadrant.RIGHT:
            switch (q2) {
            case Quadrant.MIDDLE:
                ret = Quadrant.RIGHT;
                break;
            case Quadrant.TOP:
                ret = Quadrant.TOP_RIGHT;
                break;
            case Quadrant.BOTTOM:
                ret = Quadrant.BOTTOM_RIGHT;
                break;
            default:
                ret = Quadrant.RIGHT;
                break;
            }
            break;
        }
        return ret;
    }
}
