using UnityEngine;
using System.Collections;
using uAdventure.Geo;
using System;
using System.Xml;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(GeoAction), typeof(ExitAction), typeof(EnterAction), typeof(LookToAction), typeof(InspectAction))]
    public class GeoActionWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var geoAction = target as GeoAction;

            if(!geoAction.Conditions.IsEmpty())
                DOMWriterUtility.DOMWrite(node, geoAction.Conditions);

            if (!geoAction.Effects.IsEmpty())
                DOMWriterUtility.DOMWrite(node, geoAction.Effects);

            if (geoAction is ExitAction)
                FillNode(node, geoAction as ExitAction, options);

            if (geoAction is EnterAction)
                FillNode(node, geoAction as EnterAction, options);

            if (geoAction is LookToAction)
                FillNode(node, geoAction as LookToAction, options);

            if (geoAction is InspectAction)
                FillNode(node, geoAction as InspectAction, options);
        }

        protected void FillNode(XmlNode node, ExitAction target, params IDOMWriterParam[] options)
        {
            AddChild(node, "only-from-inside", target.OnlyFromInside.ToString());
        }

        protected void FillNode(XmlNode node, EnterAction target, params IDOMWriterParam[] options)
        {
            AddChild(node, "only-from-outside", target.OnlyFromOutside.ToString());

        }

        protected void FillNode(XmlNode node, LookToAction target, params IDOMWriterParam[] options)
        {
            AddChild(node, "inside", target.Inside.ToString());
            if (target.Center)
            {
                AddChild(node, "center", target.Center.ToString());
            }
            else
            {
                AddChild(node, "direction", target.Direction.x + " " + target.Direction.y);
            }

        }

        protected void FillNode(XmlNode node, InspectAction target, params IDOMWriterParam[] options)
        {
            AddChild(node, "inside", target.Inside.ToString());
        }

        private void AddChild(XmlNode parent, string name, string content)
        {
            var doc = Writer.GetDoc();
            var elem = doc.CreateElement(name);
            elem.InnerText = content;
            parent.AppendChild(elem);
        }

        protected override string GetElementNameFor(object target)
        {
            if (target is ExitAction)
                return "exit-action";
            if (target is EnterAction)
                return "enter-action";
            if (target is LookToAction)
                return "lookto-action";
            if (target is InspectAction)
                return "inspect-action";

            return "";
        }
    }

}
