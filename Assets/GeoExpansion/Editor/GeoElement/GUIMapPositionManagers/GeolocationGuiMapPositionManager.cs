using System;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;
using MapzenGo.Helpers;

namespace uAdventure.Geo
{
    public class GeolocationGuiMapPositionManager : AbstractGuiMapPositionManager
    {
        private Vector2d position;
        private float scale;
        private float rotation;
        private float interactionRange;
        private Texture2D particle;

        public override Type ForType { get { return typeof(GeolocationTransformManager); } }

        protected override void UpdateValues()
        {
            var parameters = transformManagerDataControl;

            position = parameters.GetValue(position, new[]
            {
                Converters<Vector2>.Create(v2 => v2.ToVector2d())
            }, "Position", "position");

            rotation = parameters.GetValue(rotation, "Rotation", "rotation");
            interactionRange = parameters.GetValue(interactionRange, "InteractionRange", "interactionRange");

            particle = Controller.ResourceManager.getImage(parameters["RevealParticleTexture"] as string);

            scale = parameters.Scale;
        }

        public override void OnDrawingGizmosSelected(MapEditor mapEditor, Vector2[] points)
        {
            base.OnDrawingGizmosSelected(mapEditor, points);

            var center = points.Select(p => ToScreenPoint(mapEditor, p)).Center();

            Handles.BeginGUI();

            var influencePixels = mapEditor.MetersToPixelsAt(position, interactionRange);

            Handles.color = Color.black;
            Handles.DrawWireArc(center, Vector3.back, Vector2.up, 360, influencePixels);
            Handles.EndGUI();
        }


        protected override void OnRectChanged(MapEditor mapEditor, Rect previousScreenRect, Rect newScreenRect)
        {
            base.OnRectChanged(mapEditor, previousScreenRect, newScreenRect);
            var rectBase = new Vector2(Mathf.RoundToInt(newScreenRect.x + 0.5f * newScreenRect.width), Mathf.RoundToInt(newScreenRect.y + newScreenRect.height));
            transformManagerDataControl["Position"] = mapEditor.PixelToLatLon(mapEditor.RelativeToAbsolute(rectBase.ToVector2d()));
        }

        public override Vector2 ToScreenPoint(MapEditor mapEditor, Vector2 point)
        {
            // Scale of 1 means 1 pixel is 1 map pixel in scale 19
            // Scale of 2 means 1 pixel is 0.5 map pixel in scale 18
            // and so on...

            var pixelScaleAt = GM.GetPixelsPerMeter(position.x, 19) / GM.GetPixelsPerMeter(0, 19);
            var pixelScale = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(scale, scale), 19), mapEditor.Zoom) * pixelScaleAt;

            var center = mapEditor.PixelToRelative(GM.MetersToPixels(GM.LatLonToMeters(position.x, position.y), mapEditor.Zoom)).ToVector2();
            return center + new Vector2((float)(point.x * pixelScale.x), (float)(point.y * pixelScale.y));
        }

        public override Vector2 FromScreenPoint(MapEditor mapEditor, Vector2 point)
        {

            var pixelScaleAt = GM.GetPixelsPerMeter(position.x, 19) / GM.GetPixelsPerMeter(0, 19);
            // First we get the scale of the pixels based on the current zoom
            var pixelScale = GM.MetersToPixels(GM.PixelsToMeters(new Vector2d(scale, scale), 19), mapEditor.Zoom) * pixelScaleAt;
            // Then we calculate the center
            var center = mapEditor.PixelToRelative(GM.MetersToPixels(GM.LatLonToMeters(position.x, position.y), mapEditor.Zoom)).ToVector2();
            // We make the point relative to center
            point -= center;
            // And finally we unscale the relative point
            return new Vector2(point.x / (float)pixelScale.x, point.y / (float)pixelScale.y);
        }
    }
}
