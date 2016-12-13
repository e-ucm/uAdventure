using UnityEngine;
using System.Collections.Generic;

public class GMLGeometry
{

    public enum GeometryType { Point, LineString, Polygon }

    public GMLGeometry()
    {
        Points = new List<Vector2d>();
    }

    public GeometryType Type { get; set; }
    public List<Vector2d> Points { get; set; }
}
