using UnityEngine;
using System.Collections;
using System.Xml;
using System;

namespace uAdventure.Core
{
    public class AnimationHandler_
    {
        /**
         * Animation being read.
         */
        private Animation animation;

        private ImageLoaderFactory factory;

        public AnimationHandler_(InputStreamCreator isCreator, ImageLoaderFactory imageloader)
        {
            this.factory = imageloader;
        }

        public void Parse(string path_)
        {
            XmlDocument xmld = new XmlDocument();

            if (path_.EndsWith(".eaa"))
                path_ += ".xml";
            else if (!path_.EndsWith(".eaa.xml"))
                path_ += ".eaa.xml";

            xmld.Load(path_);

            XmlElement element = xmld.DocumentElement;

            string tmpArgVal;

            XmlNode animationNode = element.SelectSingleNode("/animation");

            tmpArgVal = animationNode.Attributes["id"].Value;
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                animation = new Animation(tmpArgVal, factory);
                animation.getFrames().Clear();
                animation.getTransitions().Clear();
            }

			animation.setSlides("yes".Equals (animationNode.Attributes["slides"].Value));
			animation.setUseTransitions("yes".Equals (animationNode.Attributes["usetransitions"].Value));

            if (element.SelectSingleNode("documentation") != null)
                animation.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			// FRAMES
			foreach (var frame in DOMParserUtility.DOMParse<Frame>(element.SelectNodes("/animation/frame"), animation.getImageLoaderFactory ()))
				animation.addFrame (frame);

			// TRANSITIONS
			foreach (var transition in DOMParserUtility.DOMParse<Transition>(element.SelectNodes("/animation/transition")))
				animation.getTransitions ().Add (transition);


			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("/animation/resources")))
				animation.addResources(res);

        }

        public Animation getAnimation()
        {
            return animation;
        }
    }
}