using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Runner
{
    public class ObjectMB : Representable, Interactuable, IActionReceiver, IPointerClickHandler,
        IDragHandler, IBeginDragHandler, IEndDragHandler, ITargetSelectedHandler
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.DRAG_TO, Action.EXAMINE, Action.GRAB, Action.USE };

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
                        {
                            var actions = Element.getActions().Checked();
                            if (actions.Any())
                            {
                                Game.Instance.Execute(new EffectHolder(actions.First().getEffects()));
                                ret = InteractuableResult.DOES_SOMETHING;
                            }
                        }
                        break;
                    case Item.BehaviourType.NORMAL:
                        var availableActions = Element.getActions().Valid(restrictedActions).ToList();

                        ActionsUtil.AddExamineIfNotExists(Element, availableActions);

                        //if there is an action, we show them
                        if (availableActions.Count > 0)
                        {
                            Game.Instance.showActions(availableActions, Input.mousePosition, this);
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
            Vector3 pointerPos = eventData == null ? Input.mousePosition : (Vector3)eventData.position;
            Vector2 pos = Camera.main.ScreenToWorldPoint(pointerPos);
            this.transform.localPosition = pos;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            dragActions = from action in Element.getActions()
                          where action.getType() == Action.DRAG_TO && ConditionChecker.check(action.getConditions())
                          group action by action.getTargetId() into sameTargetActions
                          select sameTargetActions.First();

            //uAdventureRaycaster.Instance.Override = this.gameObject;
            EventSystem.current.SetSelectedGameObject(this.gameObject);

            if (dragActions.Count() > 0)
            {
                dragging = true;
                this.GetComponent<Collider>().enabled = false;
                if (eventData != null)
                    eventData.Use();
            }

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                OnTargetSelected(eventData);
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            uAdventureInputModule.DropTargetSelected(eventData);
        }

        public void ActionSelected(Action action)
        {
            switch (action.getType())
            {
                case Action.DRAG_TO:
                    OnBeginDrag(null);
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    break;
                default:
                    Game.Instance.Execute(new EffectHolder(action.Effects));
                    break;
            }
        }

        public void OnTargetSelected(PointerEventData data)
        {
            var target = data.dragging ? data.pointerCurrentRaycast.gameObject : data.pointerPress;

            if (dragging)
            {
                if (Element.isReturnsWhenDragged())
                {
                    Positionate();
                    this.GetComponent<Collider>().enabled = true;
                }

                dragging = false;
                data.Use();

                if(target != null)
                {
                    string id = target.name;
                    if (id != null)
                    {
                        var tmpActions = dragActions.Where(a => a.getTargetId() == id);
                        var action = tmpActions.Any() ? tmpActions.First() : null;

                        if (action != null)
                        {
                            Game.Instance.Execute(new EffectHolder(action.getEffects()));
                        }
                    }
                }
            }
        }
    }
}