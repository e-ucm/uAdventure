using UnityEngine;
using System.Collections;
using System.Linq;
using uAdventure.Core;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;

namespace uAdventure.Geo
{
    [DOMParser("map-scene")]
    [DOMParser(typeof(MapScene))]
    public class MapSceneParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var mapScene = new MapScene(element.Attributes["id"].Value);
            var chapter = parameters[0] as Chapter;

            mapScene.CameraType = ExParsers.ParseEnum<CameraType>(ExString.Default(element.GetAttribute("cameraType"), CameraType.Aerial2D.ToString()));
            mapScene.RenderStyle = ExParsers.ParseEnum<RenderStyle>(ExString.Default(element.GetAttribute("renderStyle"), RenderStyle.Tile.ToString()));
            mapScene.TileMetaIdentifier = ExString.Default(element.GetAttribute("tileMetaIdentifier"), "OSMTile");
            mapScene.UsesGameplayArea = ExString.EqualsDefault(element.GetAttribute("usesGameplayArea"), "yes", false);
            mapScene.GameplayArea = ExParsers.ParseDefault(element.GetAttribute("gameplayArea"), new RectD(Vector2d.zero, Vector2d.zero));
            mapScene.LatLon = ExParsers.ParseDefault(element.GetAttribute("center"), Vector2d.zero);
            mapScene.Zoom = ExParsers.ParseDefault(element.GetAttribute("zoom"), 19);

            bool initialScene = ExString.EqualsDefault(element.GetAttribute("start"), "yes", false);
            if (initialScene)
            {
                chapter.setTargetId(mapScene.getId());
            }

            int layer = 0;
            foreach (var e in element.SelectNodes("map-element"))
            {
                var mapElementNode = e as XmlElement;
                MapElement mapElement = null;
                XmlElement extElemNode;
                var targetId = mapElementNode.GetAttribute("targetId");
                if ((extElemNode = (XmlElement)mapElementNode.SelectSingleNode("ext-elem-ref")) != null)
                {
                    var extElem = new ExtElemReference(targetId);
                    mapElement = extElem;
                    extElem.TransformManagerDescriptor = GetDescriptor(ExString.Default(extElemNode.GetAttribute("transformManager"),
                        typeof(GeopositionedDescriptor).FullName));

                    foreach(var param in extElem.TransformManagerDescriptor.ParameterDescription)
                    {
                        var paramNode = extElemNode.SelectSingleNode("param[@name=\"" + param.Key + "\"]");
                        if(paramNode != null)
                        {
                            extElem.TransformManagerParameters.Add(param.Key, parseParam(param.Value.Type, paramNode.InnerText));
                        }
                    }
                }
                else
                {
                    mapElement = new GeoReference(targetId);
                }

                mapElement.Conditions = DOMParserUtility.DOMParse<Conditions>(mapElementNode.SelectSingleNode("condition") as XmlElement, parameters);
                mapElement.Layer = ExParsers.ParseDefault(mapElementNode.GetAttribute("layer"), layer);
                mapElement.Scale = ExParsers.ParseDefault(mapElementNode.GetAttribute("scale"), CultureInfo.InvariantCulture, 1f);
                mapElement.Orientation = (Orientation) ExParsers.ParseDefault(mapElementNode.GetAttribute("orientation"), 2);
                layer = Mathf.Max(mapElement.Layer, layer) + 1;
                mapScene.Elements.Add(mapElement);
            }

            return mapScene;
        }

        private object parseParam(Type paramType, string innerText)
        {
            // Basic types
            if(paramType == typeof(float))
            {
                return float.Parse(innerText);
            }
            if (paramType == typeof(int))
            {
                return int.Parse(innerText);
            }
            if (paramType == typeof(string))
            {
                return innerText;
            }
            if (paramType == typeof(bool))
            {
                return bool.Parse(innerText);
            }
            if (paramType == typeof(double))
            {
                return double.Parse(innerText);
            }
            if (paramType == typeof(char))
            {
                return innerText[0];
            }
            // Unity types
            if (paramType == typeof(Vector2))
            {
                // Remove '(' and ')', then split and then convert to numbers
                var numbers = innerText.Substring(1, innerText.Length - 2).Split(',').ToList().ConvertAll(s => float.Parse(s.Trim()));
                return new Vector2(numbers[0], numbers[1]);
            }
            if (paramType == typeof(Vector2d))
            {
                // Remove '(' and ')', then split and then convert to numbers
                var numbers = innerText.Substring(1, innerText.Length - 2).Split(',').ToList().ConvertAll(s => double.Parse(s.Trim()));
                return new Vector2d(numbers[0], numbers[1]);
            }
            if (paramType == typeof(Vector3))
            {
                // Remove '(' and ')', then split and then convert to numbers
                var numbers = innerText.Substring(1, innerText.Length - 2).Split(',').ToList().ConvertAll(s => float.Parse(s.Trim()));
                return new Vector3(numbers[0], numbers[1], numbers[2]);
            }
            if (paramType == typeof(Vector3d))
            {
                // Remove '(' and ')', then split and then convert to numbers
                var numbers = innerText.Substring(1, innerText.Length - 2).Split(',').ToList().ConvertAll(s => double.Parse(s.Trim()));
                return new Vector3d(numbers[0], numbers[1], numbers[2]);
            }

            return null;
        }

        private ITransformManagerDescriptor GetDescriptor(string type)
        {
            ITransformManagerDescriptor r = null;

            Type t = Type.GetType(type);
            var manager = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Keys.ToList().Find(k => k == t);
            if ( manager != null ) r = TransformManagerDescriptorFactory.Instance.CreateDescriptor(manager);

            return r;
        }
    }

}
