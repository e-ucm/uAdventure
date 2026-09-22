using UnityEngine;
using System.Collections;
using uAdventure.Editor;
using System;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMWriter(typeof(NavigationControlEffect))]
    public class NavigationControlEffectDOMWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var navControlEffect = target as NavigationControlEffect;
            var elem = node as XmlElement;

            elem.SetAttribute("type", navControlEffect.Type.ToString());

            switch (navControlEffect.Type)
            {
                case NavigationControlEffect.ControlType.Index:
                    elem.SetAttribute("index", navControlEffect.Index.ToString());
                    break;
                case NavigationControlEffect.ControlType.ReferenceId:
                    elem.SetAttribute("reference", navControlEffect.Reference);
                    break;
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "navigation-control";
        }
    }
}