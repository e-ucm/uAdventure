using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.GraphmapsWithMesh;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEngine;
using NUnit.Framework.Constraints;

namespace uAdventure.Geo
{
    public class GMLGeometryDataControl : DataControl
    {
        private readonly GMLGeometry gmlGeometry;
        private readonly ConditionsController conditionsController;

        public ConditionsController Conditions
        {
            get { return conditionsController; }
        }

        public RectD BoundingBox
        {
            get { return Points.Length == 0 ? new RectD(Vector2d.zero, Vector2d.zero) : Points.ToRectD(); }
        }

        public GMLGeometryDataControl(GMLGeometry gmlGeometry)
        {
            this.gmlGeometry = gmlGeometry;
            this.conditionsController = new ConditionsController(gmlGeometry.Conditions);
        }

        public Vector2d Center
        {
            get { return gmlGeometry.Center; }
        }

        public Vector2d[] Points
        {
            get { return gmlGeometry.Points; }
        }

        public float Influence
        {
            get
            {
                return gmlGeometry.Influence;
            }
            set
            {
                controller.AddTool(new ChangeFloatValueTool(gmlGeometry, value, "Influence"));
            }
        }


        public GMLGeometry.GeometryType Type
        {
            get
            {
                return gmlGeometry.Type;
            }

            set
            {
                controller.AddTool(new ChangeGeometryTypeTool(gmlGeometry, value));
            }
        }

        public bool AddPoint(Vector2d point)
        {
            return controller.AddTool(new AddRemovePointTool(gmlGeometry, point));
        }

        public bool InsertPoint(int index, Vector2d point)
        {
            return controller.AddTool(new AddRemovePointTool(gmlGeometry, index, point));
        }

        public bool RemovePoint(int index)
        {
            return controller.AddTool(new AddRemovePointTool(gmlGeometry, index));
        }

        public bool ModifyPoint(int index, Vector2d point)
        {
            return controller.AddTool(new ChangePointTool(gmlGeometry, index, point));
        }

        public bool Move(Vector2d delta)
        {
            return controller.AddTool(new MoveTool(gmlGeometry, delta));
        }

        private class MoveTool : Tool
        {
            private readonly GMLGeometry geometry;
            private Vector2d delta;

            public MoveTool(GMLGeometry geometry, Vector2d delta)
            {
                this.geometry = geometry;
                this.delta = delta;
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool canUndo()
            {
                return true;
            }

            public override bool combine(Tool other)
            {
                var otherChange = other as MoveTool;
                if (otherChange == null || otherChange.geometry != geometry)
                {
                    return false;
                }

                delta += otherChange.delta;

                return true;
            }

            public override bool doTool()
            {
                return Move(delta);
            }

            public override bool redoTool()
            {
                return Move(delta);
            }

            public override bool undoTool()
            {
                return Move(-delta);
            }

            private bool Move(Vector2d movement)
            {
                for (int i = 0, length = geometry.Points.Length; i < length; i++)
                {
                    geometry.Points[i] += movement;
                }

                return true;
            }
        }

        private class ChangePointTool : Tool
        {
            private Vector2d newValue;
            private readonly Vector2d oldValue;
            private readonly int index;
            private readonly GMLGeometry geometry;
            private readonly bool work = false;

            public ChangePointTool(GMLGeometry geometry, int index, Vector2d newValue)
            {
                this.geometry = geometry;
                this.index = index;
                this.newValue = newValue;
                if(index >= 0 && index < geometry.Points.Length)
                {
                    work = true;
                    this.oldValue = geometry.Points[index];
                }
            }

            public override bool canRedo()
            {
                return work;
            }

            public override bool canUndo()
            {
                return work;
            }

            public override bool combine(Tool other)
            {
                var otherChange = other as ChangePointTool;
                if (otherChange == null || !otherChange.work || otherChange.geometry != geometry ||
                    otherChange.index != index)
                {
                    return false;
                }

                newValue = otherChange.newValue;

                return true;
            }

            public override bool doTool()
            {
                return Set(newValue);
            }

            public override bool redoTool()
            {
                return Set(newValue);
            }

            public override bool undoTool()
            {
                return Set(oldValue);
            }

            private bool Set(Vector2d value)
            {
                if (!work)
                {
                    return false;
                }

                geometry.Points[index] = value;
                return true;
            }
        }

        private class ChangeGeometryTypeTool : Tool
        {
            private readonly GMLGeometry gmlGeometry;
            private readonly Vector2d[] previousPoints;
            private readonly GMLGeometry.GeometryType previousGeometryType;
            private GMLGeometry.GeometryType newGeometryType;

