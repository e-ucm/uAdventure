using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("character")]
	[DOMParser(typeof(NPC))]
	public class CharacterSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			// PARAMETERS
			Chapter chapter = parameters [0] as Chapter;

			NPC npc = new NPC(element.GetAttribute("id"));

			List<Description> descriptions = new List<Description>();

			// DOCUMENTATION
			var doc = element.SelectSingleNode("documentation");
			if (doc != null) npc.setDocumentation(doc.InnerText);
			
			// DESCRIPTIONS
			npc.setDescriptions(DOMParserUtility.DOMParse <Description> (element.SelectNodes("description"), parameters).ToList ());

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				npc.addResources (res);

			// ACTIONS
			npc.setActions (DOMParserUtility.DOMParse<Action>(element.SelectSingleNode("actions").ChildNodes, parameters).ToList());

			// CONVERSATIONS
			foreach (XmlElement conversation in element.SelectNodes("conversation-ref"))
			{
				string idTarget = conversation.GetAttribute("idTarget") ?? "";

				var conversationReference = new ConversationReference(idTarget);
				conversationReference.setConditions(
					DOMParserUtility.DOMParse<Conditions> (conversation.SelectSingleNode("condition") as XmlElement, parameters) ?? new Conditions());
				conversationReference.setDocumentation(conversation.SelectSingleNode("documentation").InnerText);

				Action action = new Action(Action.TALK_TO);
				action.setConditions(conversationReference.getConditions());
				action.setDocumentation(conversationReference.getDocumentation());
				TriggerConversationEffect effect = new TriggerConversationEffect(conversationReference.getTargetId());
				action.getEffects().add(effect);
				npc.addAction(action);
			}

			// CONVERSATION COLORS
			var textcolor = element.SelectSingleNode ("textcolor") as XmlElement;
			if(textcolor != null)
			{
				npc.setShowsSpeechBubbles("yes".Equals(textcolor.GetAttribute("showsSpeechBubble")));
				npc.setBubbleBkgColor(textcolor.GetAttribute("bubbleBkgColor") ?? npc.getBubbleBkgColor ());
				npc.setBubbleBorderColor(textcolor.GetAttribute("bubbleBorderColor") ?? npc.getBubbleBorderColor ());

				var frontcolor = textcolor.SelectSingleNode("frontcolor") as XmlElement;
				if (frontcolor != null) npc.setTextFrontColor(frontcolor.GetAttribute("color") ?? "");

				var bordercolor = textcolor.SelectSingleNode("bordercolor") as XmlElement;
				if (bordercolor != null) npc.setTextBorderColor(bordercolor.GetAttribute("color") ?? "");
			}

			// VOICE
			var voice = element.SelectSingleNode("voice") as XmlElement;
			if (voice != null)
			{
				npc.setAlwaysSynthesizer("yes".Equals (voice.GetAttribute("synthesizeAlways")));
				npc.setVoice(voice.GetAttribute("name") ?? "");
			}

			return npc;
		}
    }
}