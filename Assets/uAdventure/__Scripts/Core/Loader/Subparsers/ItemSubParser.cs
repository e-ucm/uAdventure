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
			Item parsedObject = new Item(element.GetAttribute("id"));

            if (element.SelectSingleNode("documentation") != null)
                parsedObject.setDocumentation(element.SelectSingleNode("documentation").InnerText);
			
			switch(element.GetAttribute("behaviour")){
				case "atrezzo": parsedObject.setBehaviour(Item.BehaviourType.ATREZZO); break;
				case "first-action": parsedObject.setBehaviour(Item.BehaviourType.FIRST_ACTION); break;
				default: parsedObject.setBehaviour(Item.BehaviourType.NORMAL); break;
			}

            parsedObject.setReturnsWhenDragged(ExString.EqualsDefault(element.GetAttribute("returnsWhenDragged"), "yes", true));
			parsedObject.setResourcesTransitionTime(ExParsers.ParseDefault(element.GetAttribute("resources-transition-time"), 0L));

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
            {
                parsedObject.addResources(res);
            }

			// DESCRIPTIONS
			parsedObject.setDescriptions(DOMParserUtility.DOMParse<Description>(element.SelectNodes ("description"), parameters).ToList());

			// ACTIONS
			var actionsNode = element.SelectSingleNode("actions");
			if(actionsNode != null)
            {
                foreach (var res in DOMParserUtility.DOMParse<Action>(actionsNode.ChildNodes, parameters))
                {
                    parsedObject.addAction(res);
                }
            }

			return parsedObject;
        }
    }
}