using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Helpers;
using uAdventure.Core;

public class GMLGeometry : ICloneable
{

    public enum GeometryType { Point, LineString, Polygon }

    public GMLGeometry()
    {
        Points = new List<Vector2d>();
        Conditions = new Conditions();
    }

    public GeometryType Type { get; set; }
    public List<Vector2d> Points { get; set; }
    public float Influence { get; set; }
    public Conditions Conditions { get; set; }

    public void AddPoint(Vector2d point)
    {
        switch (Type)
        {
            case GMLGeometry.GeometryType.Point:
                if (Points.Count == 1) Points[0] = point;
                else Points.Add(point);
                break;
            case GMLGeometry.GeometryType.LineString:
                Points.Add(point);
                break;
            case GMLGeometry.GeometryType.Polygon:
                if (Points.Count <= 1)
                {
                    Points.Add(point);
                }
                else
                {
                    // Find the closest index
                    var min = Points.Min(p => (p - point).magnitude);
                    var closest = Points.FindIndex(p => (p - point).magnitude == min);

                    // Fix the previous and next
                    var prev = closest == 0 ? Points.Count - 1 : closest - 1;
                    var next = (closest + 1) % Points.Count;
                    // Calculate the normal to both adjacent axis to closest point
                    var c = Points[closest];
                    var v1 = (Points[closest] - Points[prev]).normalized;
                    var v2 = (Points[closest] - Points[next]).normalized;

                    var closestNormal = (v1 + v2).normalized;
                    var convex = Vector3.Cross(v1.ToVector2(), v2.ToVector2()).z > 0;

                    var pointVector = (point - c);
                    var left = Vector3.Cross(closestNormal.ToVector2(), pointVector.ToVector2()).z > 0;

                    Debug.Log(convex ? "Convex" : "Concave");
                    if ((left && convex) || (!left && !convex))
                    {
                        Debug.Log("Prev");
                        // We insert at the closest
                        Points.Insert(closest, point);
                    }
                    else
                    {
                        Debug.Log("Next");
                        // We insert at the next
                        Points.Insert(next, point);
                    }
                }
                break;
        }
    }
    
    public bool InsideMargin(Vector2d point, float margin)
    {
        return Inside(Points, Type, point) || InsidePointRadius(Points, point, margin) || InsideEdgeRange(Points, point, margin);
    }

    private static bool Inside(List<Vector2d> points, GMLGeometry.GeometryType type, Vector2d point)
    {
        var meters = GM.LatLonToMeters(point);

        var inside = false;

        switch (type)
        {
            case GeometryType.Point:
                break;
            case GeometryType.LineString:
                break;
            case GeometryType.Polygon:
                var originPoints = points.ConvertAll(p => GM.LatLonToMeters(p) - meters);
                for (int i = 0; i < originPoints.Count; i++)
                {
                    if (((originPoints[i].y > 0) != (originPoints[(i + 1) % originPoints.Count].y > 0))
                    && ((originPoints[i].y > 0) == (originPoints[i].y * originPoints[(i + 1) % originPoints.Count].x > originPoints[(i + 1) % originPoints.Count].y * originPoints[i].x)))
                        inside = !inside;
                }
                break;
            default:
                break;
        }

        

        return inside;
    }

    private static bool InsidePointRadius(List<Vector2d> points, Vector2d point, float radius)
    {
        return points.Any(p => (GM.SeparationInMeters(p, point) <= radius));
    }

    private static bool InsideEdgeRange(List<Vector2d> points, Vector2d point, float radius)
    {
        if (points.Count <= 1) // If there are not edges, no sense to do it...
            return false;

        Vector2d closestPoin = Vector2d.zero;
        int l = points.Count - 1;
        for (int i = 0; i < points.Count; i++)
        {
            closestPoin = GetClosestPointOnLineSegment(points[l], points[i], point);
            //Debug.Log(GM.SeparationInMeters(closestPoin, point));
            l = i;
            if (GM.SeparationInMeters(closestPoin, point) <= radius)
                return true;
        }
        return false;
    }

    //http://stackoverflow.com/questions/3120357/get-closest-point-to-a-line
    public static Vector2d GetClosestPointOnLineSegment(Vector2d A, Vector2d B, Vector2d P)
    {
        Vector2d AP = P - A;       //Vector from A to P   
        Vector2d AB = B - A;       //Vector from A to B  

        double magnitudeAB = AB.sqrMagnitude;     //Magnitude of AB vector (it's length squared)     
        double ABAPproduct = Vector2d.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        double distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            return A;
        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + AB * distance;
        }
    }

    public bool InsideInfluence(Vector2d point)
    {
        return InsideInfluence(point, 0);
    }


    public bool InsideInfluence(Vector2d point, float extraMargin)
    {
        return InsideMargin(point, Influence + extraMargin);
    }

    public object Clone()
    {
        var clone = this.MemberwiseClone() as GMLGeometry;
        clone.Points = Points.ToList();
        clone.Conditions = Conditions.Clone() as Conditions;
        return clone;
    }

    public Vector2d Center {
        get
        {
            if (Points.Count == 0)
                return Vector2d.zero;
            else if (Points.Count == 1)
                return Points[0];

            return Points.Aggregate(new Vector2d(), (p, n) => p + n) / Points.Count;
        }
    }
}
