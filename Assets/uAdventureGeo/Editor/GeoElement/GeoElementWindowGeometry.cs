using MapzenGo.Helpers;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

using ClipperLib;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;
using System;
using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.ConjugateGradient;

namespace uAdventure.Geo
{
    [EditorComponent(typeof(GeoElementDataControl), Name = "Geo.GeoElementWindow.Geometry.Title", Order = 15)]
    public class GeoElementWindowGeometry : MapEditorWindow, EditorComponent
    {
        private readonly string[] actions = new[] { "Geo.GeoElementWindow.Geometry.Move", "Geo.GeoElementWindow.Geometry.Add", "Geo.GeoElementWindow.Geometry.Remove" }.Select(TC.get).ToArray();
        private readonly string[] typeNames;

        private GeoElementDataControl workingGeoElement;
        private DataControlList geometriesList;
        private int ActionSelected;
        private readonly Texture2D conditionsTex, noConditionsTex;

        public DataControl Target { get; set; }

        public bool Collapsed { get; set; }

        public GeoElementWindowGeometry(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, MapEditor mapEditor, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, mapEditor, aOptions)
        {
            PreviewTitle = "Item.Preview".Traslate();

            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

            typeNames = Enum.GetNames(typeof(GMLGeometry.GeometryType)).Select(n => TC.get("Geo.GeoElement.Geometry.Type." + n + ".Name")).ToArray();
            
            AbstractEditorComponent.RegisterComponent(this);

            geometriesList = new DataControlList
            {
                Columns = new List<ColumnList.Column>
                {
                    new ColumnList.Column
                    {
                        Text = "Name"
                    },
                    new ColumnList.Column
                    {
                        Text = "Type"
                    },
                    new ColumnList.Column
                    {
                        Text = "Conditions"
                    }
                },
                drawCell = (rect, row, column, active, focused) =>
                {
                    var geometry = this.geometriesList.list[row] as GMLGeometryDataControl;
                    switch (column)
                    {
                        case 0:
                            {
                                if (active)
                                {
                                    var indexRect = new Rect(rect.position, new Vector2(30, rect.height));
                                    var nameRect = new Rect(rect.position + Vector2.right * 30, new Vector2(rect.width - 30, rect.height));
                                    EditorGUI.BeginChangeCheck();
                                    GUI.Label(indexRect, (row + 1) + ":");
                                    var newName = GUI.TextField(nameRect, geometry.Name);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        geometry.Name = newName;
                                    }
                                }
                                else
                                {
                                    GUI.Label(rect, (row + 1) + ":" + geometry.Name);
                                }
                            }
                            break;
                        case 1:
                            {
                                GUI.Label(rect, TC.get("Geo.GeoElement.Geometry.Type." + geometry.Type + ".Name"));
                            }
                            break;
                        case 2:
                            if (GUI.Button(rect,
                                geometry.Conditions.getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                this.geometriesList.index = row;
                                var window = ScriptableObject.CreateInstance<ConditionEditorWindow>();
                                window.Init(geometry.Conditions);
                            }
                            break;
                    }
                },
                onSelectCallback = list =>
                {
                    if (list.index > -1)
                    {
                        workingGeoElement.SelectedGeometry = list.index;
                        Center(workingGeoElement.GMLGeometries[workingGeoElement.SelectedGeometry]);
                    }
                },
                onRemoveCallback = list =>
                {
                    if (workingGeoElement.SelectedGeometry > -1)
                    {
                        workingGeoElement.SelectedGeometry = Mathf.Max(0, workingGeoElement.SelectedGeometry - 1);
                        Center(workingGeoElement.GMLGeometries[workingGeoElement.SelectedGeometry]);
                        list.index = workingGeoElement.SelectedGeometry;
                    }
                }
            };
        }

        public override void Draw(int aID)
        {
            var previousSelected = componentBasedEditor.SelectedElement;
            componentBasedEditor.SelectedElement = GeoElement;
            base.Draw(aID);
            componentBasedEditor.SelectedElement = previousSelected;
        }

        protected override bool HasToDrawPreviewInspector()
        {
            return false;
        }

