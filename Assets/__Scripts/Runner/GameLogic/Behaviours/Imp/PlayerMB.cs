using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public class PlayerMB : Representable
    {

        static PlayerMB instance;
        Vector2 start_pos, end_pos;
        float progress = 0.0f;
        float player_speed = 0.5f;
        float speed = 0f;

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
        }

        protected override void Update()
        {
            base.Update();

            if (moving && progress < 1.0f)
            {
                // TODO newpos is never used, why?
                //Vector2 newpos = Vector2.Lerp(start_pos, end_pos, progress);
                base.setPosition(getPosition());
                progress += Time.deltaTime * speed;
            }
            else if (moves.Count > 0)
            {
                move(moves.Dequeue());
            }
            else if (moving && moves.Count <= 0)
            {
                base.setAnimation(NPC.RESOURCE_TYPE_STAND_UP);
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

            this.start_pos = this.transform.localPosition;
            this.end_pos = new Vector2(point.Key.x, point.Key.y + (getHeight() * point.Value) / 2);

            if (this.start_pos.x < this.end_pos.x)
            {
                base.setAnimation(NPC.RESOURCE_TYPE_WALK_RIGHT);
            }
            else base.setAnimation(NPC.RESOURCE_TYPE_WALK_LEFT);

            speed = player_speed * (50 / Vector2.Distance(start_pos, end_pos));
        }

    }
}