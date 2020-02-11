using UnityEngine;
using TriangleNet.Geometry;
using MapzenGo.Helpers;
using TriangleNet;
using MapzenGo.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using uAdventure.Runner;
using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using AssetPackage;

namespace uAdventure.Geo
{
    public class GeoElementMB : MonoBehaviour
    {

        public Material poiMat, pathMat, polyMat;

        public GameObject Tooltip;

        public Tile Tile { get; set; }
        public GeoElement Element { get; set; }
        public GeoReference Reference { get; set; }

        public GMLGeometry Geometry
        {
            get { return Element.Geometries.Checked().FirstOrDefault(); }
        }

        public bool inside;

        protected GeoPositionedCharacter player;
        private List<IGeoActionManager> geoActionManagers;



        public void UpdateConditions()
        {
            var display = !Element.IsRemoved() &&
                          (Reference.Conditions == null || ConditionChecker.check(Reference.Conditions));
            this.gameObject.SetActive(display);
        }

        // Use this for initialization
        void Start()
        {
            UpdateConditions();

            player = FindObjectOfType<GeoPositionedCharacter>();

            geoActionManagers = new List<IGeoActionManager>();
            foreach (var action in Element.Actions)
            {
                var newManager = GeoActionManagerFactory.Instance.CreateFor(action);
                newManager.Element = Element;
                newManager.Holder = this;
                geoActionManagers.Add(newManager);
            }

            var mesh = GetComponent<MeshFilter>().mesh;

            var selectedGeometry = Geometry;
            if (selectedGeometry != null && selectedGeometry.Points != null && selectedGeometry.Points.Length > 0)
            {
                switch (selectedGeometry.Type)
                {
                    case GMLGeometry.GeometryType.Point:
                        {
                            var poi = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            GetComponent<MeshFilter>().mesh = poi.GetComponent<MeshFilter>().mesh;
                            GetComponent<MeshRenderer>().material = poiMat;
                            DestroyImmediate(poi);


                            var dotMerc = GM.LatLonToMeters(selectedGeometry.Points[0].x, selectedGeometry.Points[0].y);
                            var localMercPos = dotMerc - Tile.Rect.Center;

                            transform.localPosition = new Vector3((float)localMercPos.x, 1f, (float)localMercPos.y);
                            transform.localScale = new Vector3(10, 10, 10);
                        }
                        break;
                    case GMLGeometry.GeometryType.LineString:
                        {
                            // First we extend the line for it to have some width using clipper
                            Path polygon = selectedGeometry.Points.ToList()
                                .ConvertAll(p => GM.LatLonToMeters(p.x, p.y)) // To meters
                                .ConvertAll(p => new IntPoint(p.x, p.y)); // To IntPoint type usable by clipper

                            Paths solution = new Paths();

                            ClipperOffset c = new ClipperOffset();

                            c.AddPath(polygon, JoinType.jtSquare, EndType.etOpenSquare);
                            c.Execute(ref solution, 5); // 5 meters

                            var r = solution[0].ConvertAll(p => new Vector2d(p.X, p.Y)).ToArray(); // ConvertBack to Vector2d all the points (meters)

                            // Then create the polygon that represents the path
                            CreatePolygonMesh(r, ref mesh);
                            GetComponent<MeshRenderer>().material = pathMat;
                        }
                        break;
                    case GMLGeometry.GeometryType.Polygon:
                        CreatePolygonMesh(selectedGeometry.Points.ToList().ConvertAll(p => GM.LatLonToMeters(p.x, p.y)).ToArray(), ref mesh);
                        GetComponent<MeshRenderer>().material = polyMat;
                        break;
                    default:
                        break;
                }

                inside = selectedGeometry.InsideInfluence(player.LatLon);
            }



            if (!string.IsNullOrEmpty(Element.getName()))
            {
                var tooltip = GameObject.Instantiate(Tooltip);
                tooltip.transform.SetParent(this.transform);
                tooltip.transform.localRotation = Quaternion.Euler(90, 0, 0);
                tooltip.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
                tooltip.transform.localPosition = Tooltip.transform.localPosition * (tooltip.transform.localScale.x / Tooltip.transform.localScale.x);

                tooltip.GetComponentInChildren<UnityEngine.UI.Text>().text = Element.getName();
            }

        }

        // Update is called once per frame
        void Update()
        {
            geoActionManagers.ForEach(g => g.Update());
        }

