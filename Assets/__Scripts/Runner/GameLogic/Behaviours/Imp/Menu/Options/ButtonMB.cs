using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class ButtonMB : MonoBehaviour, Interactuable, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ResourcesUni resource;

        private Action action;
        private string actionName;

        private bool interactable = false;
        private SpriteRenderer spriteRenderer;

        bool showText = false;

        public Action Action
        {
            set
            {
                this.action = value;
            }
            get { return this.action; }
        }

        public IActionReceiver Receiver { get; set; }

        // Use this for initialization
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (this.action.getType() == Action.CUSTOM)
            {
                actionName = ((CustomAction)action).getName();

                CustomAction ca = action as CustomAction;
                foreach (ResourcesUni ru in ca.getResources())
                {
                    if (ConditionChecker.check(ru.getConditions()))
                    {
                        this.resource = ru;
                    }
                }
            }
            else
            {
                /*resource = new ResourcesUni ();
                CustomButton button = Game.Instance.getButton (ActionNameWrapper.Names [action.getType ()],DescriptorData.NORMAL_BUTTON);
                CustomButton highlighted = Game.Instance.getButton (ActionNameWrapper.Names [action.getType ()],DescriptorData.HIGHLIGHTED_BUTTON);

                if (button == null || highlighted == null) {
                    button = Game.Instance.getButton (ActionNameWrapper.AuxNames [action.getType ()], DescriptorData.NORMAL_BUTTON);
                    highlighted = Game.Instance.getButton (ActionNameWrapper.AuxNames [action.getType ()], DescriptorData.HIGHLIGHTED_BUTTON);
                }

                resource.addAsset (DescriptorData.NORMAL_BUTTON, button.getPath());
                resource.addAsset (DescriptorData.HIGHLIGHTED_BUTTON, highlighted.getPath());*/

                resource = GUIManager.Instance.Provider.getButton(this.action);

                actionName = ConstantNames.L["ES"].Actions[action.getType()];
            }

            Sprite tmp;
            if (this.action.getType() == Action.CUSTOM)
                tmp = Game.Instance.ResourceManager.getSprite(resource.getAssetPath("buttonNormal"));
            else
                tmp = Game.Instance.ResourceManager.getSprite(resource.getAssetPath(DescriptorData.NORMAL_BUTTON));

            spriteRenderer.sprite = tmp;
            this.gameObject.AddComponent<PolygonCollider2D>();
            //this.transform.localScale = new Vector3(tmp.width / 75f, tmp.height / 75f, 1);
        }

        public bool canBeInteracted()
        {
            return interactable;
        }

        public void setInteractuable(bool state)
        {
            this.interactable = state;
        }

        public InteractuableResult Interacted(PointerEventData eventData = null)
        {
            MenuMB.Instance.hide(true);
            if (Receiver != null)
            {
                Receiver.ActionSelected(action);
            }
            else
            {
                Game.Instance.Execute(new EffectHolder(action.getEffects()));
            }
            return InteractuableResult.DOES_SOMETHING;
        }

        void OnGUI()
        {
            if (showText)
            {
                GUILayout.BeginArea(new Rect(Input.mousePosition.x - 100, Screen.height - Input.mousePosition.y + 20, 200, 300));
                GUILayout.Label(actionName, Game.Instance.Style.label);
                GUILayout.EndArea();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            showText = true;
            transform.parent.GetComponent<OptionMB>().Highlight = true;
            if (this.action.getType() == Action.CUSTOM)
                spriteRenderer.sprite = Game.Instance.ResourceManager.getSprite(resource.getAssetPath("buttonOver"));
            else
                spriteRenderer.sprite = Game.Instance.ResourceManager.getSprite(resource.getAssetPath(DescriptorData.HIGHLIGHTED_BUTTON));

            GUIManager.Instance.showHand(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            showText = false;
            transform.parent.GetComponent<OptionMB>().Highlight = false;
            if (this.action.getType() == Action.CUSTOM)
                spriteRenderer.sprite = Game.Instance.ResourceManager.getSprite(resource.getAssetPath("buttonNormal"));
            else
                spriteRenderer.sprite = Game.Instance.ResourceManager.getSprite(resource.getAssetPath(DescriptorData.NORMAL_BUTTON));

            GUIManager.Instance.showHand(false);
        }
    }
}