using UnityEngine;
using System.Xml;

// TODO Possible unnecesary coupling
using uAdventure.Runner;
using System.Linq;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class AnimationHandler
    {
        /**
         * Animation being read.
         */
        private Animation animation;

        private readonly ResourceManager resourceManager;
        private readonly List<Incidence> incidences;

        public AnimationHandler(ResourceManager resourceManager, List<Incidence> incidences)
        {
            this.resourceManager = resourceManager;
            this.incidences = incidences;
        }

        public void Parse(string path)
        {
            string xml = resourceManager.getText(path);
            if (!string.IsNullOrEmpty(xml))
            {
                ParseXml(xml);
            }
        }

        public void ParseXml(string xml)
        {
            XmlDocument xmld = new XmlDocument();
            xmld.LoadXml(xml);

            if (!string.IsNullOrEmpty(xml))
            {
                xmld.LoadXml(xml);

                XmlElement element = xmld.DocumentElement;

                string tmpArgVal;

                XmlElement animationNode = element.SelectSingleNode("/animation") as XmlElement;

                tmpArgVal = animationNode.Attributes["id"].Value;
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    animation = new Animation(tmpArgVal);
                    animation.getFrames().Clear();
                    animation.getTransitions().Clear();
                }

                animation.setSlides(ExString.EqualsDefault(animationNode.GetAttribute("slides"), "yes", false));
                animation.setUseTransitions(ExString.EqualsDefault(animationNode.GetAttribute("usetransitions"),"yes", false));

                if (element.SelectSingleNode("documentation") != null)
                {
                    animation.setDocumentation(element.SelectSingleNode("documentation").InnerText);
                }

                // FRAMES
                foreach (var frame in DOMParserUtility.DOMParse<Frame>(element.SelectNodes("/animation/frame")))
                {
                    animation.addFrame(frame);
                }

                // TRANSITIONS
                foreach (var transition in DOMParserUtility.DOMParse<Transition>(element.SelectNodes("/animation/transition")))
                {
                    animation.getTransitions().Add(transition);
                }


                // RESOURCES
                foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(element.SelectNodes("/animation/resources")))
                {
                    animation.addResources(res);
                }
            }
        }

        public Animation GetAnimation()
        {
            return animation;
        }
    }
}