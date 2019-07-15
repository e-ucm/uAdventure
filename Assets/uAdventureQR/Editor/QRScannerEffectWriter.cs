using uAdventure.Editor;
using System.Xml;

namespace uAdventure.QR
{
    [DOMWriter(typeof(QRScannerEffect))]
    public class QRScannerEffectWriter : ParametrizedDOMWriter
    {
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var qrPromptEffect = target as QRScannerEffect;
            var elem = node as XmlElement;

            elem.SetAttribute("selection-type", qrPromptEffect.SelectionType.ToString());
            AddNode(node, "message", qrPromptEffect.ScannerMessage);
            foreach (var qr in qrPromptEffect.ValidIds)
            {
                AddNode(node, "qr-id", qr);
            }
        }

        protected override string GetElementNameFor(object target)
        {
            return "qr-scanner";
        }
    }
}