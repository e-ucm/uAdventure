using uAdventure.Core;
using System;
using UnityEngine;
using System.Xml;

namespace uAdventure.Geo
{

    [DOMParser("geo-action", "exit-action", "enter-action", "lookto-action", "inspect-action")]
    [DOMParser(typeof(GeoAction), typeof(ExitAction), typeof(EnterAction), typeof(LookToAction), typeof(InspectAction))]
    public class GeoActionParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            GeoAction r;
            switch (element.Name)
            {
                default:
                case "exit-action":
                    ExitAction exit = new ExitAction();
                    exit.OnlyFromInside = bool.Parse(element.SelectSingleNode("only-from-inside").InnerText);
                    r = exit;
                    break;
                case "enter-action":
                    EnterAction enter = new EnterAction();
                    enter.OnlyFromOutside = bool.Parse(element.SelectSingleNode("only-from-outside").InnerText);
                    r = enter;
                    break;
                case "lookto-action":
                    LookToAction lookto = new LookToAction();
                    lookto.Inside = bool.Parse(element.SelectSingleNode("inside").InnerText);
                    var centerNode = element.SelectSingleNode("center");
                    lookto.Center = false;
                    if (centerNode != null)
                    {
                        lookto.Center = bool.Parse(centerNode.InnerText);
                    }
                    if (!lookto.Center)
                    {
                        var components = element.SelectSingleNode("direction").InnerText.Split(' ');
                        lookto.Direction = new Vector2(float.Parse(components[0]), float.Parse(components[1]));
                    }
                    r = lookto;
                    break;
                case "inspect-action":
                    InspectAction inspect = new InspectAction();
                    inspect.Inside = bool.Parse(element.SelectSingleNode("inside").InnerText);
                    r = inspect;
                    break;
            }

            ParseBasic(element, r, parameters);

            return r;
        }

		private void ParseBasic(XmlElement element, GeoAction action, params object[] parameters)
        {

			action.Conditions = DOMParserUtility.DOMParse (element.SelectSingleNode("condition"), parameters) as Conditions ?? new Conditions();
			action.Effects = DOMParserUtility.DOMParse (element.SelectSingleNode("effect"), parameters) 	  as Effects ?? new Effects();

        }
    }
}

