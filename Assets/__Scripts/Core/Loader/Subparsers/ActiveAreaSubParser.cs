using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("examine","grab","use","talk-to","use-with","give-to","drag-to","custom","custom-interact")]
	[DOMParser(typeof(Action))]
	public class ActiveAreaSubParser : IDOMParser
    {
        private string generateId()
        {
			return "area_" + Guid.NewGuid ().ToString ("N");
        }

		public object DOMParse(XmlElement element, params object[] parameters)
		{
			XmlNodeList
				points = element.SelectNodes ("point"),
				descriptions = element.SelectNodes ("description"),
				actionss = element.SelectNodes ("actions");
			XmlElement conditions = element.SelectSingleNode("condition") as XmlElement;

            string tmpArgVal;

            int x = 0, y = 0, width = 0, height = 0;
            string id = null;
            bool rectangular = true;
            int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
            bool hasInfluence = false;

			rectangular = (element.GetAttribute ("rectangular") ?? "yes").Equals ("yes");
			x 		= int.Parse(element.GetAttribute("x") ?? "0");
			y 		= int.Parse(element.GetAttribute("y") ?? "0");
			width 	= int.Parse(element.GetAttribute("width") ?? "0");
			height	= int.Parse(element.GetAttribute("height") ?? "0");
			id 		= element.GetAttribute("id") ?? "";

			hasInfluence = "yes".Equals (element.GetAttribute ("hasInfluenceArea"));
			influenceX = int.Parse(element.GetAttribute("influenceX") ?? "0");
			influenceY = int.Parse(element.GetAttribute("influenceY") ?? "0");
			influenceWidth = int.Parse(element.GetAttribute("influenceWidth") ?? "0");
			influenceHeight = int.Parse(element.GetAttribute("influenceHeight") ?? "0");

            ActiveArea activeArea = new ActiveArea((id == null ? generateId() : id), rectangular, x, y, width, height);
            if (hasInfluence)
            {
                InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                activeArea.setInfluenceArea(influenceArea);
            }

            if (element.SelectSingleNode("documentation") != null)
                activeArea.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			activeArea.setDescriptions(DOMParserUtility.DOMParse<Description> (descriptions, parameters).ToList());

            foreach (XmlElement el in points)
            {
                if (activeArea != null)
                {
                    int x_ = 0, y_ = 0;

					x_ 		= int.Parse(el.GetAttribute("x") ?? "0");
					y_ 		= int.Parse(el.GetAttribute("y") ?? "0");

                    Vector2 point = new Vector2(x_, y_);
                    activeArea.addVector2(point);
                }
            }

			activeArea.setActions (DOMParserUtility.DOMParse <Action>(actionss, parameters).ToList ());
			activeArea.setConditions(DOMParserUtility.DOMParse (conditions, parameters) as Conditions ?? new Conditions ());

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