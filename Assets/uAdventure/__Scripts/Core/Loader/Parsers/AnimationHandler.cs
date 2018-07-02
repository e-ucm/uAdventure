using System.Xml;
using uAdventure.Runner;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class AnimationHandler : XmlHandler<Animation>
    {

        public AnimationHandler(ResourceManager resourceManager, List<Incidence> incidences) : base(resourceManager, incidences)
        {
        }

        protected override Animation CreateObject()
        {
            return new Animation("");
        }

        protected override Animation ParseXml(XmlDocument doc)
        {
            XmlElement element = doc.DocumentElement;
            XmlElement animationNode = element.SelectSingleNode("/animation") as XmlElement;
            if (animationNode == null)
            {
                return null;
            }

            var animation = new Animation(animationNode.GetAttribute("id") ?? "newAnimationId");
            animation.getFrames().Clear();
            animation.getTransitions().Clear();

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

            return animation;
        }
    }
}