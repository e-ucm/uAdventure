using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public struct MovementPoint
    {
        public Vector2 destination;
        public float distance;
        public float scale;

        public float getProgress(float speed, float deltaTime)
        {
            return Mathf.Abs((speed * deltaTime) / distance);
        }

        public Vector2 getPointAt(float progress, Vector2 origin)
        {
            return Vector2.Lerp(origin, destination, progress);
        }
        public float getScaleAt(float progress, float original)
        {
            return Mathf.Lerp(original, scale, progress);
        }

        public override string ToString()
        {
            return "([" + destination.x + ", " + destination.y + "], " + distance + ", " + scale + ")";
        }
    }

    public class PlayerMB : Representable
    {

        public enum Orientation
        {
            N, E, S, O
        }

        static PlayerMB instance;

        MovementPoint point;
        Vector2 origin;
        float originalScale;

        float progress = 0.0f;
        float player_speed = 300f;
        Orientation orientation = Orientation.S;
        Action toDo;
        Rectangle toDoArea;

        private int max_layer = 0;
        private Dictionary<int, float> sorting_layer;
        public Dictionary<int, float> SortingLayer {
            set { this.sorting_layer = value;  }
            get
            {
                if (this.sorting_layer == null)
                {
                    List<Representable> rl = new List<Representable>(GameObject.FindObjectsOfType<Representable>());
                    this.sorting_layer = new Dictionary<int, float>();

                    rl.Remove(this.GetComponent<Representable>());

                    if (rl.Count > 0)
                    {
                        rl.Sort((x, y) => x.Context.getLayer() - y.Context.getLayer());

                        sorting_layer.Add(-1, 60);
                        sorting_layer.Add(rl[0].Context.getLayer(), rl[0].getPosition().y);
                        rl.Remove(rl[0]);

                        foreach (Representable r in rl)
                        {
                            ElementReference c = r.Context;

                            int prelayer = c.getLayer() - 1;

                            while (sorting_layer.ContainsKey(prelayer) && prelayer > 0)
                                prelayer--;

                            if (sorting_layer[prelayer] > r.getPosition().y)
                                sorting_layer.Add(c.getLayer(), r.getPosition().y);
                            else
                                sorting_layer.Add(c.getLayer(), sorting_layer[prelayer]);

                            this.max_layer = c.getLayer();
                        }
                    }
                }
                return this.sorting_layer;
            }
        }

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
            base.setAnimation(NPC.RESOURCE_TYPE_STAND_UP);
            this.deformation = -0.3f;
        }

        protected override void Update()
        {
            base.Update();

            if (moving)
            {
                progress = Mathf.Min(1, progress + point.getProgress(player_speed, Time.deltaTime));
                Context.setScale(point.getScaleAt(progress, originalScale));
                base.setPosition(point.getPointAt(progress, origin));
                sortZ();

                var isInside = (toDo != null && toDoArea.Contains(getPosition(), toDo.getKeepDistance()));

                if (progress == 1.0f || isInside)
                {
                    if (!isInside && moves.Count > 0)
                    {
                        move(moves.Dequeue());
                    }
                    else
                    {
                        moving = false;

                        switch (this.orientation)
                        {
                            case Orientation.N:
                                base.setAnimation(NPC.RESOURCE_TYPE_STAND_UP);
                                break;
                            case Orientation.E:
                                base.setAnimation(NPC.RESOURCE_TYPE_STAND_RIGHT);
                                break;
                            case Orientation.S:
                                base.setAnimation(NPC.RESOURCE_TYPE_STAND_DOWN);
                                break;
                            case Orientation.O:
                                base.setAnimation(NPC.RESOURCE_TYPE_STAND_LEFT);
                                break;
                        }

                        if (toDo != null)
                        {
                            Game.Instance.Execute(new EffectHolder(toDo.Effects));
                            toDo = null;
                        }
                    }
                }
            }

        }

        bool moving = false;
        Queue<MovementPoint> moves = new Queue<MovementPoint>();

        public void move(MovementPoint[] points)
        {
            if (points == null)
                return;

            moves = new Queue<MovementPoint>(points);
            if(points.Length > 0)
                move(moves.Dequeue());
        }
        public void moveInstant(Vector2 point)
        {
            //this.transform.localPosition = new Vector3 (point.x, point.y + this.transform.localScale.y/2,-context.getLayer());
            base.setPosition(point);
        }

        void move(MovementPoint point)
        {
            moving = true;
            progress = 0.0f;

            this.point = point;
            this.origin = this.getPosition();
            this.originalScale = Context.getScale();
            this.orientation = getOrientation(origin, point.destination);

            switch(this.orientation)
            {
                case Orientation.N:
                    base.setAnimation(NPC.RESOURCE_TYPE_WALK_UP);
                    break;
                case Orientation.E:
                    base.setAnimation(NPC.RESOURCE_TYPE_WALK_RIGHT);
                    break;
                case Orientation.S:
                    base.setAnimation(NPC.RESOURCE_TYPE_WALK_DOWN);
                    break;
                case Orientation.O:
                    base.setAnimation(NPC.RESOURCE_TYPE_WALK_LEFT);
                    break;
            }
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
            var accesible = TrajectoryHandler.GetAccessibleTrajectory(getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            Vector2[] intersections;

            if (area.Contains(this.getPosition(), action.getKeepDistance()))
            {
                Game.Instance.Execute(new EffectHolder(action.Effects));
            }
            else if (TrajectoryHandler.TrajectoryRectangleIntersections(area, accesible, out intersections))
            {
                toDo = action;
                toDoArea = area;
                move(accesible.route(getPosition(), intersections));

            }
        }

        void sortZ()
        {
            Vector2 currentpos = base.getPosition();
            for (int i = max_layer; i >= -1; i--)
            {
                if(SortingLayer.ContainsKey(i) && SortingLayer[i] > currentpos.y){
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -0.3f - i);
                    this.Context.setLayer(i);
                    break;
                }
            }
        }

        Orientation getOrientation(Vector2 source, Vector2 target)
        {
            Orientation o = Orientation.S;

            float angle = getAngle(source, target);

            if (angle >= 45 && angle < 135)
            {
                o = Orientation.S;
            }
            else if (angle >= 135 && angle < 225)
            {
                o = Orientation.O;
            }
            else if (angle >= 225 && angle < 315)
            {
                o = Orientation.N;
            }
            else if (angle >= 315 || angle < 45)
            {
                o = Orientation.E;
            }

            return o;
        }

        float getAngle(Vector2 source, Vector2 target)
        {
            Vector2 horizon = new Vector2(1, 0);
            Vector2 line = target - source;

            float angle = Vector2.Angle(horizon, line);
            Vector3 cross = Vector3.Cross(horizon, line);

            if (cross.z > 0)
                angle = 360 - angle;

            return angle;
        }
    }
}