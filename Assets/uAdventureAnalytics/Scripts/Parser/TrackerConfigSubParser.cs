using System.Xml;
using uAdventure.Core;

namespace uAdventure.Analytics
{
    [DOMParser("tracker")]
    [DOMParser(typeof(TrackerConfig))]
    public class TrackerConfigSubParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            var trackerConfig = new TrackerConfig();

            trackerConfig.setRawCopy(element.GetAttribute("rawCopy") == "yes");
            trackerConfig.setHost(element.GetAttribute("host"));
            trackerConfig.setTrackingCode(element.GetAttribute("trackingCode"));
            trackerConfig.setDebug(element.GetAttribute("debug") == "yes");
            trackerConfig.setFlushInterval(ExParsers.ParseDefault(element.GetAttribute("flushInterval"), 3));

            if (element.HasAttribute("storageType"))
            {
                trackerConfig.setStorageType(ExParsers.ParseEnum<TrackerConfig.StorageType>(element.GetAttribute("storageType")));
            }

            if (element.HasAttribute("traceFormat"))
            {
                trackerConfig.setTraceFormat(ExParsers.ParseEnum<TrackerConfig.TraceFormat>(element.GetAttribute("traceFormat")));
            }

            return trackerConfig;
        }
    }
}
