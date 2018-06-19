using System.Collections.Specialized;
using System.Xml;

using uAdventure.Core;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Timer))]
    public class TimerDOMWriter : ParametrizedDOMWriter
    {

        public TimerDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "timer";
        }


        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var timer = target as Timer;

            var timerElement = node as XmlElement;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Create the root node

            // Set the time attribute
            timerElement.SetAttribute("time", timer.getTime().ToString());
            timerElement.SetAttribute("usesEndCondition", timer.isUsesEndCondition() ? "yes" : "no");
            timerElement.SetAttribute("multipleStarts", timer.isMultipleStarts() ? "yes" : "no");
            timerElement.SetAttribute("runsInLoop", timer.isRunsInLoop() ? "yes" : "no");
            timerElement.SetAttribute("showTime", timer.isShowTime() ? "yes" : "no");
            timerElement.SetAttribute("displayName", timer.getDisplayName());
            timerElement.SetAttribute("countDown", timer.isCountDown() ? "yes" : "no");
            timerElement.SetAttribute("showWhenStopped", timer.isShowWhenStopped() ? "yes" : "no");

            // Append the documentation (if avalaible)
            if (timer.getDocumentation() != null)
            {
                XmlNode timerDocumentationNode = doc.CreateElement("documentation");
                timerDocumentationNode.AppendChild(doc.CreateTextNode(timer.getDocumentation()));
                timerElement.AppendChild(timerDocumentationNode);
            }

            OrderedDictionary conditionsAndEffects = new OrderedDictionary();


            // Append the init conditions (if avalaible)
            if (!timer.getInitCond().IsEmpty())
            {
                conditionsAndEffects.Add(ConditionsDOMWriter.INIT_CONDITIONS, timer.getInitCond());
            }

            // Append the end-conditions (if avalaible)
            if (!timer.getEndCond().IsEmpty())
            {
                conditionsAndEffects.Add(ConditionsDOMWriter.END_CONDITIONS, timer.getEndCond());
            }

            // Append the effects (if avalaible)
            if (!timer.getEffects().IsEmpty())
            {
                conditionsAndEffects.Add(EffectsDOMWriter.EFFECTS, timer.getEffects());
            }

            // Append the post-effects (if avalaible)
            if (!timer.getPostEffects().IsEmpty())
            {
                conditionsAndEffects.Add(EffectsDOMWriter.POST_EFFECTS, timer.getPostEffects());
            }

            DOMWriterUtility.DOMWrite(timerElement, conditionsAndEffects, DOMWriterUtility.DontCreateElement());
        }
    }
}