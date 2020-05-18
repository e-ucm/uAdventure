using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Editor;
using System;
using System.Xml;
using System.Globalization;

namespace uAdventure.Geo
{
    [DOMWriter(typeof(TriggerZonedSceneEffect))]
    public class TriggerZonedSceneEffectDOMWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var triggerZonedSceneEffect = target as TriggerZonedSceneEffect;
            
            var element = node as XmlElement;

            element.SetAttribute("idTarget", triggerZonedSceneEffect.getTargetId());
            element.SetAttribute("x", triggerZonedSceneEffect.getX().ToString());
            element.SetAttribute("y", triggerZonedSceneEffect.getY().ToString());
            element.SetAttribute("zoneId", triggerZonedSceneEffect.ZoneId);
            element.SetAttribute("transitionTime", triggerZonedSceneEffect.getTransitionTime().ToString());
            element.SetAttribute("transitionType", ((int)triggerZonedSceneEffect.getTransitionType()).ToString());
            if (triggerZonedSceneEffect.DestinyScale >= 0)
            {
                element.SetAttribute("scale", triggerZonedSceneEffect.DestinyScale.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "trigger-zoned-scene";
        }
    }
}