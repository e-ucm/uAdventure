using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;
using System.Xml;

namespace uAdventure.QR
{
    [DOMParser(typeof(QR))]
    [DOMParser("qr-code")]
    public class QRCodeParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var qr = new QR(element.Attributes["id"].Value);
            qr.Content = element.SelectSingleNode("content").InnerText;
            qr.Documentation = element.SelectSingleNode("documentation").InnerText;
            qr.Documentation = element.SelectSingleNode("documentation").InnerText;

			qr.Conditions = DOMParserUtility.DOMParse (element.SelectSingleNode("condition"), parameters) as Conditions ?? new Conditions();
			qr.Effects 	  = DOMParserUtility.DOMParse (element.SelectSingleNode("effect"), parameters) 	  as Effects ?? new Effects();

            return qr;
        }
    }
}