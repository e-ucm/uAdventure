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
            int x = 0, y = 0, width = 0, height = 0;

			x 		= int.Parse(element.GetAttribute("x") ?? "0");
			y 		= int.Parse(element.GetAttribute("y") ?? "0");
			width 	= int.Parse(element.GetAttribute("width") ?? "0");
			height	= int.Parse(element.GetAttribute("height") ?? "0");

            Barrier barrier = new Barrier(generateId(), x, y, width, height);

            if (element.SelectSingleNode("documentation") != null)
                barrier.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			barrier.setConditions (DOMParserUtility.DOMParse (element.SelectNodes("condition"), parameters) as Conditions ?? new Conditions());

			return barrier;
        }

		private string generateId()
		{
			return "barrier_" + Guid.NewGuid ().ToString ("N");
		}

    }
}