using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uAdventure.Core;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class UIItem : UIRepresentable, Interactuable, IActionReceiver, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ITargetSelectedHandler
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM_INTERACT, Action.GIVE_TO, Action.USE_WITH, Action.EXAMINE };

        private bool isInteractive;
        private int targetActionType;

        protected override void Start()
        {
            base.Start();
            if (!base.SetTexture(Item.RESOURCE_TYPE_ICON))
                base.SetTexture(Item.RESOURCE_TYPE_IMAGE);
        }


        public bool canBeInteracted() { return isInteractive; }
        public void setInteractuable(bool state) { isInteractive = state; }

        public InteractuableResult Interacted(PointerEventData eventData)
        {
            var result = InteractuableResult.IGNORES;

            if (isInteractive)
            {
                switch (((Item)Element).getBehaviour())
                {
                    case Item.BehaviourType.FIRST_ACTION:
                        {
                            var actions = Element.getActions().Checked();
                            if (actions.Any())
                            {
                                Game.Instance.Execute(new EffectHolder(actions.First().getEffects()));
                                result = InteractuableResult.DOES_SOMETHING;
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
                            result = InteractuableResult.DOES_SOMETHING;
                        }
                        break;
                    case Item.BehaviourType.ATREZZO:
                    default:
                        result = InteractuableResult.IGNORES;
                        break;
                }
            }

            return result;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GUIManager.Instance.showHand(true);
            this.setInteractuable(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.Instance.showHand(false);
            this.setInteractuable(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }

        public void ActionSelected(Action action)
        {
            switch (action.getType())
            {
                case Action.GIVE_TO:
                case Action.USE_WITH:
                    var texture = sprite.texture;
                    Cursor.SetCursor(texture, new Vector2(texture.width, texture.height) / 2f, CursorMode.Auto);
                    GUIManager.Instance.lockCursor();
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    targetActionType = action.getType();
                    break;
                default:
                    Game.Instance.Execute(new EffectHolder(action.getEffects()));
                    break;
            }
        }

        public void OnTargetSelected(PointerEventData data)
        {
            if (data.hovered.Count == 0)
                return;

            var target = data.dragging ? data.pointerCurrentRaycast.gameObject : data.pointerPress;
            GUIManager.Instance.releaseCursor();
            
            data.Use();

            if (target != null)
            {
                string id = target.name;
                if (id != null)
                {
                    var targetAction = Element.getActions()
                        .Checked()
                        .Where(a => a.getType() == targetActionType && a.getTargetId() == id)
                        .FirstOrDefault();

                    if (targetAction != null)
                    {
                        Game.Instance.Execute(new EffectHolder(targetAction.Effects));
                    }
                }
            }
        }
    }
}
