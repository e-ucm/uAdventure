using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(RectangleArea), Name = "Area", Order = 0)]
    public class RectangleComponentEditor : AbstractEditorComponentWithPreview
    {

        private string[] types;
        private GUIContent[] actions;

        private int ActionSelected = 0;

        public RectangleComponentEditor(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            types = new string[] { "Rectangle", "Area" };
            actions = new GUIContent[] { new GUIContent("Move"), new GUIContent("Add"), new GUIContent("Remove") };
        }

        protected override void DrawInspector()
        {
            var rectangleArea = Target as RectangleArea;

            EditorGUI.BeginChangeCheck();
            var selected = GUILayout.Toolbar(rectangleArea.isRectangular() ? 0 : 1, types);

            // If we change the area type
            if (EditorGUI.EndChangeCheck())
            {
                if (!rectangleArea.isRectangular())
                {
                    // If it becomes rectangular, we turn the area into the circunscribed rectangle
                    var surroundingRectangle = GetRect(rectangleArea.getPoints().ToArray());
                    rectangleArea.getRectangle().setValues(
                        Mathf.RoundToInt(surroundingRectangle.x),
                        Mathf.RoundToInt(surroundingRectangle.y),
                        Mathf.RoundToInt(surroundingRectangle.width),
                        Mathf.RoundToInt(surroundingRectangle.height));
                    rectangleArea.setRectangular(true);
                    ActionSelected = 0;
                }
                else
                {
                    // If it becomes an area, we turn the rectangle into its separated points
                    var rectanglePoints = GetPoints(GetRect(rectangleArea.getRectangle()));
                    rectangleArea.getPoints().Clear();
                    rectangleArea.getPoints().AddRange(rectanglePoints);
                    rectangleArea.setRectangular(false);
                }
            }

            if (selected == 0) // Rectangular
            {
                var rectangle = rectangleArea.getRectangle();

                EditorGUI.BeginChangeCheck();
                // Position
                var newPosition = EditorGUILayout.Vector2Field("Position", new Vector2(rectangle.getX(), rectangle.getY()));
                // Size
                var newSize = EditorGUILayout.Vector2Field("size", new Vector2(rectangle.getWidth(), rectangle.getHeight()));
                if (EditorGUI.EndChangeCheck())
                {
                    rectangle.setValues(
                        Mathf.RoundToInt(newPosition.x),
                        Mathf.RoundToInt(newPosition.y),
                        Mathf.RoundToInt(newSize.x),
                        Mathf.RoundToInt(newSize.y));
                }

            }
            else // Area
            {
                GUILayout.Label("Points: " + rectangleArea.getPoints().Count);
                ActionSelected = GUILayout.Toolbar(ActionSelected, actions);
            }
        }

        public override bool Update()
        {
            if(Event.current.type == EventType.MouseDown)
            {
                var rectangleArea = Target as RectangleArea;
                var points = WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, 800f, 600f);
                return Inside(points, Event.current.mousePosition);
            }

            return false;
        }

        public override void OnDrawingGizmos()
        {
            Handles.BeginGUI();

            Color backgroundColor = Color.gray, lineColor = Color.black;
            if (Target is ActiveAreaDataControl)
            {
                backgroundColor = new Color(0f, 0.8f, 0f, 0.5f);
                lineColor = new Color(0f, 0.5f, 0f, 1f);
            }
            else if (Target is ExitDataControl)
            {
                backgroundColor = new Color(0.8f, 0f, 0f, 0.5f);
                lineColor = new Color(0.5f, 0f, 0f, 1f);
            }
            else if (Target is BarrierDataControl)
            {
                backgroundColor = new Color(0.8f, 0.8f, 0f, 0.5f);
                lineColor = new Color(0.5f, 0.5f, 0f, 1f);
            }

            var rectangleArea = Target as RectangleArea;
            if (Event.current.type == EventType.Repaint)
            {
                DrawArea(WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, 800f, 600f), lineColor, backgroundColor);
            }
            Handles.EndGUI();
        }

        public override void OnDrawingGizmosSelected()
        {
            Handles.BeginGUI();

            Color lineColor = Color.red;
            var rectangleArea = Target as RectangleArea;
            var points = WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, 800f, 600f);

            switch (Event.current.type)
            {
                case EventType.Repaint: DrawPolyLine(points, true, lineColor, 5f); break;
            }

            bool pointsChanged = false;

            int index = 1;
            int rectCornerChanged = -1;
            foreach (var point in points)
            {
                switch (Event.current.type)
                {
                    case EventType.Repaint: if (rectangleArea.isRectangular()) DrawSquare(point, Color.yellow, Color.black); else DrawPoint(point, Color.cyan, Color.black); break;
                    case EventType.MouseDown:
                        switch (ActionSelected)
                        {
                            case 0:
                                if ((point - Event.current.mousePosition).magnitude < 10)
                                {
                                    GUIUtility.hotControl = rectangleArea.GetHashCode() + index;
                                    Event.current.Use();
                                }
                                break;
                            case 2:
                                if ((point - Event.current.mousePosition).magnitude < 10)
                                {
                                    rectangleArea.deletePoint(rectangleArea.getPoints()[index - 1]);
                                    Event.current.Use();
                                }
                                break;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == rectangleArea.GetHashCode() + index)
                        {
                            points[index - 1].x += Event.current.delta.x;
                            points[index - 1].y += Event.current.delta.y;
                            pointsChanged = true;
                            rectCornerChanged = index - 1;
                            Event.current.Use();
                        }
                        break;
                    case EventType.mouseUp:
                        if (GUIUtility.hotControl == rectangleArea.GetHashCode() + index)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;
                }
                index++;
            }


            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    switch (ActionSelected)
                    {
                        case 0:
                            if (Inside(points, Event.current.mousePosition))
                            {
                                GUIUtility.hotControl = rectangleArea.GetHashCode();
                                Event.current.Use();
                            }
                            break;
                        case 1:
                            var point = Event.current.mousePosition; //ViewportToWorld(Event.current.mousePosition, SceneEditor.Current.Viewport, new Vector2(800f, 600f));
                            
                            var pointlist = points.ToList();
                            pointlist.Insert(FindInsertPos(points, point), point);
                            points = pointlist.ToArray();

                            pointsChanged = true;
                            Event.current.Use();
                            break;
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == rectangleArea.GetHashCode())
                    {
                        points = points.Select(p => p + Event.current.delta).ToArray();
                        pointsChanged = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.mouseUp:
                    if (GUIUtility.hotControl == rectangleArea.GetHashCode())
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.repaint:
                    if(ActionSelected == 1)
                    {
                        var pos = FindInsertPos(points, Event.current.mousePosition);
                        Handles.DrawLine(points[(points.Length + pos - 1) % points.Length], Event.current.mousePosition);
                        Handles.DrawLine(Event.current.mousePosition, points[pos % points.Length]);
                    }

                    break;
            }


            if (pointsChanged)
            {
                this.Repaint();
                // Update points in the selection
                if (rectangleArea.isRectangular())
                {
                    switch (rectCornerChanged)
                    {
                        case 0: points[3].x += Event.current.delta.x; points[1].y += Event.current.delta.y; break;
                        case 1: points[2].x += Event.current.delta.x; points[0].y += Event.current.delta.y; break;
                        case 2: points[1].x += Event.current.delta.x; points[3].y += Event.current.delta.y; break;
                        case 3: points[0].x += Event.current.delta.x; points[2].y += Event.current.delta.y; break;
                    }

                    var rect = GetRect(ViewportToWorld(points, SceneEditor.Current.Viewport, 800f, 600f));
                    rectangleArea.getRectangle().setValues(
                        Mathf.RoundToInt(rect.x),
                        Mathf.RoundToInt(rect.y),
                        Mathf.RoundToInt(rect.width),
                        Mathf.RoundToInt(rect.height));
                }
                else
                {
                    rectangleArea.getPoints().Clear();
                    rectangleArea.getPoints().AddRange(ViewportToWorld(points, SceneEditor.Current.Viewport, 800f, 600f));
                }
            }

            Handles.EndGUI();
        }

        private int FindInsertPos(Vector2[] points, Vector2 point)
        {
            var insertIn = 0;
            var min = float.MaxValue;
            for (int i = 0; i < points.Length; i++)
            {
                var dist = HandleUtility.DistancePointToLineSegment(point, points[i], points[(i + 1) % points.Length]);
                if (dist < min)
                {
                    min = dist;
                    insertIn = i;
                }
            }
            return insertIn + 1;
        }

        private bool Inside(Vector2[] points, Vector2 mousePosition)
        {
            bool inside = false;
            var originPoints = points.Select(p => p - mousePosition).ToList();
            for (int i = 0; i < originPoints.Count; i++)
            {
                if (((originPoints[i].y > 0) != (originPoints[(i + 1) % originPoints.Count].y > 0))
                && ((originPoints[i].y > 0) == (originPoints[i].y * originPoints[(i + 1) % originPoints.Count].x > originPoints[(i + 1) % originPoints.Count].y * originPoints[i].x)))
                    inside = !inside;
            }

            return inside;
        }

        private Vector2[] GetPoints(RectangleArea rectangleArea)
        {
            Vector2[] points = new Vector2[0];
            if (rectangleArea.isRectangular())
            {
                Rect rect = GetRect(rectangleArea.getRectangle());
                points = GetPoints(rect);
            }
            else
            {
                points = rectangleArea.getPoints().ToArray();
            }

            return points;
        }

        private Vector2[] GetPoints(Rect rect)
        {
            return new Vector2[] {
                rect.position,
                rect.position + new Vector2(rect.width, 0),
                rect.position + rect.size,
                rect.position + new Vector2(0, rect.height)};
        }

        private Rect GetRect(Vector2[] points)
        {
            var minX = points.Min(p => p.x);
            var minY = points.Min(p => p.y);
            var maxX = points.Max(p => p.x);
            var maxY = points.Max(p => p.y);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private Rect GetRect(Rectangle rectangle)
        {
            return new Rect(new Vector2(rectangle.getX(), rectangle.getY()), new Vector2(rectangle.getWidth(), rectangle.getHeight()));
        }

        private void DrawArea(Vector2[] points, Color lineColor, Color backgroundColor)
        {
            DrawPolyLine(points, true, lineColor);
            DrawPolygon(points, backgroundColor);
        }

        private void DrawSelectedArea(Vector2[] points, Color lineColor, Color backgroundColor)
        {
            DrawPolyLine(points, true, lineColor, 5f);
        }

        private void DrawSquare(Vector2 position, Color innerColor, Color outerColor)
        {
            Handles.color = innerColor;
            Handles.DrawSolidRectangleWithOutline(new Rect(position.x - 5, position.y - 5, 10, 10), innerColor, outerColor);
        }

        private void DrawPoint(Vector2 position, Color innerColor, Color outerColor)
        {
            Handles.color = outerColor;
            Handles.DrawSolidDisc(position, Vector3.forward, 5.5f);
            Handles.color = innerColor;
            Handles.DrawSolidDisc(position, Vector3.forward, 4.5f);
        }

        private void DrawPolyLine(Vector2[] points, bool closed, Color color, float width = 2f)
        {
            Handles.color = color;
            Handles.DrawAAPolyLine(width, V2ToV3(points));
            if (closed)
                Handles.DrawAAPolyLine(width, V2ToV3(new Vector2[] { points[0], points[points.Length - 1] }));
        }

        private void DrawPolygon(Vector2[] points, Color color)
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

        private Vector3[] V2ToV3(Vector2[] points)
        {
            return points.Select(p => new Vector3(p.x, p.y, 0f)).ToArray();
        }

        

        private Vector2[] WorldToViewport(Vector2[] points, Rect viewport, float originalWidth, float originalHeight)
        {
            var screen = new Vector2(originalWidth, originalHeight);
            return points.Select(p => viewport.position + Multiply(Divide(p, screen), viewport.size)).ToArray();
        }

        private Vector2 ViewportToWorld(Vector2 point, Rect viewport, Vector2 screen)
        {
            return Multiply(Divide((point - viewport.position), viewport.size), screen);
        }

        private Vector2[] ViewportToWorld(Vector2[] points, Rect viewport, float originalWidth, float originalHeight)
        {
            var screen = new Vector2(originalWidth, originalHeight);
            return points.Select(p => ViewportToWorld(p, viewport, screen)).ToArray();
        }

        private Vector2 Divide(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }


        private Vector2 Multiply(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }
    }
}
