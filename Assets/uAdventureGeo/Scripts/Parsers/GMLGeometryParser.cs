using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Geo
{
    [DOMParser("geometry")]
    [DOMParser(typeof(GMLGeometry))]
    public class GMLGeometryParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var gmlNode = element.FirstChild;

            var geometry = new GMLGeometry();
            geometry.Influence = float.Parse(element.GetAttribute("influence"), CultureInfo.InvariantCulture);
            XmlNode pointsNode;
            switch (gmlNode.Name)
            {
                default: // case "Point":
                    geometry.Type = GMLGeometry.GeometryType.Point;
                    pointsNode = gmlNode.FirstChild;
                    break;
                case "LineString":
                    geometry.Type = GMLGeometry.GeometryType.LineString;
                    pointsNode = gmlNode.FirstChild;
                    break;
                case "Polygon":
                    geometry.Type = GMLGeometry.GeometryType.Polygon;
                    pointsNode = gmlNode.FirstChild.FirstChild.FirstChild; // Polygon -> external -> Ring -> points
                    break;
            }

            geometry.Points = UnzipPoints(pointsNode.InnerText);
            return geometry;
        }

        private Vector2d[] UnzipPoints(string pointList)
        {
            string[] zippedPoints = string.IsNullOrEmpty(pointList) ? null : pointList.Split(' ');
            if (zippedPoints == null || zippedPoints.Length < 2)
            {
                return new Vector2d[0];
            }

            var points = new List<Vector2d>();
            for (int i = 0; i < zippedPoints.Length; i += 2) 
            {
                points.Add(new Vector2d(double.Parse(zippedPoints[i], CultureInfo.InvariantCulture), double.Parse(zippedPoints[i + 1], CultureInfo.InvariantCulture)));
            }

            return points.ToArray();
        }
    }
}
