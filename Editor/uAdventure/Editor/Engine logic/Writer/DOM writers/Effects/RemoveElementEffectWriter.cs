using System.Xml;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.QR
{
    [DOMWriter(typeof(RemoveElementEffect))]
    public class RemoveElementEffectWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var removeElementEffect = target as RemoveElementEffect;
            var elem = node as XmlElement;

            elem.SetAttribute("idTarget", removeElementEffect.getTargetId());
        }

        protected override string GetElementNameFor(object target)
        {
            return "remove-element";
        }
    }
}