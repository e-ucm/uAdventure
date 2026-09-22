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
                    var surroundingRectangle = rectangleArea.getPoints().ToArray().ToRect();
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
                    var rectanglePoints = GetRect(rectangleArea.getRectangle()).ToPoints();
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
                if (!rectangleArea.isRectangular() && ActionSelected == 1)
                {
                    return true;
                }
                var points = WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, SceneEditor.Current.Size.x, SceneEditor.Current.Size.y);
                return points.Inside(Event.current.mousePosition) || points.ToList().FindIndex(p => (p - Event.current.mousePosition).magnitude <= 10f) != -1;
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
                DrawArea(WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, SceneEditor.Current.Size.x, SceneEditor.Current.Size.y), lineColor, backgroundColor);
            }
            Handles.EndGUI();
        }

        public override void OnDrawingGizmosSelected()
        {
            Handles.BeginGUI();

            Color lineColor = Color.red;
            var rectangleArea = Target as RectangleArea;


            var points = WorldToViewport(GetPoints(rectangleArea), SceneEditor.Current.Viewport, SceneEditor.Current.Size.x, SceneEditor.Current.Size.y);

            switch (Event.current.type)
            {
                case EventType.Repaint: HandleUtil.DrawPolyLine(points, true, lineColor, 5f); break;
            }

            bool pointsChanged = false;

            int index = 1;
            int rectCornerChanged = -1;
            foreach (var point in points)
            {
                var pointControlId = GUIUtility.GetControlID("Rectangle Point".GetHashCode(), FocusType.Passive, new Rect(point, new Vector2(10,10)));
                switch (Event.current.GetTypeForControl(pointControlId))
                {
                    case EventType.Repaint: if (rectangleArea.isRectangular()) HandleUtil.DrawSquare(point, 10f, Color.yellow, Color.black); else HandleUtil.DrawPoint(point, 4.5f, Color.cyan, Color.black); break;
                    case EventType.MouseDown:
                        switch (ActionSelected)
                        {
                            case 0:
                                if (GUIUtility.hotControl == 0 && (point - Event.current.mousePosition).magnitude < 10)
                                {
                                    GUIUtility.hotControl = pointControlId;
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
                        if (GUIUtility.hotControl == pointControlId)
                        {
                            points[index - 1].x += Event.current.delta.x;
                            points[index - 1].y += Event.current.delta.y;
                            pointsChanged = true;
                            rectCornerChanged = index - 1;
                            Event.current.Use();
                        }
                        break;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == pointControlId)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;
                }
                index++;
            }

            var rectangleControlID = GUIUtility.GetControlID("Rectangle".GetHashCode(), FocusType.Passive);
            switch (Event.current.GetTypeForControl(rectangleControlID))
            {
                case EventType.MouseDown:
                    switch (ActionSelected)
                    {
                        case 0:
                            if (GUIUtility.hotControl == 0 && points.Inside(Event.current.mousePosition))
                            {
                                GUIUtility.hotControl = rectangleControlID;
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
                    if (GUIUtility.hotControl == rectangleControlID)
                    {
                        points = points.Select(p => p + Event.current.delta).ToArray();
                        pointsChanged = true;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == rectangleControlID)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;
                case EventType.Repaint:
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

                    var rect = ViewportToWorld(points, SceneEditor.Current.Viewport, SceneEditor.Current.Size.x, SceneEditor.Current.Size.y).ToRect();
                    rectangleArea.getRectangle().setValues(
                        Mathf.RoundToInt(rect.x),
                        Mathf.RoundToInt(rect.y),
                        Mathf.RoundToInt(rect.width),
                        Mathf.RoundToInt(rect.height));
                }
                else
                {
                    rectangleArea.getPoints().Clear();
                    rectangleArea.getPoints().AddRange(ViewportToWorld(points, SceneEditor.Current.Viewport, SceneEditor.Current.Size.x, SceneEditor.Current.Size.y));
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

        private Vector2[] GetPoints(RectangleArea rectangleArea)
        {
            Vector2[] points = new Vector2[0];
            if (rectangleArea.isRectangular())
            {
                Rect rect = GetRect(rectangleArea.getRectangle());
                points = rect.ToPoints();
            }
            else
            {
                points = rectangleArea.getPoints().ToArray();
            }

            return points;
        }

        private Rect GetRect(Rectangle rectangle)
        {
            return new Rect(new Vector2(rectangle.getX(), rectangle.getY()), new Vector2(rectangle.getWidth(), rectangle.getHeight()));
        }

        private void DrawArea(Vector2[] points, Color lineColor, Color backgroundColor)
        {
            HandleUtil.DrawPolyLine(points, true, lineColor);
            HandleUtil.DrawPolygon(points, backgroundColor);
        }

        private void DrawSelectedArea(Vector2[] points, Color lineColor, Color backgroundColor)
        {
            HandleUtil.DrawPolyLine(points, true, lineColor, 5f);
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
