using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace uAdventure.Runner
{
    [AddComponentMenu("Event/uAdventure Input Module")]
    public class uAdventureInputModule : StandaloneInputModule
    {
        public LayerMask NoTargetMask;
        private static GameObject lookingForTarget;
        private static GameObject targetSelectedHandler;

        private static void Execute(ITargetSelectedHandler handler, BaseEventData eventData)
        {
            handler.OnTargetSelected(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }

        private static ExecuteEvents.EventFunction<ITargetSelectedHandler> s_targetSelected = Execute;

        public static GameObject LookingForTarget{
            get
            {
                return lookingForTarget;
            }
            set
            {
                lookingForTarget = value;
                GetEventHandler(value);
            }
        }

        private static void GetEventHandler(GameObject value)
        {
            targetSelectedHandler = value == null ? null : ExecuteEvents.GetEventHandler<ITargetSelectedHandler>(value);
            if (!targetSelectedHandler && value)
            {
                var component = value.GetComponent<ITargetSelectedHandler>() ?? value.GetComponentInChildren<ITargetSelectedHandler>();
                if (component != null)
                {
                    targetSelectedHandler = value;
                }
            }
        }

        public static void DropTargetSelected(PointerEventData eventData)
        {
            var targetSelected = ExecuteEvents.GetEventHandler<ITargetSelectedHandler>(eventData.pointerDrag);
            if (targetSelected != null)
            {
                ExecuteEvents.Execute(targetSelected, eventData, s_targetSelected);
            }
        }
#region CopiedFromParent

        private bool ShouldIgnoreEventsOnNoFocus()
        {
            // Nothing to do so far, this was related to remote connection
            return false;
        }

        protected override void Start()
        {
            base.Start();

            if(FindObjectsOfType<uAdventureInputModule>().Length > 1)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public override void Process()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            {
                return;
            }

            bool usedEvent = SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                {
                    usedEvent |= SendMoveEventToSelectedObject();
                }

                if (!usedEvent)
                {
                    SendSubmitEventToSelectedObject();
                }
            }

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents() && input.mousePresent)
            {
                ProcessMouseEvent();
            }
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                {
                    RemovePointerData(pointer);
                }
            }
            return input.touchCount > 0;
        }
        

        protected new void ProcessMouseEvent()
        {
            ProcessMouseEvent(0);
        }

        /// <summary>
        /// Process all mouse events.
        /// </summary>
        protected new void ProcessMouseEvent(int id)
        {
            var mouseData = GetMousePointerEventData(id);
            var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;
            var rightButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Right).eventData;
            var middleButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Middle).eventData;

            // Process the first mouse button fully
            ProcessMousePress(leftButtonData);
            ProcessMove(leftButtonData.buttonData);
            ProcessDrag(leftButtonData.buttonData);

            // Now process right / middle clicks
            ProcessMousePress(rightButtonData);
            ProcessDrag(rightButtonData.buttonData);
            ProcessMousePress(middleButtonData);
            ProcessDrag(middleButtonData.buttonData);

            if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }

#endregion

        protected new void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (pressed)
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo)
                {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                {
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                }

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                    {
                        ++pointerEvent.clickCount;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                var go = currentOverGo;
                while (go != null && pointerEvent.pointerDrag == null)
                {
                    go = pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);

                    if (pointerEvent.pointerDrag != null)
                    {
                        // If it executes the initialice potential drag, but doesnt use it, we release it
                        if (ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, s_ConfirmWantsDragHandler) && !pointerEvent.used)
                        {
                            pointerEvent.pointerDrag = null;
                            go = go.transform.parent != null ? go.transform.parent.gameObject : null;
                        }
                        else
                        {
                            ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                        }
                    }
                }
            }

            // PointerUp notification
            if (released)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
                
                PointerClickAndDrop(pointerEvent);

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }
        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        protected new void ProcessMousePress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                {
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
                }

                float time = Time.unscaledTime;

                if (newPressed == pointerEvent.lastPress)
                {
                    var diffTime = time - pointerEvent.clickTime;
                    if (diffTime < 0.3f)
                    {
                        ++pointerEvent.clickCount;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }

                    pointerEvent.clickTime = time;
                }
                else
                {
                    pointerEvent.clickCount = 1;
                }

                pointerEvent.pointerPress = newPressed;
                pointerEvent.rawPointerPress = currentOverGo;

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                var go = currentOverGo;
                while (go != null && pointerEvent.pointerDrag == null)
                {
                    go = pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(go);

                    if (pointerEvent.pointerDrag != null)
                    {
                        // If it executes the initialice potential drag, but doesnt use it, we release it
                        if (ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, s_ConfirmWantsDragHandler) && !pointerEvent.used)
                        {
                            pointerEvent.pointerDrag = null;
                            go = go.transform.parent != null ? go.transform.parent.gameObject : null;
                        }
                        else
                        {
                            ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
                        }
                    }
                }
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                PointerClickAndDrop(pointerEvent);

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
                }

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        private void PointerClickAndDrop(PointerEventData pointerEvent)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);
            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                var isEligibleForTarget = pointerUpHandler == null || ((1 << pointerUpHandler.layer) & NoTargetMask.value) == 0;
                if (isEligibleForTarget && lookingForTarget != null && pointerEvent.eligibleForClick)
                {
                    // Clean the looking
                    var auxTargetSelectedHandler = targetSelectedHandler;
                    lookingForTarget = null;
                    targetSelectedHandler = null;
                    foreach(var t in auxTargetSelectedHandler.GetComponents<ITargetSelectedHandler>())
                    {
                        s_targetSelected(t, pointerEvent);
                    }
                }
                else
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }
        }

        private static readonly ExecuteEvents.EventFunction<IConfirmWantsDrag> s_ConfirmWantsDragHandler = Execute;

        private static void Execute(IConfirmWantsDrag handler, BaseEventData eventData)
        {
            handler.OnConfirmWantsDrag(ExecuteEvents.ValidateEventData<PointerEventData>(eventData));
        }
    }
}
