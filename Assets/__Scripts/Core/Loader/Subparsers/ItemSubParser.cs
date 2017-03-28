using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace uAdventure.Core
{
	[DOMParser("object")]
	[DOMParser(typeof(Item))]
	public class ItemSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
        {
			Item parsedObject = new Item(element.GetAttribute("id") ?? "");

            if (element.SelectSingleNode("documentation") != null)
                parsedObject.setDocumentation(element.SelectSingleNode("documentation").InnerText);
			
			switch(element.GetAttribute("behaviour")){
				case "atrezzo": parsedObject.setBehaviour(Item.BehaviourType.ATREZZO); break;
				case "irst-action": parsedObject.setBehaviour(Item.BehaviourType.FIRST_ACTION); break;
				default: parsedObject.setBehaviour(Item.BehaviourType.NORMAL); break;
			}

			parsedObject.setReturnsWhenDragged("yes".Equals (element.GetAttribute("returnsWhenDragged") ?? "yes"));
			parsedObject.setResourcesTransitionTime(ExParsers.ParseDefault(element.GetAttribute("resources-transition-time"), 0L));

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				parsedObject.addResources (res);

			// DESCRIPTIONS
			parsedObject.setDescriptions(DOMParserUtility.DOMParse<Description>(element.SelectNodes ("description")).ToList());

			// ACTIONS
			foreach(var res in DOMParserUtility.DOMParse <Action> (element.SelectSingleNode("actions").ChildNodes, parameters))
				parsedObject.addAction (res);

			return parsedObject;
        }
    }
}