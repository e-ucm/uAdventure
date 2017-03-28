using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMParser("trigger-zoned-scene")]
    public class TriggerZonedSceneParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var r = new TriggerZonedSceneEffect(
                element.GetAttribute("idTarget"),
                element.GetAttribute("zoneId"),
                int.Parse(element.GetAttribute("x")),
                int.Parse(element.GetAttribute("y")));

            return r;
        }
    }
}

