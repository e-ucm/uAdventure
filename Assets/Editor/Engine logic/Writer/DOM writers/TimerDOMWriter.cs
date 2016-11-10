using UnityEngine;
using System.Collections;
using System.Xml;

public class TimerDOMWriter
{

    /**
     * Private constructor.
     */

    private TimerDOMWriter()
    {

    }

    public static XmlNode buildDOM(Timer timer)
    {

        XmlElement timerElement = null;

        // Create the necessary elements to create the DOM
        XmlDocument doc = Writer.GetDoc();

        // Create the root node
        timerElement = doc.CreateElement("timer");

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

        // Append the init conditions (if avalaible)
        if (!timer.getInitCond().isEmpty())
        {
            XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(ConditionsDOMWriter.INIT_CONDITIONS,
                timer.getInitCond());
            doc.ImportNode(conditionsNode, true);
            timerElement.AppendChild(conditionsNode);
        }

        // Append the end-conditions (if avalaible)
        if (!timer.getEndCond().isEmpty())
        {
            XmlNode conditionsNode = ConditionsDOMWriter.buildDOM(ConditionsDOMWriter.END_CONDITIONS, timer.getEndCond());
            doc.ImportNode(conditionsNode, true);
            timerElement.AppendChild(conditionsNode);
        }

        // Append the effects (if avalaible)
        if (!timer.getEffects().isEmpty())
        {
            XmlNode effectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.EFFECTS, timer.getEffects());
            doc.ImportNode(effectsNode, true);
            timerElement.AppendChild(effectsNode);
        }

        // Append the post-effects (if avalaible)
        if (!timer.getPostEffects().isEmpty())
        {
            XmlNode postEffectsNode = EffectsDOMWriter.buildDOM(EffectsDOMWriter.POST_EFFECTS, timer.getPostEffects());
            doc.ImportNode(postEffectsNode, true);
            timerElement.AppendChild(postEffectsNode);
        }

        return timerElement;
    }
}