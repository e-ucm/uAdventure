using UnityEngine;
using uAdventure.Core;
using System.Collections.Generic;

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

    [RequireComponent(typeof(Representable))]
    public class Mover : MonoBehaviour
    {
        public delegate void OnMovementFinished(object data);
        public delegate void OnMovementCancelled(object data);

        private Representable representable;

        // Representation
        private float originalScale;
        // Layering
        private int max_layer = 0;
        private Dictionary<int, float> sorting_layer;
        // Movement control
        private bool moving = false;
        private float progress = 0.0f;
        private Queue<MovementPoint> moves = new Queue<MovementPoint>();
        // Movement parameters
        private float player_speed = 300f;
        private MovementPoint point;
        private Vector2 origin;
        private Rectangle toArea;
        private float distanceToArea;

        // Callback
        private object data;
        private OnMovementFinished onMovementFinished;
        private OnMovementCancelled onMovementCancelled;

        public Dictionary<int, float> SortingLayer
        {
            set { this.sorting_layer = value; }
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


        /*  ## ANIMATION METHOD ##
         * 
         * private Animation current_anim;
         * 
         * Texture2D tmp = current_anim.getFrame(0).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(0).getTime();//Duration/1000f;
         * 
         * return (current_anim.getFrame(current_frame).getImage(false,false,0).texture.height) * context.getScale();
         * 
         * current_frame = (current_frame + 1) % current_anim.getFrames().Count;
            Texture2D tmp = current_anim.getFrame(current_frame).getImage(false,false,0).texture;
            update_ratio = current_anim.getFrame(current_frame).getTime();
         * 
         */

        protected void Start()
        {
            representable = GetComponent<Representable>();
            if (representable)
                representable.Play("stand");
        }

        protected void Update()
        {
            UpdateMovement();
        }

        // Public movement methods

        public bool Move(Vector2 point)
        {
            return Move(point, null, null, null);
        }

        public bool Move(Vector2 point, object data, OnMovementFinished onMovementFinished, OnMovementCancelled onMovementCancelled)
        {
            AbortCurrentMovement();

            this.data = data;
            this.onMovementFinished = onMovementFinished;
            this.onMovementCancelled = onMovementCancelled;

            var accesible = TrajectoryHandler.GetAccessibleTrajectory(representable.getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            var route = accesible.route(representable.getPosition(), point);
            if (route != null && route.Length > 0)
            {
                toArea = null;
                MoveRoute(route);
                return true;
            }
            return false;
        }
        public bool Move(Rectangle area, float distance)
        {
            return Move(area, distance, null, null, null);
        }

        public bool Move(Rectangle area, float distance, object data, OnMovementFinished onMovementFinished, OnMovementCancelled onMovementCancelled)
        {
            AbortCurrentMovement();

            this.data = data;
            this.onMovementFinished = onMovementFinished;
            this.onMovementCancelled = onMovementCancelled;

            if (area.Contains(representable.getPosition(), 0))
            {
                if(onMovementFinished != null)
                    onMovementFinished(data);
                return true;
            }

            var accesible = TrajectoryHandler.GetAccessibleTrajectory(representable.getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            Vector2[] intersections;
            if (TrajectoryHandler.TrajectoryRectangleIntersections(area, accesible, out intersections))
            {
                var route = accesible.route(representable.getPosition(), intersections);
                if (route != null && route.Length > 0)
                {
                    toArea = area;
                    distanceToArea = distance;
                    MoveRoute(route);
                    return true;
                }
            }
            return false;
        }

        public void MoveRoute(MovementPoint[] points)
        {
            AbortCurrentMovement();

            if (points == null)
                return;

            moves = new Queue<MovementPoint>(points);
            if (points.Length > 0)
                MoveToPoint(moves.Dequeue());
        }


        public void MoveInstant(Vector2 point)
        {
            AbortCurrentMovement();
            //this.transform.localPosition = new Vector3 (point.x, point.y + this.transform.localScale.y/2,-context.getLayer());
            representable.setPosition(point);
        }

        // Private movement management methods

        private void AbortCurrentMovement()
        {
            toArea = null;
            if (moving)
            {
                representable.Play("stand");
                // Clear the main variables
                moving = false;
                progress = 0.0f;
                moves.Clear();
                // Notify the chidls
                if(onMovementCancelled != null)
                    onMovementCancelled(data);

                onMovementCancelled = null;
                onMovementFinished = null;
                data = null;
            }
        }

        private void MoveToPoint(MovementPoint point)
        {
            moving = true;
            progress = 0.0f;

            this.point = point;
            this.origin = representable.getPosition();
            this.originalScale = representable.Context.getScale();
            representable.Orientation = (point.destination - origin).ToOrientation();
            representable.Play("walk");
        }

        protected void UpdateMovement()
        {
            if (moving)
            {
                progress = progress + point.getProgress(player_speed, Time.deltaTime);
                representable.Context.setScale(point.getScaleAt(progress, originalScale));
                representable.setPosition(point.getPointAt(progress, origin));
                SortZ();

                var isInside = (toArea != null && toArea.Contains(representable.getPosition(), distanceToArea));

                if (progress >= 1.0f || isInside)
                {
                    if (!isInside && moves.Count > 0)
                    {
                        MoveToPoint(moves.Dequeue());
                    }
                    else
                    {
                        moving = false;
                        representable.Play("stand");
                        if(onMovementFinished != null)
                            onMovementFinished(data);
                    }
                }
            }
        }

        private void SortZ()
        {
            Vector2 currentpos = representable.getPosition();
            for (int i = max_layer; i >= -1; i--)
            {
                if (SortingLayer.ContainsKey(i) && SortingLayer[i] > currentpos.y)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -0.3f - i);
                    representable.Context.setLayer(i);
                    break;
                }
            }
        }
    }
}