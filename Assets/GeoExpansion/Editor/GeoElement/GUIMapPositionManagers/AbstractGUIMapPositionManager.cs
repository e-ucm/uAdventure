using System;
using System.Linq;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Geo
{
    public abstract class AbstractGuiMapPositionManager : IGuiMapPositionManager
    {
        protected TransformManagerDataControl transformManagerDataControl;
        protected IDisposable updateNotifier;

        public abstract Type ForType { get; }

        public Texture2D Texture { get; set; }

        public void Configure(TransformManagerDataControl transformManagerDataControl)
        {
            var previousPositionManager = transformManagerDataControl.GUIMapPositionManager as AbstractGuiMapPositionManager;

            this.transformManagerDataControl = transformManagerDataControl;

            if (previousPositionManager != null && MapEditor.Current != null)
            {
                var rect = new Rect(-500, -1000, 1000, 1000);
                var screenRect = rect.ToPoints().Select(p => previousPositionManager.ToScreenPoint(MapEditor.Current, p)).ToArray().ToRect();

                GuiMapPositionManagerUtility.InsertDefaults(transformManagerDataControl.ParameterDescription, transformManagerDataControl, true);
                UpdateValues();
                previousPositionManager.updateNotifier.Dispose();
                var previousScreenRect = rect.ToPoints().Select(p => ToScreenPoint(MapEditor.Current, p)).ToArray().ToRect();
                OnRectChanged(MapEditor.Current, previousScreenRect, screenRect);
            }
            else
            {
                GuiMapPositionManagerUtility.InsertDefaults(transformManagerDataControl.ParameterDescription, transformManagerDataControl, false);
                UpdateValues();
            }

            updateNotifier = this.transformManagerDataControl.Subscribe(this);
        }

        #region IDisposable implementation

        public void OnCompleted() { /* Not needed */}

        public void OnError(Exception error) { /* Not needed */}
        
        public void OnNext(DataControl value)
        {
            // When the transform manager is changed we get a OnNext so we update the values
            UpdateValues();
        }

        #endregion

        protected abstract void UpdateValues();

        public abstract Vector2 ToScreenPoint(MapEditor mapEditor, Vector2 point);

        public abstract Vector2 FromScreenPoint(MapEditor mapEditor, Vector2 point);

        public virtual bool IsGizmosSelected(MapEditor mapEditor, Vector2[] points)
        {
            var screenPoints = points.Select(p => ToScreenPoint(mapEditor, p)).ToArray();
            var selected = false;

            if (GUIUtility.hotControl == 0)
            {
                var rectContains = screenPoints.ToRect().Contains(Event.current.mousePosition);
                var anyHandleContains = screenPoints.Any(p => (p - Event.current.mousePosition).magnitude <= 10f);

                selected = rectContains || anyHandleContains;
            }

            return selected;
        }

        public virtual void OnDrawingGizmosSelected(MapEditor mapEditor, Vector2[] points)
        {
            var screenPoints = points.Select(p => ToScreenPoint(mapEditor, p)).ToArray();
            var rect = screenPoints.ToRect();

            var id = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            EditorGUI.BeginChangeCheck();
            var newRect = HandleUtil.HandleFixedRatioRect(id, rect, rect.width / rect.height, 10f,
                (polygon, over, active) => HandleUtil.DrawPolyLine(polygon, true, Color.red, over || active ? 4f : 2f),
                (point, over, active) => HandleUtil.DrawPoint(point, 4.5f, Color.blue, over || active ? 2f: 1f, MapEditor.GetColor(over || active ? Color.red: Color.black)));

            if (EditorGUI.EndChangeCheck())
            {
                OnRectChanged(mapEditor, rect, newRect);
            }

            var movementId = GUIUtility.GetControlID(GetHashCode() + 1, FocusType.Passive);
            EditorGUI.BeginChangeCheck();
            var rectMoved = HandleUtil.HandleRectMovement(movementId, rect);
            if (EditorGUI.EndChangeCheck())
            {
                OnRectChanged(mapEditor, rect, rectMoved);
            }
        }


        /// <summary>
        /// Use this method to manage the parameters when changing the screen rect.
        /// By default this mehtod will change the scale when called, so make sure you add "base.OnRectChanged" to your
        /// implementation if you want the scale to be automatically handled, or do not otherwise.
        /// </summary>
        /// <param name="mapEditor"></param>
        /// <param name="previousScreenRect"></param>
        /// <param name="newScreenRect"></param>
        protected virtual void OnRectChanged(MapEditor mapEditor, Rect previousScreenRect, Rect newScreenRect)
        {
            // In on rect changed we manage the scale, as most positioners will store and manage the scale the same way
            // However, this behavior can be avoided or overriten as this method is virtual

            var previousLocalRect = previousScreenRect.ToPoints().Select(p => FromScreenPoint(mapEditor, p)).ToArray().ToRect();
            var localRect = newScreenRect.ToPoints().Select(p => FromScreenPoint(mapEditor, p)).ToArray().ToRect();

            var scale = transformManagerDataControl.Scale;

            var originalWidth = previousLocalRect.width / scale;
            var newScale = localRect.width / originalWidth;

            // And then we set the values in the reference
            if (!Mathf.Approximately(scale, newScale))
            {
                transformManagerDataControl.Scale = newScale;
            }
        }

        public Vector2d ToScreenPoint(MapEditor mapEditor, Vector2d point)
        {
            return ToScreenPoint(mapEditor, point.ToVector2()).ToVector2d();
        }

        public Vector2d FromScreenPoint(MapEditor mapEditor, Vector2d point)
        {
            return FromScreenPoint(mapEditor, point.ToVector2()).ToVector2d();
        }

        public bool IsGizmosSelected(MapEditor mapEditor, Vector2d[] points)
        {
            return IsGizmosSelected(mapEditor, points.Select(p => p.ToVector2()).ToArray());
        }

        public void OnDrawingGizmosSelected(MapEditor mapEditor, Vector2d[] points)
        {
            OnDrawingGizmosSelected(mapEditor, points.Select(p => p.ToVector2()).ToArray());
        }
    }
}
