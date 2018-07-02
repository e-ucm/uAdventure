using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("examine","grab","use","talk-to","use-with","give-to","drag-to","custom","custom-interact")]
	[DOMParser(typeof(ActiveArea))]
	public class ActiveAreaSubParser : IDOMParser
    {
        private string generateId()
        {
			return "area_" + Guid.NewGuid ().ToString ("N");
        }

		public object DOMParse(XmlElement element, params object[] parameters)
		{
			XmlNodeList points    = element.SelectNodes ("point"),
			descriptions          = element.SelectNodes ("description");
            XmlElement conditions = element.SelectSingleNode("condition") as XmlElement,
            actions               = element.SelectSingleNode("actions") as XmlElement,
            documentation         = element.SelectSingleNode("documentation") as XmlElement;

            string id             = ExString.Default(element.GetAttribute("id"), null);
            bool rectangular      = ExString.EqualsDefault(element.GetAttribute ("rectangular"), "yes", true);

			int x 		          = ExParsers.ParseDefault(element.GetAttribute("x"), 0),
			    y 		          = ExParsers.ParseDefault(element.GetAttribute("y"), 0),
			    width 	          = ExParsers.ParseDefault(element.GetAttribute("width"), 0),
			    height	          = ExParsers.ParseDefault(element.GetAttribute("height"), 0);

			bool hasInfluence     = ExString.EqualsDefault(element.GetAttribute("hasInfluenceArea"), "yes", false);
			int influenceX        = ExParsers.ParseDefault(element.GetAttribute("influenceX"), 0),
			    influenceY        = ExParsers.ParseDefault(element.GetAttribute("influenceY"), 0),
			    influenceWidth    = ExParsers.ParseDefault(element.GetAttribute("influenceWidth"), 0),
			    influenceHeight   = ExParsers.ParseDefault(element.GetAttribute("influenceHeight"), 0);


            var activeArea = new ActiveArea((id ?? generateId()), rectangular, x, y, width, height);

            switch (element.GetAttribute("behaviour"))
            {
                case "atrezzo":      activeArea.setBehaviour(Item.BehaviourType.ATREZZO);      break;
                case "first-action": activeArea.setBehaviour(Item.BehaviourType.FIRST_ACTION); break;
                default:             activeArea.setBehaviour(Item.BehaviourType.NORMAL);       break;
            }

            if (hasInfluence)
            {
                var influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                activeArea.setInfluenceArea(influenceArea);
            }

            if (documentation != null)
            {
                activeArea.setDocumentation(documentation.InnerText);
            }

			activeArea.setDescriptions(DOMParserUtility.DOMParse<Description> (descriptions, parameters).ToList());
            
            foreach (XmlElement el in points)
            {
                activeArea.addVector2(
                    new Vector2(ExParsers.ParseDefault(el.GetAttribute("x"), 0),
                                ExParsers.ParseDefault(el.GetAttribute("y"), 0)));
            }

            if (actions != null)
            {
                activeArea.setActions(DOMParserUtility.DOMParse<Action>(actions.ChildNodes, parameters).ToList());
            }
			if (conditions != null)
            {
                activeArea.setConditions(DOMParserUtility.DOMParse(conditions, parameters) as Conditions ?? new Conditions());
            }

			return activeArea;
        }

        //TODO: test if it's working
        //public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
        //{

        //        // If it is a effect tag, create new effects and switch the state
        //        else if (qName.Equals("effect"))
        //        {
        //            subParser = new EffectSubParser(currentEffects, chapter);
        //            subParsing = SUBPARSING_EFFECT;
        //        }
        //    }

    }
}