            public ChangeGeometryTypeTool(GMLGeometry gmlGeometry, GMLGeometry.GeometryType geometryType)
            {
                this.gmlGeometry = gmlGeometry;
                this.previousPoints = gmlGeometry.Points.ToArray();
                this.previousGeometryType = gmlGeometry.Type;
                this.newGeometryType = geometryType;
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool canUndo()
            {
                return true;
            }

            public override bool combine(Tool other)
            {
                var otherChange = other as ChangeGeometryTypeTool;
                if (otherChange == null || otherChange.gmlGeometry != gmlGeometry)
                {
                    return false;
                }

                newGeometryType = otherChange.newGeometryType;
                return true;
            }

            public override bool doTool()
            {
                if (newGeometryType == GMLGeometry.GeometryType.Point)
                {
                    gmlGeometry.Points = new [] { gmlGeometry.Center };
                }

                gmlGeometry.Type = newGeometryType;
                return true;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool undoTool()
            {
                gmlGeometry.Points = previousPoints;
                gmlGeometry.Type = previousGeometryType;
                return true;
            }
        }

        private class AddRemovePointTool : Editor.Tool
        {
            private readonly GMLGeometry gmlGeometry;

            private readonly bool add;

            private readonly int index;

            private readonly Vector2d point;

            private bool replaced = false;

            private Vector2d pointReplaced;

            public AddRemovePointTool(GMLGeometry gmlGeometry, Vector2d point)
            {
                this.gmlGeometry = gmlGeometry;
                this.add = true;
                this.index = GetIndex(point);
                this.point = point;
            }

            public AddRemovePointTool(GMLGeometry gmlGeometry, int index)
            {
                this.gmlGeometry = gmlGeometry;
                this.add = false;
                this.index = index;
                this.point = gmlGeometry.Points[index];
            }

            public AddRemovePointTool(GMLGeometry gmlGeometry, int index, Vector2d point)
            {
                this.gmlGeometry = gmlGeometry;
                this.add = true;
                this.index = index;
                this.point = point;
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool canUndo()
            {
                return true;
            }

            public override bool combine(Tool other)
            {
                return false;
            }

            public override bool doTool()
            {
                return add ? Add() : Remove();
            }

            public override bool redoTool()
            {
                return add ? Add() : Remove();
            }

            public override bool undoTool()
            {
                return add ? Remove() : Add();
            }

            private bool Add()
            {
                if (gmlGeometry.Type == GMLGeometry.GeometryType.Point && gmlGeometry.Points.Length == 1)
                {
                    pointReplaced = gmlGeometry.Points[0];
                    replaced = true;
                    gmlGeometry.Points[0] = point;
                }
                else
                {
                    var ps = gmlGeometry.Points.ToList();
                    ps.Insert(index, point);
                    gmlGeometry.Points = ps.ToArray();
                }

                return true;
            }
            private bool Remove()
            {
                if (replaced)
                {
                    gmlGeometry.Points[0] = pointReplaced;
                }
                else
                {
                    var ps = gmlGeometry.Points.ToList();
                    ps.RemoveAt(index);
                    gmlGeometry.Points = ps.ToArray();
                }

                return true;
            }

            private int FindInsertPos(Vector2[] points, Vector2 point)
            {
                if (points.Length == 0)
                {
                    return 0;
                }

                var insertIn = 0;
                var min = float.MaxValue;
                for (int i = 0; i < points.Length; i++)
                {
                    var dist = UnityEditor.HandleUtility.DistancePointToLineSegment(point, points[i], points[(i + 1) % points.Length]);
                    if (dist < min)
                    {
                        min = dist;
                        insertIn = i;
                    }
                }
                return insertIn + 1;
            }

            public int GetIndex(Vector2d point)
            {
                switch (gmlGeometry.Type)
                {
                    case GMLGeometry.GeometryType.LineString:
                        return gmlGeometry.Points.Length;
                    case GMLGeometry.GeometryType.Polygon:
                        if (gmlGeometry.Points.Length <= 1)
                        {
                            return gmlGeometry.Points.Length;
                        }
                        else
                        {
                            var ps = gmlGeometry.Points.ToList().ConvertAll(p => p.ToVector2()).ToArray();
                            return FindInsertPos(ps, point.ToVector2());
                            // Find the closest index
                            /*var min = ps.Min(p => (p - point).magnitude);
                            var closest = ps.FindIndex(p => (p - point).magnitude == min);

                            // Fix the previous and next
                            var prev = closest == 0 ? ps.Count - 1 : closest - 1;
                            var next = (closest + 1) % ps.Count;
                            // Calculate the normal to both adjacent axis to closest point
                            var c = ps[closest];
                            var v1 = (ps[closest] - ps[prev]).normalized;
                            var v2 = (ps[closest] - ps[next]).normalized;

                            var closestNormal = (v1 + v2).normalized;
                            var convex = Vector3.Cross(v1.ToVector2(), v2.ToVector2()).z > 0;

                            var pointVector = (point - c);
                            var left = Vector3.Cross(closestNormal.ToVector2(), pointVector.ToVector2()).z > 0;

                            Debug.Log(convex ? "Convex" : "Concave");
                            if ((left && convex) || (!left && !convex))
                            {
                                Debug.Log("Prev");
                                // We insert at the closest
                                return closest;
                            }
                            else
                            {
                                Debug.Log("Next");
                                // We insert at the next
                                return next;
                            }*/
                        }
                }
                return 0;
            }
        }

        public override bool addElement(int type, string id)
        {
            return false;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return true;
        }

        public override bool canBeMoved()
        {
            return true;
        }

        public override bool canBeRenamed()
        {
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            return 0;
        }

        public override int countIdentifierReferences(string id)
        {
            return 0;
        }

        public override void deleteAssetReferences(string assetPath)
        {
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return false;
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
        }

        public override object getContent()
        {
            return gmlGeometry;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return gmlGeometry.Points.Any();
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return false;
        }

        public override void recursiveSearch()
        {
            check(Name, "Name");
        }

        public string Name
        {
            get { return gmlGeometry.Name; }
            set { renameElement(value); }
        }

        public override string renameElement(string newName)
        {
            if (controller.AddTool(new ChangeNameTool(gmlGeometry, newName)))
            {
                return newName;
            }

            return gmlGeometry.Name;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            ConditionsController.updateVarFlagSummary(varFlagSummary, conditionsController.Conditions);
        }
    }
}