using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.QR
{
    [DOMParser(typeof(QRPromptEffect))]
    [DOMParser("qr-prompt")]
    public class QRPromptEffectParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var qrPrompt = new QRPromptEffect();

            qrPrompt.SelectionType = element.Attributes["selection-type"].Value.ToEnum<QRPromptEffect.ListType>();
            qrPrompt.PromptMessage = element.SelectSingleNode("message").InnerText;

            foreach (var listElem in element.SelectNodes("qr-id"))
                qrPrompt.ValidIds.Add((listElem as XmlElement).InnerText);

            return qrPrompt;
        }
    }
}