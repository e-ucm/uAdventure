using UnityEngine;
using System.Collections;

using uAdventure.Runner;
using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMParser("navigation-control")]
    [DOMParser(typeof(NavigationControlEffect))]
    public class NavigationControlEffectParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var navControlEff = new NavigationControlEffect();

            navControlEff.Type = element.GetAttribute("type").ToEnum<NavigationControlEffect.ControlType>();
            switch (navControlEff.Type)
            {
                case NavigationControlEffect.ControlType.Index:
                    navControlEff.Index = int.Parse(element.GetAttribute("index"));
                    break;
                case NavigationControlEffect.ControlType.ReferenceId:
                    navControlEff.Reference = element.GetAttribute("reference");
                    break;
            }

            return navControlEff;
        }
    }
}

