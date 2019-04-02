using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("transition")]
	[DOMParser(typeof(Transition))]
	public class TransitionSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			var transition = new Transition();

			switch(element.GetAttribute("type")){
				case "none": 		transition.setType(TransitionType.NoTransition); break;
				case "fadein": 		transition.setType(TransitionType.FadeIn); break;
				case "vertical": 	transition.setType(TransitionType.TopToBottom); break;
				case "horizontal": 	transition.setType(TransitionType.LeftToRight); break;
                default:
                    transition.setType((TransitionType) ExParsers.ParseDefault(element.GetAttribute("type"), 0));
                    break;
			}

			transition.setTime(ExParsers.ParseDefault (element.GetAttribute("time"), 0));
            return transition;
        }
    }
}