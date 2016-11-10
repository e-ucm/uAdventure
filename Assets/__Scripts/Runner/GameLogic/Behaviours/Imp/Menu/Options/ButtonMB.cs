using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonMB : MonoBehaviour, Interactuable {
     

    public ResourcesUni resource;

    Action action;
    string name;
    public Action Action{
        set { 
            this.action = value;
        }
        get { return this.action; }
    }

    bool showText = false;

	// Use this for initialization
	void Start () {
        if (this.action.getType () == Action.CUSTOM) {
            name = ((CustomAction)action).getName ();

            CustomAction ca = action as CustomAction;
            foreach (ResourcesUni ru in ca.getResources()) {
                if (ConditionChecker.check (ru.getConditions ())) {
                    this.resource = ru;
                }
            }
        }else {
            /*resource = new ResourcesUni ();
            CustomButton button = Game.Instance.getButton (ActionNameWrapper.Names [action.getType ()],DescriptorData.NORMAL_BUTTON);
            CustomButton highlighted = Game.Instance.getButton (ActionNameWrapper.Names [action.getType ()],DescriptorData.HIGHLIGHTED_BUTTON);

            if (button == null || highlighted == null) {
                button = Game.Instance.getButton (ActionNameWrapper.AuxNames [action.getType ()], DescriptorData.NORMAL_BUTTON);
                highlighted = Game.Instance.getButton (ActionNameWrapper.AuxNames [action.getType ()], DescriptorData.HIGHLIGHTED_BUTTON);
            }

            resource.addAsset (DescriptorData.NORMAL_BUTTON, button.getPath());
            resource.addAsset (DescriptorData.HIGHLIGHTED_BUTTON, highlighted.getPath());*/

			resource = GUIManager.Instance.Provider.getButton (this.action);

            name = ConstantNames.L ["ES"].Actions [action.getType ()];
        }

        Texture2D tmp;
        if(this.action.getType() == Action.CUSTOM)
            tmp = ResourceManager.Instance.getImage (resource.getAssetPath ("buttonNormal"));
        else
            tmp = ResourceManager.Instance.getImage (resource.getAssetPath (DescriptorData.NORMAL_BUTTON));
        
        this.GetComponent<Renderer> ().material.mainTexture = tmp;
        this.transform.localScale = new Vector3 (tmp.width / 75f, tmp.height / 75f, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter(){
        showText = true;
        transform.parent.GetComponent<OptionMB> ().Highlight = true;
        if(this.action.getType() == Action.CUSTOM)
            GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (resource.getAssetPath ("buttonOver"));
        else
            GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (resource.getAssetPath (DescriptorData.HIGHLIGHTED_BUTTON));

		GUIManager.Instance.showHand (true);
    }

    void OnMouseExit() {
        showText = false;
        transform.parent.GetComponent<OptionMB> ().Highlight = false;
        if(this.action.getType() == Action.CUSTOM)
            GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (resource.getAssetPath ("buttonNormal"));
        else
            GetComponent<Renderer> ().material.mainTexture = ResourceManager.Instance.getImage (resource.getAssetPath (DescriptorData.NORMAL_BUTTON));

		GUIManager.Instance.showHand (false);
    }

	bool interactable = false;
	public bool canBeInteracted(){
		return interactable;
	}

	public void setInteractuable(bool state){
		this.interactable = state;
	}

    public InteractuableResult Interacted (RaycastHit hit = new RaycastHit()){
        Game.Instance.Execute(new EffectHolder(action.getEffects()));
        return InteractuableResult.DOES_SOMETHING;
    }

    void OnGUI () {
        if (showText) {
            GUILayout.BeginArea (new Rect (Input.mousePosition.x-100, Screen.height-Input.mousePosition.y + 20, 200, 300));
            GUILayout.Label (name,Game.Instance.Style.label);
            GUILayout.EndArea ();
        }
    }
}
