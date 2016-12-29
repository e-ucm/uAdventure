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

            ConditionSubParser_ conditionsParser = new ConditionSubParser_(qr.Conditions, (Chapter)parameters[0]);
            var conditions = element.SelectNodes("condition");
            foreach (XmlElement cond in conditions)
            {
                conditionsParser.ParseElement(cond);
            }

            EffectSubParser_ effectsSubParser = new EffectSubParser_(qr.Effects, (Chapter)parameters[0]);
            var effects = element.SelectNodes("effect");
            foreach (XmlElement effect in effects)
            {
                effectsSubParser.ParseElement(effect);
            }

            return qr;
        }
    }
}