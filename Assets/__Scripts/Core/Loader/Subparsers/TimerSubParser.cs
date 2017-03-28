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
			string time = element.GetAttribute("time") ?? "";
			string displayName = element.GetAttribute("displayName") ?? "timer";

			bool usesEndCondition = "yes".Equals(element.GetAttribute("usesEndCondition") ?? "yes");
			bool runsInLoop 	  = "yes".Equals(element.GetAttribute("runsInLoop")       ?? "yes");
			bool multipleStarts   = "yes".Equals(element.GetAttribute("multipleStarts")   ?? "yes");
			bool showTime 		  = "yes".Equals(element.GetAttribute("showTime"));
			bool countDown 		  = "yes".Equals(element.GetAttribute("countDown"));
			bool showWhenStopped  = "yes".Equals(element.GetAttribute("showWhenStopped"));

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

			timer.setInitCond(DOMParserUtility.DOMParse<Conditions>(element.SelectSingleNode("init-condition")) ?? new Conditions());
			timer.setEndCond(DOMParserUtility.DOMParse<Conditions>(element.SelectSingleNode("end-condition"))   ?? new Conditions());
			timer.setEffects(DOMParserUtility.DOMParse<Effects>(element.SelectSingleNode("effect")) 			?? new Effects());
			timer.setPostEffects(DOMParserUtility.DOMParse<Effects>(element.SelectSingleNode("post-effect")) 	?? new Effects());

            return timer;
        }
    }
}