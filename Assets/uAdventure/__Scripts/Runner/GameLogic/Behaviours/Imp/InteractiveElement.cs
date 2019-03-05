using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;
using AssetPackage;

namespace uAdventure.Runner
{
    public abstract class InteractiveElement : MonoBehaviour, Interactuable, IActionReceiver, IPointerClickHandler, ITargetSelectedHandler,
        IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IConfirmWantsDrag
    {
        private bool interactable = false;
        private bool dragging = false;
        private IEnumerable<Action> dragActions;
        private readonly Dictionary<EffectHolder, Action> executingAction = new Dictionary<EffectHolder, Action>();

        protected Element element;
        protected Element Element
        {
            get
            {
                if(element == null)
                {
                    var representable = GetComponent<Representable>();
                    if (representable)
                    {
                        return this.element = representable.Element;
                    }
                    var area = GetComponent<Area>();
                    if (area && area.Element is ActiveArea)
                    {
                        return this.element = area.Element as ActiveArea;
                    }
                }
                return element;
            }
        }

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

        protected virtual void Start()
        {
        }

        protected virtual void Update()
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
            if(state != this.interactable)
            {
                this.interactable = state;
                if (state)
                {
                    OnPointerEnter();
                }
                else
                {
                    OnPointerLeave();
                }
            }
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult ret = InteractuableResult.IGNORES;

            if (interactable)
            {
                switch (GetBehaviourType())
                {
                    case Item.BehaviourType.FIRST_ACTION:
                        {
                            var actions = Element.getActions().Valid(AvailableActions);
                            if (actions.Any())
                            {
                                ActionSelected(actions.First());
                                ret = InteractuableResult.DOES_SOMETHING;
                            }
                        }
                        break;
                    case Item.BehaviourType.NORMAL:
                        var availableActions = Element.getActions().Valid(AvailableActions).ToList();
                        ActionsUtil.AddExamineIfNotExists(Element, availableActions);

                        //if there is an action, we show them
                        if (availableActions.Count > 0)
                        {
                            Game.Instance.showActions(availableActions, Input.mousePosition, this);
                            ret = InteractuableResult.DOES_SOMETHING;
                        }
                        break;
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
            Vector2 pointerPos = eventData == null ? (Vector2)Input.mousePosition : eventData.position;
            Vector3 pos = Camera.main.ScreenToWorldPoint(pointerPos);
            this.transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            if (eventData != null)
            {
                eventData.Use();
            }
        }

        public void OnConfirmWantsDrag(PointerEventData data)
        {
            dragActions = from action in Element.getActions().Valid(AvailableActions)
                          where action.getType() == Action.DRAG_TO && ConditionChecker.check(action.getConditions())
                          group action by action.getTargetId() into sameTargetActions
                          select sameTargetActions.First();

            if (dragActions.Any())
            {
                data.Use();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //uAdventureRaycaster.Instance.Override = this.gameObject;
            EventSystem.current.SetSelectedGameObject(this.gameObject);

            this.GetComponent<Collider>().enabled = false;
            if (eventData != null)
            {
                eventData.Use();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnTargetSelected(eventData);
            eventData.Use();
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
                    dragging = true;
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    break;
                default:
                    if (Game.Instance.GameState.IsFirstPerson || !action.isNeedsGoTo())
                    {
                        Game.Instance.GameState.BeginChangeAmbit();
                        OnActionStarted(action);
                        Game.Instance.Execute(new EffectHolder(action.Effects), OnActionFinished);
                    }
                    else
                    {
                        var sceneMB = FindObjectOfType<SceneMB>();
                        Rectangle area = GetInteractionArea(sceneMB);
                        PlayerMB.Instance.Do(action, area, OnActionStarted, OnActionFinished);
                    }
                    break;
            }
        }
        private void OnActionStarted(object interactuable)
        {
            Action action = interactuable as Action;
            CompletablesController.Instance.ElementInteracted(this, action.getType().ToString(), action.getTargetId());
        }

        private void OnActionFinished(object interactuable)
        {
            Action action = interactuable as Action;
            if(interactuable is EffectHolder)
            {
                var effectHolder = interactuable as EffectHolder;
                if (executingAction.ContainsKey(effectHolder))
                {
                    action = executingAction[effectHolder];
                    executingAction.Remove(effectHolder);
                }
            }

            if (action == null)
            {
                return;
            }

            string actionType = string.Empty;
            switch (action.getType())
            {
                case Action.CUSTOM:          actionType = (action as CustomAction).getName();   break;
                case Action.CUSTOM_INTERACT: actionType = (action as CustomAction).getName();   break;
                case Action.DRAG_TO:         actionType = "drag_to";                            break;
                case Action.EXAMINE:         actionType = "examine";                            break;
                case Action.GIVE_TO:         actionType = "give_to";                            break;
                case Action.GRAB:            actionType = "grab";                               break;
                case Action.TALK_TO:         actionType = "talk_to";                            break;
                case Action.USE:             actionType = "use";                                break;
                case Action.USE_WITH:        actionType = "use_with";                           break;
            }

            if (!string.IsNullOrEmpty(action.getTargetId()))
            {
                TrackerAsset.Instance.setVar("action_target", action.getTargetId());
            }

            if (!string.IsNullOrEmpty(actionType))
            {
                TrackerAsset.Instance.setVar("action_type", actionType);
            }

            Game.Instance.GameState.EndChangeAmbitAsExtensions();
            switch (Element.GetType().ToString())
            {
                case "NPC":        TrackerAsset.Instance.GameObject.Interacted(Element.getId(), GameObjectTracker.TrackedGameObject.Npc);  break;
                case "Item":       TrackerAsset.Instance.GameObject.Interacted(Element.getId(), GameObjectTracker.TrackedGameObject.Item); break;
                case "ActiveArea": TrackerAsset.Instance.GameObject.Interacted(Element.getId(), GameObjectTracker.TrackedGameObject.Item); break;
            }

        }

        public void OnTargetSelected(PointerEventData data)
        {
            var target = data.dragging ? data.pointerCurrentRaycast.gameObject : data.pointerPress;

            if (Element.isReturnsWhenDragged())
            {
                var representable = GetComponent<Representable>();
                if (representable)
                {
                    representable.Positionate();
                }

                this.GetComponent<Collider>().enabled = true;
            }

            dragging = false;
            data.Use();

            if (target != null)
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

        // Abstract methods
        protected abstract int[] AvailableActions { get; }
        protected abstract Rectangle GetInteractionArea(SceneMB sceneMB);
        protected virtual void OnPointerEnter() {}
        protected virtual void OnPointerLeave() { }
        protected virtual Item.BehaviourType GetBehaviourType() { return Item.BehaviourType.NORMAL; }
    }
}