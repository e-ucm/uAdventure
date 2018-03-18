using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibTessDotNet;

using uAdventure.Core;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    public class ActiveAreaMB : Area, Interactuable, IPointerClickHandler
    {
        private ActiveArea aad;
        public ActiveArea Element
        {
            get { return aad; }
            set { aad = value; }
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
                    foreach (Action a in aad.getActions())
                    {
                        if (ConditionChecker.check(a.getConditions()))
                        {
                            Game.Instance.Execute(new EffectHolder(a.getEffects()));
                            break;
                        }
                    }
                    ret = InteractuableResult.DOES_SOMETHING;
                    break;
                case Item.BehaviourType.NORMAL:
                    List<Action> available = new List<Action>();
                    foreach (Action a in aad.getActions())
                    {
                        if (ConditionChecker.check(a.getConditions()))
                        {
                            bool addaction = true;
                            foreach (Action a2 in available)
                            {
                                if ((a.getType() == Action.CUSTOM || a.getType() == Action.CUSTOM_INTERACT) && (a2.getType() == Action.CUSTOM || a2.getType() == Action.CUSTOM_INTERACT))
                                {
                                    if (((CustomAction)a).getName() == ((CustomAction)a2).getName())
                                    {
                                        addaction = false;
                                        break;
                                    }
                                }
                                else if (a.getType() == a2.getType())
                                {
                                    addaction = false;
                                    break;
                                }
                            }

                            if (addaction)
                                available.Add(a);
                        }
                    }

                    //We check if it's an examine action, otherwise we create one and add it
                    bool addexamine = true;
                    string desc = aad.getDescription(0).getDetailedDescription();
                    if (desc != "")
                    {
                        foreach (Action a in available)
                        {
                            if (a.getType() == Action.EXAMINE)
                            {
                                addexamine = false;
                                break;
                            }
                        }

                        if (addexamine)
                        {
                            Action ex = new Action(Action.EXAMINE);
                            Effects exeff = new Effects();
                            exeff.Add(new SpeakPlayerEffect(desc));
                            ex.setEffects(exeff);
                            available.Add(ex);
                        }
                    }

                    if (available.Count > 0)
                    {
                        Game.Instance.showActions(available, Input.mousePosition);
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
    }
}