using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("cutscene", "slidescene", "videoscene")]
	[DOMParser(typeof(Cutscene), typeof(Slidescene), typeof(Videoscene))]
	public class CutsceneSubParser : IDOMParser
    {

		public object DOMParse(XmlElement element, params object[] parameters)
        {
			var chapter = parameters [0] as Chapter;

			Cutscene cutscene;

			XmlNodeList
			endsgame = element.SelectNodes ("end-game"),
			nextsscene = element.SelectNodes ("next-scene");

			string slidesceneId = element.GetAttribute("id");
			bool initialScene = ExString.EqualsDefault(element.GetAttribute("start"), "yes", false);

            if (element.Name.Equals("slidescene")) cutscene = new Slidescene(slidesceneId);
            else                                   cutscene = new Videoscene(slidesceneId);
            if (initialScene)
                chapter.setTargetId(slidesceneId);

			//XAPI ELEMENTS
			cutscene.setXApiClass(element.GetAttribute("class"));
			cutscene.setXApiType(element.GetAttribute("type"));
            //END OF XAPI

			cutscene.setTargetId(element.GetAttribute("idTarget"));
			cutscene.setPositionX(ExParsers.ParseDefault(element.GetAttribute("destinyX"), int.MinValue));
			cutscene.setPositionY(ExParsers.ParseDefault(element.GetAttribute("destinyY"), int.MinValue));
			cutscene.setTransitionType((TransitionType)ExParsers.ParseDefault(element.GetAttribute("transitionType"),  0));
			cutscene.setTransitionTime(ExParsers.ParseDefault(element.GetAttribute("transitionTime"), 0));

            if (element.SelectSingleNode("name") != null)
                cutscene.setName(element.SelectSingleNode("name").InnerText);
            if (element.SelectSingleNode("documentation") != null)
                cutscene.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			cutscene.setEffects(DOMParserUtility.DOMParse (element.SelectSingleNode("effect"), parameters) as Effects ?? new Effects());

            if (cutscene is Videoscene)
				((Videoscene)cutscene).setCanSkip(ExString.EqualsDefault(element.GetAttribute("canSkip"), "yes", true));

			string next = ExString.Default(element.GetAttribute("next"), "go-back");
            if      (next.Equals("go-back"))		cutscene.setNext(Cutscene.GOBACK);
            else if (next.Equals("new-scene"))		cutscene.setNext(Cutscene.NEWSCENE);
            else if (next.Equals("end-chapter"))	cutscene.setNext(Cutscene.ENDCHAPTER);

			// RESOURCES
			foreach(var res in DOMParserUtility.DOMParse <ResourcesUni> (element.SelectNodes("resources"), parameters))
				cutscene.addResources (res);

            for(int i = 0; i < endsgame.Count; i++)
            {
                cutscene.setNext(Cutscene.ENDCHAPTER);
            }

            foreach (XmlElement el in nextsscene)
            {
				var currentNextScene = new NextScene(el.GetAttribute("idTarget"), 
					ExParsers.ParseDefault(element.GetAttribute("destinyX"), int.MinValue),
					ExParsers.ParseDefault(element.GetAttribute("destinyY"), int.MinValue));
				
				currentNextScene.setTransitionType((TransitionType)ExParsers.ParseDefault(element.GetAttribute("transitionType"), 0));
				currentNextScene.setTransitionTime(ExParsers.ParseDefault(element.GetAttribute("transitionTime"), 0));
				currentNextScene.setConditions(DOMParserUtility.DOMParse (el.SelectSingleNode ("condition"), parameters) as Conditions ?? new Conditions());
				currentNextScene.setEffects(DOMParserUtility.DOMParse (el.SelectSingleNode ("effect"), parameters) as Effects ?? new Effects());
				currentNextScene.setPostEffects(DOMParserUtility.DOMParse (el.SelectSingleNode ("post-effect"), parameters) as Effects ?? new Effects());

                cutscene.addNextScene(currentNextScene);
            }

			return cutscene;
        }

    }
}