using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;

namespace uAdventure.Runner
{
    public class ActiveAreaMB : Area, Interactuable, IPointerClickHandler, IDropHandler, IActionReceiver
    {
        private static readonly int[] restrictedActions = { Action.CUSTOM, Action.EXAMINE, Action.USE };

        private ActiveArea aad;
        public ActiveArea Element
        {
            get { return aad; }
            set
            {
                aad = value;
                if (aad != null)
                {
                    this.gameObject.name = aad.getId();
                }
            }
        }

        public static Vector3 Vec3toVector3(Vec3 _v)
        {
            return new Vector3(_v.X, _v.Y, _v.Z);
        }

        void Start()
        {
            adaptate();
        }

        bool interactable = false;
        public bool canBeInteracted()
        {
            return interactable;
        }

        public void setInteractuable(bool state)
        {
            this.interactable = state;
        }

        public InteractuableResult Interacted(PointerEventData eventData)
        {
            InteractuableResult ret = InteractuableResult.IGNORES;

            if (aad.getInfluenceArea() != null)
            {

            }

            switch (aad.getBehaviour())
            {
                case Item.BehaviourType.FIRST_ACTION:
                    {
                        var actions = Element.getActions().Checked();
                        if (actions.Any())
                        {
                            Game.Instance.Execute(new EffectHolder(actions.First().getEffects()));
                            ret = InteractuableResult.DOES_SOMETHING;
                        }
                    }
                    break;
                case Item.BehaviourType.NORMAL:
                    var availableActions = Element.getActions().Valid(restrictedActions).ToList();

                    ActionsUtil.AddExamineIfNotExists(Element, availableActions);

                    //if there is an action, we show them
                    if (availableActions.Count > 0)
                    {
                        Game.Instance.showActions(availableActions, Input.mousePosition, this);
                        ret = InteractuableResult.DOES_SOMETHING;
                    }
                    break;
                case Item.BehaviourType.ATREZZO:
                default:
                    ret = InteractuableResult.IGNORES;
                    break;
            }

            return ret;
        }


        private bool mouseInInfluenceArea(InfluenceArea area)
        {
            return true;
        }

        private void adaptate()
        {
            if (!this.aad.isRectangular() && this.aad.getInfluenceArea() != null)
            {
                this.transform.localScale = new Vector3(1, 1, 1);

                Mesh mesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                Tess tess = new LibTessDotNet.Tess();

                ContourVertex[] contour = new ContourVertex[aad.getPoints().Count];
                int i = 0;

                float minx = float.MaxValue, miny = float.MaxValue, maxx = 0, maxy = 0;
                foreach (Vector2 v in this.aad.getPoints())
                {
                    if (v.x < minx)
                        minx = v.x;

                    if (v.x > maxx)
                        maxx = v.x;

                    if (v.y < miny)
                        miny = v.y;

                    if (v.y > maxy)
                        maxy = v.y;
                }

                minx = (minx + (maxx - minx) / 2) / 10;
                miny = 60 - (miny + (maxy - miny) / 2) / 10;

                foreach (Vector2 v in this.aad.getPoints())
                {
                    contour[i].Position = new LibTessDotNet.Vec3 { X = v.x / 10f - minx, Y = 60 - v.y / 10f - miny, Z = this.transform.position.z };
                    i++;
                }

                tess.AddContour(contour, LibTessDotNet.ContourOrientation.CounterClockwise);
                tess.Tessellate(WindingRule.EvenOdd, LibTessDotNet.ElementType.Polygons, 3);

                List<int> triangles = new List<int>();
                int numTriangles = tess.ElementCount;
                for (i = 0; i < numTriangles; i++)
                {
                    vertices.Add(Vec3toVector3(tess.Vertices[tess.Elements[i * 3]].Position));
                    vertices.Add(Vec3toVector3(tess.Vertices[tess.Elements[i * 3 + 1]].Position));
                    vertices.Add(Vec3toVector3(tess.Vertices[tess.Elements[i * 3 + 2]].Position));
                    triangles.AddRange(new int[] { i * 3, i * 3 + 1, i * 3 + 2 });
                }

                mesh.SetVertices(vertices);
                mesh.SetTriangles(triangles, 0);

                this.GetComponent<MeshFilter>().sharedMesh = mesh;
                this.GetComponent<MeshCollider>().sharedMesh = mesh;
            }
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
            if (Game.Instance.GameState.IsFirstPerson|| !action.isNeedsGoTo())
                Game.Instance.Execute(new EffectHolder(action.Effects));
            else
            {
                var sceneMB = FindObjectOfType<SceneMB>();
                var scene = sceneMB.sceneData as Scene;
                Rectangle area = null;
                if (scene!= null && scene.getTrajectory() == null)
                {
                    // If no trajectory I have to move the area to the trajectory for it to be connected
                    area = aad.MoveAreaToTrajectory(sceneMB.Trajectory);
                }
                else
                {
                    area = new InfluenceArea(aad.getX() - 20, aad.getY() - 20, aad.getWidth() + 40, aad.getHeight() + 40);
                    if(aad.getInfluenceArea() != null && aad.getInfluenceArea().isExists())
                    {
                        var points = aad.isRectangular() ? aad.ToRect().ToPoints() : aad.getPoints().ToArray();
                        var topLeft = points.ToRect().position;
                        area = aad.getInfluenceArea().MoveArea(topLeft);
                    }
                }
                PlayerMB.Instance.Do(action, area);
            }
        }

    }
}