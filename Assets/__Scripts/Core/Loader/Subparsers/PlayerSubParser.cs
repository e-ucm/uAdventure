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
            Player player = new Player();

            if (element.SelectSingleNode("documentation") != null)
                player.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				player.addResources (res);

            // TEXT COLORS
            CharacterSubParser.ParseConversationColors(player, element);

            // VOICE
            CharacterSubParser.ParseVoice(player, element);

            // DESCRIPTIONS
			player.setDescriptions(DOMParserUtility.DOMParse<Description>(element.SelectNodes ("description"), parameters).ToList());

			// TODO Find a way to set this
			var chapter = parameters [0] as Chapter;
            chapter.setPlayer(player);
			return null;
        }
    }
}