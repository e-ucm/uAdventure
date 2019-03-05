using UnityEngine;
using System.Collections;
using uAdventure.Editor;
using System;
using System.Xml;

namespace uAdventure.QR
{
    [DOMWriter(typeof(QRPromptEffect))]
    public class QRPromtEffectWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var qrPromptEffect = target as QRPromptEffect;
            var elem = node as XmlElement;

            elem.SetAttribute("selection-type", qrPromptEffect.SelectionType.ToString());
            AddNode(node, "message", qrPromptEffect.PromptMessage);
            foreach (var qr in qrPromptEffect.ValidIds)
            {
                AddNode(node, "qr-id", qr);
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "qr-prompt";
        }
    }
}