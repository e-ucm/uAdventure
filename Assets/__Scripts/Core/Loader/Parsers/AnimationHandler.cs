using UnityEngine;
using System.Xml;

// TODO Possible unnecesary coupling
using uAdventure.Runner;
using System.Text.RegularExpressions;
using System.Linq;

namespace uAdventure.Core
{
    public class AnimationHandler_
    {
        /**
         * Animation being read.
         */
        private Animation animation;

        private ImageLoaderFactory factory;

        string directory = "";

        public AnimationHandler_(ImageLoaderFactory imageloader)
        {
            this.factory = imageloader;
        }
        
        private static string[] extensions = { ".png", ".jpg", ".jpeg" };

        public void Parse(string path_)
        {
            XmlDocument xmld = new XmlDocument();

            string xml = ResourceManager.Instance.getText(path_);
            if (!string.IsNullOrEmpty(xml))
            {
                xmld.LoadXml(xml);

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

                animation.setSlides("yes".Equals(animationNode.Attributes["slides"].Value));
                animation.setUseTransitions("yes".Equals(animationNode.Attributes["usetransitions"].Value));

                if (element.SelectSingleNode("documentation") != null)
                    animation.setDocumentation(element.SelectSingleNode("documentation").InnerText);

                // FRAMES
                foreach (var frame in DOMParserUtility.DOMParse<Frame>(element.SelectNodes("/animation/frame"), animation.getImageLoaderFactory()))
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
                animation = new Animation(path_.Split('/').Last(), factory);
                animation.getFrames().Clear();
                animation.getTransitions().Clear();

                xmld = new XmlDocument();
                int num = 1;
                string ruta = path_;
                var type = ResourceManager.Instance.getLoadingType();

                Sprite img = null;
                string extension = string.Empty;

                do
                {
                    // Put the extension
                    if (type == ResourceManager.LoadingType.SYSTEM_IO)
                    {
                        foreach (string tryExtension in extensions)
                        {
                            if (System.IO.File.Exists(path_ + tryExtension))
                            {
                                extension = tryExtension;
                                break;
                            }
                        }
                    }

                    ruta = path_ + "_" + intToStr(num) + extension;

                    img = factory.getImageFromPath(ruta);
                    if (img)
                    {
                        if(type == ResourceManager.LoadingType.RESOURCES_LOAD)
                        {
                            if (ruta.StartsWith("Assets/"))
                                ruta = ruta.Substring("Assets/".Length);
                            if (ruta.StartsWith("Resources/"))
                                ruta = ruta.Substring("Resources/".Length);
                        }

                        var newFrame = new Frame(animation.getImageLoaderFactory(), ruta, 1000, false);
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