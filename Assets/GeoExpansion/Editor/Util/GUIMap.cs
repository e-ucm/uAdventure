using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using MapzenGo.Helpers;
using MapzenGo.Helpers.VectorD;
using uAdventure.Core;
using System;
using uAdventure.Editor;

using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

public class GUIMap
{
    /* -----------------------------
     * Attributes
     *-----------------------------*/

    // Delegates
    public delegate void RepaintDelegate();

    // Attributes
    public RepaintDelegate Repaint;
    public int Zoom { get { return zoom; }
        set
        {
            zoom = Mathf.Clamp(value, 0, 19);
            centerPixel = GM.MetersToPixels(GM.LatLonToMeters(Center.x, Center.y), zoom);
        }
    }


    public List<ExtElemReferenceGUIMapPositionManager> PositionedResources { get; set; }
    public List<GMLGeometry> Geometries { get; set; }
    public GMLGeometry selectedGeometry;
    public double SelectPointDistance { get; set; }
    public Vector2d Center { get { return center; }
        set
        {
            center = value;
            centerPixel = GM.MetersToPixels(GM.LatLonToMeters(center.x, center.y), Zoom);
        }
    }
    public Vector2d GeoMousePosition { get; set; }

    // private attributes
    protected int zoom = 0;
    protected Vector2d center;
    protected Vector2d centerPixel;
    private Vector2d PATR; // Will be calculated in the begining of each iteration

    protected int selectedPoint;

    /* -----------------------------
     * Constructor
     *-----------------------------*/

    public GUIMap()
    {
        Geometries = new List<GMLGeometry>();
        SelectPointDistance = 15.0;
        
        PositionedResources = new List<ExtElemReferenceGUIMapPositionManager>();
    }

    /* -----------------------------
     *  Main method
     *-----------------------------*/

    public bool DrawMap(Rect area)
    {

        bool r = false;
        GUI.BeginGroup(area);
        area = new Rect(Vector2.zero, area.size);
        // update the pixel absolute to relative convert variable
        PATR = -(centerPixel - (area.size / 2f).ToVector2d() - area.position.ToVector2d());

        var mousePos = Event.current.mousePosition.ToVector2d();
        var delta = new Vector2d(Event.current.delta.x, Event.current.delta.y);

        if(Event.current.type != EventType.layout)
            GeoMousePosition = GM.MetersToLatLon(GM.PixelsToMeters(RelativeToAbsolute(mousePos), Zoom));

        switch (Event.current.type)
        {
            case EventType.Repaint:
                {
                    // Draw the tiles
                    DrawTiles(area);

                    // Draw the GeoShapes
                    DrawGeometries(area);

                    // Draw the Resources
                    DrawResources(area);

                    if(selectedGeometry != null && selectedGeometry.Points.Count > 0)
                    {
                        var pixels = PixelsToRelative(LatLonToPixels(selectedGeometry.Points)).ConvertAll(p => p.ToVector2());
                        var v2mousepos = mousePos.ToVector2();
                        // Find the closest index
                        var min = pixels.Min(p => (p - v2mousepos).magnitude);
                        var closest = pixels.FindIndex(p => (p - v2mousepos).magnitude == min);
                        
                        // Fix the previous and next
                        var prev = closest == 0 ? pixels.Count - 1 : closest - 1;
                        var next = (closest + 1) % pixels.Count;
                        // Calculate the normal to both adjacent axis to closest point
                        var c = pixels[closest];
                        var v1 = (pixels[closest] - pixels[prev]).normalized;
                        var v2 = (pixels[closest] - pixels[next]).normalized;

                        var closestNormal = (v1 + v2).normalized;
                        var convex = Vector3.Cross(v1, v2).z > 0;
                            
                        var mouseVector = (v2mousepos - c); 
                        var left = Vector3.Cross(closestNormal, mouseVector).z > 0;
                        Handles.DrawLine(pixels[closest], v2mousepos);
                        if ((left && convex) || (!left && !convex))
                        {
                            Handles.DrawLine(pixels[prev], v2mousepos);
                        }
                        else
                        {
                            Handles.DrawLine(pixels[next], v2mousepos);
                        }
                        
                    }
                }
                break;

            case EventType.ScrollWheel:
                {
                    // Changezoom
                    Zoom += Mathf.FloorToInt(-Event.current.delta.y / 3f);
                    Event.current.Use();
                }
                break;


            case EventType.mouseDrag:
                {
                    // MoveLatLon or center var 
                    if (area.Contains(Event.current.mousePosition))
                    {
                        if (selectedGeometry != null)
                        {
                            var pixels = LatLonToPixels(selectedGeometry.Points);

                            // Find the closest point
                            /*var point = PixelsToRelative(pixels)
                                .FindIndex(p => (p - mousePos).magnitude < SelectPointDistance);*/
                            // If there's a point, move the point
                            if (selectedPoint != -1) pixels[selectedPoint] += delta;
                            // Otherwise, move the pixel
                            else pixels = pixels.ConvertAll(p => p + delta);
                            // Then update the points
                            selectedGeometry.Points = PixelsToLatLon(pixels);
                        }
                        else
                        {
                            Center = GM.MetersToLatLon(GM.PixelsToMeters(centerPixel - delta, Zoom));
                        }
                        Event.current.Use();
                    }
                }
                break;
            case EventType.mouseDown:
                {
                    selectedGeometry = Geometries.Find(g => 
                    {
                        List<Vector2d> points = PixelsToRelative(LatLonToPixels(g.Points))
                            .ConvertAll(p => p - Event.current.mousePosition.ToVector2d());

                        selectedPoint = points.FindIndex(p => p.magnitude < SelectPointDistance);

                        return g.Inside(GeoMousePosition) || selectedPoint != -1; 
                    });

                    if (area.Contains(Event.current.mousePosition))
                    {
                        GUI.FocusControl(null);
                        r = true;
                    }

                }
                break;
        }

        GUI.EndGroup();
        return r;

    }


