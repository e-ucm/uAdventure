using UnityEngine;
using MapzenGo.Helpers;
using System;

namespace uAdventure.Geo
{
    public class GUIMap
    {
        /* -----------------------------
         * Attributes
         *-----------------------------*/

            
        public static readonly double MinLatitude = -85.05112877980659;
        public static readonly double MaxLatitude = 85.05112877980659;
        public static readonly double MinLongitude = -180d;
        public static readonly double MaxLongitude = 180d;

        // Delegates
        public delegate void RepaintDelegate();

        // Attributes
        public RepaintDelegate Repaint;
        public int Zoom
        {
            get { return zoom; }
            set
            {
                zoom = Mathf.Clamp(value, 3, 19);
                centerPixel = GM.MetersToPixels(GM.LatLonToMeters(Center.x, Center.y), zoom);
            }
        }
        
        public Vector2d Center
        {
            get { return center; }
            set
            {
                center = value;
                centerPixel = GM.MetersToPixels(GM.LatLonToMeters(center.x, center.y), Zoom);
            }
        }

        public ITileMeta TileMeta { get; set; }

        public Vector2d GeoMousePosition { get; set; }

        // private attributes
        protected int zoom = 3;
        protected int id = -1;
        protected Vector2d center = new Vector2d(30,0);
        protected Vector2d centerPixel;
        protected Rect screenRect;
        private bool waitingForValidScreen;
        private RectD waitingBoundingBox;
        /// <summary>
        /// Pixel Absolute To Relative Ratio
        /// </summary>
        public Vector2d PATR { get; private set; } // Will be calculated in the begining of each iteration

        protected int selectedPoint;

        /* -----------------------------
         * Constructor
         *-----------------------------*/

        public GUIMap()
        {
            //System.Globalization.RegionInfo.CurrentRegion.
        }

        /* -----------------------------
         *  Main method
         *-----------------------------*/

        public void DrawMap(Rect area)
        {
            id = GUIUtility.GetControlID("GUIMap".GetHashCode() + GetHashCode(), FocusType.Passive, area);

            GUI.BeginGroup(area);
            area = new Rect(Vector2.zero, area.size);
            // update the pixel absolute to relative convert variable
            PATR = -(centerPixel - (area.size / 2f).ToVector2d() - area.position.ToVector2d());

            var mousePos = Event.current.mousePosition.ToVector2d();

            if (Event.current.type != EventType.Layout)
            {
                GeoMousePosition = GM.MetersToLatLon(GM.PixelsToMeters(RelativeToAbsolute(mousePos), Zoom));
            }

            if (waitingForValidScreen)
            {
                waitingForValidScreen = false;
                ZoomToBoundingBox(waitingBoundingBox, 0, 0);
            }

            if (Event.current.type == EventType.Repaint)
            {
                // Draw the tiles
                screenRect = area;
                DrawTiles(area);

                /*if (selectedGeometry != null && selectedGeometry.Points.Count > 0)
                {
                    var pixels = PixelsToRelative(LatLonToPixels(selectedGeometry.Points)).ConvertAll(p => p.ToVector2());
                    var v2mousepos = mousePos.ToVector2();
                    // Find the closest index
                    var min = pixels.Min(p => (p - v2mousepos).magnitude);
                    var closest = pixels.FindIndex(p => (p - v2mousepos).magnitude == min);

                    // Fix the previous and next
                    var prev = closest == 0 ? pixels.Count - 1 : closest - 1;
                    var next = (closest + 1) % pixels.Count;
                    // Calculate the normal to both adjacent axis to closest point
                    var c = pixels[closest];
                    var v1 = (pixels[closest] - pixels[prev]).normalized;
                    var v2 = (pixels[closest] - pixels[next]).normalized;

                    var closestNormal = (v1 + v2).normalized;
                    var convex = Vector3.Cross(v1, v2).z > 0;

                    var mouseVector = (v2mousepos - c);
                    var left = Vector3.Cross(closestNormal, mouseVector).z > 0;
                    Handles.DrawLine(pixels[closest], v2mousepos);
                    if ((left && convex) || (!left && !convex))
                    {
                        Handles.DrawLine(pixels[prev], v2mousepos);
                    }
                    else
                    {
                        Handles.DrawLine(pixels[next], v2mousepos);
                    }

                }*/
            }

            GUI.EndGroup();

        }

