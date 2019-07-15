using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.QR
{
    [DOMParser(typeof(QRScannerEffect))]
    [DOMParser("qr-scanner")]
    public class QRScannerEffectParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var qrPrompt = new QRScannerEffect();

            qrPrompt.SelectionType = element.Attributes["selection-type"].Value.ToEnum<QRScannerEffect.ListType>();
            qrPrompt.ScannerMessage = element.SelectSingleNode("message").InnerText;

            foreach (var listElem in element.SelectNodes("qr-id"))
                qrPrompt.ValidIds.Add((listElem as XmlElement).InnerText);

            return qrPrompt;
        }
    }
}