using System;
using MapzenGo.Helpers;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class ScreenGuiMapPositionManager : AbstractGuiMapPositionManager
    {
        private const float gameWidth = 800f;
        private const float gameHeight = 600f;

        private Vector2 position;
        private float rotation;
        private float scale;

        public override Type ForType { get { return typeof(ScreenTransformManager); } }

        protected override void UpdateValues()
        {
            var parameters = transformManagerDataControl;

            position = parameters.GetValue(position, new[]
            {
                Converters<Vector2d>.Create(v2d => v2d.ToVector2())
            }, "Position", "position");

            rotation = parameters.GetValue(rotation, "Rotation", "rotation");

            scale = parameters.Scale;
        }

        public void Draw(GUIMap map, Rect area)
        {
            UpdateValues();
            var corner = area.position + new Vector2(position.x * area.width / gameWidth, area.height - (position.y * area.height / gameHeight));
            if (Texture != null)
            {
                var size = new Vector2(Texture.width * scale, Texture.height * scale);
                GUI.DrawTexture(new Rect(corner - new Vector2(size.x / 2f, size.y), size), Texture);
            }
        }

        protected override void OnRectChanged(MapEditor mapEditor, Rect previousScreenRect, Rect newScreenRect)
        {
            base.OnRectChanged(mapEditor, previousScreenRect, newScreenRect);

            var screen = mapEditor.ScreenRect;
            var newPosition = newScreenRect.Base() - screen.position;
            // We calculate the positioner center in the screen (from 0 to 1)
            var positionerCenter01 = new Vector2(newPosition.x / screen.width, newPosition.y / screen.height);
            // Then we calculate the positioner center pixel in the screen real position
            transformManagerDataControl["Position"] = new Vector2(positionerCenter01.x * gameWidth, positionerCenter01.y * gameHeight);
        }

        public override Vector2 ToScreenPoint(MapEditor mapEditor, Vector2 point)
        {
            var screen = mapEditor.ScreenRect;
            var heightRatio = screen.height / gameHeight;
            // First we scale relative to the (0,0) and revert the y
            var scaled = new Vector2((point.x * scale)* heightRatio, (point.y * scale)* heightRatio);
            // Sidely, we calculate the positioner center in the screen (from 0 to 1)
            var positionerCenter01 = new Vector2(position.x / gameWidth, position.y / gameHeight);
            // Then we calculate the positioner center pixel in the screen real position
            var positionerCenterPixel = screen.position + new Vector2(positionerCenter01.x * screen.width, positionerCenter01.y * screen.height);
            // And we combine the relative position to the center position
            return scaled + positionerCenterPixel;
        }

        public override Vector2 FromScreenPoint(MapEditor mapEditor, Vector2 point)
        {
            var screen = mapEditor.ScreenRect;
            var heightRatio = gameHeight / screen.height;
            // We calculate the positioner center in the screen (from 0 to 1)
            var positionerCenter01 = new Vector2(position.x / gameWidth, position.y / gameHeight);
            // Then we calculate the positioner center pixel in the screen real position
            var positionerCenterPixel = screen.position + new Vector2(positionerCenter01.x * screen.width, positionerCenter01.y * screen.height);
            // Then we get the local position
            var localPos = point - positionerCenterPixel;
            // And we return the unscaled version with the Y reverted
            return new Vector2((localPos.x * heightRatio) / scale, (localPos.y * heightRatio) / scale);
        }
    }
}