    /* -----------------------------
     *  Drawing methods
     *-----------------------------*/

    protected void DrawTiles(Rect area)
    {
        // Download and draw tiles
        var tile = GM.MetersToTile(GM.LatLonToMeters(Center.x, Center.y), Zoom);

        Vector2d topLeftCorner = GM.PixelsToTile(centerPixel - new Vector2d(area.width / 2f, area.height / 2f)),
            bottomRightCorner = GM.PixelsToTile(centerPixel + new Vector2d(area.width / 2f, area.height / 2f));

        for (double x = topLeftCorner.x; x <= bottomRightCorner.x; x++)
        {
            for (double y = topLeftCorner.y; y <= bottomRightCorner.y; y++)
            {
                var tp = TileProvider.GetTile(new Vector3d(x, y, Zoom), (texture) => { if (Repaint != null) Repaint(); });
                var tileBounds = GM.TileBounds(new Vector2d(x, y), Zoom);
                var tileRect = ExtensionRect.FromCorners(
                    GM.MetersToPixels(tileBounds.Min, Zoom).ToVector2(),
                    GM.MetersToPixels(tileBounds.Min + tileBounds.Size, Zoom).ToVector2());

                var windowRect = new Rect(tileRect.position + PATR.ToVector2(), tileRect.size);
                var areaRect = windowRect.Intersection(area);
                if (areaRect.width < 0 || areaRect.height < 0)
                    continue;

                GUI.DrawTextureWithTexCoords(areaRect, tp.Texture, windowRect.ToTexCoords(areaRect));
            }
        }
    }

    protected void DrawGeometries(Rect area)
    {        
        foreach(var g in Geometries)
        {
            // Convert from lat lon to pixel relative to the rect
            List<Vector2d> points = PixelsToRelative(LatLonToPixels(g.Points));
            if (points.Count == 0)
                continue;


            Handles.BeginGUI();
            switch (g.Type)
            {
                case GMLGeometry.GeometryType.Point:
                    DrawPoint(points[0].ToVector2());
                    break;
                case GMLGeometry.GeometryType.LineString:
                    DrawPolyLine(points.ConvertAll(p => p.ToVector2()).ToArray());
                    points.ForEach(p => DrawPoint(p.ToVector2()));
                    break;
                case GMLGeometry.GeometryType.Polygon:
                   
                    DrawPolygon(points.ConvertAll(p => p.ToVector2()).ToArray());

                    var cicle = new List<Vector2d>();
                    cicle.AddRange(points);
                    cicle.Add(points[0]);

                    DrawPolyLine(cicle.ConvertAll(p => p.ToVector2()).ToArray());
                    points.ForEach(p => DrawPoint(p.ToVector2()));
                    break;
            }

            DrawInfluenceArea(points.ConvertAll(p => p.ToVector2()).ToArray(), (float)GM.MetersToPixels(new Vector2d(g.Influence,0), Zoom).x - (float)GM.MetersToPixels(new Vector2d(0,0),Zoom).x, g.Type);

            Handles.EndGUI();

        }
    }

