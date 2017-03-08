using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace uAdventure.Core
{
    public class PlayerSubParser_ : Subparser_
    {
        /**
         * Player being parsed
         */
        private Player player;

        /**
         * Current resources being parsed
         */
        private ResourcesUni currentResources;

        /**
         * Current conditions being parsed
         */
        private Conditions currentConditions;

        private List<Description> descriptions;

        private Description description;

        public PlayerSubParser_(Chapter chapter) : base(chapter)
        {
            descriptions = new List<Description>();
        }

        public override void ParseElement(XmlElement element)
        {
            XmlNodeList
                resourcess = element.SelectNodes("resources"),
                assets,
                conditions,
                textcolors = element.SelectNodes("textcolor"),
                bordercolos,
                frontcolors,
                voices = element.SelectNodes("voice"),
                descriptionss = element.SelectNodes("description");

            string tmpArgVal;

            player = new Player();
            descriptions = new List<Description>();
            player.setDescriptions(descriptions);

            if (element.SelectSingleNode("documentation") != null)
                player.setDocumentation(element.SelectSingleNode("documentation").InnerText);

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

                player.addResources(currentResources);
            }

            foreach (XmlElement el in textcolors)
            {
                tmpArgVal = el.GetAttribute("showsSpeechBubble");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    player.setShowsSpeechBubbles(tmpArgVal.Equals("yes"));
                }

                tmpArgVal = el.GetAttribute("bubbleBkgColor");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    player.setBubbleBkgColor(tmpArgVal);
                }

                tmpArgVal = el.GetAttribute("bubbleBorderColor");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    player.setBubbleBorderColor(tmpArgVal);
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

                    player.setTextFrontColor(color);
                }

                bordercolos = el.SelectNodes("bordercolor");
                foreach (XmlElement ell in bordercolos)
                {
                    string color = "";

                    tmpArgVal = ell.GetAttribute("color");
                    if (!string.IsNullOrEmpty(tmpArgVal))
                    {
                        color = tmpArgVal;
                    }

                    player.setTextBorderColor(color);
                }
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

                player.setAlwaysSynthesizer(alwaysSynthesizer);
                player.setVoice(voice);
            }


            foreach (XmlElement el in descriptionss)
            {
                description = new Description();
                new DescriptionsSubParser_(description, chapter).ParseElement(el);
                this.descriptions.Add(description);
            }

            chapter.setPlayer(player);
        }
    }
}