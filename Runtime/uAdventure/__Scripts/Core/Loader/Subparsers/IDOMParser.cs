using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
    public interface IDOMParser
    {
        object DOMParse(XmlElement element, params object[] parameters);
    }
}

