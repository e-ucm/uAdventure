using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("frame")]
	[DOMParser(typeof(Frame))]
	public class FrameSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
        {
			Frame frame = new Frame();

            /*XmlNodeList
                assets = element.SelectNodes("next-scene");*/

			switch(element.GetAttribute("type")){
			case "image":
				frame.setType (Frame.TYPE_IMAGE);
				break;
			case "video":
				frame.setType (Frame.TYPE_VIDEO);
				break;
            }

            if (element.SelectSingleNode("documentation") != null)
                frame.setDocumentation(element.SelectSingleNode("documentation").InnerText);

            frame.setUri(element.GetAttribute("uri"));
			frame.setWaitforclick (ExString.EqualsDefault(element.GetAttribute ("waitforclick"), "yes", false));
			frame.setSoundUri(element.GetAttribute("soundUri"));

			var time = element.GetAttribute ("time");
			if(!string.IsNullOrEmpty (time)) frame.setTime(long.Parse(time));
			var maxsoundtime = element.GetAttribute ("maxSoundTime");
			if(!string.IsNullOrEmpty (maxsoundtime))frame.setMaxSoundTime(int.Parse(maxsoundtime));

			foreach (var resources in DOMParserUtility.DOMParse<ResourcesUni>(element.SelectNodes("resources"), parameters))
				frame.addResources(resources);

			return frame;
        }
    }
}