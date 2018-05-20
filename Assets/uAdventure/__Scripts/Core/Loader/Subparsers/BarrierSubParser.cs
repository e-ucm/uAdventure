using UnityEngine;
using System.Collections;
using System.Xml;
using System;

namespace uAdventure.Core
{
	[DOMParser("barrier")]
	[DOMParser(typeof(Barrier))]
	public class BarrierSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
		{
            int x 		= ExParsers.ParseDefault(element.GetAttribute("x"), 0),
			    y 		= ExParsers.ParseDefault(element.GetAttribute("y"), 0),
			    width 	= ExParsers.ParseDefault(element.GetAttribute("width"), 0),
			    height	= ExParsers.ParseDefault(element.GetAttribute("height"), 0);

            Barrier barrier = new Barrier(generateId(), x, y, width, height);

            if (element.SelectSingleNode("documentation") != null)
                barrier.setDocumentation(element.SelectSingleNode("documentation").InnerText);
            
            barrier.setConditions (DOMParserUtility.DOMParse (element.SelectSingleNode("condition"), parameters) as Conditions ?? new Conditions());

            return barrier;
        }

		private string generateId()
		{
			return "barrier_" + Guid.NewGuid ().ToString ("N");
		}

    }
}