        protected override void DrawInspector()
        {
            var isInspector = Target != null;

            workingGeoElement = Target as GeoElementDataControl ?? GeoController.Instance.GeoElements.DataControls[GeoController.Instance.SelectedGeoElement];

            geometriesList.SetData(workingGeoElement.GMLGeometries, g => (g as ListDataControl<GeoElementDataControl, GMLGeometryDataControl>).DataControls.Cast<DataControl>().ToList());
            geometriesList.DoList(120);

            var geometry = workingGeoElement.GMLGeometries[workingGeoElement.SelectedGeometry];

            // Geometry type
            EditorGUI.BeginChangeCheck();
            GUILayout.Label("Geo.GeoElement.Geometry.Type.Title".Traslate());
            var newType = (GMLGeometry.GeometryType)GUILayout.Toolbar((int) geometry.Type, typeNames);
            if (EditorGUI.EndChangeCheck())
            {
                geometry.CurrentEditor = MapEditor.Current ?? componentBasedEditor;
                geometry.Type = newType;
            }

            // Geometry points
            EditorGUILayout.LabelField("Geo.GeoElement.Geometry.PointsCount".Traslate(geometry.Points.Length.ToString()));
            ActionSelected = GUILayout.Toolbar(ActionSelected, actions);

            // Geometry influence
            EditorGUI.BeginChangeCheck();
            var newInfluence = EditorGUILayout.FloatField("Geo.GeoElement.Geometry.Influence".Traslate(), geometry.Influence);
            if (EditorGUI.EndChangeCheck())
            {
                geometry.Influence = newInfluence;
            }
            
            if (GUILayout.Button("Geo.GeoElement.Geometry.Center".Traslate()) && geometry.Points.Length > 0)
            {
                Center(geometry);
            }
        }

        private void Center(GMLGeometryDataControl gmlGeometry)
        {
            var mapEditor = MapEditor.Current ?? componentBasedEditor;

            mapEditor.Center = gmlGeometry.Center;
            mapEditor.ZoomToBoundingBox(gmlGeometry.BoundingBox);
        }

        public bool Update()
        {
            if (Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                var geoElement = Target as GeoElementDataControl;
                var selectedElement = MapEditor.Current.SelectedElement as GeoElementDataControl;
                var geoElementRef = MapEditor.Current.SelectedElement as GeoElementRefDataControl;
                if (geoElementRef != null)
                {
                    selectedElement = geoElementRef.ReferencedDataControl as GeoElementDataControl;
                }

                if (selectedElement == geoElement && ActionSelected == 1)
                {
                    return true;
                }

                var geometry = geoElement.GMLGeometries[geoElement.SelectedGeometry];
                var points = MapEditor.Current.PixelsToRelative(MapEditor.Current.LatLonToPixels(geometry.Points));
                var mouseD = Event.current.mousePosition.ToVector2d();
                return points.Inside(mouseD) || points.ToList().FindIndex(p => (p - mouseD).magnitude <= 10f) != -1 || 
                    (geometry.Type == GMLGeometry.GeometryType.LineString && points.InsideEdgeRange(mouseD, 10.0, false));
            }

            return false;
        }

        private GeoElementDataControl GeoElement
        {
            get
            {
                return Target as GeoElementDataControl ?? GeoController.Instance.GeoElements.DataControls[GeoController.Instance.SelectedGeoElement];
            }
        }

        public EditorComponentAttribute Attribute
        {
            get { return AbstractEditorComponent.GetAttribute(GetType()); }
        }
         

        void EditorComponent.DrawInspector()
        {
            DrawInspector();
        }

        public void OnRender() { /* Not needed */ }

        public void OnPreRender() { /* Not needed */ }

        public void OnPostRender() { /* Not needed */ }

        public void OnDrawingGizmos()
        {
            var geoElement = Target as GeoElementDataControl ?? GeoElement;
            var geometry = geoElement.GMLGeometries[geoElement.SelectedGeometry];
            if (geometry == null || geometry.Points.Length == 0)
            {
                return;
            }

            var points = V2dToV2(MapEditor.Current.PixelsToRelative(MapEditor.Current.LatLonToPixels(geometry.Points)));
            

            switch (geometry.Type)
            {
                case GMLGeometry.GeometryType.Point:
                    HandleUtil.DrawPoint(points[0], 3, MapEditor.GetColor(Color.blue), MapEditor.GetColor(Color.black));
                    break;

                case GMLGeometry.GeometryType.LineString:
                    HandleUtil.DrawPolyLine(points, false, MapEditor.GetColor(Color.black));
                    break;

                case GMLGeometry.GeometryType.Polygon:
                    HandleUtil.DrawPolygon(points, MapEditor.GetColor(new Color(0.1f, 0.1f, 0.9f, 0.7f)));
                    break;
            }
            

            DrawInfluenceArea(points, MapEditor.Current.MetersToPixelsAt(geometry.Center, geometry.Influence), geometry.Type);
        }

