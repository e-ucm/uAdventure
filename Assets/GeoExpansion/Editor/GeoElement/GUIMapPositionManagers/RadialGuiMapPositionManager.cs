using MapzenGo.Helpers;
using System;
using NUnit.Framework;
using uAdventure.Editor;
using UnityEngine;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    public class RadialGuiMapPositionManager : AbstractGuiMapPositionManager
    {
        private float degree;
        private float distance;
        private Vector3 scale;
        private float rotation;
        private bool rotateAround;

        public override Type ForType { get { return typeof(RadialTransformManager); } }

        protected override void UpdateValues()
        {
            var parameters = transformManagerDataControl;

            degree = parameters.GetValue(degree, "Degree", "degree");
            distance = parameters.GetValue(distance, "Distance", "distance");
            scale = parameters.GetValue(scale, new[]
            {
                Converters<float>.Create(f => Vector3.one * f),
                Converters<Vector2>.Create(v2 => new Vector3(v2.x, v2.y, v2.x))
            }, "Scale", "scale");
            rotation = parameters.GetValue(rotation, "Rotation", "rotation");
            rotateAround = parameters.GetValue(rotateAround, "RotateAround", "rotateAround");
        }

        public override Vector2 ToScreenPoint(MapEditor mapEditor, Vector2 point)
        {
            var angle = this.degree * Mathf.Deg2Rad;

            var centerMeters = GM.LatLonToMeters(mapEditor.Center.x, mapEditor.Center.y);
            var metersPos = centerMeters + new Vector2d(distance * Mathd.Sin(angle), distance * Mathd.Cos(angle));
            var pixelsPos = GM.MetersToPixels(metersPos, mapEditor.Zoom);
            var basePixel = mapEditor.PixelToRelative(pixelsPos).ToVector2();

            var pixelScale = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(scale.x, scale.y), 19), mapEditor.Zoom);

            return basePixel + new Vector2((float)(point.x * pixelScale.x), (float)(point.y * pixelScale.y));

        }

        public override Vector2 FromScreenPoint(MapEditor mapEditor, Vector2 point)
        {
            var angle = this.degree * Mathf.Deg2Rad;

            var centerMeters = GM.LatLonToMeters(mapEditor.Center.x, mapEditor.Center.y);
            var metersPos = centerMeters + new Vector2d(distance * Mathd.Sin(angle), distance * Mathd.Cos(angle));
            var pixelsPos = GM.MetersToPixels(metersPos, mapEditor.Zoom);
            var basePixel = mapEditor.PixelToRelative(pixelsPos).ToVector2();

            var pixelScale = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(scale.x, scale.y), 19), mapEditor.Zoom);
            var relativePoint = point - basePixel;

            return new Vector2((float)(relativePoint.x / pixelScale.x), (float)(relativePoint.y / pixelScale.y));
        }

        protected override void OnRectChanged(MapEditor mapEditor, Rect previousScreenRect, Rect newScreenRect)
        {
            base.OnRectChanged(mapEditor, previousScreenRect, newScreenRect);

            var newPos = newScreenRect.Base().ToVector2d();
            var metersPos = GM.PixelsToMeters(mapEditor.RelativeToAbsolute(newPos), mapEditor.Zoom);
            var centerMeters = GM.LatLonToMeters(mapEditor.Center.x, mapEditor.Center.y);

            var metersVector = metersPos - centerMeters;
            
            float angle = Vector3.Angle(new Vector3(0.0f, 1.0f, 0.0f), new Vector3((float)metersVector.normalized.x, (float)metersVector.normalized.y, 0.0f));
            if (metersVector.x < 0.0f)
            {
                angle = 360.0f - angle;
            }

            transformManagerDataControl["Distance"] = (float)metersVector.magnitude;
            transformManagerDataControl["Degree"] = angle;
        }
    }
}