        public void ProcessEvents(Rect area)
        {
            switch (Event.current.GetTypeForControl(id))
            {
                case EventType.ScrollWheel:
                    {
                        // Changezoom
                        Zoom += Mathf.FloorToInt(-Event.current.delta.y / 3f);
                        Event.current.Use();
                    }
                    break;


                case EventType.MouseDrag:
                    {
                        // MoveLatLon or center var 
                        if (GUIUtility.hotControl == id && area.Contains(Event.current.mousePosition))
                        {
                            var delta = new Vector2d(Event.current.delta.x, Event.current.delta.y);
                            Center = GM.MetersToLatLon(GM.PixelsToMeters(centerPixel - delta, Zoom));
                            Event.current.Use();
                        }
                    }
                    break;
                case EventType.MouseDown:
                    {
                        if (area.Contains(Event.current.mousePosition))
                        {
                            //GUI.FocusControl(null);
                            GUIUtility.hotControl = id;
                            Event.current.Use();
                        }
                    }
                    break;
            }
        }


        /**
         * @since 6.0.0
         * @return the maximum zoom level where a bounding box fits into a screen,
         * or Double.MIN_VALUE if bounding box is a single point
         * https://github.com/osmdroid/osmdroid/blob/78d28e4b958ed0a12a6203d9a4d9c7af58422101/osmdroid-android/src/main/java/org/osmdroid/util/TileSystem.java
         */
        public void ZoomToBoundingBox(RectD pBoundingBox, int pScreenWidth, int pScreenHeight)
        {
            if (screenRect.width == 0 && screenRect.height == 0)
            {
                waitingForValidScreen = true;
                waitingBoundingBox = pBoundingBox;
            }

            if (pBoundingBox.Width == 0 && pBoundingBox.Height == 0)
            {
                Zoom = 19;
                return;
            }

            if (pScreenWidth == 0)
            {
                pScreenWidth = (int)screenRect.width;
            }
            if (pScreenHeight == 0)
            {
                pScreenHeight = (int)screenRect.height;
            }

            double longitudeZoom = getLongitudeZoom(pBoundingBox.Min.x + pBoundingBox.Width, pBoundingBox.Min.x, pScreenWidth);
            double latitudeZoom = getLatitudeZoom(pBoundingBox.Min.y, pBoundingBox.Min.y + pBoundingBox.Height, pScreenHeight);
            if (longitudeZoom == double.MinValue)
            {
                Zoom = (int)latitudeZoom;
            }
            else if (latitudeZoom == double.MinValue)
            {
                Zoom = (int)longitudeZoom;
            }
            else
            {
                Zoom = (int)Math.Min(latitudeZoom, longitudeZoom);
            }
        }

        /**
         * @since 6.0.0
         * @return the maximum zoom level where both longitudes fit into a screen,
         * or Double.MIN_VALUE if longitudes are equal
         * https://github.com/osmdroid/osmdroid/blob/78d28e4b958ed0a12a6203d9a4d9c7af58422101/osmdroid-android/src/main/java/org/osmdroid/util/TileSystem.java
         */
        public double getLongitudeZoom(double pEast, double pWest, int pScreenWidth)
        {
            double x01West = getX01FromLongitude(pWest);
            double x01East = getX01FromLongitude(pEast);
            double span = x01East - x01West;
            if (span < 0)
            {
                span += 1;
            }
            if (span == 0)
            {
                return double.MinValue;
            }
            return Math.Log(pScreenWidth / span / 256) / Math.Log(2);
        }

        /**
         *
         * @since 6.0.0
         * @return the maximum zoom level where both latitudes fit into a screen,
         * or Double.MIN_VALUE if latitudes are equal or ill positioned
         */
        public double getLatitudeZoom(double pNorth, double pSouth, int pScreenHeight)
        {
            double y01North = getY01FromLatitude(pNorth);
            double y01South = getY01FromLatitude(pSouth);
            double span = y01South - y01North;
            if (span <= 0)
            {
                return double.MaxValue;
            }
            return Math.Log(pScreenHeight / span / 256) / Math.Log(2);
        }