        public void OnDrawingGizmosSelected()
        {
            var geoElement = Target as GeoElementDataControl ?? GeoElement;
            var geometry = geoElement.GMLGeometries[geoElement.SelectedGeometry];
            var points = V2dToV2(MapEditor.Current.PixelsToRelative(MapEditor.Current.LatLonToPixels(geometry.Points)));

            var geometryControlId = GUIUtility.GetControlID(geometry.GetHashCode(), FocusType.Passive);
            
            // Points movement
            for (int i = 0, r = -1; i < points.Length && r == -1; i++)
            {
                var pointControl = GUIUtility.GetControlID(geometry.GetHashCode() + i + 1, FocusType.Passive);
                switch (ActionSelected)
                {
                    case 0: // Move
                        EditorGUI.BeginChangeCheck();
                        var newPoint = HandleUtil.HandlePointMovement(pointControl, points[i], 10,
                            (point, isOver, isActive) => HandleUtil.DrawPoint(point, 4, MapEditor.GetColor(Color.cyan),
                                MapEditor.GetColor(Color.black)), MouseCursor.MoveArrow);
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            var moved = MapEditor.Current.PixelToLatLon(
                                MapEditor.Current.RelativeToAbsolute(newPoint.ToVector2d()));
                            geometry.ModifyPoint(i, moved);
                        }

                        break;
                    case 1:
                        {
                            HandleUtil.DrawSquare(points[i], 7, MapEditor.GetColor(new Color(0.1f,0.1f,0.7f,0.7f)),
                                MapEditor.GetColor(Color.blue));
                        }
                        break;
                    case 2: // Remove
                        var mouseUpAsButton = false;
                        HandleUtil.HandlePointMovement(pointControl, points[i], 5,
                            (point, isOver, isActive) =>
                            {
                                var size = isOver ? 2f : 1f;
                                var borderColor = isOver ? MapEditor.GetColor(Color.yellow) : MapEditor.GetColor(Color.black);
                                HandleUtil.DrawPoint(point, 4, MapEditor.GetColor(Color.red), size, borderColor); 
                            }, out mouseUpAsButton, MouseCursor.ArrowMinus);
                        if (mouseUpAsButton)
                        {
                            geometry.RemovePoint(i);
                            r = i;
                        }
                        break;
                }
            }

            // Points adding
            if (ActionSelected == 1)
            {
                EditorGUIUtility.AddCursorRect((MapEditor.Current ?? componentBasedEditor).ScreenRect,
                    MouseCursor.ArrowPlus);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    geometry.AddPoint(MapEditor.Current.GeoMousePosition);
                    Event.current.Use();
                    return;
                }
            }

            // Geometry movement
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (Event.current.button == 0 && (
                            (geometry.Type == GMLGeometry.GeometryType.Polygon && points.Inside(Event.current.mousePosition)) ||
                            (geometry.Type == GMLGeometry.GeometryType.LineString && points.InsideEdgeRange(Event.current.mousePosition, 10f, false))
                            ))
                    {
                        GUIUtility.hotControl = geometryControlId;
                        Event.current.Use();
                    }

                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == geometryControlId)
                    {
                        var center = points.Center();
                        var movedCenter = center + Event.current.delta;

                        var latLonOrigin = MapEditor.Current.PixelToLatLon(MapEditor.Current.RelativeToAbsolute(center.ToVector2d()));
                        var latLonDestination = MapEditor.Current.PixelToLatLon(MapEditor.Current.RelativeToAbsolute(movedCenter.ToVector2d()));

                        var latLonDelta = latLonDestination - latLonOrigin;
                        geometry.Move(latLonDelta);
                        Event.current.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == geometryControlId)
                    {
                        GUIUtility.hotControl = -1;
                    }

                    break;
            }
        }

        private void DrawInfluenceArea(Vector2[] points, float radius, GMLGeometry.GeometryType type)
        {
            if (points.Length == 1)
            {
                Handles.color = MapEditor.GetColor(Color.black);
                Handles.DrawWireArc(points[0], Vector3.back, Vector2.up, 360, radius);
                return;
            }

            HandleUtil.DrawPolyLine(ExtendPolygon(new List<Vector2>(points), radius, type).ToArray(), true, MapEditor.GetColor(Color.black));
        }

        private static Vector2[] V2dToV2(Vector2d[] points)
        {
            var r = new Vector2[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                r[i] = points[i].ToVector2();
            }

            return r;
        }

        private static List<Vector2> ExtendPolygon(List<Vector2> points, float radius, GMLGeometry.GeometryType type)
        {
            Path polygon = points.ConvertAll(p => new IntPoint(p.x, p.y));

            Paths solution = new Paths();

            ClipperOffset c = new ClipperOffset();

            if (points.Count <= 2)
            {
                type = GMLGeometry.GeometryType.LineString;
            }

            c.AddPath(polygon, JoinType.jtRound, type == GMLGeometry.GeometryType.Polygon ? EndType.etClosedPolygon : EndType.etOpenRound);
            c.Execute(ref solution, radius);

            var r = solution.Count > 0 ? solution[0].ConvertAll(p => new Vector2(p.X, p.Y)) : new List<Vector2>();

            if (r.Count > 0)
            {
                r.Add(r[0]);
            }

            return r;
        }
    }
}
