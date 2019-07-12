using System.Linq;
using uAdventure.Core;
using UnityEngine;

using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using System.Collections.Generic;

/// <summary>
/// The ExtensionRect class hods up multiple util operations for rectangles.
/// </summary>
public static class ExtensionRect
{
    /// <summary>
    /// Traps a rect inside another rect. If its ouside it moves it to the closest point inside
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static Rect TrapInside(this Rect rect, Rect other)
    {
        return new Rect(
            Mathf.Clamp(rect.x, other.x, other.x + other.width - rect.width),
            Mathf.Clamp(rect.y, other.y, other.y + other.height - rect.height),
            rect.width, rect.height);
    }



    public static Rect KeepInside(this Rect rect, Rect container)
    {
        // First we take the max of both top-left corners so we guarantee that  we do not go out of the top-left,
        // then, we take the min of this (max) and the bottom-right corner minus the rect size.
        var sizeDiff = Vector2.Min(rect.size, (rect.size + container.size) / 2f);
        var position = Vector2.Min(Vector2.Max(rect.position, container.position), container.position + container.size - sizeDiff);

        return new Rect(position, rect.size);
    }

    public static Vector2[] ToPoints(this Rect rect)
    {
        return new Vector2[] {
                rect.position,
                rect.position + new Vector2(rect.width, 0),
                rect.position + rect.size,
                rect.position + new Vector2(0, rect.height)};
    }

    public static Vector3[] ToPoints3(this Rect rect)
    {
        return new Vector3[] {
            rect.position,
            rect.position + new Vector2(rect.width, 0),
            rect.position + rect.size,
            rect.position + new Vector2(0, rect.height)};
    }

