using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class PlayerMB : Representable
    {

        public enum Orientation
        {
            N, E, S, O
        }

        static PlayerMB instance;
        Vector2 start_pos, end_pos;
        float progress = 0.0f;
        float player_speed = 0.5f;
        float speed = 0f;
        Orientation orientation = Orientation.S;

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

            if (moving && progress < 1.0f)
            {
                // TODO newpos is never used, why?
                Vector2 newpos = Vector2.Lerp(start_pos, end_pos, progress);
                base.setPosition(newpos);
                sortZ();
                progress += Time.deltaTime * speed;
            }
            else if (moves.Count > 0)
            {
                move(moves.Dequeue());
            }
            else if (moving && moves.Count <= 0)
            {
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
                moving = false;
            }
        }

        bool moving = false;
        Queue<KeyValuePair<Vector2, float>> moves = new Queue<KeyValuePair<Vector2, float>>();

        public void move(KeyValuePair<Vector2, float>[] points)
        {
            moves = new Queue<KeyValuePair<Vector2, float>>(points);
        }
        public void moveInstant(Vector2 point)
        {
            //this.transform.localPosition = new Vector3 (point.x, point.y + this.transform.localScale.y/2,-context.getLayer());
            base.setPosition(point);
        }

        void move(KeyValuePair<Vector2, float> point)
        {
            moving = true;
            progress = 0.0f;

            this.start_pos = this.getPosition();
            this.end_pos = new Vector2(point.Key.x, point.Key.y + (point.Value) / 2);

            this.orientation = getOrientation(start_pos, end_pos);

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

            speed = player_speed * (50 / Vector2.Distance(start_pos, end_pos));
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