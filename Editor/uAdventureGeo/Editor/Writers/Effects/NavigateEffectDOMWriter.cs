using UnityEngine;
using System.Collections;

using uAdventure.Editor;
using System;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMWriter(typeof(NavigateEffect))]
    public class NavigateEffectDOMWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var elem = node as XmlElement;
            var navEff = target as NavigateEffect;

            elem.SetAttribute("navigation-type", navEff.NavigationType.ToString());

            foreach(var step in navEff.Steps)
            {
                var newElement = AddNode(node, "step", step.Reference);
                newElement.SetAttribute("locks", step.LockNavigation.ToString());
            }

        }

        protected override string GetElementNameFor(object target)
        {
            return "navigate";
        }
    }
}