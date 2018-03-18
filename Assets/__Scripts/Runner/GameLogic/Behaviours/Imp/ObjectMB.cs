using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Runner
{
    public class ObjectMB : Representable, Interactuable, IActionReceiver, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        bool interactable = false;
        bool dragging = false;
        IEnumerable<Action> dragActions;

        public int DifferentActions
        {
            get
            {
                int tmp = 0;
                foreach (Action a in Element.getActions())
                {
                    if (ConditionChecker.check(a.getConditions()))
                    {
                        tmp++;
                    }
                }
                return tmp;
            }
        }



        protected override void Start()
        {
            base.Start();
            base.setTexture(Item.RESOURCE_TYPE_IMAGE);
        }

        protected override void Update()
        {
            if (dragging)
            {
                OnDrag(null);
            }
        }

        public bool canBeInteracted()
        {
            return interactable;
        }

        public void setInteractuable(bool state)
        {
            this.interactable = state;

            if (state)
            {
                if (base.hasOverSprite())
                {
                    base.setTexture(Item.RESOURCE_TYPE_IMAGEOVER);
                }
            }
            else
                base.setTexture(Item.RESOURCE_TYPE_IMAGE);
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult ret = InteractuableResult.IGNORES;

            if (interactable)
            {
                switch (((Item)Element).getBehaviour())
                {
                    case Item.BehaviourType.FIRST_ACTION:
                        foreach (Action a in Element.getActions())
                        {
                            if (ConditionChecker.check(a.getConditions()))
                            {
                                Game.Instance.Execute(new EffectHolder(a.getEffects()));
                                break;
                            }
                        }
                        ret = InteractuableResult.DOES_SOMETHING;
                        break;
                    case Item.BehaviourType.NORMAL:
                        List<Action> available = new List<Action>();
                        foreach (Action a in Element.getActions())
                        {
                            if (ConditionChecker.check(a.getConditions()))
                            {
                                bool addaction = true;
                                foreach (Action a2 in available)
                                {
                                    if ((a.getType() == Action.CUSTOM || a.getType() == Action.CUSTOM_INTERACT) && (a2.getType() == Action.CUSTOM || a2.getType() == Action.CUSTOM_INTERACT))
                                    {
                                        if (((CustomAction)a).getName() == ((CustomAction)a2).getName())
                                        {
                                            addaction = false;
                                            break;
                                        }
                                    }
                                    else if (a.getType() == a2.getType())
                                    {
                                        addaction = false;
                                        break;
                                    }
                                }

                                if (addaction)
                                    available.Add(a);
                            }
                        }


                        //We check if it's an examine action, otherwise we create one and add it
                        bool addexamine = true;
                        foreach (Action a in available)
                        {
                            if (a.getType() == Action.EXAMINE)
                            {
                                addexamine = false;
                                break;
                            }
                        }

                        if (addexamine)
                        {
                            Action ex = new Action(Action.EXAMINE);
                            Effects exeff = new Effects();
                            exeff.Add(new SpeakPlayerEffect(Element.getDescription(0).getDetailedDescription()));
                            ex.setEffects(exeff);
                            available.Add(ex);
                        }

                        //if there is an action, we show them
                        if (available.Count > 0)
                        {
                            Game.Instance.showActions(available, Input.mousePosition, this);
                            ret = InteractuableResult.DOES_SOMETHING;
                        }
                        break;
                    case Item.BehaviourType.ATREZZO:
                    default:
                        ret = InteractuableResult.IGNORES;
                        break;
                }
            }

            return ret;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (dragging)
            {
                OnEndDrag(eventData);
            }
            else
            {
                Interacted(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 pointerPos = eventData == null ? Input.mousePosition : (Vector3) eventData.position;
            Vector2 pos = Camera.main.ScreenToWorldPoint(pointerPos);
            this.transform.localPosition = pos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragActions = from action in Element.getActions()
                          where action.getType() == Action.DRAG_TO && ConditionChecker.check(action.getConditions())
                          group action by action.getTargetId() into sameTargetActions
                          select sameTargetActions.First();

            uAdventureRaycaster.Instance.Override = this.gameObject;

            if (dragActions.Count() > 0)
            {
                dragging = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("Drag ended");
            if (dragging)
            {
                if(uAdventureRaycaster.Instance.Override == this.gameObject)
                    uAdventureRaycaster.Instance.Override = null;

                if (Element.isReturnsWhenDragged())
                {
                    Positionate();
                }

                dragging = false;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray);
                //bool no_interaction = true;
                Representable representable;
                ActiveAreaMB area;
                string id;

                foreach (RaycastHit hit in hits)
                {
                    representable = hit.transform.GetComponent<Representable>();
                    area = hit.transform.GetComponent<ActiveAreaMB>();
                    id = representable ? representable.Element.getId() : area ? area.Element.getId() : null;
                    if (id != null)
                    {
                        var tmpActions = dragActions.Where(a => a.getTargetId() == id);
                        var action = tmpActions.Any() ? tmpActions.First() : null;

                        if (action != null)
                        {
                            Game.Instance.Execute(new EffectHolder(action.getEffects()));
                            break;
                        }
                    }
                }
            }
        }

        public void ActionSelected(Action action)
        {
            switch (action.getType())
            {
                case Action.DRAG_TO:
                    OnBeginDrag(null);
                    break;
                default:
                    Game.Instance.Execute(new EffectHolder(action.Effects));
                    break;
            }
        }
    }
}