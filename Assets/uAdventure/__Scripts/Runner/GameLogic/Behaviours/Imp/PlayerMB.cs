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
        private Game.OnExecutionFinished onExecutionFinished;

        private Mover mover;
        private Representable representable;

        static public PlayerMB Instance
        {
            get { return instance; }
        }

        void Awake()
        {
            instance = this;
        }

        protected void Start()
        {
            mover = GetComponent<Mover>();
            representable = GetComponent<Representable>();
        }

        public void Do(Action action, Rectangle area, Game.OnExecutionFinished onExecutionFinishes = null)
        {
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
                        representable.Orientation = (representable.getPosition() - toDo.area.ToRect().center).ToOrientation(true);
                        representable.Play("use", "stand");
                        break;
                    case Action.CUSTOM:
                        representable.Orientation = (representable.getPosition() - toDo.area.ToRect().center).ToOrientation(true);
                        representable.Play("actionAnimation", "stand");
                        break;
                }
                
                Game.Instance.GameState.BeginChangeAmbit();
                Game.Instance.Execute(new EffectHolder(toDo.action.Effects), FinishedCallbackFor(toDo, onExecutionFinished));
                toDo = null;
            }
        }

        private Game.OnExecutionFinished FinishedCallbackFor(ActionArea data, Game.OnExecutionFinished onExecutionFinished)
        {
            return (_) =>
            {
                if (onExecutionFinished != null)
                    onExecutionFinished(data.action);
            };
        }

    }
}