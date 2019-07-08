using uAdventure.Core;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Geo
{
    [DOMParser("geoelement", "geo-element")]
    [DOMParser(typeof(GeoElement))]
    public class GeoElementParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            GeoElement parsed = new GeoElement(element.Attributes["id"].Value);

            foreach(var child in element.ChildNodes)
            {
                var node = child as XmlNode;
                var description = new Description();
                parsed.Descriptions = new List<Description>{description};
                switch (node.Name)
                {
                    case "descriptions":
                        parsed.Descriptions =
                            DOMParserUtility.DOMParse<Description>(node.ChildNodes, parameters).DefaultIfEmpty(new Description()).ToList();
                        break;
                    case "name":
                        description.setName(node.InnerText); break;
                    case "brief-description":
                        description.setDescription(node.InnerText); break;
                    case "detailed-description":
                        description.setDetailedDescription(node.InnerText); break;
                    case "geometries":
                        parsed.Geometries = DOMParserUtility.DOMParse<GMLGeometry>(node.ChildNodes, parameters).DefaultIfEmpty(new GMLGeometry()).ToList(); break;
                    case "geometry":
                        parsed.Geometries = new List<GMLGeometry>
                            {DOMParserUtility.DOMParse<GMLGeometry>(node, parameters)};
                        break;
                    case "actions":
                        parsed.Actions = ParseActions(node, parameters); break;
                    case "resources":
                        parsed.Resources = DOMParserUtility
                            .DOMParse<ResourcesUni>(element.SelectNodes("resources"), parameters).DefaultIfEmpty(new ResourcesUni()).ToList();
                        break;

                }
            }
            return parsed;
        }

        private List<GeoAction> ParseActions(XmlNode node, params object[] parameters)
        {
            return DOMParserUtility.DOMParse<GeoAction>(node.ChildNodes, parameters) as List<GeoAction>;
        }
    }
}
