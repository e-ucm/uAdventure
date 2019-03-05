﻿using UnityEngine;
using System.Collections;
using TriangleNet.Geometry;
using MapzenGo.Helpers;
using TriangleNet;
using uAdventure.Geo;
using MapzenGo.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using uAdventure.Runner;
using RAGE.Analytics;
using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using AssetPackage;

public class GeoElementMB : MonoBehaviour {

    public Material poiMat, pathMat, polyMat;

    public GameObject Tooltip;

    public Tile Tile { get; set; }
    public GeoElement Element { get; set; }
    public GeoReference Reference { get; set; }

    public bool inside;

    protected GeoPositionedCharacter player;
    private List<GeoActionManager> geoActionManagers;

    // Use this for initialization
    void Start () {

        player = FindObjectOfType<GeoPositionedCharacter>();

        geoActionManagers = new List<GeoActionManager>();
        foreach (var action in Element.Actions)
        {
            var newManager = GeoActionManagerFactory.Instance.CreateFor(action);
            newManager.Element = Element;
            newManager.Holder = this;
            geoActionManagers.Add(newManager);
        }

        var mesh = GetComponent<MeshFilter>().mesh;

        switch (Element.Geometry.Type)
        {
            case GMLGeometry.GeometryType.Point:
                {
                    var poi = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    GetComponent<MeshFilter>().mesh = poi.GetComponent<MeshFilter>().mesh;
                    GetComponent<MeshRenderer>().material = poiMat;
                    DestroyImmediate(poi);


                    var dotMerc = GM.LatLonToMeters(Element.Geometry.Points[0].x, Element.Geometry.Points[0].y);
                    var localMercPos = dotMerc - Tile.Rect.Center;

                    transform.localPosition = new Vector3((float)localMercPos.x, 1f, (float)localMercPos.y);
                    transform.localScale = new Vector3(10, 10, 10);
                }
                break;
            case GMLGeometry.GeometryType.LineString:
                {
                    // First we extend the line for it to have some width using clipper
                    Path polygon = Element.Geometry.Points
                        .ConvertAll(p => GM.LatLonToMeters(p.x, p.y)) // To meters
                        .ConvertAll(p => new IntPoint(p.x, p.y)); // To IntPoint type usable by clipper

                    Paths solution = new Paths();

                    ClipperOffset c = new ClipperOffset();

                    c.AddPath(polygon, JoinType.jtSquare, EndType.etOpenSquare);
                    c.Execute(ref solution, 5); // 5 meters

                    var r = solution[0].ConvertAll(p => new Vector2d(p.X, p.Y)); // ConvertBack to Vector2d all the points (meters)

                    // Then create the polygon that represents the path
                    CreatePolygonMesh(r, ref mesh); 
                    GetComponent<MeshRenderer>().material = pathMat;
                }
                break;
            case GMLGeometry.GeometryType.Polygon:
                CreatePolygonMesh(Element.Geometry.Points.ConvertAll(p => GM.LatLonToMeters(p.x, p.y)), ref mesh);
                GetComponent<MeshRenderer>().material = polyMat;
                break;
            default:
                break;

        }

        if(Element.Name != "")
        {
            var tooltip = GameObject.Instantiate(Tooltip);
            tooltip.transform.SetParent(this.transform);
            tooltip.transform.localRotation = Quaternion.Euler(90, 0, 0);
            tooltip.transform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1 / transform.localScale.z);
            tooltip.transform.localPosition = Tooltip.transform.localPosition * (tooltip.transform.localScale.x/ Tooltip.transform.localScale.x);

            tooltip.GetComponentInChildren<UnityEngine.UI.Text>().text = Element.Name;
        }

