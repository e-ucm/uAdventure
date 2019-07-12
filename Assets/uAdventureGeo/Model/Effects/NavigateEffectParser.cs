using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMParser("navigate")]
    [DOMParser(typeof(NavigateEffect))]
    public class NavigateEffectParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var navEff = new NavigateEffect();

            navEff.NavigationType = element.GetAttribute("navigation-type").ToEnum<NavigationType>();
            foreach(var stepNode in element.SelectNodes("step"))
            {
                var stepElem = stepNode as XmlElement;
                navEff.Steps.Add(new NavigationStep(stepElem.InnerText, bool.Parse(stepElem.GetAttribute("locks"))));
            }

            return navEff;
        }
    }
}