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

            var oldDescriptionSystem = new Description();
            parsed.Descriptions = new List<Description> ();
            foreach (var child in element.ChildNodes)
            {
                var node = child as XmlNode;
                switch (node.Name)
                {
                    case "description":
                        parsed.Descriptions.Add(DOMParserUtility.DOMParse<Description>(node, parameters)); 
                        break;
                    case "name":
                        oldDescriptionSystem.setName(node.InnerText); break;
                    case "brief-description":
                        oldDescriptionSystem.setDescription(node.InnerText); break;
                    case "detailed-description":
                        oldDescriptionSystem.setDetailedDescription(node.InnerText); break;
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

            if (parsed.Descriptions.Count == 0)
            {
                parsed.Descriptions.Add(oldDescriptionSystem);
            }
            return parsed;
        }

        private List<GeoAction> ParseActions(XmlNode node, params object[] parameters)
        {
            return DOMParserUtility.DOMParse<GeoAction>(node.ChildNodes, parameters) as List<GeoAction>;
        }
    }
}
