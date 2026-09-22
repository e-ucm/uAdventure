using System.Xml;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    [DOMWriter(typeof(TrackerConfig))]
    public class TrackerConfigDOMWriter : ParametrizedDOMWriter
    {

        public TrackerConfigDOMWriter()
        {
        }

        protected override string GetElementNameFor(object target)
        {
            return "tracker";
        }

        /**
         * Returns the DOM element for the chapter
         * 
         * @param chapter
         *            Chapter data to be written
         * @return DOM element with the chapter data
         */
        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var trackerConfig = target as TrackerConfig;

            //Tracker config element
            XmlElement trackerConfigElement = node as XmlElement;

            trackerConfigElement.SetAttribute("rawCopy", trackerConfig.getRawCopy() ? "yes" : "no");

            trackerConfigElement.SetAttribute("storageType", trackerConfig.getStorageType().ToString());
            trackerConfigElement.SetAttribute("traceFormat", trackerConfig.getTraceFormat().ToString());

            trackerConfigElement.SetAttribute("host", trackerConfig.getHost());
            trackerConfigElement.SetAttribute("trackingCode", trackerConfig.getTrackingCode());
            trackerConfigElement.SetAttribute("flushInterval", trackerConfig.getFlushInterval().ToString());

            trackerConfigElement.SetAttribute("debug", trackerConfig.getDebug() ? "yes" : "no");
        }
    }
}