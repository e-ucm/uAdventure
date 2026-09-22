using UnityEngine;
using System.Collections;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser("timer")]
	[DOMParser(typeof(Timer))]
	public class TimerSubParser : IDOMParser
	{
		public object DOMParse(XmlElement element, params object[] parameters)
        {
			string time = element.GetAttribute("time");
			string displayName    = ExString.Default(element.GetAttribute("displayName"), "timer");
            
            bool usesEndCondition = ExString.EqualsDefault(element.GetAttribute("usesEndCondition"), "yes", true);
			bool runsInLoop 	  = ExString.EqualsDefault(element.GetAttribute("runsInLoop"), "yes", true);
            bool multipleStarts   = ExString.EqualsDefault(element.GetAttribute("multipleStarts"), "yes", true);
            bool showTime 		  = ExString.EqualsDefault(element.GetAttribute("showTime"), "yes", false);
            bool countDown 		  = ExString.EqualsDefault(element.GetAttribute("countDown"), "yes", false);
            bool showWhenStopped  = ExString.EqualsDefault(element.GetAttribute("showWhenStopped"), "yes", false);

            Timer timer = new Timer(long.Parse(time));
            timer.setRunsInLoop(runsInLoop);
            timer.setUsesEndCondition(usesEndCondition);
            timer.setMultipleStarts(multipleStarts);
            timer.setShowTime(showTime);
            timer.setDisplayName(displayName);
            timer.setCountDown(countDown);
            timer.setShowWhenStopped(showWhenStopped);

            if (element.SelectSingleNode("documentation") != null)
                timer.setDocumentation(element.SelectSingleNode("documentation").InnerText);

			timer.setInitCond(DOMParserUtility.DOMParse<Conditions>(element.SelectSingleNode("init-condition"), parameters) ?? new Conditions());
			timer.setEndCond(DOMParserUtility.DOMParse<Conditions>(element.SelectSingleNode("end-condition"), parameters)   ?? new Conditions());
			timer.setEffects(DOMParserUtility.DOMParse<Effects>(element.SelectSingleNode("effect"), parameters) 			?? new Effects());
			timer.setPostEffects(DOMParserUtility.DOMParse<Effects>(element.SelectSingleNode("post-effect"), parameters) 	?? new Effects());

            return timer;
        }
    }
}