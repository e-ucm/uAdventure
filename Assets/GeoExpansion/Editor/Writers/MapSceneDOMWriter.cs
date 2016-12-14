using UnityEngine;
using System.Collections;
using System;

using uAdventure.Editor;
using System.Xml;

namespace uAdventure.Geo
{
    [DOMWriter(typeof(MapScene))]
    public class MapSceneDOMWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            throw new NotImplementedException();
        }

        protected override string GetElementNameFor(object target)
        {
            throw new NotImplementedException();
        }
    }
}

