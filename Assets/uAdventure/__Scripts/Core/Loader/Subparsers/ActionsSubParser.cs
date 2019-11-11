using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("examine","grab","use","talk-to","use-with","give-to","drag-to","custom","custom-interact")]
	[DOMParser(typeof(Action))]
	public class ActionsSubParser : IDOMParser
    {
		public object DOMParse(XmlElement element, params object[] parameters)
		{
			Action currentAction = new Action(0);

            //First we parse the elements every action haves:
			bool currentNeedsGoTo       = ExString.EqualsDefault(element.GetAttribute("needsGoTo"), "yes", false);
			int currentKeepDistance     = ExParsers.ParseDefault(element.GetAttribute ("keepDistance"), 0);
			bool activateNotEffects     = ExString.EqualsDefault(element.GetAttribute("not-effects"), "yes", false);
			bool activateClickEffects   = ExString.EqualsDefault(element.GetAttribute("click-effects"), "yes", false);
			string currentIdTarget = element.GetAttribute ("idTarget");

			Conditions conditions = DOMParserUtility.DOMParse<Conditions> ((XmlElement)element.SelectSingleNode("condition"), parameters) 	?? new Conditions();
			Effects effects 	  = DOMParserUtility.DOMParse<Effects> ((XmlElement)element.SelectSingleNode("effect"), parameters) 		?? new Effects();
			Effects clickeffects  = DOMParserUtility.DOMParse<Effects> ((XmlElement)element.SelectSingleNode("click-effect"), parameters) 	?? new Effects();
			Effects noteffects 	  = DOMParserUtility.DOMParse<Effects> ((XmlElement)element.SelectSingleNode("not-effect"), parameters) 	?? new Effects();
            
            // Then we instantiate the correct action by name.
            // We also parse the elements that are unique of that action.
			switch (element.Name)
            {
				case "examine": 	currentAction = new Action(Action.EXAMINE, conditions, effects, noteffects); break;
				case "grab": 		currentAction = new Action(Action.GRAB, conditions, effects, noteffects); break;
				case "use": 		currentAction = new Action(Action.USE, conditions, effects, noteffects); break;
				case "talk-to": 	currentAction = new Action(Action.TALK_TO, conditions, effects, noteffects); break;
				case "use-with": 	currentAction = new Action(Action.USE_WITH, currentIdTarget, conditions, effects, noteffects, clickeffects); break;
				case "give-to": 	currentAction = new Action(Action.GIVE_TO, currentIdTarget, conditions, effects, noteffects, clickeffects); break;
				case "drag-to": 	currentAction = new Action(Action.DRAG_TO, currentIdTarget, conditions, effects, noteffects, clickeffects); break;
                case "custom":
                case "custom-interact":
					CustomAction customAction = new CustomAction((element.Name == "custom") ? Action.CUSTOM : Action.CUSTOM_INTERACT);
					customAction.setName(element.GetAttribute("name"));
					customAction.addResources(
						DOMParserUtility.DOMParse<ResourcesUni>((XmlElement)element.SelectSingleNode("resources"), parameters) ?? new ResourcesUni());

                    currentAction = customAction;
                    break;
            }

            // Documentation
            var documentationNode = element.SelectSingleNode("documentation");
            if (documentationNode != null)
            {
                currentAction.setDocumentation(documentationNode.InnerText);
            }

            // Lastly we set al the attributes to the action
            currentAction.setConditions(conditions);
			currentAction.setEffects(effects);
			currentAction.setNotEffects(noteffects);
            currentAction.setKeepDistance(currentKeepDistance);
            currentAction.setNeedsGoTo(currentNeedsGoTo);
            currentAction.setActivatedNotEffects(activateNotEffects);
			currentAction.setClickEffects(clickeffects);
            currentAction.setActivatedClickEffects(activateClickEffects);

			return currentAction;
        }
    }
}