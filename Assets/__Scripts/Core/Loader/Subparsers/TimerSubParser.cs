using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse timers
 */
public class TimerSubParser : SubParser
{

    /* Attributes */
    /**
     * Constant for subparsing nothing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Constant for subparsing effect tag
     */
    private const int SUBPARSING_EFFECT = 2;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

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

    /**
     * The subparser for the condition or effect tags
     */
    private SubParser subParser;

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public TimerSubParser(Chapter chapter) : base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a timer tag, create a new timer with its time
            if (qName.Equals("timer"))
            {
                string time = "";
                bool usesEndCondition = true;
                bool runsInLoop = true;
                bool multipleStarts = true;
                bool countDown = false, showWhenStopped = false, showTime = false;
                string displayName = "timer";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("time"))
                        time = entry.Value.ToString();
                    if (entry.Key.Equals("usesEndCondition"))
                        usesEndCondition = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("runsInLoop"))
                        runsInLoop = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("multipleStarts"))
                        multipleStarts = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("showTime"))
                        showTime = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("displayName"))
                        displayName = entry.Value.ToString();
                    if (entry.Key.Equals("countDown"))
                        countDown = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("showWhenStopped"))
                        showWhenStopped = entry.Value.ToString().Equals("yes");
                }

                timer = new Timer(long.Parse(time));
                timer.setRunsInLoop(runsInLoop);
                timer.setUsesEndCondition(usesEndCondition);
                timer.setMultipleStarts(multipleStarts);
                timer.setShowTime(showTime);
                timer.setDisplayName(displayName);
                timer.setCountDown(countDown);
                timer.setShowWhenStopped(showWhenStopped);
            }

            // If it is a condition tag, create the new condition, the subparser and switch the state
            else if (qName.Equals("init-condition") || qName.Equals("end-condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }

            // If it is a effect tag, create the new effect, the subparser and switch the state
            else if (qName.Equals("effect") || qName.Equals("post-effect"))
            {
                currentEffects = new Effects();
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

        }

        // If it is reading an effect or a condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a timer tag, add it to the game data
            if (qName.Equals("timer"))
            {
                chapter.addTimer(timer);
            }

            // If it is a documentation tag, hold the documentation in the slidescene
            else if (qName.Equals("documentation"))
            {
                timer.setDocumentation(currentstring.ToString().Trim());
            }

            // Reset the current string
            currentstring = string.Empty;
        }

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the condition tag is being closed
            if (qName.Equals("init-condition"))
            {
                timer.setInitCond(currentConditions);

                // Switch the state
                subParsing = SUBPARSING_NONE;
            }

            // If the condition tag is being closed
            if (qName.Equals("end-condition"))
            {
                timer.setEndCond(currentConditions);

                // Switch the state
                subParsing = SUBPARSING_NONE;
            }
        }

        // If an effect is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the effect tag is being closed, store the effect in the next scene and switch the state
            if (qName.Equals("effect"))
            {
                timer.setEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }

            // If the effect tag is being closed, add the post-effects to the current next scene and switch the state
            if (qName.Equals("post-effect"))
            {
                timer.setPostEffects(currentEffects);
                subParsing = SUBPARSING_NONE;
            }
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If it is reading an effect or a condition
        else
            subParser.characters(buf, offset, len);
    }
}