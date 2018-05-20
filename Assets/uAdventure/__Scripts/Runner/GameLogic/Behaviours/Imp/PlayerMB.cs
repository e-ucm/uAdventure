using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{

    public class PlayerMB : CharacterMB
    {


        static PlayerMB instance;
        Action toDo;
        Rectangle toDoArea;



        static public PlayerMB Instance
        {
            get { return instance; }
        }

        void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            this.gameObject.name = base.Element.getId();
            base.Start();
            this.deformation = -0.3f;
        }


        /*public void Do(Action action, Vector2 objectCenter)
        {
            var accesible = TrajectoryHandler.GetAccessibleTrajectory(getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            Vector2[] intersections;
            var area = new InfluenceArea((int)objectCenter.x + influenceArea.getX() - influenceArea.getWidth() / 2,
                (int)objectCenter.y + influenceArea.getY() - influenceArea.getHeight() / 2, influenceArea.getWidth(), influenceArea.getHeight());
            if(TrajectoryHandler.TrajectoryRectangleIntersections(area, accesible, out intersections))
            {
                toDo = action;
                move(accesible.route(getPosition(), intersections));
            }
        }*/

        public void Do(Action action, Rectangle area)
        {
            toDoArea = area;
            toDo = action;
            if (!Move(area, action.getKeepDistance()))
            {
                toDo = null;
            }
            else
            {
                toDo = action;
            }
        }

        protected override void OnMovementCancelled()
        {
            base.OnMovementCancelled();

            toDo = null;
        }

        protected override void OnMovementFinished()
        {
            base.OnMovementFinished();

            if (toDo != null)
            {
                switch (toDo.getType())
                {
                    case Action.USE:
                    case Action.USE_WITH:
                    case Action.GRAB:
                        Orientation = (this.getPosition() - toDoArea.ToRect().center).ToOrientation(true);
                        Play("use", "stand");
                        break;
                    case Action.CUSTOM:
                        Orientation = (this.getPosition() - toDoArea.ToRect().center).ToOrientation(true);
                        Play("actionAnimation", "stand");
                        break;
                }

                Game.Instance.Execute(new EffectHolder(toDo.Effects));
                toDo = null;
            }
        }

    }
}