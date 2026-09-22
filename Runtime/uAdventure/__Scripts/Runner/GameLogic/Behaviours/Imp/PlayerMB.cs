using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Representable))]
    [RequireComponent(typeof(Mover))]
    public class PlayerMB : MonoBehaviour
    {
        private class ActionArea
        {
            public Action action;
            public Rectangle area;

            public ActionArea(Action action, Rectangle area)
            {
                this.action = action;
                this.area = area;
            }
        }

        private static PlayerMB instance;
        private Game.ExecutionEvent onExecutionStarted;
        private Game.ExecutionEvent onExecutionFinished;

        private Mover mover;
        private Representable representable;
        private ScenePositioner scenePositioner; 

        static public PlayerMB Instance
        {
            get { return instance; }
        }

        protected void Awake()
        {
            instance = this;
        }

        protected void Start()
        {
            mover = GetComponent<Mover>();
            representable = GetComponent<Representable>();
            scenePositioner = GetComponent<ScenePositioner>();
        }

        public void Do(Action action, Rectangle area, Game.ExecutionEvent onExecutionStarted = null, Game.ExecutionEvent onExecutionFinishes = null)
        {
            this.onExecutionStarted = onExecutionStarted;
            this.onExecutionFinished = onExecutionFinishes;
            mover.Move(area, action.getKeepDistance(), new ActionArea(action, area), OnMovementFinished, OnMovementCancelled);
        }

        protected void OnMovementCancelled(object data)
        {

        }

        protected void OnMovementFinished(object data)
        {
            var toDo = data as ActionArea;
            if (toDo != null)
            {
                switch (toDo.action.getType())
                {
                    case Action.USE:
                    case Action.USE_WITH:
                    case Action.GRAB:
                        representable.Orientation = (scenePositioner.Position - toDo.area.ToRect().center).ToOrientation(true);
                        representable.Play("use", "stand");
                        break;
                    case Action.CUSTOM:
                        representable.Orientation = (scenePositioner.Position - toDo.area.ToRect().center).ToOrientation(true);
                        representable.Play("actionAnimation", "stand");
                        break;
                }
                
                if(onExecutionStarted != null)
                {
                    onExecutionStarted(toDo.action);
                }
                Game.Instance.Execute(new EffectHolder(toDo.action.Effects), FinishedCallbackFor(toDo, onExecutionFinished));
            }
        }

        private Game.ExecutionEvent FinishedCallbackFor(ActionArea data, Game.ExecutionEvent onExecutionFinished)
        {
            return _ =>
            {
                if (onExecutionFinished != null)
                    onExecutionFinished(data.action);
            };
        }

    }
}