using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("transition")]
	[DOMParser(typeof(Transition))]
	public class TransitionSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			Transition transition = new Transition();

			switch(element.GetAttribute("type")){
				case "none": 		transition.setType(Transition.TYPE_NONE); break;
				case "fadein": 		transition.setType(Transition.TYPE_FADEIN); break;
				case "vertical": 	transition.setType(Transition.TYPE_VERTICAL); break;
				case "horizontal": 	transition.setType(Transition.TYPE_HORIZONTAL); break;
			}

			transition.setTime(ExParsers.ParseDefault (element.GetAttribute("time"), 0));
            return transition;
        }
    }
}