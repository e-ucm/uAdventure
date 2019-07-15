using System;
using UnityEngine;


namespace MapzenGo.Helpers
{
    //SOURCE: http://stackoverflow.com/questions/12896139/geographic-coordinates-converter
    public static class GM
    {
        private const int TileSize = 256;
        private const int EarthRadius = 6378137;
        private const double InitialResolution = 2 * Math.PI * EarthRadius / TileSize;
        private const double OriginShift = 2 * Math.PI * EarthRadius / 2;

        public static float distFrom(float lat1, float lng1, float lat2, float lng2)
        {
            double earthRadius = 6371000; //meters
            double dLat = (lat2 - lat1)*Mathf.Deg2Rad;
            double dLng = Mathf.Deg2Rad * (lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Mathf.Deg2Rad * (lat1)) * Math.Cos(Mathf.Deg2Rad * (lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            float dist = (float)(earthRadius * c);

            return dist;
        }

        public static double SeparationInMeters(Vector2d latlon1, Vector2d latlon2)
        {
            return (LatLonToMeters(latlon1) - LatLonToMeters(latlon2)).magnitude;
        }

        public static Vector2d LatLonToMeters(Vector2d v)
        {
            return LatLonToMeters(v.x, v.y);
        }


        public static double GetPixelsPerMeter(double lat, double zoom)
        {
            double pixelsPerTile = TileSize;
            double numTiles = Math.Pow(2, zoom);
            double radiusAtLat = Math.Cos(lat * Mathf.Deg2Rad) * GM.EarthRadius;
            double perimeterAtLat = 2d * Math.PI * radiusAtLat;
            double metersPerTile = perimeterAtLat / numTiles;
            return pixelsPerTile / metersPerTile;
        }

        //Converts given lat/lon in WGS84 Datum to XY in Spherical Mercator EPSG:900913
        public static Vector2d LatLonToMeters(double lat, double lon)
        {
            var p = new Vector2d();
            p.x = (lon * OriginShift / 180);
            p.y = (Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180));
            p.y = (p.y * OriginShift / 180);
            return new Vector2d(p.x, p.y);
        }

        //Converts pixel coordinates in given zoom level of pyramid to EPSG:900913
        public static Vector2d PixelsToMeters(Vector2d p, int zoom)
        {
            var res = Resolution(zoom);
            var met = new Vector2d();
            met.x = (p.x * res - OriginShift);
            met.y = -(p.y * res - OriginShift);
            return met;
        }

        //Converts EPSG:900913 to pyramid pixel coordinates in given zoom level
        public static Vector2d MetersToPixels(Vector2d m, int zoom)
        {
            var res = Resolution(zoom);
            var pix = new Vector2d();
            pix.x = ((m.x + OriginShift) / res);
            pix.y = ((-m.y + OriginShift) / res);
            return pix;
        }

        //Returns a TMS (NOT Google!) tile covering region in given pixel coordinates
        public static Vector2d PixelsToTile(Vector2d p)
        {
            var t = new Vector2d();
            t.x = (int)Math.Ceiling(p.x / (double)TileSize) - 1;
            t.y = (int)Math.Ceiling(p.y / (double)TileSize) - 1;
            return t;
        }

        public static Vector2d PixelsToRaster(Vector2d p, int zoom)
        {
            var mapSize = TileSize << zoom;
            return new Vector2d(p.x, mapSize - p.y);
        }

        //Returns tile for given mercator coordinates
        public static Vector2d MetersToTile(Vector2d m, int zoom)
        {
            var p = MetersToPixels(m, zoom);
            return PixelsToTile(p);
        }

        //Returns bounds of the given tile in EPSG:900913 coordinates
        public static RectD TileBounds(Vector2d t, int zoom)
        {
            var min = PixelsToMeters(new Vector2d(t.x * TileSize, t.y * TileSize), zoom);
            var max = PixelsToMeters(new Vector2d((t.x + 1) * TileSize, (t.y + 1) * TileSize), zoom);
            return new RectD(min, max - min);
        }

        public static Vector2d MetersToLatLon(Vector2d m)
        {
            var ll = new Vector2d();
            ll.y = (m.x / OriginShift) * 180;
            ll.x = (m.y / OriginShift) * 180;
            ll.x = 180 / Math.PI * (2 * Math.Atan(Math.Exp(ll.x * Math.PI / 180)) - Math.PI / 2);
            return ll;
        }

        //Returns bounds of the given tile in latutude/longitude using WGS84 datum
        //public static RectD TileLatLonBounds(Vector2d t, int zoom)
        //{
        //    var bound = TileBounds(t, zoom);
        //    var min = MetersToLatLon(new Vector2d(bound.Min.x, bound.Min.y));
        //    var max = MetersToLatLon(new Vector2d(bound.Min.x + bound.Size.x, bound.Min.y + bound.Size.y));
        //    return new RectD(min.x, min.y, Math.Abs(max.x - min.x), Math.Abs(max.y - min.y));
        //}

        //Resolution (meters/pixel) for given zoom level (measured at Equator)
        public static double Resolution(int zoom)
        {
            return InitialResolution / (Math.Pow(2, zoom));
        }

        public static double ZoomForPixelSize(double pixelSize)
        {
            for (var i = 0; i < 30; i++)
                if (pixelSize > Resolution(i))
                    return i != 0 ? i - 1 : 0;
            throw new InvalidOperationException();
        }

        // Switch to Google Tile representation from TMS
        public static Vector2d ToGoogleTile(Vector2d t, int zoom)
        {
            return new Vector2d(t.x, ((int)Math.Pow(2, zoom) - 1) - t.y);
        }

        // Switch to TMS Tile representation from Google
        public static Vector2d ToTmsTile(Vector2d t, int zoom)
        {
            return new Vector2d(t.x, ((int)Math.Pow(2, zoom) - 1) - t.y);
        }
    }
}
