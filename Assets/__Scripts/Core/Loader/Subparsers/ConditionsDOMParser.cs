using UnityEngine;
using System.Collections;
using System;
using System.Xml;

namespace uAdventure.Core
{
    /// <summary>
    /// This parser ir an adapter for the original subparser
    /// </summary>
    [DOMParser("condition")]
    [DOMParser(typeof(Conditions))]
    public class ConditionsDOMParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var output = new Conditions();
            new ConditionSubParser_(output, parameters[0] as Chapter).ParseElement(element);
            return output;
        }
    }
}