        private void CreatePolygonMesh(Vector2d[] points, ref UnityEngine.Mesh mesh)
        {
            var inp = new InputGeometry(points.Length);
            int i = 0;

            foreach (var p in points)
            {
                var localMercPos = p - Tile.Rect.Center;
                inp.AddPoint(localMercPos.x, localMercPos.y);
                inp.AddSegment(i, (i + 1) % points.Length);
                i++;
            }

            var md = new MeshData();

            CreateMesh(inp, md);

            //I want object center to be in the middle of object, not at the corner of the tile
            var center = ChangeToRelativePositions(md.Vertices);
            transform.localPosition = center;

            mesh.vertices = md.Vertices.ToArray();
            mesh.triangles = md.Indices.ToArray();
            mesh.SetUVs(0, md.UV);
            mesh.RecalculateNormals();
        }

        private void CreateMesh(InputGeometry corners, MeshData meshdata)
        {
            var mesh = new TriangleNet.Mesh();
            mesh.Behavior.Algorithm = TriangulationAlgorithm.SweepLine;
            mesh.Behavior.Quality = true;
            mesh.Triangulate(corners);

            var vertsStartCount = meshdata.Vertices.Count;
            meshdata.Vertices.AddRange(corners.Points.Select(x => new Vector3((float)x.X, 0, (float)x.Y)).ToList());
            meshdata.UV.AddRange(corners.Points.Select(x => new Vector2((float)x.X, (float)x.Y)).ToList());
            foreach (var tri in mesh.Triangles)
            {
                meshdata.Indices.Add(vertsStartCount + tri.P1);
                meshdata.Indices.Add(vertsStartCount + tri.P0);
                meshdata.Indices.Add(vertsStartCount + tri.P2);
            }
        }

        private Vector3 ChangeToRelativePositions(List<Vector3> landuseCorners)
        {
            var landuseCenter = landuseCorners.Aggregate((acc, cur) => acc + cur) / landuseCorners.Count;
            for (int i = 0; i < landuseCorners.Count; i++)
            {
                //using corner position relative to landuse center
                landuseCorners[i] = landuseCorners[i] - landuseCenter;
            }
            return landuseCenter;
        }

        void OnDestroy()
        {
            var ua = uAdventurePlugin.FindObjectOfType<uAdventurePlugin>();
            if (ua)
            {
                ua.ReleaseElement(Reference);
            }
        }

        // ---------------------------------------
        // GEO Actions
        // --------------------------------------

        private class GeoActionManagerFactory
        {
            private static GeoActionManagerFactory instance;
            public static GeoActionManagerFactory Instance { get { return instance ?? (instance = new GeoActionManagerFactory()); } }

            private readonly List<IGeoActionManager> geoActionManagers;
            private GeoActionManagerFactory()
            {
                geoActionManagers = new List<IGeoActionManager>()
            {
                new ExitGeoActionManager(),
                new EnterGeoActionManager(),
                new InspectGeoActionManager(),
                new LookToGeoActionManager()
            };
            }

            public IGeoActionManager CreateFor(GeoAction geoAction)
            {
                // Create a clone using activator
                var r = (IGeoActionManager)Activator.CreateInstance(geoActionManagers.Find(m => m.ActionType == geoAction.GetType()).GetType());
                r.Action = geoAction;
                return r;
            }
        }

        // Interface
        private interface IGeoActionManager
        {
            GeoAction Action { get; set; }
            GeoElementMB Holder { get; set; }
            GeoElement Element { get; set; }
            Type ActionType { get; }
            void Update();
        }

        // Abstract class
        private abstract class AbstractGeoActionManager : IGeoActionManager
        {
            public GeoElementMB Holder { get; set; }
            public GeoElement Element { get; set; }
            public GeoAction Action { get; set; }
            public abstract Type ActionType { get; }

            public virtual void Start() { }

            public virtual void Update()
            {
                if (Check())
                {
                    Execute();
                }
            }

            protected virtual bool Check()
            {
                return !Game.Instance.isSomethingRunning() && ConditionChecker.check(Action.Conditions) && CustomChecks();
            }

            protected abstract bool CustomChecks();
            protected virtual void Execute()
            {
                EffectHolder eh = new EffectHolder(Action.Effects);
                Game.Instance.Execute(eh, ActionFinished);
            }

            protected virtual void ActionFinished(object interactuable){}

            protected Vector2d LatLon
            {
                get
                {
                    return GeoExtension.Instance.IsStarted() && GeoExtension.Instance.IsLocationValid()
                        ? Input.location.lastData.LatLonD()
                        : Holder.player.LatLon;
                }
            }

        }

        // Individual actions
        private class EnterGeoActionManager : AbstractGeoActionManager
        {
            private bool first = true;
            private bool wasInside = false;
            private Vector2d latLonOnExecute;

