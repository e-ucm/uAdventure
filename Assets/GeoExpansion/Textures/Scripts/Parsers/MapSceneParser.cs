using UnityEngine;
using System.Collections;
using System.Linq;
using uAdventure.Core;
using System;
using System.Xml;
using System.Collections.Generic;

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
            mapScene.CameraType = element.Attributes["cameraType"].Value.ToEnum<CameraType>();
            
            var tmpArgVal = element.GetAttribute("start");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                if (tmpArgVal.Equals("yes"))
                {
                    chapter.setTargetId(mapScene.getId());
                }
            }

            tmpArgVal = element.GetAttribute("center");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                mapScene.LatLon = (Vector2d) parseParam(typeof(Vector2d), tmpArgVal);
            }

            foreach (var e in element.SelectNodes("map-element"))
            {
                var mapElementNode = e as XmlElement;
                MapElement mapElement = null;
                XmlElement extElemNode;
                if((extElemNode = (XmlElement)mapElementNode.SelectSingleNode("ext-elem-ref")) != null)
                {
                    var extElem = new ExtElemReference(mapElementNode.Attributes["targetId"].Value);
                    mapElement = extElem;
                    extElem.TransformManagerDescriptor = GetDescriptor(extElemNode.Attributes["transformManager"].Value);
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
                    mapElement = new GeoReference(mapElementNode.Attributes["targetId"].Value);
                }

                mapElement.Conditions = (Conditions) DOMParserUtility.DOMParse(mapElementNode.SelectSingleNode("condition") as XmlElement, parameters);
                mapElement.Layer = int.Parse(mapElementNode.Attributes["layer"].Value);
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

        private ExtElemReferenceTransformManagerDescriptor GetDescriptor(string type)
        {
            ExtElemReferenceTransformManagerDescriptor r = null;

            Type t = Type.GetType(type);
            var manager = TransformManagerDescriptorFactory.Instance.AvaliableTransformManagers.Keys.ToList().Find(k => k == t);
            if ( manager != null ) r = TransformManagerDescriptorFactory.Instance.CreateDescriptor(manager);

            return r;
        }
    }

}