        /**
         * Converts a longitude to its "X01" value,
         * id est a double between 0 and 1 for the whole longitude range
         * @since 6.0.0
         */
        public double getX01FromLongitude(double longitude)
        {
            return (longitude - MinLongitude) / (MaxLongitude - MinLongitude);
        }

        /**
         * Converts a latitude to its "Y01" value,
         * id est a double between 0 and 1 for the whole latitude range
         * @since 6.0.0
         */
        public double getY01FromLatitude(double latitude)
        {
            return (latitude - MinLatitude) / (MaxLatitude - MinLatitude);
        }

        /**
         * @since 6.0.0
         */
        public static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /* -----------------------------
         *  Drawing methods
         *-----------------------------*/

        protected void DrawTiles(Rect area)
        {
            // Download and draw tiles
            Vector2d topLeftCorner = GM.PixelsToTile(centerPixel - new Vector2d(area.width / 2f, area.height / 2f)),
                bottomRightCorner = GM.PixelsToTile(centerPixel + new Vector2d(area.width / 2f, area.height / 2f));

            for (double x = topLeftCorner.x; x <= bottomRightCorner.x; x++)
            {
                for (double y = topLeftCorner.y; y <= bottomRightCorner.y; y++)
                {
                    ITilePromise tp = null;
                    if (TileMeta == null)
                    {
                        tp = TileProvider.Instance.GetTile(new Vector3d(x, y, Zoom), (_) => { if (Repaint != null) { Repaint(); } });
                    }
                    else
                    {
                        tp = TileProvider.Instance.GetTile(new Vector3d(x, y, Zoom), TileMeta, (_) => { if (Repaint != null) { Repaint(); } });
                    }

                    var tileBounds = GM.TileBounds(new Vector2d(x, y), Zoom);
                    var tileRect = ExtensionRect.FromCorners(
                        GM.MetersToPixels(tileBounds.Min, Zoom).ToVector2(),
                        GM.MetersToPixels(tileBounds.Min + tileBounds.Size, Zoom).ToVector2());

                    var windowRect = new Rect(tileRect.position + PATR.ToVector2(), tileRect.size);
                    var areaRect = windowRect.Intersection(area);
                    if (areaRect.width < 0 || areaRect.height < 0)
                        continue;

                    if (tp != null && tp.Data != null)
                    {
                        GUI.DrawTextureWithTexCoords(areaRect, tp.Data as Texture2D, windowRect.ToTexCoords(areaRect));
                    }
                }
            }
        }
        public Vector2d LatLonToPixel(Vector2d point)
        {
            return GM.MetersToPixels(GM.LatLonToMeters(point.x, point.y), Zoom);
        }

        public Vector2d[] LatLonToPixels(Vector2d[] points)
        {
            return Convert(points, p => GM.MetersToPixels(GM.LatLonToMeters(p.x, p.y), Zoom));
        }

        public Vector2d PixelToLatLon(Vector2d point)
        {
            return GM.MetersToLatLon(GM.PixelsToMeters(point, Zoom));
        }

        public Vector2d[] PixelsToLatLon(Vector2d[] points)
        {
            return Convert(points, p => GM.MetersToLatLon(GM.PixelsToMeters(p, Zoom)));
        }

        public Vector2d PixelToRelative(Vector2d pixel)
        {
            return pixel + PATR;
        }

        public Vector2d RelativeToAbsolute(Vector2d pixel)
        {
            return pixel - PATR;
        }

        public Vector2d[] PixelsToRelative(Vector2d[] pixels)
        {
            return Convert(pixels, p => PixelToRelative(p));
        }

        private Vector2d[] Convert(Vector2d[] points, Func<Vector2d, Vector2d> operation)
        {
            var r = new Vector2d[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                r[i] = operation(points[i]);
            }

            return r;
        }
    }
}