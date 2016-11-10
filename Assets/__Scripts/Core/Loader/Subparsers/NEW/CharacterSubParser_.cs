using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class CharacterSubParser_ : Subparser_
{

    /**
     * The character being read
     */
    private NPC npc;

    /**
     * Current resources being read
     */
    private ResourcesUni currentResources;

    /**
     * Current conversation reference being read
     */
    private ConversationReference conversationReference;

    /**
     * Current conditions being read
     */
    private Conditions currentConditions;

    private List<Description> descriptions;

    private Description description;

    public CharacterSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            descriptionss = element.SelectNodes("description"),
            assets,
            conditions,
            frontcolors,
            bordercolors,
            textcolors = element.SelectNodes("textcolor"),
            conversationsref = element.SelectNodes("conversation-ref"),
            voices = element.SelectNodes("voice"),
            actionss = element.SelectNodes("actions");

        string tmpArgVal;

        string characterId = element.GetAttribute("id");

        npc = new NPC(characterId);

        descriptions = new List<Description>();
        npc.setDescriptions(descriptions);

        if (element.SelectSingleNode("documentation") != null)
            npc.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        foreach (XmlElement el in resourcess)
        {
            currentResources = new ResourcesUni();
            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentResources.setName(el.GetAttribute(tmpArgVal));
            }

            assets = el.SelectNodes("asset");
            foreach (XmlElement ell in assets)
            {
                string type = "";
                string path = "";

                tmpArgVal = ell.GetAttribute("type");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    type = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("uri");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    path = tmpArgVal;
                }
                currentResources.addAsset(type, path);
            }

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentResources.setConditions(currentConditions);
            }

            npc.addResources(currentResources);
        }


        foreach (XmlElement el in textcolors)
        {
            tmpArgVal = el.GetAttribute("showsSpeechBubble");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                npc.setShowsSpeechBubbles(tmpArgVal.Equals("yes"));
            }

            tmpArgVal = el.GetAttribute("bubbleBkgColor");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                npc.setBubbleBkgColor(tmpArgVal);
            }

            tmpArgVal = el.GetAttribute("bubbleBorderColor");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                npc.setBubbleBorderColor(tmpArgVal);
            }

			frontcolors = el.SelectNodes("frontcolor");
            foreach (XmlElement ell in frontcolors)
            {
                string color = "";

                tmpArgVal = ell.GetAttribute("color");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    color = tmpArgVal;
                }

                npc.setTextFrontColor(color);
            }

			bordercolors = el.SelectNodes("bordercolor");
            foreach (XmlElement ell in bordercolors)
            {
                string color = "";

                tmpArgVal = ell.GetAttribute("color");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    color = tmpArgVal;
                }

                npc.setTextBorderColor(color);
            }
        }


        foreach (XmlElement el in conversationsref)
        {
            string idTarget = "";

            tmpArgVal = el.GetAttribute("idTarget");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                idTarget = tmpArgVal;
            }

            conversationReference = new ConversationReference(idTarget);

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);

                conversationReference.setConditions(currentConditions);
            }

            conversationReference.setDocumentation(el.SelectSingleNode("documentation").InnerText);

            Action action = new Action(Action.TALK_TO);
            action.setConditions(conversationReference.getConditions());
            action.setDocumentation(conversationReference.getDocumentation());
            TriggerConversationEffect effect = new TriggerConversationEffect(conversationReference.getTargetId());
            action.getEffects().add(effect);
            npc.addAction(action);
        }

        foreach (XmlElement el in voices)
        {
            string voice = "";
            string response;
            bool alwaysSynthesizer = false;

            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                voice = tmpArgVal;
            }

            tmpArgVal = el.GetAttribute("synthesizeAlways");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                response = tmpArgVal;
                if (response.Equals("yes"))
                    alwaysSynthesizer = true;
            }

            npc.setAlwaysSynthesizer(alwaysSynthesizer);
            npc.setVoice(voice);
        }

        foreach (XmlElement el in actionss)
        {
            new ActionsSubParser_(chapter, npc).ParseElement(el);
        }

        foreach (XmlElement el in descriptionss)
        {
            description = new Description();
            new DescriptionsSubParser_(description, chapter).ParseElement(el);
            this.descriptions.Add(description);
        }

        chapter.addCharacter(npc);
    }
}