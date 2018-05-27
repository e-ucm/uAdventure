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
            if (resourceManager.getAnimationsCache().ContainsKey(path_))
            {
                animation = resourceManager.getAnimationsCache()[path_];
                return;
            }

            XmlDocument xmld = new XmlDocument();

            string xml = resourceManager.getText(path_);
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

                do
                {
                    ruta = path_ + "_" + intToStr(num);

                    img = resourceManager.getImage(ruta);
                    if (img)
                    {
                        var newFrame = new Frame(ruta, 100, false);
                        animation.addFrame(newFrame);
                        animation.getTransitions().Add(new Transition());
                        num++;
                    }

                } while (img);
            }

            if(animation != null)
            {
                resourceManager.getAnimationsCache()[path_] = animation;
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