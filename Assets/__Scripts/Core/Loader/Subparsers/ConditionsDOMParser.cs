using UnityEngine;
using System.Collections;
using System;
using System.Xml;

namespace uAdventure.Core
{
    /// <summary>
    /// This parser ir an adapter for the original subparser
    /// </summary>
    [DOMParser("condition")]
    [DOMParser(typeof(Conditions))]
    public class ConditionsDOMParser : IDOMParser
	{
        public object DOMParse(XmlElement element, params object[] parameters)
		{
			var chapter = parameters [0] as Chapter;
			var conditions = new Conditions ();

			foreach (var child in element.ChildNodes)
				ParseCondition (conditions, child, parameters);

			return conditions;
        }

		private void ParseCondition(Conditions c, XmlElement element, params object[] parameters){

			switch (element.Name) {
			case "either":
				c.add(parseEither (element, parameters));
				break;
			case "active":
			case "inactive":
				c.add(parseFlag (element, parameters));
				break;
			case "greater-than":
			case "greater-equals-than":
			case "less-than":
			case "equals":
			case "not-equals":
				c.add(parseVar (element, parameters));
				break;
			case "global-state-ref":
				c.add(parseGlobal (element, parameters));
				break;

			}
		}

		private Conditions parseEither(XmlElement element, params object[] parameters){
			var c = new Conditions ();

			foreach (var child in element.ChildNodes) 
				ParseCondition (c, child, parameters);

			return c;
		}

		private Condition parseFlag(XmlElement element, params object[] parameters){

			var chapter = parameters [0] as Chapter;

			int state = 0;
			switch (element.Name) {
			case "active":
				state = FlagCondition.FLAG_ACTIVE;
				break;
			case "inactive":
				state = FlagCondition.FLAG_INACTIVE;
				break;
			}

			var flag = element.GetAttribute ("flag") ?? "";
			chapter.addFlag (flag);
			return new FlagCondition(flag,state);
		}


		private Condition parseVar(XmlElement element, params object[] parameters){
			var chapter = parameters [0] as Chapter;
			int state = 0;
			switch (element.Name) {
			case "greater-than":
				state = VarCondition.VAR_GREATER_THAN;
				break;
			case "greater-equals-than":
				state = VarCondition.VAR_GREATER_EQUALS_THAN;
				break;
			case "less-than":
				state = VarCondition.VAR_LESS_THAN;
				break;
			case "equals":
				state = VarCondition.VAR_EQUALS;
				break;
			case "not-equals":
				state = VarCondition.VAR_NOT_EQUALS;
				break;
			}

			// The var
			string variable = element.GetAttribute("var");
			// The value
			int value = int.Parse(element.GetAttribute("value") ?? "0");

			chapter.addVar (variable);
			return new VarCondition (variable, state, value);

		}

		private Condition parseGlobal(XmlElement element, params object[] parameters){
			// Id
			string id =  element.GetAttribute("id");
			// VALUE WAS ADDED IN EAD1.4 - It allows negating a global state
			string val = element.GetAttribute("value");
			bool value = "true".Equals (val != null ? val.ToLower() : val);
			return new GlobalStateCondition(id,	value ? GlobalStateCondition.GS_SATISFIED : GlobalStateCondition.GS_NOT_SATISFIED);
		}
    }
}