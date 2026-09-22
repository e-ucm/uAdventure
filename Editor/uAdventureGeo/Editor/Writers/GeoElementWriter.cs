using UnityEngine;
using uAdventure.Geo;
using System;
using System.Xml;
using System.Linq;
using System.Globalization;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(GeoElement))]
    public class GeoElementWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var geoelement = target as GeoElement;
            var doc = Writer.GetDoc();

            (node as XmlElement).SetAttribute("id", geoelement.Id);

            // Descriptions
            DOMWriterUtility.DOMWrite(node, geoelement.Descriptions);

            // Geometries
            var geometries = doc.CreateElement("geometries");
            node.AppendChild(geometries);
            foreach (var geometry in geoelement.Geometries)
            {
                DumpGML(geometries, "geometry", geometry);
            }


            var actions = doc.CreateElement("actions");
            node.AppendChild(actions);
            DOMWriterUtility.DOMWrite(actions, geoelement.Actions);
        }

        private void DumpGML(XmlNode parent, string name, GMLGeometry content)
        {
            var doc = Writer.GetDoc();
            // base element
            var elem = doc.CreateElement(name);
            parent.AppendChild(elem);
            elem.SetAttribute("influence", content.Influence.ToString(CultureInfo.InvariantCulture));

            // Dump geometry type
            XmlNode gmlElement;
            switch (content.Type)
            {
                case GMLGeometry.GeometryType.Point:
                    gmlElement = doc.CreateElement("gml:Point");
                    DumpPosList(gmlElement, content.Points);
                    break;
                case GMLGeometry.GeometryType.LineString:
                    gmlElement = doc.CreateElement("gml:LineString");
                    DumpPosList(gmlElement, content.Points);
                    break;
                default: // case GMLGeometry.GeometryType.Polygon:
                    gmlElement = doc.CreateElement("gml:Polygon");
                    var exterior = doc.CreateElement("gml:exterior");
                    gmlElement.AppendChild(exterior);
                    var linearRing = doc.CreateElement("gml:LinearRing");
                    exterior.AppendChild(linearRing);
                    DumpPosList(linearRing, content.Points);
                    break;
            }
            elem.AppendChild(gmlElement);
        }

        private void DumpPosList(XmlNode parent, Vector2d[] points)
        {

            var doc = Writer.GetDoc();
            // base element
            var elem = doc.CreateElement(points.Length > 1 ? "gml:posList" : "gml:pos");
            parent.AppendChild(elem);

            elem.InnerText = String.Join(" ", points.Select(p => p.x.ToString(CultureInfo.InvariantCulture) + " " + p.y.ToString(CultureInfo.InvariantCulture)).ToArray());
        }

        protected override string GetElementNameFor(object target)
        {
            return "geo-element";
        }
    }
}

