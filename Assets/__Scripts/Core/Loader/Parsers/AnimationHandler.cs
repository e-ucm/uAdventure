using UnityEngine;
using System.Xml;

// TODO Possible unnecesary coupling
using uAdventure.Runner;
using System.Linq;

namespace uAdventure.Core
{
    public class AnimationHandler
    {
        /**
         * Animation being read.
         */
        private Animation animation;

        string directory = "";

        private ResourceManager resourceManager;

        public AnimationHandler(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public void Parse(string path_)
        {
            XmlDocument xmld = new XmlDocument();

            string xml = resourceManager.getText(path_);
            if (!string.IsNullOrEmpty(xml))
            {
                xmld.LoadXml(xml);

                XmlElement element = xmld.DocumentElement;

                string tmpArgVal;

                XmlNode animationNode = element.SelectSingleNode("/animation");

                tmpArgVal = animationNode.Attributes["id"].Value;
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    animation = new Animation(tmpArgVal);
                    animation.getFrames().Clear();
                    animation.getTransitions().Clear();
                }

                animation.setSlides("yes".Equals(animationNode.Attributes["slides"].Value));
                animation.setUseTransitions("yes".Equals(animationNode.Attributes["usetransitions"].Value));

                if (element.SelectSingleNode("documentation") != null)
                    animation.setDocumentation(element.SelectSingleNode("documentation").InnerText);

                // FRAMES
                foreach (var frame in DOMParserUtility.DOMParse<Frame>(element.SelectNodes("/animation/frame")))
                    animation.addFrame(frame);

                // TRANSITIONS
                foreach (var transition in DOMParserUtility.DOMParse<Transition>(element.SelectNodes("/animation/transition")))
                    animation.getTransitions().Add(transition);


                // RESOURCES
                foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(element.SelectNodes("/animation/resources")))
                    animation.addResources(res);
            }
            else
            {
                animation = new Animation(path_.Split('/').Last());
                animation.getFrames().Clear();
                animation.getTransitions().Clear();

                xmld = new XmlDocument();
                int num = 1;
                string ruta = path_;

                Texture2D img = null;
                string extension = string.Empty;

                do
                {
                    ruta = path_ + "_" + intToStr(num);

                    img = resourceManager.getImage(ruta);
                    if (img)
                    {
                        var newFrame = new Frame(ruta, 100, false);
                        animation.addFrame(newFrame);
                        num++;
                    }

                } while (img);
            }
        }

        public Animation getAnimation()
        {
            return animation;
        }


        private static string intToStr(int number)
        {
            if (number < 10)
                return "0" + number;
            else
                return number.ToString();
        }
    }
}