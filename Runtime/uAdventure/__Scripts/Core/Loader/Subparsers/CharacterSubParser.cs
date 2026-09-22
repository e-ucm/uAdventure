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
			NPC npc = new NPC(element.GetAttribute("id"));

			// DOCUMENTATION
			var doc = element.SelectSingleNode("documentation");
			if (doc != null) npc.setDocumentation(doc.InnerText);

            switch (element.GetAttribute("behaviour"))
            {
                case "atrezzo": npc.setBehaviour(Item.BehaviourType.ATREZZO); break;
                case "first-action": npc.setBehaviour(Item.BehaviourType.FIRST_ACTION); break;
                default: npc.setBehaviour(Item.BehaviourType.NORMAL); break;
            }

            // DESCRIPTIONS
            npc.setDescriptions(DOMParserUtility.DOMParse <Description> (element.SelectNodes("description"), parameters).ToList ());

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				npc.addResources (res);

			// ACTIONS
			var actionsNode = element.SelectSingleNode("actions");
			if(actionsNode != null)
				npc.setActions (DOMParserUtility.DOMParse<Action>(actionsNode.ChildNodes, parameters).ToList());

			// CONVERSATIONS
			foreach (XmlElement conversation in element.SelectNodes("conversation-ref"))
			{
				string idTarget = conversation.GetAttribute("idTarget");

				var conversationReference = new ConversationReference(idTarget);
				conversationReference.setConditions(
					DOMParserUtility.DOMParse<Conditions> (conversation.SelectSingleNode("condition") as XmlElement, parameters) ?? new Conditions());
				conversationReference.setDocumentation(conversation.SelectSingleNode("documentation").InnerText);

				Action action = new Action(Action.TALK_TO);
				action.setConditions(conversationReference.getConditions());
				action.setDocumentation(conversationReference.getDocumentation());
				TriggerConversationEffect effect = new TriggerConversationEffect(conversationReference.getTargetId());
				action.getEffects().Add(effect);
				npc.addAction(action);
			}

            // CONVERSATION COLORS
            ParseConversationColors(npc, element);

            // VOICE
            ParseVoice(npc, element);

			return npc;
		}

        public static void ParseConversationColors(NPC npc, XmlElement element)
        { 
            var textcolor = element.SelectSingleNode("textcolor") as XmlElement;
            if (textcolor != null)
            {
                npc.setShowsSpeechBubbles(ExString.EqualsDefault(textcolor.GetAttribute("showsSpeechBubble"), "yes", npc.getShowsSpeechBubbles()));
                npc.setBubbleBkgColor(ExParsers.ParseDefault(textcolor.GetAttribute("bubbleBkgColor"), npc.getBubbleBkgColor()));
                npc.setBubbleBorderColor(ExParsers.ParseDefault(textcolor.GetAttribute("bubbleBorderColor"), npc.getBubbleBorderColor()));

                var frontcolor = textcolor.SelectSingleNode("frontcolor") as XmlElement;
                if (frontcolor != null) npc.setTextFrontColor(ExParsers.ParseDefault(frontcolor.GetAttribute("color"), npc.getTextFrontColor()));

                var bordercolor = textcolor.SelectSingleNode("bordercolor") as XmlElement;
                if (bordercolor != null) npc.setTextBorderColor(ExParsers.ParseDefault(bordercolor.GetAttribute("color"), npc.getTextBorderColor()));
            }
        }

        public static void ParseVoice(NPC npc, XmlElement element)
        {
            var voice = element.SelectSingleNode("voice") as XmlElement;
            if (voice != null)
            {
                npc.setAlwaysSynthesizer(ExString.EqualsDefault(voice.GetAttribute("synthesizeAlways"), "yes", npc.isAlwaysSynthesizer()));
                npc.setVoice(voice.GetAttribute("name"));
            }
        }
    }
}