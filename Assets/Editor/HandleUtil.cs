using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class HandleUtil {


    public static Vector2 HandlePointMovement(int controlId, Vector2 point, object maxDistance, Action<Vector2> draw)
    {
        switch (Event.current.type)
        {
            case EventType.Repaint:  draw(point); break;
            case EventType.MouseDown:
                if ((point - Event.current.mousePosition).magnitude < 10)
                {
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlId)
                {
                    point.x += Event.current.delta.x;
                    point.y += Event.current.delta.y;
                    GUI.changed = true;
                    Event.current.Use();
                }
                break;
            case EventType.mouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
                break;
        }

        return point;
    }

    /// <summary>
    /// Handle a rect movement. Compatible con GUI.change.
    /// </summary>
    /// <param name="controlId"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Rect HandleRectMovement(int controlId, Rect rect)
    {
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (rect.Contains(Event.current.mousePosition) && GUIUtility.hotControl == 0)
                {
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
                break;
            case EventType.MouseMove:
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlId)
                {
                    rect = rect.Move(Event.current.delta);
                    GUI.changed = true;
                    Event.current.Use();
                }
                break;
        }

        return rect;
    }

    /// <summary>
    /// Draws a square with outline of the desired size
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="innerColor"></param>
    /// <param name="outerColor"></param>
    public static void DrawSquare(Vector2 position, float size, Color innerColor, Color outerColor)
    {
        Handles.color = innerColor;
        Handles.DrawSolidRectangleWithOutline(new Rect(position.x - (size/2f), position.y - (size/2f), size, size), innerColor, outerColor);
    }

    /// <summary>
    /// Draws a circle point of the desired size with outline
    /// </summary>
    /// <param name="position"></param>
    /// <param name="size"></param>
    /// <param name="innerColor"></param>
    /// <param name="outerColor"></param>
    public static void DrawPoint(Vector2 position, float size, Color innerColor, Color outerColor)
    {
        Handles.color = outerColor;
        Handles.DrawSolidDisc(position, Vector3.forward, size + 1f);
        Handles.color = innerColor;
        Handles.DrawSolidDisc(position, Vector3.forward, size);
    }

    /// <summary>
    /// Draws a polyline of the desired width and color
    /// </summary>
    /// <param name="points"></param>
    /// <param name="closed"></param>
    /// <param name="color"></param>
    /// <param name="width"></param>
    public static void DrawPolyLine(Vector2[] points, bool closed, Color color, float width = 2f)
    {
        Handles.color = color;
        Handles.DrawAAPolyLine(width, V2ToV3(points));
        if (closed)
            Handles.DrawAAPolyLine(width, V2ToV3(new Vector2[] { points[0], points[points.Length - 1] }));
    }

    /// <summary>
    /// Draws any polygon in the given color
    /// </summary>
    /// <param name="points"></param>
    /// <param name="color"></param>
    public static void DrawPolygon(Vector2[] points, Color color)
    {
        Triangulator2 tr = new Triangulator2(points);
        Handles.color = color;
        int[] indices = tr.Triangulate();
        Vector3[] triangle = new Vector3[3];
        var v3p = V2ToV3(points);
        for (int i = 2; i < indices.Length; i += 3)
        {
            triangle[0] = v3p[indices[i - 2]];
            triangle[1] = v3p[indices[i - 1]];
            triangle[2] = v3p[indices[i]];
            Handles.DrawAAConvexPolygon(triangle);
        }
    }

    /// <summary>
    /// Converts V2 to V3 points
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private static Vector3[] V2ToV3(Vector2[] points)
    {
        return points.Select(p => new Vector3(p.x, p.y, 0f)).ToArray();
    }
}
