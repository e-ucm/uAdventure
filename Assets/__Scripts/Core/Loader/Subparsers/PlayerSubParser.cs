using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("player")]
	[DOMParser(typeof(Player))]
	public class PlayerSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
        {
            string tmpArgVal;

            Player player = new Player();

            if (element.SelectSingleNode("documentation") != null)
                player.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				player.addResources (res);

			var textcolor = element.SelectSingleNode("textcolor") as XmlElement;
			if(textcolor != null)
            {
				tmpArgVal = textcolor.GetAttribute("showsSpeechBubble");
                if (!string.IsNullOrEmpty(tmpArgVal))
					player.setShowsSpeechBubbles(tmpArgVal.Equals("yes"));

				tmpArgVal = textcolor.GetAttribute("bubbleBkgColor");
                if (!string.IsNullOrEmpty(tmpArgVal))
                    player.setBubbleBkgColor(tmpArgVal);

				tmpArgVal = textcolor.GetAttribute("bubbleBorderColor");
                if (!string.IsNullOrEmpty(tmpArgVal))
                    player.setBubbleBorderColor(tmpArgVal);

				var frontcolor = textcolor.SelectSingleNode("frontcolor") as XmlElement;
				if(frontcolor != null)
					player.setTextFrontColor(frontcolor.GetAttribute("color") ?? "");

				var bordercolor = textcolor.SelectSingleNode("bordercolor") as XmlElement;
				if(bordercolor != null) 
					player.setTextBorderColor(bordercolor.GetAttribute("color") ?? "");
            }

			var voice = element.SelectSingleNode("voice") as XmlElement;
			if (voice != null)
            {
				player.setAlwaysSynthesizer("yes".Equals (voice.GetAttribute("synthesizeAlways")));
				player.setVoice(voice.GetAttribute("name") ?? "");
            }

			player.setDescriptions(DOMParserUtility.DOMParse<Description>(element.SelectNodes ("description")).ToList());

			// TODO ??????
			var chapter = parameters [0] as Chapter;
            chapter.setPlayer(player);
			return null;
        }
    }
}