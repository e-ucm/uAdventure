using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse actions
 */
public class ActionsSubParser : SubParser
{

    /**
     * Indicates the current element being subparsed
     */
    private int subParsing;

    /**
     * Indicates the current element being read
     */
    private int reading;

    /**
     * Stores the current conditions being read
     */
    private Conditions currentConditions;

    /**
     * Stores the current effects being read
     */
    private Effects currentEffects;

    /**
     * Stores the current not-effects being read
     */
    private Effects currentNotEffects;

    /**
     * Stores the current click-effects being read
     * 
     */
    private Effects currentClickEffects;

    /**
     * Stores the current documentation being read
     */
    private string currentDocumentation;

    /**
     * Stores the current IdTarget being read
     */
    private string currentIdTarget;

    /**
     * Stores the current Name being read
     */
    private string currentName;

    /**
     * Stores the current needsGoTo being read
     */
    private bool currentNeedsGoTo;

    /**
     * Stores the current keepDinstance being read
     */
    private int currentKeepDistance;

    /**
     * Stores the current customAction being read
     */
    private CustomAction currentCustomAction;

    /**
     * Stores the current Resources being read
     */
    private ResourcesUni currentResources;

    /**
     * The current subparser being used
     */
    private SubParser subParser;

    /**
     * Constant for no subparsing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading an action
     */
    private const int READING_ACTION = 1;

    /**
     * Constant for reading resources
     */
    private const int READING_RESOURCES = 2;

    /**
     * Constant for subparsing conditions
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Constant for subparsing effects
     */
    private const int SUBPARSING_EFFECT = 2;

    /**
     * The element into which the actions are written
     */
    private Element element;

    /**
     * Activate not effects
     */
    bool activateNotEffects;

    /**
     * Activate click effects
     */
    bool activateClickEffects;

