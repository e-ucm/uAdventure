using UnityEngine;
using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;
using UnityEngine.EventSystems;
using System.Linq;
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
    public enum Orientation
    {
        N, E, S, O
    }


    public class CharacterMB : Representable, Interactuable, IPointerClickHandler, IDropHandler, IActionReceiver
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.TALK_TO, Action.EXAMINE };

        // Interactivity
        bool interactable = false;
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

        protected override void Start()
        {
            this.gameObject.name = base.Element.getId();
            base.Start();
            Play("stand");
        }

        protected override void Update()
        {
            base.Update();
            UpdateMovement();
        }

        public bool canBeInteracted()
        {
            return interactable;
        }

        public void setInteractuable(bool state)
        {
            this.interactable = state;
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult res = InteractuableResult.IGNORES;

            if (interactable)
            {
                var availableActions = Element.getActions().Valid(restrictedActions).ToList();

                ActionsUtil.AddExamineIfNotExists(Element, availableActions);

                //if there is an action, we show them
                if (availableActions.Count > 0)
                {
                    Game.Instance.showActions(availableActions, Input.mousePosition, this);
                    res = InteractuableResult.DOES_SOMETHING;
                }

                Tracker.T.trackedGameObject.Interacted(((NPC)Element).getId(), GameObjectTracker.TrackedGameObject.Npc);
                Tracker.T.RequestFlush();
            }

            return res;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Interacted(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            uAdventureInputModule.DropTargetSelected(eventData);
        }

        public void ActionSelected(Action action)
        {
            if(Game.Instance.GameState.IsFirstPerson || !action.isNeedsGoTo())
                Game.Instance.Execute(new EffectHolder(action.Effects));
            else
            {
                var sceneMB = FindObjectOfType<SceneMB>();
                var scene = sceneMB.sceneData as Scene;
                var topLeft = new Vector2(Context.getX() - Texture.width / 2f, Context.getY() - Texture.height);
                Rectangle area = new InfluenceArea((int)topLeft.x - 20, (int)topLeft.y - 20, Texture.width + 40, Texture.height + 40);
                if (scene != null && scene.getTrajectory() == null)
                {
                    // If no trajectory I have to move the area to the trajectory for it to be connected
                    area = area.MoveAreaToTrajectory(sceneMB.Trajectory);
                }
                else if (Context.getInfluenceArea() != null && Context.getInfluenceArea().isExists())
                {
                    area = Context.getInfluenceArea().MoveArea(topLeft);
                }
                PlayerMB.Instance.Do(action, area);
            }
        }

        // Public movement methods

        public bool Move(Vector2 point)
        {
            AbortCurrentMovement();

            var accesible = TrajectoryHandler.GetAccessibleTrajectory(getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            var route = accesible.route(getPosition(), point);
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
            AbortCurrentMovement();
            if (area.Contains(this.getPosition(), 0))
            {
                OnMovementFinished(); 
                return true;
            }

            var accesible = TrajectoryHandler.GetAccessibleTrajectory(getPosition(), FindObjectOfType<SceneMB>().Trajectory);
            Vector2[] intersections;
            if (TrajectoryHandler.TrajectoryRectangleIntersections(area, accesible, out intersections))
            {
                var route = accesible.route(getPosition(), intersections);
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
            base.setPosition(point);
        }

        // Private movement management methods

        private void AbortCurrentMovement()
        {
            toArea = null;
            if (moving)
            {
                Play("stand");
                // Clear the main variables
                moving = false;
                progress = 0.0f;
                moves.Clear();
                // Notify the chidls
                OnMovementCancelled();
            }
        }

        private void MoveToPoint(MovementPoint point)
        {
            moving = true;
            progress = 0.0f;

            this.point = point;
            this.origin = this.getPosition();
            this.originalScale = Context.getScale();
            this.orientation = (point.destination-origin).ToOrientation();
            Play("walk");
        }

        protected void UpdateMovement()
        {
            if (moving)
            {
                progress = progress + point.getProgress(player_speed, Time.deltaTime);
                Context.setScale(point.getScaleAt(progress, originalScale));
                base.setPosition(point.getPointAt(progress, origin));
                SortZ();

                var isInside = (toArea != null && toArea.Contains(getPosition(), distanceToArea));

                if (progress >= 1.0f || isInside)
                {
                    if (!isInside && moves.Count > 0)
                    {
                        MoveToPoint(moves.Dequeue());
                    }
                    else
                    {
                        moving = false;
                        Play("stand");
                        OnMovementFinished();
                    }
                }
            }
        }

        private void SortZ()
        {
            Vector2 currentpos = base.getPosition();
            for (int i = max_layer; i >= -1; i--)
            {
                if (SortingLayer.ContainsKey(i) && SortingLayer[i] > currentpos.y)
                {
                    this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, -0.3f - i);
                    this.Context.setLayer(i);
                    break;
                }
            }
        }

        protected virtual void OnMovementCancelled() { }

        protected virtual void OnMovementFinished() { }
    }
}