        inside = Element.Geometry.InsideInfluence(player.LatLon);
    }

	// Update is called once per frame
	void Update () {
        geoActionManagers.ForEach(g => g.Update());
    }

    private void CreatePolygonMesh(List<Vector2d> points, ref UnityEngine.Mesh mesh)
    {
        var inp = new InputGeometry(points.Count);
        int i = 0;

        foreach (var p in points)
        {
            var localMercPos = p - Tile.Rect.Center;
            inp.AddPoint(localMercPos.x, localMercPos.y);
            inp.AddSegment(i, (i + 1) % points.Count);
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
        if(ua)
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
        public static GeoActionManagerFactory Instance { get { return instance == null ? instance = new GeoActionManagerFactory() : instance; } }

        private List<GeoActionManager> geoActionManagers;
        private GeoActionManagerFactory()
        {
            geoActionManagers = new List<GeoActionManager>()
            {
                new ExitGeoActionManager(),
                new EnterGeoActionManager(),
                new InspectGeoActionManager(),
                new LookToGeoActionManager()
            };
        }

        public GeoActionManager CreateFor(GeoAction geoAction)
        {
            // Create a clone using activator
            var r = (GeoActionManager) Activator.CreateInstance(geoActionManagers.Find(m => m.ActionType == geoAction.GetType()).GetType());
            r.Action = geoAction;
            return r;
        }
    }

    // Interface
    private interface GeoActionManager
    {
        GeoAction Action { get; set; }
        GeoElementMB Holder { get; set; }
        GeoElement Element { get; set; }
        Type ActionType { get; }
        void Update();
    }

    // Abstract class
    private abstract class AbstractGeoActionManager : GeoActionManager
    {
        public GeoElementMB Holder { get; set; }
        public GeoElement Element { get; set; }
        public GeoAction Action { get; set; }
        public abstract Type ActionType { get; }

        public virtual void Start() {}

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
            Game.Instance.Execute(eh);
        }

        protected Vector2d LatLon
        {
            get
            {
                return GPSController.Instance.IsStarted() && GPSController.Instance.IsLocationValid()
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

        public override Type ActionType { get { return typeof(EnterAction); } }

        public override void Start()
        {
            wasInside = Element.Geometry.InsideInfluence(LatLon);
        }

        protected override bool CustomChecks()
        {
            EnterAction ea = Action as EnterAction;
            var r = false;

            if (first)
            {
                r = wasInside && !ea.OnlyFromOutside;
                first = true;
            }

            if (Element.Geometry.InsideInfluence(LatLon))
            {
                if (!wasInside)
                {
                    wasInside = true;
                    r = wasInside;
                }
            }
            return r;
        }

        protected override void Execute()
        {
            var latlon = LatLon.ToVector2();

            TrackerAsset.Instance.setGeopoint(latlon.x, latlon.y);
            TrackerAsset.Instance.setVar("geocommand", "enter");
            TrackerAsset.Instance.Accessible.Accessed(Element.Id, AccessibleTracker.Accessible.Zone);
            TrackerAsset.Instance.Flush();
            base.Execute();
        }
    }

    private class ExitGeoActionManager : AbstractGeoActionManager
    {
        private bool first = true;
        private bool wasInside = false;

        public override Type ActionType { get { return typeof(ExitAction); } }

        public override void Start()
        {
            wasInside = Element.Geometry.InsideInfluence(LatLon);
        }

        protected override bool CustomChecks()
        {
            ExitAction ea = Action as ExitAction;
            var r = false;

            if (first)
            {
                r = !wasInside && !ea.OnlyFromInside;
                first = false;
            }

            if (!Element.Geometry.InsideInfluence(LatLon))
            {
                if (wasInside)
                {
                    wasInside = false;
                    r = wasInside;
                }
            }
            return r;
        }

        protected override void Execute()
        {
            var latlon = LatLon.ToVector2();

            TrackerAsset.Instance.setGeopoint(latlon.x, latlon.y);
            TrackerAsset.Instance.setVar("geocommand", "exit");
            TrackerAsset.Instance.Accessible.Accessed(Element.Id, AccessibleTracker.Accessible.Zone);
            TrackerAsset.Instance.Flush();

            base.Execute();
        }
    }

    private class LookToGeoActionManager : AbstractGeoActionManager
    {
        public override Type ActionType { get { return typeof(LookToAction); } }

        protected override bool CustomChecks()
        {
            LookToAction ea = Action as LookToAction;
            var r = false;

            if (!ea.Inside || Element.Geometry.InsideInfluence(LatLon))
            {
                if (ea.Center)
                {
                    r = Holder.player.IsLookingTo(Element.Geometry.Center);
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
            var latlon = LatLon.ToVector2();

            TrackerAsset.Instance.setGeopoint(latlon.x, latlon.y);
            TrackerAsset.Instance.setVar("geocommand", "look");
            TrackerAsset.Instance.Accessible.Accessed(Element.Id, AccessibleTracker.Accessible.Zone);
            TrackerAsset.Instance.Flush();

            base.Execute();
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
            TrackerAsset.Instance.setVar("geocommand", "inspect");
            TrackerAsset.Instance.GameObject.Interacted(Element.Id, GameObjectTracker.TrackedGameObject.GameObject);
            TrackerAsset.Instance.Flush();

            base.Execute();
        }
    }
}