    private void DrawInfluenceArea(Vector2[] points, float radius, GMLGeometry.GeometryType type)
    {
        if(points.Length == 1)
        {
            Handles.color = Color.black;
            Handles.DrawWireArc(points[0], Vector3.back, Vector2.up, 360, radius);
            return;
        }

        DrawPolyLine(ExtendPolygon(new List<Vector2>(points), radius, type).ToArray());

        /*var prev = points[points.Length - 1];
        Vector2 prTh, thNx, prevVector = Vector2.zero;
        for(int i = 0; i<= points.Length; i++)
        {
            prTh = points[i % points.Length] - prev;
            thNx = points[(i + 1) % points.Length] - points[i % points.Length];

            prTh = RotateVector(prTh, -90).normalized;
            thNx = RotateVector(thNx, -90).normalized;

            if (i > 0) {
                if (Vector3.Cross(prTh, thNx).z < 0)
                {
                    Handles.color = Color.blue;
                    Handles.DrawLine(prev + prevVector * radius, points[i % points.Length] + prTh * radius);
                    Handles.DrawWireArc(points[i % points.Length], Vector3.back, prTh, Vector3.Angle(prTh, thNx), radius);
                }
                else
                {
                    Handles.DrawLine(prev + prevVector, LineIntersectionPoint(prev + prevVector * radius*2, prev + prevVector*radius, points[i % points.Length] + prTh * radius*2, points[i % points.Length] + prTh * radius));
                }
            }
            prevVector = thNx;
            prev = points[i % points.Length];*/
        
    }

    private void DrawResources(Rect area)
    {
        foreach(var r in PositionedResources)
        {
            r.Draw(this, area);
        }
    }

    private void DrawPoint(Vector2 position)
    {
        Handles.color = Color.black;
        Handles.DrawSolidDisc(position, Vector3.forward, 4f);
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(position, Vector3.forward, 3.5f);
    }

    private void DrawPolyLine(Vector2[] points)
    {
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(2f, V2ToV3(points));
    }

    private void DrawPolygon(Vector2[] points)
    {
        Triangulator2 tr = new Triangulator2(points);
        Handles.color = new Color(.2f,.2f,.9f,.5f);
        int[] indices = tr.Triangulate();
        Vector3[] triangle = new Vector3[3];
        var v3p = V2ToV3(points);
        for (int i = 2; i<indices.Length; i+=3)
        {
            triangle[0] = v3p[indices[i-2]];
            triangle[1] = v3p[indices[i-1]];
            triangle[2] = v3p[indices[i]];
            Handles.DrawAAConvexPolygon(triangle);
        }
    }

    private Vector3[] V2ToV3(Vector2[] points)
    {
        var l = new List<Vector2>();
        l.AddRange(points);
        return l.ConvertAll(p => new Vector3(p.x, p.y, 0f)).ToArray();
    }

    public List<Vector2d> LatLonToPixels(List<Vector2d> points)
    {        
        return points.ConvertAll(p => GM.MetersToPixels(GM.LatLonToMeters(p.x, p.y), Zoom)); 
    }

    public List<Vector2d> PixelsToLatLon(List<Vector2d> points)
    {
        return points.ConvertAll(p => GM.MetersToLatLon(GM.PixelsToMeters(p, Zoom)));
    }

    public Vector2d PixelToRelative(Vector2d pixel)
    {
        return pixel + PATR;
    }

    public Vector2d RelativeToAbsolute(Vector2d pixel)
    {
        return pixel - PATR;
    }

    public List<Vector2d> PixelsToRelative(List<Vector2d> pixels)
    {
        return pixels.ConvertAll(p => PixelToRelative(p));
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {

        return Quaternion.Euler(0, 0, -angle) * vector;


    }

    // http://www.wyrmtale.com/blog/2013/115/2d-line-intersection-in-c
    Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

    public List<Vector2> ExtendPolygon(List<Vector2> points, float radius, GMLGeometry.GeometryType type)
    {
        Path polygon = points.ConvertAll(p => new IntPoint(p.x, p.y));
        
        Paths solution = new Paths();

        ClipperOffset c = new ClipperOffset();
        
        c.AddPath(polygon, JoinType.jtRound, type == GMLGeometry.GeometryType.Polygon ? EndType.etClosedPolygon : EndType.etOpenRound);
        c.Execute(ref solution, radius);
        
        var r = solution.Count > 0 ? solution[0].ConvertAll(p => new Vector2(p.X, p.Y)) : new List<Vector2>();

        if(r.Count > 0)
            r.Add(r[0]);

        return r;
    }
}