    public static Rect ToRect(this Vector2[] points)
    {
        var minX = points.Min(p => p.x);
        var minY = points.Min(p => p.y);
        var maxX = points.Max(p => p.x);
        var maxY = points.Max(p => p.y);

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    public static Vector2[] Extend(this Vector2[] points, float radius)
    {
        if (Mathf.Approximately(radius, 0))
            return points;

        Path polygon = points.ToList().ConvertAll(p => new IntPoint(p.x, p.y));

        Paths solution = new Paths();

        ClipperOffset c = new ClipperOffset();

        c.AddPath(polygon, JoinType.jtRound, EndType.etClosedPolygon);
        c.Execute(ref solution, radius);

        var r = solution.Count > 0 ? solution[0].ConvertAll(p => new Vector2(p.X, p.Y)) : new List<Vector2>();

        if (r.Count > 0)
            r.Add(r[0]);

        return r.ToArray();
    }


    public static bool InsideMargin(this Vector2d[] points, Vector2d point, float margin)
    {
        return points.Inside(point) || points.Any(p => p.InsideRadius(point, margin)) || points.InsideEdgeRange(point, margin, true);
    }
    public static bool InsideMargin(this Vector2[] points, Vector2 point, float margin)
    {
        return points.Inside(point) || points.Any(p => p.InsideRadius(point, margin)) || points.InsideEdgeRange(point, margin, true);
    }
    public static bool Inside(this Vector2[] points, Vector2 point)
    {
        bool inside = false;
        var originPoints = points.Select(p => p - point).ToList();
        for (int i = 0; i < originPoints.Count; i++)
        {
            if (((originPoints[i].y > 0) != (originPoints[(i + 1) % originPoints.Count].y > 0))
                && ((originPoints[i].y > 0) == (originPoints[i].y * originPoints[(i + 1) % originPoints.Count].x > originPoints[(i + 1) % originPoints.Count].y * originPoints[i].x)))
                inside = !inside;
        }

        return inside;
    }

    public static bool Inside(this Vector2d[] points, Vector2d point)
    {
        bool inside = false;
        var originPoints = points.Select(p => p - point).ToList();
        for (int i = 0; i < originPoints.Count; i++)
        {
            if (((originPoints[i].y > 0) != (originPoints[(i + 1) % originPoints.Count].y > 0))
            && ((originPoints[i].y > 0) == (originPoints[i].y * originPoints[(i + 1) % originPoints.Count].x > originPoints[(i + 1) % originPoints.Count].y * originPoints[i].x)))
                inside = !inside;
        }

        return inside;
    }

    public static bool InsideRadius(this Vector2 p, Vector2 point, float radius)
    {
        return (point - p).sqrMagnitude <= radius * radius;
    }

    public static bool InsideRadius(this Vector2d p, Vector2d point, double radius)
    {
        return (point - p).sqrMagnitude <= radius * radius;
    }

    public static bool InsideEdgeRange(this Vector2[] points, Vector2 point, float radius, bool closed)
    {
        if (points.Length <= 1)
        {
            // If there are not edges, no sense to do it...
            return false;
        }

        Vector2 closestPoin;
        int l = closed ? points.Length - 1 : 0;
        float sqrRadius = radius * radius;
        for (int i = closed ? 0 : 1, end = points.Length; i < end; ++i)
        {
            closestPoin = uAdventure.Runner.LineHandler.GetClosestPointOnLineSegment(points[l], points[i], point);
            l = i;
            if ((closestPoin - point).sqrMagnitude <= sqrRadius)
            {
                return true;
            }
        }
        return false;
    }

    public static bool InsideEdgeRange(this Vector2d[] points, Vector2d point, double radius, bool closed)
    {
        if (points.Length <= 1)
        {
            // If there are not edges, no sense to do it...
            return false;
        }

        Vector2d closestPoin;
        int l = closed ? points.Length - 1 : 0;
        double sqrRadius = radius * radius;
        for (int i = closed ? 0 : 1, end = points.Length; i < end; ++i)
        {
            closestPoin = uAdventure.Runner.LineHandler.GetClosestPointOnLineSegment(points[l], points[i], point);
            l = i;
            if ((closestPoin - point).sqrMagnitude <= sqrRadius)
            {
                return true;
            }
        }
        return false;
    }

    public static Rect AdjustToRatio(this Rect rect, float width, float height)
    {
        return rect.AdjustToRatio(width / height);
    }

    public static Rect AdjustToRatio(this Rect rect, float ratio)
    {
        var r = new Rect(rect);
        var originalCenter = r.center;

        var myRatio = r.width / r.height;
        if(myRatio > ratio) r.width = r.height * ratio;
        else r.height = r.width / ratio;

        r.center = originalCenter;
        return r;
    }

    public static Rect AdjustToViewport(this Rect rect, float originalWidth, float originalHeight, Rect viewport)
    {
        return new Rect(
                (rect.x / originalWidth) * viewport.width + viewport.x,
                (rect.y / originalHeight) * viewport.height + viewport.y,
                (rect.width / originalWidth) * viewport.width,
                (rect.height / originalHeight) * viewport.height
            );
    }

    public static Rect ViewportToScreen(this Rect rect, float screenWidth, float screenHeight, Rect viewport)
    {
        return new Rect(
                ((rect.x - viewport.x) / viewport.width) * screenWidth,
                ((rect.y - viewport.y) / viewport.height) * screenHeight,
                (rect.width / viewport.width) * screenWidth,
                (rect.height / viewport.height) * screenHeight
            );
    }

    public static Vector2 Base(this Rect rect)
    {
        return rect.position + new Vector2(rect.width * 0.5f, rect.height);
    }

    /// <summary>
    /// Performs the intersection of two Rects.
    /// </summary>
    /// <param name="rect">First rect</param>
    /// <param name="other">Second rect</param>
    /// <returns>Intersected rect</returns>
    public static Rect Intersection(this Rect rect, Rect other)
    {
        Vector2 r0o = rect.position,
            r0e = rect.position + rect.size,
            r1o = other.position,
            r1e = other.position + other.size;

        return FromCorners(
            new Vector2(Mathf.Max(r0o.x, r1o.x), Mathf.Max(r0o.y, r1o.y)),
            new Vector2(Mathf.Min(r0e.x, r1e.x), Mathf.Min(r0e.y, r1e.y)));

    }

    /// <summary>
    /// Transforms a rect to TexCoords using another rect that represents the drawing region.
    /// </summary>
    /// <param name="rect">Rect that occupies the whole texture</param>
    /// <param name="slice">Slice to get TexCoords of</param>
    /// <returns>TexCoords</returns>
    public static Rect ToTexCoords(this Rect rect, Rect slice)
    {
        return new Rect(
            (slice.x - rect.x) / rect.width,
            1f - (((slice.y + slice.height) - (rect.y + rect.height)) / rect.height),
            slice.width / rect.width,
            slice.height / rect.height
            );
    }

    /// <summary>
    /// Creates a rectangle using two points.
    /// </summary>
    /// <param name="o">Origin</param>
    /// <param name="e">End</param>
    /// <returns>New Rect</returns>
    public static Rect FromCorners(Vector2 o, Vector2 e)
    {
        return new Rect(o, e - o);
    }

    /// <summary>
    /// Moves a rectangle position the passed offset.
    /// </summary>
    /// <param name="target">Rect to move</param>
    /// <param name="move">Distance to move</param>
    /// <returns>Moved rect</returns>
    public static Rect Move(this Rect target, Vector2 move)
    {
        return new Rect(move.x + target.x, move.y + target.y, target.width, target.height);
    }

    /// <summary>
    /// Adapts a rect to the GUI, moving it from absolute coords, to relative and clapping the overflows.
    /// </summary>
    /// <param name="rect">The rect to move</param>
    /// <param name="guiSpace">The region of the parent component</param>
    /// <returns>Rect adapted to relative GUI space</returns>
    public static Rect GUIAdapt(this Rect rect, Rect guiSpace)
    {
        return rect.Move(guiSpace.position).Intersection(guiSpace);
    }

    /// <summary>
    /// Transforms a uAdventure Rect to a Unity Rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Rect ToRect(this Rectangle rect)
    {
        return new Rect(rect.getX(), rect.getY(), rect.getWidth(), rect.getHeight());
    }


    /// <summary>
    /// Check if the point is inside of the rect within a distance
    /// </summary>
    /// <param name="rect"></param>
    /// <returns>true if inside</returns>
    public static bool Contains(this Rectangle rect, Vector2 point, float margin = 0f)
    {
        if(Mathf.Approximately(margin, 0f) && rect.isRectangular())
        {
            return  rect.ToRect().Contains(point);
        }
        else
        {
            var points = rect.isRectangular() ? rect.ToRect().ToPoints() : rect.getPoints().ToArray();
            return points.InsideMargin(point, margin);
        }
    }

    public static Rectangle MoveArea(this Rectangle area, Vector2 point)
    {
        var r = new InfluenceArea();
        if (area.isRectangular())
        {
            r.setRectangular(true);
            r.setX(area.getX() + (int)point.x);
            r.setY(area.getY() + (int)point.y);
            r.setWidth(area.getWidth());
            r.setHeight(area.getHeight());
        }
        else
        {
            var moved = area.getPoints().ConvertAll(p => p + point);
            r.setRectangular(false);
            r.getPoints().Clear();
            r.getPoints().AddRange(moved);
        }

        return r;
    }

    public static Rectangle MoveAreaToTrajectory(this Rectangle area, uAdventure.Runner.TrajectoryHandler trajectory)
    {
        var points = area.isRectangular() ? area.ToRect().ToPoints() : area.getPoints().ToArray();

        var aa = new ActiveArea("aux", false, 0, 0, 0, 0);
        var pointsList = aa.getPoints();
        var center = points.Aggregate((v1, v2) => v1 + v2) / points.Length;
        var adjust = -center + trajectory.closestPoint(center);
        foreach (var p in points)
            pointsList.Add(p + adjust);

        return aa;
    }

    public static Rect ToRect(this RectInt rect)
    {
        return new Rect(rect.x, rect.y, rect.width, rect.height);
    }
    public static RectInt ToRectInt(this Rect rect)
    {
        return new RectInt(Mathf.RoundToInt(rect.x), Mathf.RoundToInt(rect.y), Mathf.RoundToInt(rect.width), Mathf.RoundToInt(rect.height));
    }

    public static bool IsSameRect(this RectInt rect, RectInt other)
    {
        return rect.position == other.position && rect.size == other.size;
    }


    public static RectD ToRectD(this Rect rect)
    {
        return new RectD(rect.position.ToVector2d(), rect.size.ToVector2d());
    }

    public static RectD ToRectD(this Vector2d[] points)
    {
        var minX = points.Min(p => p.x);
        var minY = points.Min(p => p.y);
        var maxX = points.Max(p => p.x);
        var maxY = points.Max(p => p.y);

        return new RectD(new Vector2d(minX, minY), new Vector2d(maxX - minX, maxY - minY));
    }
    public static Vector2d[] ToPoints(this RectD rect)
    {
        return new[]
        {
            rect.Min,
            rect.Min + new Vector2d(rect.Size.x, 0),
            rect.Min + rect.Size,
            rect.Min + new Vector2d(0, rect.Size.y)
        };
    }


    public static Rect[] Divide(this Rect r, int slices)
    {
        Rect[] rects = new Rect[slices];
        var sliceWidth = r.width / slices;
        rects[0] = new Rect(r.x, r.y, sliceWidth, r.height);
        for (int i = 1; i < slices; ++i)
        {
            rects[i] = rects[i - 1];
            rects[i].x += sliceWidth;
        }
        return rects;
    }

    public static Rect[,] Divide(this Rect r, int slices, int verticalSlices)
    {
        Rect[,] rects = new Rect[verticalSlices, slices];
        var verticals = DivideRectVertically(r, verticalSlices);
        for (int i = 0; i < verticals.Length; i++)
        {
            var horizontals = Divide(verticals[i], slices);
            for (int j = 0; j < slices; j++)
            {
                rects[i, j] = horizontals[j];
            }
        }
        return rects;
    }

    public static Rect[] DivideRectVertically(Rect r, int slices)
    {
        Rect[] rects = new Rect[slices];
        var sliceHeight = r.height / slices;
        rects[0] = new Rect(r.x, r.y, r.width, sliceHeight);
        for (int i = 1; i < slices; ++i)
        {
            rects[i] = rects[i - 1];
            rects[i].y += sliceHeight;
        }
        return rects;
    }
}
