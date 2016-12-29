using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Xml;
using System.Collections.Generic;

namespace uAdventure.Geo
{
    [DOMParser("geoelement", "geo-element")]
    [DOMParser(typeof(GeoElement))]
    public class GeoElementParser : IDOMParser
    {
        public GeoElementParser()
        {
        }

        public object DOMParse(XmlElement element, params object[] parameters)
        {
            GeoElement parsed = new GeoElement(element.Attributes["id"].Value);

            foreach(var child in element.ChildNodes)
            {
                var node = child as XmlNode;
                switch (node.Name)
                {
                    case "name":
                        parsed.Name = node.InnerText; break;
                    case "description":
                        parsed.FullDescription = node.InnerText; break;
                    case "brief-description":
                        parsed.BriefDescription = node.InnerText; break;
                    case "detailed-description":
                        parsed.DetailedDescription = node.InnerText; break;
                    case "influence":
                        parsed.Influence = float.Parse(node.InnerText); break;
                    case "geometry":
                        parsed.Geometry = ParseGeometry(node); break;
                    case "actions":
                        parsed.Actions = ParseActions(node, parameters); break;
                }
            }
            // TODO parsed.Image = element.SelectNodes("/image")[0].InnerText;
            return parsed;
        }

        private List<GeoAction> ParseActions(XmlNode node, params object[] parameters)
        {
            return DOMParserUtility.DOMParse<GeoAction>(node.ChildNodes, parameters) as List<GeoAction>;
        }

        private GMLGeometry ParseGeometry(XmlNode node)
        {
            var gmlNode = node.FirstChild;
            var geometry = new GMLGeometry();
            XmlNode pointsNode;
            switch (gmlNode.Name)
            {
                default:
                case "Point":
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

        private List<Vector2d> UnzipPoints(string pointList)
        {
            var points = new List<Vector2d>();
            var zippedPoints = pointList.Split(' ');
            for (int i = 0; i < zippedPoints.Length; i += 2)
                points.Add(new Vector2d(double.Parse(zippedPoints[i]), double.Parse(zippedPoints[i + 1])));

            return points;
        }
    }
}
