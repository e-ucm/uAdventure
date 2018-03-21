using uAdventure.Core;
using System.Xml;

namespace uAdventure.QR
{
    [DOMParser(typeof(RemoveElementEffect))]
    [DOMParser("remove-element")]
    public class RemoveElementEffectParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            return new RemoveElementEffect(element.GetAttribute("idTarget"));
        }
    }
}