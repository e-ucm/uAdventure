using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MapzenGo.Helpers;
using uAdventure.Core;
using uAdventure.Runner;

public class GMLGeometry : ICloneable, ICheckable, Named
{

    public enum GeometryType { Point, LineString, Polygon }

    public GMLGeometry()
    {
        Points = new Vector2d[0];
        Conditions = new Conditions();
    }

    public GeometryType Type { get; set; }
    public Vector2d[] Points { get; set; }
    public float Influence { get; set; }
    public Conditions Conditions { get; set; }
    public string Name { get; set; }

    
    public bool InsideMargin(Vector2d point, float margin)
    {
        return Inside(Points, Type, point) || InsidePointRadius(Points, point, margin) || InsideEdgeRange(Points, point, margin);
    }

    private static bool Inside(Vector2d[] points, GMLGeometry.GeometryType type, Vector2d point)
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
                var originPoints = new Vector2d[points.Length];

                for (int i = 0; i < points.Length; i++)
                {
                    originPoints[i] = GM.LatLonToMeters(points[i]) - meters;
                }

                for (int i = 0; i < originPoints.Length; i++)
                {
                    if (((originPoints[i].y > 0) != (originPoints[(i + 1) % originPoints.Length].y > 0))
                    && ((originPoints[i].y > 0) == (originPoints[i].y * originPoints[(i + 1) % originPoints.Length].x > originPoints[(i + 1) % originPoints.Length].y * originPoints[i].x)))
                        inside = !inside;
                }
                break;
            default:
                break;
        }

        

        return inside;
    }

    private static bool InsidePointRadius(Vector2d[] points, Vector2d point, float radius)
    {
        return points.Any(p => (GM.SeparationInMeters(p, point) <= radius));
    }

    private static bool InsideEdgeRange(Vector2d[] points, Vector2d point, float radius)
    {
        if (points.Length <= 1)
        {
            // If there are not edges, no sense to do it...
            return false;
        }

        Vector2d closestPoin;
        int l = points.Length - 1;
        for (int i = 0; i < points.Length; i++)
        {
            closestPoin = GetClosestPointOnLineSegment(points[l], points[i], point);
            l = i;
            if (GM.SeparationInMeters(closestPoin, point) <= radius)
            {
                return true;
            }
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

    public int GetClosestSegment(Vector2d point)
    {
        if (this.Type == GeometryType.Polygon || this.Type == GeometryType.Point)
        {
            throw new NotImplementedException();
        }

        double distance = Mathf.Infinity;
        int index = -1;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var closestPoint = GetClosestPointOnLineSegment(Points[i], Points[i + 1], point);
            var pointdistance = (closestPoint - point).sqrMagnitude;
            if (distance > pointdistance)
            {
                distance = pointdistance;
                index = i;
            }
        }
        return index;
    }

    public Vector2d GetClosestPoint(Vector2d point)
    {
        if (this.Type == GeometryType.Polygon && InsideMargin(point, 0))
        {
            return point;
        }

        switch (this.Type)
        {
            case GeometryType.Point:
                return point;

            case GeometryType.LineString:
            case GeometryType.Polygon:
                Vector2d closest = Vector2d.zero;
                double distance = Mathf.Infinity;
                for (int i = 0; i < Points.Length - 1; i++)
                {
                    var closestPoint = GetClosestPointOnLineSegment(Points[i], Points[i+1], point);
                    var pointdistance = (closestPoint - point).sqrMagnitude;
                    if (distance > pointdistance)
                    {
                        distance = pointdistance;
                        closest = closestPoint;
                    }
                }
                if(closest == Vector2d.zero)
                {
                    return point;
                }
                else
                {
                    return closest;
                }
        }

        return point;
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
        clone.Points = Points;
        clone.Conditions = Conditions.Clone() as Conditions;
        return clone;
    }

    public void setName(string name)
    {
        this.Name = name;
    }

    public string getName()
    {
        return Name;
    }

    public Vector2d Center {
        get
        {
            if (Points.Length == 0)
            {
                return Vector2d.zero;
            }
            else if (Points.Length == 1)
            {
                return Points[0];
            }

            return Points.Aggregate(new Vector2d(), (p, n) => p + n) / Points.Length;
        }
    }
}
