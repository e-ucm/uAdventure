using LibTessDotNet;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(Interactuable))]
    public class Area : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Interactuable interactuable;
        private Rectangle element;
        public Rectangle Element
        {
            get { return element; }
            set
            {
                element = value;
                if (element != null && element is HasId)
                {
                    this.gameObject.name = (element as HasId).getId();
                }
            }
        }
        protected virtual void Start()
        {
            Adaptate();
        }

        private void OnEnable()
        {
            interactuable = GetComponent<Interactuable>();
            if(interactuable == null)
            {
                this.enabled = false;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GUIManager.Instance.ShowHand(true);
            interactuable.setInteractuable(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GUIManager.Instance.ShowHand(false);
            interactuable.setInteractuable(false);
        }

        public void Adaptate()
        {
            if (!element.isRectangular())
            {
                this.transform.localScale = new Vector3(1, 1, 1);

                Mesh mesh = new Mesh();
                List<Vector3> vertices = new List<Vector3>();
                Tess tess = new LibTessDotNet.Tess();

                ContourVertex[] contour = new ContourVertex[element.getPoints().Count];
                int i = 0;

                float minx = float.MaxValue, miny = float.MaxValue, maxx = 0, maxy = 0;
                foreach (Vector2 v in element.getPoints())
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

                foreach (Vector2 v in this.element.getPoints())
                {
                    contour[i].Position = new LibTessDotNet.Vec3 { X = v.x / 10f - minx, Y = 60 - v.y / 10f - miny, Z = 0 };
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
        public static Vector3 Vec3toVector3(Vec3 _v)
        {
            return new Vector3(_v.X, _v.Y, _v.Z);
        }
    }
}