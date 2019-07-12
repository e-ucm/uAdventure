using MapzenGo.Helpers;
using UnityEngine;

namespace uAdventure.Geo
{
    public struct AreaMeta
    {
        private bool limitsSet;
        private RectD limits;

        public RectD Area { get; set; }
        public string Meta { get; set; }

        public RectD Limits
        {
            get
            {
                if (!limitsSet)
                {
                    limits = GetTileLimits(Area, 19);
                    limitsSet = true;
                }

                return limits;
            }
        }

        public AreaMeta(MapScene mapScene)
            : this(mapScene.GameplayArea, mapScene.TileMetaIdentifier)
        {
        }

        public AreaMeta(MapSceneDataControl mapScene)
            : this(mapScene.GameplayArea.BoundingBox, mapScene.GameplayArea.TileMetaIdentifier)
        {
        }

        public AreaMeta(RectD area, string meta)
        {
            Area = area;
            Meta = meta;
            limits = new RectD(new Vector2d(), new Vector2d());
            limitsSet = false;
        }

        public static RectD GetTileLimits(RectD boundingBox, int zoom)
        {
            var bottomLeft = GM.MetersToTile(GM.LatLonToMeters(boundingBox.Min.x, boundingBox.Min.y), zoom);
            var topRight = GM.MetersToTile(GM.LatLonToMeters(boundingBox.Min.x + boundingBox.Width, boundingBox.Min.y + boundingBox.Height), zoom);

            var topLeftCorner = new Vector2d(
                Mathf.Min((int)bottomLeft.x, (int)topRight.x),
                Mathf.Min((int)bottomLeft.y, (int)topRight.y));
            var bottomRightCorner = new Vector2d(
                Mathf.Max((int)bottomLeft.x, (int)topRight.x),
                Mathf.Max((int)bottomLeft.y, (int)topRight.y));

            return new RectD(topLeftCorner, bottomRightCorner - topLeftCorner);
        }
    }
}
