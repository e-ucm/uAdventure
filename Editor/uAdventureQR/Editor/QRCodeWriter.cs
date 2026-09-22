using UnityEngine;
using System.Collections;
using uAdventure.Editor;
using System;
using System.Xml;

namespace uAdventure.QR
{
    [DOMWriter(typeof(QR))]
    public class QRCodeWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var element = node as XmlElement;
            var qr = target as QR;
            element.SetAttribute("id", qr.Id);
            AddNode(element, "content", qr.Content);
            AddNode(element, "documentation", qr.Documentation);
            DOMWriterUtility.DOMWrite(element, qr.Conditions);
            DOMWriterUtility.DOMWrite(element, qr.Effects);
        }

        protected override string GetElementNameFor(object target)
        {
            return "qr-code";
        }
    }
}