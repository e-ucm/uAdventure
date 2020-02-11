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
        private static readonly int[] restrictedActions = { Action.CUSTOM_INTERACT, Action.GIVE_TO, Action.USE_WITH, Action.EXAMINE, Action.USE, Action.CUSTOM };

        private bool isInteractive;
        private int targetActionType;
        private Texture2D cursor;

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
                            var actions = Element.getActions().Valid(restrictedActions).Distinct().Checked();
                            if (actions.Any())
                            {
                                ActionSelected(actions.First());
                                result = InteractuableResult.DOES_SOMETHING;
                            }
                        }
                        break;
                    case Item.BehaviourType.NORMAL:
                        var availableActions = Element.getActions().Valid(restrictedActions).Distinct().ToList();

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
            GUIManager.Instance.ShowHand(true);
            this.setInteractuable(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.Instance.ShowHand(false);
            this.setInteractuable(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }

        private void ManuallyCopyTexture(Texture2D origin, Texture2D destination)
        {
            int destX = destination.width, destY = destination.height;
            for (int i = 0; i < destX; ++i)
            {
                for (int j = 0; j < destY; ++j)
                {
                    destination.SetPixel(i, j, origin.GetPixel((int)(((float)i / destX) * origin.width), (int)(((float)j / destY) * origin.height)));
                }
            }

            cursor.Apply();
        }

        public void ActionSelected(Action action)
        {
            InventoryManager.Instance.Opened = false;
            switch (action.getType())
            {
                case Action.GIVE_TO:
                case Action.USE_WITH:
                case Action.CUSTOM_INTERACT:
                    // The texture that is already shown is either the icon or
                    if (!cursor)
                    {
                        cursor = new Texture2D(64, 64, TextureFormat.RGBA32, false, true);
                        try
                        {
                            var texture = sprite.texture;
                            ManuallyCopyTexture(texture, cursor);
                        }
                        catch
                        {
                            var texture = Transparent.CreateReadableTexture(sprite.texture);
                            ManuallyCopyTexture(texture, cursor);
                        }
                    }

                    Cursor.SetCursor(cursor, new Vector2(cursor.width, cursor.height) / 2f, CursorMode.ForceSoftware);
                    GUIManager.Instance.LockCursor();
                    uAdventureInputModule.LookingForTarget = this.gameObject;
                    targetActionType = action.getType();
                    break;
                default:
                    OnActionStarted(action);
                    Game.Instance.Execute(new EffectHolder(action.Effects), OnActionFinished);
                    break;
            }
        }
        private void OnActionStarted(object interactuable)
        {
            Game.Instance.ElementInteracted(false, Element, interactuable as Action);
        }

        private void OnActionFinished(object interactuable)
        {
            Action action = interactuable as Action;
            if (interactuable is EffectHolder)
            {
                var effectHolder = interactuable as EffectHolder;
                action = Element.getActions().Where(a => a.Effects == effectHolder.originalEffects).FirstOrDefault();
            }

            if (action == null)
                return;

            Game.Instance.ElementInteracted(true, Element, action);
        }

        public void OnTargetSelected(PointerEventData data)
        {
            if (data.hovered.Count == 0)
                return;

            var target = data.dragging ? data.pointerCurrentRaycast.gameObject : data.pointerPress;
            GUIManager.Instance.ReleaseCursor();
            
            data.Use();

            if (target == null)
            {
                return;
            }

            var id = target.name;
            if (id == null)
            {
                return;
            }

            var targetAction = Element
                .getActions()
                .Checked()
                .FirstOrDefault(a => a.getType() == targetActionType && a.getTargetId() == id);

            if (targetAction != null)
            {
                OnActionStarted(targetAction);
                Game.Instance.Execute(new EffectHolder(targetAction.Effects), OnActionFinished);
            }
        }
    }
}
