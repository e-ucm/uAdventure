using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Runner
{
    public abstract class InteractiveElement : MonoBehaviour, Interactuable, IActionReceiver, IPointerClickHandler, ITargetSelectedHandler,
        IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IConfirmWantsDrag
    {
        private bool interactable = false;
        private bool dragging = false;
        private IEnumerable<Action> targetActions;
        private readonly Dictionary<EffectHolder, Action> executingAction = new Dictionary<EffectHolder, Action>();
        private List<Action> lastActions;

        protected Element element;
        public Element Element
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
        protected virtual void Awake()
        {
            Game.Instance.GameState.OnConditionChanged += OnConditionChanged;
        }

        protected void OnDestroy()
        {
            if (Game.Instance)
            {
                Game.Instance.GameState.OnConditionChanged -= OnConditionChanged;
            }
        }

        protected virtual void OnConditionChanged(string condition, int value)
        {
        }

        protected virtual void Start()
        {
            OnConditionChanged(null, 0);
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
                if (state) OnPointerEnter();
                else OnPointerLeave();
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
                            lastActions = actions.ToList();
                            if (actions.Any())
                            {
                                ActionSelected(actions.First());
                                ret = InteractuableResult.DOES_SOMETHING;
                            }
                        }
                        break;
                    case Item.BehaviourType.NORMAL:
                        var availableActions = Element.getActions().Valid(AvailableActions).Distinct().ToList();
                        ActionsUtil.AddExamineIfNotExists(Element, availableActions);
                        lastActions = availableActions;

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
                eventData.Use();
        }

        public void OnConfirmWantsDrag(PointerEventData data)
        {
            if (!AvailableActions.Contains(Action.DRAG_TO))
            {
                return;
            }

            targetActions = GetDistinctInteractiveActionsOfType(Action.DRAG_TO);

            if (targetActions.Any())
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
                    targetActions = GetDistinctInteractiveActionsOfType(action.getType());
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    break;
                case Action.GIVE_TO:
                case Action.CUSTOM_INTERACT:
                case Action.USE_WITH:
                    dragging = true;
                    targetActions = GetDistinctInteractiveActionsOfType(action.getType());
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    break;
                default:
                    if (Game.Instance.GameState.IsFirstPerson || !action.isNeedsGoTo())
                    {
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

        private List<Action> GetDistinctInteractiveActionsOfType(int type)
        {
            return Element.getActions().Where(a => a.getType() == type).DistinctTarget().ToList();
        }

        private void OnActionStarted(object interactuable)
        {
            Game.Instance.ElementInteracted(false, Element, interactuable as Action);
        }

        private void OnActionFinished(object interactuable)
        {
            Action action = interactuable as Action;
            if(interactuable is EffectHolder)
            {
                var effectHolder = interactuable as EffectHolder;
                action = lastActions.Where(a => a.Effects == effectHolder.originalEffects).FirstOrDefault();
            }

            if (action == null)
                return;

            Game.Instance.ElementInteracted(true, Element, action);
        }

        public void OnTargetSelected(PointerEventData data)
        {
            var target = data.dragging ? data.pointerCurrentRaycast.gameObject : data.pointerPress;

            if (Element.isReturnsWhenDragged())
            {
                var representable = GetComponent<Representable>();
                if (representable)
                {
                    representable.Adaptate();
                }
                this.GetComponent<Collider>().enabled = true;
            }

            data.Use();

            if (target != null)
            {
                string id = target.name;
                if (id != null)
                {
                    var tmpActions = targetActions.Where(a => a.getTargetId() == id);
                    var action = tmpActions.Any() ? tmpActions.First() : null;

                    if (action != null)
                    {
                        Game.Instance.Execute(new EffectHolder(action.getEffects()));
                    }
                }
            }

            dragging = false;
        }

        // Abstract methods
        protected abstract int[] AvailableActions { get; }
        protected abstract Rectangle GetInteractionArea(SceneMB sceneMB);
        protected virtual void OnPointerEnter() {}
        protected virtual void OnPointerLeave() { }
        protected virtual Item.BehaviourType GetBehaviourType() { return Item.BehaviourType.NORMAL; }
    }
}