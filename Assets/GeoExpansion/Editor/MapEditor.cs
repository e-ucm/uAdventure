using MapzenGo.Helpers;
using uAdventure.Editor;
using UnityEngine;

namespace uAdventure.Geo
{
    public class MapEditor : ComponentBasedEditor<MapEditor>
    {
        private readonly GUIMap guiMap;
        private readonly PlaceSearcher placeSearcher;

        #region Properties

        public IGuiMapPositionManager PositionManager { get; set; }

        public Vector2d GeoMousePosition
        {
            get { return guiMap.GeoMousePosition; }
        }

        public int Zoom
        {
            get { return guiMap.Zoom; }
            set { guiMap.Zoom = value; }
        }

        public ITileMeta TileMeta
        {
            get { return guiMap.TileMeta; }
            set { guiMap.TileMeta = value; }
        }

        public Rect ScreenRect { get; protected set; }

        public PlaceSearcher PlaceSearcher
        {
            get { return placeSearcher; }
        }

        #endregion

        #region Constructor

        public MapEditor() : base()
        {
            guiMap = new GUIMap();
            placeSearcher = ScriptableObject.CreateInstance<PlaceSearcher>();
        }

        #endregion


        #region Editor Methods

        protected override void BeforeDrawElements(Rect rect)
        {
            ScreenRect = rect;
            guiMap.DrawMap(rect);
        }
        protected override void AfterDrawElements(Rect rect)
        {
            guiMap.ProcessEvents(rect);
        }

        #endregion

        #region Aux Methods

        public float MetersToPixelsAt(Vector2d point, float meters)
        {
            return (float) GM.GetPixelsPerMeter(point.x, Zoom) * meters;
        }

        public Vector2d LatLonToPixel(Vector2d point)
        {
            return guiMap.LatLonToPixel(point);
        }

        public Vector2d[] LatLonToPixels(Vector2d[] points)
        {
            return guiMap.LatLonToPixels(points);
        }
        public Vector2d PixelToLatLon(Vector2d point)
        {
            return guiMap.PixelToLatLon(point);
        }

        public Vector2d[] PixelsToLatLon(Vector2d[] points)
        {
            return guiMap.PixelsToLatLon(points);
        }

        public Vector2d PixelToRelative(Vector2d pixel)
        {
            return guiMap.PixelToRelative(pixel);
        }

        public Vector2d RelativeToAbsolute(Vector2d pixel)
        {
            return guiMap.RelativeToAbsolute(pixel);
        }

        public Vector2d[] PixelsToRelative(Vector2d[] pixels)
        {
            return guiMap.PixelsToRelative(pixels);
        }

        public GUIMap.RepaintDelegate Repaint
        {
            get { return guiMap.Repaint; }
            set { guiMap.Repaint = value; }
        }

        public Vector2d Center
        {
            get { return guiMap.Center; }
            set { guiMap.Center = value; }
        }

        public void ZoomToBoundingBox(RectD boundingBox)
        {
            guiMap.ZoomToBoundingBox(boundingBox, 0, 0);
        }

        public override Vector2d[] ToRelative(Vector2d[] points)
        {
            var r = new Vector2d[points.Length];

            if (PositionManager != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    r[i] = PositionManager.ToScreenPoint(this, points[i]);
                }
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    r[i] = PixelToRelative(points[i]);
                }
            }

            return r;
        }

        public override Vector2d[] FromRelative(Vector2d[] points)
        {
            var r = new Vector2d[points.Length];

            if (PositionManager != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    r[i] = PositionManager.FromScreenPoint(this, points[i]);
                }
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    r[i] = RelativeToAbsolute(points[i]);
                }
            }

            return r;
        }

        #endregion
    }
}