    /**
     * Default constructor
     * 
     * @param chapter
     *            The chapter that is being parsed
     * @param element
     *            The element where to add the actions
     */
    public ActionsSubParser(Chapter chapter, Element element) : base(chapter)
    {
        this.element = element;
        subParsing = SUBPARSING_NONE;
        reading = READING_NONE;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.cargador.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.XmlAttribute)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is an examine, use or grab tag, create new conditions and effects
            if (qName.Equals("examine") || qName.Equals("grab") || qName.Equals("use") || qName.Equals("talk-to"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("needsGoTo"))
                        currentNeedsGoTo = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("keepDistance"))
                        currentKeepDistance = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("not-effects"))
                        activateNotEffects = entry.Value.ToString().Equals("yes");
                }
                currentConditions = new Conditions();
                currentEffects = new Effects();
                currentNotEffects = new Effects();
                currentClickEffects = new Effects();
                currentDocumentation = null;
                reading = READING_ACTION;
            }

            // If it is an use-with or give-to tag, create new conditions and effects, and store the idTarget
            else if (qName.Equals("use-with") || qName.Equals("give-to") || qName.Equals("drag-to"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("idTarget"))
                        currentIdTarget = entry.Value.ToString();
                    if (entry.Key.Equals("needsGoTo"))
                        currentNeedsGoTo = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("keepDistance"))
                        currentKeepDistance = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("not-effects"))
                        activateNotEffects = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("click-effects"))
                        activateClickEffects = entry.Value.ToString().Equals("yes");
                }
                currentConditions = new Conditions();
                currentEffects = new Effects();
                currentNotEffects = new Effects();
                currentClickEffects = new Effects();
                currentDocumentation = null;
                reading = READING_ACTION;
            }

            else if (qName.Equals("custom") || qName.Equals("custom-interact"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("idTarget"))
                        currentIdTarget = entry.Value.ToString();
                    if (entry.Key.Equals("name"))
                        currentName = entry.Value.ToString();
                    if (entry.Key.Equals("needsGoTo"))
                        currentNeedsGoTo = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("keepDistance"))
                        currentKeepDistance = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("not-effects"))
                        activateNotEffects = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("click-effects"))
                        activateClickEffects = entry.Value.ToString().Equals("yes");
                }

                currentConditions = new Conditions();
                currentEffects = new Effects();
                currentNotEffects = new Effects();
                currentClickEffects = new Effects();
                currentDocumentation = null;
                if (qName.Equals("custom"))
                    currentCustomAction = new CustomAction(Action.CUSTOM);
                else
                    currentCustomAction = new CustomAction(Action.CUSTOM_INTERACT);
                reading = READING_ACTION;
            }

            // If it is a resources tag, create the new resources and switch the state
            else if (qName.Equals("resources"))
            {
                currentResources = new ResourcesUni();
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("name"))
                        currentResources.setName(entry.Value.ToString());
                }

                reading = READING_RESOURCES;
            }

            // If it is an asset tag, read it and add it to the current resources
            else if (qName.Equals("asset"))
            {
                string type = "";
                string path = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("type"))
                        type = entry.Value.ToString();
                    if (entry.Key.Equals("uri"))
                        path = entry.Value.ToString();
                }

                // If the asset is not an special one
                //				if( !AssetsController.isAssetSpecial( path ) )
                currentResources.addAsset(type, path);
            }

            // If it is a condition tag, create new conditions and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }

            // If it is a effect tag, create new effects and switch the state
            else if (qName.Equals("effect"))
            {
                subParser = new EffectSubParser(currentEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }
            // If it is a not-effect tag, create new effects and switch the state
            else if (qName.Equals("not-effect"))
            {
                subParser = new EffectSubParser(currentNotEffects, chapter);
                subParsing = SUBPARSING_EFFECT;
            }

            // If it is a click-effect tag, create new effects and switch the state
            else if (qName.Equals("click-effect"))
            {
                subParser = new EffectSubParser(currentClickEffects, chapter);
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
     * @see es.eucm.eadventure.engine.cargador.subparsers.SubParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a resources tag, add it to the object
            if (qName.Equals("resources"))
            {
                if (reading == READING_RESOURCES)
                {
                    currentCustomAction.addResources(currentResources);
                    reading = READING_NONE;
                }
            }

            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                currentDocumentation = currentstring.ToString().Trim();
            }

            // If it is a examine tag, store the new action in the object
            else if (qName.Equals("examine"))
            {
                Action examineAction = new Action(Action.EXAMINE, currentConditions, currentEffects, currentNotEffects);
                examineAction.setDocumentation(currentDocumentation);
                examineAction.setKeepDistance(currentKeepDistance);
                examineAction.setNeedsGoTo(currentNeedsGoTo);
                examineAction.setActivatedNotEffects(activateNotEffects);
                examineAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(examineAction);
                reading = READING_NONE;
            }

            // If it is a grab tag, store the new action in the object
            else if (qName.Equals("grab"))
            {
                Action grabAction = new Action(Action.GRAB, currentConditions, currentEffects, currentNotEffects);
                grabAction.setDocumentation(currentDocumentation);
                grabAction.setKeepDistance(currentKeepDistance);
                grabAction.setNeedsGoTo(currentNeedsGoTo);
                grabAction.setActivatedNotEffects(activateNotEffects);
                grabAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(grabAction);
                reading = READING_NONE;
            }

            // If it is an use tag, store the new action in the object
            else if (qName.Equals("use"))
            {
                Action useAction = new Action(Action.USE, currentConditions, currentEffects, currentNotEffects);
                useAction.setDocumentation(currentDocumentation);
                useAction.setNeedsGoTo(currentNeedsGoTo);
                useAction.setKeepDistance(currentKeepDistance);
                useAction.setActivatedNotEffects(activateNotEffects);
                useAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(useAction);
                reading = READING_NONE;
            }

            // If it is an use tag, store the new action in the object
            else if (qName.Equals("talk-to"))
            {
                Action talkToAction = new Action(Action.TALK_TO, currentConditions, currentEffects, currentNotEffects);
                talkToAction.setDocumentation(currentDocumentation);
                talkToAction.setNeedsGoTo(currentNeedsGoTo);
                talkToAction.setKeepDistance(currentKeepDistance);
                talkToAction.setActivatedNotEffects(activateNotEffects);
                talkToAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(talkToAction);
                reading = READING_NONE;
            }

            // If it is an use-with tag, store the new action in the object
            else if (qName.Equals("use-with"))
            {
                Action useWithAction = new Action(Action.USE_WITH, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects);
                useWithAction.setDocumentation(currentDocumentation);
                useWithAction.setKeepDistance(currentKeepDistance);
                useWithAction.setNeedsGoTo(currentNeedsGoTo);
                useWithAction.setActivatedNotEffects(activateNotEffects);
                useWithAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(useWithAction);
                reading = READING_NONE;
            }

            // If it is an use-with tag, store the new action in the object
            else if (qName.Equals("drag-to"))
            {
                Action useWithAction = new Action(Action.DRAG_TO, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects);
                useWithAction.setDocumentation(currentDocumentation);
                useWithAction.setKeepDistance(currentKeepDistance);
                useWithAction.setNeedsGoTo(currentNeedsGoTo);
                useWithAction.setActivatedNotEffects(activateNotEffects);
                useWithAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(useWithAction);
                reading = READING_NONE;
            }

            // If it is a give-to tag, store the new action in the object
            else if (qName.Equals("give-to"))
            {
                Action giveToAction = new Action(Action.GIVE_TO, currentIdTarget, currentConditions, currentEffects, currentNotEffects, currentClickEffects);
                giveToAction.setDocumentation(currentDocumentation);
                giveToAction.setKeepDistance(currentKeepDistance);
                giveToAction.setNeedsGoTo(currentNeedsGoTo);
                giveToAction.setActivatedNotEffects(activateNotEffects);
                giveToAction.setActivatedClickEffects(activateClickEffects);
                element.addAction(giveToAction);
                reading = READING_NONE;
            }

            // If it is a custom tag, store the new custom action in the object
            else if (qName.Equals("custom"))
            {
                currentCustomAction.setName(currentName);
                currentCustomAction.setConditions(currentConditions);
                currentCustomAction.setEffects(currentEffects);
                currentCustomAction.setNotEffects(currentNotEffects);
                currentCustomAction.setDocumentation(currentDocumentation);
                currentCustomAction.setKeepDistance(currentKeepDistance);
                currentCustomAction.setNeedsGoTo(currentNeedsGoTo);
                currentCustomAction.setActivatedNotEffects(activateNotEffects);
                currentCustomAction.setClickEffects(currentClickEffects);
                currentCustomAction.setActivatedClickEffects(activateClickEffects);
                //				customAction.addResources(currentResources);
                element.addAction(currentCustomAction);
                currentCustomAction = null;
                reading = READING_NONE;
            }

            // If it is a custom-interact tag, store the new custom interact action in the object
            else if (qName.Equals("custom-interact"))
            {
                currentCustomAction.setConditions(currentConditions);
                currentCustomAction.setEffects(currentEffects);
                currentCustomAction.setNotEffects(currentNotEffects);
                currentCustomAction.setName(currentName);
                currentCustomAction.setTargetId(currentIdTarget);
                currentCustomAction.setDocumentation(currentDocumentation);
                currentCustomAction.setKeepDistance(currentKeepDistance);
                currentCustomAction.setNeedsGoTo(currentNeedsGoTo);
                currentCustomAction.setActivatedNotEffects(activateNotEffects);
                currentCustomAction.setClickEffects(currentClickEffects);
                currentCustomAction.setActivatedClickEffects(activateClickEffects);
                //				customAction.addResources(currentResources);
                element.addAction(currentCustomAction);
                currentCustomAction = null;
                reading = READING_NONE;
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
            if (qName.Equals("condition"))
            {
                // Store the conditions in the resources
                if (reading == READING_RESOURCES)
                    currentResources.setConditions(currentConditions);

                // Switch state
                subParsing = SUBPARSING_NONE;
            }
        }

        // If an effect is being subparsed
        else if (subParsing == SUBPARSING_EFFECT)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the effect tag is being closed, switch the state
            if (qName.Equals("effect"))
            {
                subParsing = SUBPARSING_NONE;
            }
            // If the not-effect tag is being closed, switch the state
            else if (qName.Equals("not-effect"))
            {
                subParsing = SUBPARSING_NONE;
            }
            // If the not-effect tag is being closed, switch the state
            else if (qName.Equals("click-effect"))
            {
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

        // If it is reading an effect or a condition, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
