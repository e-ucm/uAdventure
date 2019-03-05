﻿using UnityEngine;
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

        protected void Start()
        {
            representable = GetComponent<Representable>();
            if (representable)
            {
                representable.Play("stand");
            }
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
                {
                    onMovementFinished(data);
                }

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
            {
                return;
            }

            moves = new Queue<MovementPoint>(points);
            if (points.Length > 0)
            {
                MoveToPoint(moves.Dequeue());
            }
        }


        public void MoveInstant(Vector2 point)
        {
            AbortCurrentMovement();
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
                {
                    onMovementCancelled(data);
                }

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
                        {
                            onMovementFinished(data);
                        }
                    }
                }
            }
        }
    }
}