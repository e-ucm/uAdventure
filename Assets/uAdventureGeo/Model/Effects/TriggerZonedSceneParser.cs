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
            
            var triggerSceneEffect = new TriggerZonedSceneEffect(
                element.GetAttribute("idTarget"),
                element.GetAttribute("zoneId"),
                int.Parse(element.GetAttribute("x")),
                int.Parse(element.GetAttribute("y")))
            {
                DestinyScale = ExParsers.ParseDefault(element.GetAttribute("scale"), float.MinValue)
            };

            triggerSceneEffect.setTransitionTime(ExParsers.ParseDefault(element.GetAttribute("transitionTime"), 0));
            triggerSceneEffect.setTransitionType((TransitionType)ExParsers.ParseDefault(element.GetAttribute("transitionType"), 0));
            

            return triggerSceneEffect;
        }
    }
}

