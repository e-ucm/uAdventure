using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class ObjectMB : Representable, Interactuable
    {

        bool interactable = false;
        bool dragging = false;
        Action drag;

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
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.transform.localPosition = pos;
                if (Input.GetMouseButtonUp(0))
                {
                    dragging = false;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray);
                    //bool no_interaction = true;
                    ActiveAreaMB aa;

                    foreach (RaycastHit hit in hits)
                    {
                        aa = hit.transform.GetComponent<ActiveAreaMB>();
                        if (aa != null)
                        {
                            if (aa.aaData.getId() == drag.getTargetId())
                            {
                                Game.Instance.Execute(new EffectHolder(drag.getEffects()));
                                break;
                            }
                        }
                    }
                }
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

        public InteractuableResult Interacted(RaycastHit hit = new RaycastHit())
        {
            InteractuableResult ret = InteractuableResult.IGNORES;

            if (interactable)
            {
                if (Element.isReturnsWhenDragged())
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
                                exeff.add(new SpeakPlayerEffect(Element.getDescription(0).getDetailedDescription()));
                                ex.setEffects(exeff);
                                available.Add(ex);
                            }

                            //if there is an action, we show them
                            if (available.Count > 0)
                            {
                                Game.Instance.showActions(available, Input.mousePosition);
                                ret = InteractuableResult.DOES_SOMETHING;
                            }
                            break;
                        case Item.BehaviourType.ATREZZO:
                        default:
                            ret = InteractuableResult.IGNORES;
                            break;
                    }
                }
                else
                {
                    if (drag == null)
                    {
                        foreach (Action action in Element.getActions())
                        {
                            if (action.getType() == Action.DRAG_TO)
                            {
                                drag = action;
                                break;
                            }
                        }
                    }
                    if (ConditionChecker.check(drag.getConditions()))
                    {
                        dragging = true;
                        ret = InteractuableResult.DOES_SOMETHING;
                    }
                }
            }

            return ret;
        }
    }
}