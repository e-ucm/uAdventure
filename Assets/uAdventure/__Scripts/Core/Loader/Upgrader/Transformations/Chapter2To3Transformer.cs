using System;
using System.IO;
using System.Linq;
using System.Xml;
using uAdventure.Runner;

namespace uAdventure.Core.XmlUpgrader
{
    internal class Chapter2To3Transformer : ITransformer
    {
        public string TargetFile { get { return "chapter*.xml"; } }

        public int TargetVersion { get { return 2; } }

        public int DestinationVersion { get { return 3; } }

        public string Upgrade(string input, string path, ResourceManager resourceManager)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(input);

            // ########################################################
            // ############### CHANGES IN MAP SCENES ##################
            // ########################################################

            // FIXING MAP-SCENE FLOATING POINTS TO INVARIANT CULTURE
            var mapScenes = doc.SelectNodes(".//map-scene");
            foreach (XmlNode mapScene in mapScenes)
            {
                FixFloatingPoint(mapScene.Attributes["gameplayArea"]);
                FixVectorFloatingPoint(mapScene.Attributes["center"]);
            }

            var extElemRefs = doc.SelectNodes(".//map-scene/map-element/ext-elem-ref");
            foreach (XmlElement extElemRef in extElemRefs)
            {
                // MOVING SCALE AND ORIENTATION TO EXT-ELEMENT-REF NODE
                XmlElement mapElement = extElemRef.ParentNode as XmlElement;
                if (mapElement.HasAttribute("scale"))
                {
                    extElemRef.SetAttribute("scale", mapElement.GetAttribute("scale"));
                }
                if (mapElement.HasAttribute("orientation"))
                {
                    extElemRef.SetAttribute("orientation", mapElement.GetAttribute("orientation"));
                }

                // FIXING FLOATING POINTS IN TRANSFORM MANAGERS
                foreach(XmlElement param in extElemRef.SelectNodes("param"))
                {
                    switch (param.GetAttribute("name"))
                    {
                        case "Position":
                            FixVectorFloatingPoint(param);
                            break;
                        case "Rotation":
                        case "InteractionRange":
                        case "Distance":
                        case "Degree":
                            FixFloatingPoint(param);
                            break;
                    }
                }
            }

            // ########################################################
            // ############### CHANGES IN GEOMETRIES ##################
            // ########################################################
            var geometries = doc.SelectNodes(".//geoelement/geometries/geometry");
            foreach(XmlNode geometry in geometries)
            {
                // FIXING INFLUENCE FLOATING POINT
                FixFloatingPoint(geometry.Attributes["influence"]);


                // FIXING GML GEOMETRIES POINTS
                XmlNode points = null;
                switch (geometry.FirstChild.Name)
                {
                    case "Polygon":
                        points = geometry.FirstChild.SelectSingleNode("exterior/LinearRing/posList");
                        break;
                    case "Point":
                        points = geometry.FirstChild.SelectSingleNode("pos");
                        break;
                    case "LineString":
                        points = geometry.FirstChild.SelectSingleNode("posList");
                        break;
                }

                FixFloatingPoint(points);
            }

            // RENAME GEOELEMENT TO GEO-ELEMENT
            foreach (XmlNode geoElement in doc.SelectNodes(".//geoelement"))
            {
                RenameXMLNode(doc, geoElement, "geo-element");
            }

            using (StringWriter sw = new StringWriter())
            using (XmlWriter xwo = XmlWriter.Create(sw))
            {
                doc.WriteContentTo(xwo);
                xwo.Flush();
                sw.Flush();
                File.WriteAllText("test.xml", sw.ToString());
                return sw.ToString();
            }
        }

        private void FixVectorFloatingPoint(XmlNode xmlNode)
        {
            var points = xmlNode.InnerText.Split(new string[] { ", " }, StringSplitOptions.None);
            xmlNode.InnerText = points.Select(p => p.Replace(",", ".")).Aggregate((p1, p2) => p1 + ", " + p2);
        }

        private static void FixFloatingPoint(XmlNode node)
        {
            node.InnerText = node.InnerText.Replace(",", ".");
        }

        public static void RenameXMLNode(XmlDocument doc, XmlNode oldRoot, string newname)
        {
            XmlElement newRoot = doc.CreateElement(newname);
            foreach (XmlAttribute attribute in oldRoot.Attributes)
            {
                newRoot.SetAttribute(attribute.Name, attribute.Value);
            }

            foreach (XmlNode childNode in oldRoot.ChildNodes)
            {
                newRoot.AppendChild(childNode.CloneNode(true));
            }
            XmlNode parent = oldRoot.ParentNode;
            parent.InsertBefore(newRoot, oldRoot);
            parent.RemoveChild(oldRoot);
        }
    }
}
