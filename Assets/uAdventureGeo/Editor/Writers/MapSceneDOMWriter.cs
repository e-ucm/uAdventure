using UnityEngine;
using System.Collections;
using System;
using System.Linq;

using uAdventure.Editor;
using System.Xml;
using System.Globalization;

namespace uAdventure.Geo
{
    using CIP = ChapterDOMWriter.ChapterTargetIDParam;

    [DOMWriter(typeof(MapScene))]
    public class MapSceneDOMWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var mapScene = target as MapScene;
            var element = node as XmlElement;

            element.SetAttribute("id", mapScene.Id);
            element.SetAttribute("cameraType", mapScene.CameraType.ToString());
            element.SetAttribute("renderStyle", mapScene.RenderStyle.ToString());
            element.SetAttribute("tileMetaIdentifier", mapScene.TileMetaIdentifier);
            element.SetAttribute("usesGameplayArea", mapScene.UsesGameplayArea ? "yes" : "no");
            element.SetAttribute("gameplayArea", ToStringWithCulture(mapScene.GameplayArea, CultureInfo.InvariantCulture));
            element.SetAttribute("center", ToStringWithCulture(mapScene.LatLon, CultureInfo.InvariantCulture));
            element.SetAttribute("zoom", mapScene.Zoom.ToString());

            if (options.Any(o => o is CIP && (o as CIP).TargetId.Equals(mapScene.getId())))
            {
                element.SetAttribute("start", "yes");
            }
            else
            {
                element.SetAttribute("start", "no");
            }

            foreach (var mapElement in mapScene.Elements)
            {
                DumpMapElement(node, mapElement, options);   
            }
        }


        protected void DumpMapElement(XmlNode node, MapElement mapElement, params IDOMWriterParam[] options)
        {
            var mapElementNode = Writer.GetDoc().CreateElement("map-element");
            mapElementNode.SetAttribute("targetId", mapElement.getTargetId());
            mapElementNode.SetAttribute("layer", mapElement.Layer.ToString());
            node.AppendChild(mapElementNode);

            DOMWriterUtility.DOMWrite(mapElementNode, mapElement.Conditions, options);

            if (mapElement is ExtElemReference)
            {
                var elemRef = mapElement as ExtElemReference;
                var elemRefNode = Writer.GetDoc().CreateElement("ext-elem-ref");
                mapElementNode.AppendChild(elemRefNode);

                elemRefNode.SetAttribute("scale", ToStringWithCulture(mapElement.Scale, CultureInfo.InvariantCulture));
                elemRefNode.SetAttribute("orientation", ((int)mapElement.Orientation).ToString());

                elemRefNode.SetAttribute("transformManager", elemRef.TransformManagerDescriptor.GetType().ToString());
                foreach(var param in elemRef.TransformManagerDescriptor.ParameterDescription)
                {
                    var paramElem = Writer.GetDoc().CreateElement("param");
                    paramElem.SetAttribute("name", param.Key);
                    paramElem.InnerText = ToStringWithCulture(elemRef.TransformManagerParameters[param.Key], CultureInfo.InvariantCulture);
                    elemRefNode.AppendChild(paramElem);
                }

                var actions = Writer.GetDoc().CreateElement("actions");
                elemRefNode.AppendChild(actions);
                DOMWriterUtility.DOMWrite(actions, elemRef.Actions);
            }
        }

        private static string ToStringWithCulture(object elem, CultureInfo culture)
        {
            if (elem is float f)
            {
                return f.ToString(culture);
            }
            else if (elem is double d)
            {
                return d.ToString(culture);
            }
            else if (elem is Vector2 v2)
            {
                return "(" + v2[0].ToString(culture) + ", " + v2[1].ToString(culture) + ")";
            }
            else if (elem is Vector2d v2d)
            {
                return "(" + v2d[0].ToString(culture) + ", " + v2d[1].ToString(culture) + ")";
            }
            else if (elem is Vector3 v3)
            {
                return "(" + v3[0].ToString(culture) + ", " + v3[1].ToString(culture) + ", " + v3[2].ToString(culture) + ")";
            }
            else if (elem is Vector3d v3d)
            {
                return "(" + v3d[0].ToString(culture) + ", " + v3d[1].ToString(culture) + ", " + v3d[2].ToString(culture) + ")";
            }
            else if (elem is RectD rectd)
            {
                return rectd.Min.x.ToString(culture) + " " + rectd.Min.y.ToString(culture) + " " + rectd.Size.x.ToString(culture) + " " + rectd.Size.y.ToString(culture);
            }
            else
            {
                return elem.ToString();
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "map-scene";
        }
    }
}

