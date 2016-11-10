using UnityEngine;
using System.Collections;
using System.Xml;

public class TimerSubParser_ : Subparser_
{
    /**
     * Stores the current timer being parsed
     */
    private Timer timer;

    /**
     * Stores the current conditions being parsed
     */
    private Conditions currentConditions;

    /**
     * Stores the current effects being parsed
     */
    private Effects currentEffects;

    public TimerSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            initscondition = element.SelectNodes("init-condition"),
            endscondition = element.SelectNodes("end-condition"),
            effects = element.SelectNodes("effect"),
            postseffect = element.SelectNodes("post-effect");

        string tmpArgVal;

        string time = "";
        bool usesEndCondition = true;
        bool runsInLoop = true;
        bool multipleStarts = true;
        bool countDown = false, showWhenStopped = false, showTime = false;
        string displayName = "timer";

        tmpArgVal = element.GetAttribute("time");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            time = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("usesEndCondition");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            usesEndCondition = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("runsInLoop");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            runsInLoop = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("multipleStarts");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            multipleStarts = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("showTime");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            showTime = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("displayName");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            displayName = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("countDown");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            countDown = tmpArgVal.Equals("yes");
        }
        tmpArgVal = element.GetAttribute("showWhenStopped");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            showWhenStopped = tmpArgVal.Equals("yes");
        }

        timer = new Timer(long.Parse(time));
        timer.setRunsInLoop(runsInLoop);
        timer.setUsesEndCondition(usesEndCondition);
        timer.setMultipleStarts(multipleStarts);
        timer.setShowTime(showTime);
        timer.setDisplayName(displayName);
        timer.setCountDown(countDown);
        timer.setShowWhenStopped(showWhenStopped);

        if (element.SelectSingleNode("documentation") != null)
            timer.setDocumentation(element.SelectSingleNode("documentation").InnerText);

        foreach (XmlElement el in initscondition)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(el);
            timer.setInitCond(currentConditions);
        }

        foreach (XmlElement el in endscondition)
        {
            currentConditions = new Conditions();
            new ConditionSubParser_(currentConditions, chapter).ParseElement(el);
            timer.setEndCond(currentConditions);
        }

        foreach (XmlElement el in effects)
        {
            currentEffects = new Effects();
            new EffectSubParser_(currentEffects, chapter).ParseElement(el);
            timer.setEffects(currentEffects);
        }

        foreach (XmlElement el in postseffect)
        {
            currentEffects = new Effects();
            new EffectSubParser_(currentEffects, chapter).ParseElement(el);
            timer.setPostEffects(currentEffects);
        }

        chapter.addTimer(timer);
    }
}