            public override Type ActionType { get { return typeof(EnterAction); } }

            public override void Start()
            {
                wasInside = Holder.Geometry.InsideInfluence(LatLon);
            }

            protected override bool CustomChecks()
            {
                EnterAction ea = Action as EnterAction;
                var r = false;

                if (Holder.Geometry.InsideInfluence(LatLon))
                {
                    if (!wasInside)
                    {
                        r = wasInside = true;
                    }
                }
                else
                {
                    wasInside = false;
                }

                if (first)
                {
                    first = false;
                    if (ea.OnlyFromOutside && wasInside)
                    {
                        r = false;
                    }
                }

                return r;
            }

            protected override void Execute()
            {
                Game.Instance.GameState.BeginChangeAmbit();
                latLonOnExecute = LatLon;
                base.Execute();
            }

            protected override void ActionFinished(object interactuable)
            {
                Game.Instance.GameState.EndChangeAmbitAsExtensions();
                TrackerExtension.Movement.Entered(Element.Id, latLonOnExecute);
                TrackerAsset.Instance.Flush();
            }
        }

        private class ExitGeoActionManager : AbstractGeoActionManager
        {
            private bool first = true;
            private bool wasOutside = false;
            private Vector2d latLonOnExecute;

            public override Type ActionType { get { return typeof(ExitAction); } }

            public override void Start()
            {
                wasOutside = Holder.Geometry.InsideInfluence(LatLon);
            }

            protected override bool CustomChecks()
            {
                ExitAction ea = Action as ExitAction;
                var r = false;

                if (!Holder.Geometry.InsideInfluence(LatLon))
                {
                    if (!wasOutside)
                    {
                        r = wasOutside = true;
                    }
                }
                else
                {
                    wasOutside = false;
                }

                if (first)
                {
                    first = false;
                    if (ea.OnlyFromInside && wasOutside)
                    {
                        r = false;
                    }
                }

                return r;
            }

            protected override void Execute()
            {
                Game.Instance.GameState.BeginChangeAmbit();
                latLonOnExecute = LatLon;
                base.Execute();
            }

            protected override void ActionFinished(object interactuable)
            {
                Game.Instance.GameState.EndChangeAmbitAsExtensions();
                TrackerExtension.Movement.Exited(Element.Id, latLonOnExecute);
                TrackerAsset.Instance.Flush();
            }
        }

        private class LookToGeoActionManager : AbstractGeoActionManager
        {
            private Vector3d orientationOnExecute;
            private Vector2d latLonOnExecute;

            public override Type ActionType { get { return typeof(LookToAction); } }

            protected override bool CustomChecks()
            {
                LookToAction ea = Action as LookToAction;
                var r = false;

                if (!ea.Inside || Holder.Geometry.InsideInfluence(LatLon))
                {
                    if (ea.Center)
                    {
                        r = Holder.player.IsLookingTo(Holder.Geometry.Center);
                    }
                    else
                    {
                        r = Holder.player.IsLookingTowards(ea.Direction.ToVector2d());
                    }
                }

                return r;
            }

            protected override void Execute()
            {
                Game.Instance.GameState.BeginChangeAmbit();
                latLonOnExecute = LatLon;
                orientationOnExecute = Holder.player.Orientation;
                base.Execute();
            }

            protected override void ActionFinished(object interactuable)
            {
                Game.Instance.GameState.EndChangeAmbitAsExtensions();
                TrackerExtension.Movement.Looked(Element.Id, orientationOnExecute, latLonOnExecute);
                TrackerAsset.Instance.Flush();
            }
        }

        private class InspectGeoActionManager : AbstractGeoActionManager
        {
            private Collider collider;
            public override Type ActionType { get { return typeof(InspectAction); } }

            public override void Start()
            {
                base.Start();
                collider = Holder.gameObject.AddComponent<MeshCollider>();
            }

            protected override bool CustomChecks()
            {
                var r = false;

                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (collider.Raycast(ray, out hit, float.MaxValue))
                    {
                        r = true;
                    }
                }

                return r;
            }

            protected override void Execute()
            {
                Game.Instance.GameState.BeginChangeAmbit();
                base.Execute();
            }

            protected override void ActionFinished(object interactuable)
            {
                Game.Instance.GameState.EndChangeAmbitAsExtensions();
                TrackerAsset.Instance.setVar("geocommand", "inspect");
                TrackerAsset.Instance.GameObject.Interacted(Element.Id, GameObjectTracker.TrackedGameObject.GameObject);
                TrackerAsset.Instance.Flush();
            }
        }
    }
}