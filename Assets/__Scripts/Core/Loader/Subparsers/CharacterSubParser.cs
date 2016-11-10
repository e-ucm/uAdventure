using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse characters
 */
public class CharacterSubParser : SubParser {

    /* Attributes */

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading resources tag
     */
    private const int READING_RESOURCES = 1;

    /**
     * Constant for reading conversation reference tag
     */
    private const int READING_CONVERSATION_REFERENCE = 2;

    /**
     * Constant for subparsing nothing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Constant for subparsing the actions tag
     */
    private const int SUBPARSING_ACTIONS = 2;

    /**
     * Constant for subparsing description tag.
     */
    private const int SUBPARSING_DESCRIPTION = 3;

    /**
     * Stores the current element being parsed
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * The character being read
     */
    private NPC npc;

    /**
     * Current resources being read
     */
    private ResourcesUni currentResources;

    /**
     * Current conversation reference being read
     */
    private ConversationReference conversationReference;

    /**
     * Current conditions being read
     */
    private Conditions currentConditions;

    /**
     * Subparser for the conditions
     */
    private SubParser subParser;


    private List<Description> descriptions;

    private Description description;

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public CharacterSubParser(Chapter chapter):base(chapter)
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

            // If it is a character tag, store the id of the character
            if (qName.Equals("character"))
            {
                string characterId = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("id"))
                        characterId = entry.Value.ToString();

                npc = new NPC(characterId);

                descriptions = new List<Description>();
                npc.setDescriptions(descriptions);
            }

            // If it is a resources tag, create the new resources, and switch the element being parsed
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
                //if( !AssetsController.isAssetSpecial( path ) )
                currentResources.addAsset(type, path);
            }

            // If it is a frontcolor or bordercolor tag, pick the color
            else if (qName.Equals("frontcolor") || qName.Equals("bordercolor"))
            {
                string color = "";

                // Pick the color
                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("color"))
                        color = entry.Value.ToString();

                // Set the color in the npc
                if (qName.Equals("frontcolor"))
                    npc.setTextFrontColor(color);
                if (qName.Equals("bordercolor"))
                    npc.setTextBorderColor(color);
            }

            else if (qName.Equals("textcolor"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("showsSpeechBubble"))
                        npc.setShowsSpeechBubbles(entry.Value.ToString().Equals("yes"));
                    if (entry.Key.Equals("bubbleBkgColor"))
                        npc.setBubbleBkgColor(entry.Value.ToString());
                    if (entry.Key.Equals("bubbleBorderColor"))
                        npc.setBubbleBorderColor(entry.Value.ToString());
                }
            }

            // If it is a conversation reference tag, store the destination id, and switch the element being parsed
            else if (qName.Equals("conversation-ref"))
            {
                string idTarget = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("idTarget"))
                        idTarget = entry.Value.ToString();

                conversationReference = new ConversationReference(idTarget);
                reading = READING_CONVERSATION_REFERENCE;
            }

            // If it is a condition tag, create a new subparser
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }
            // If it is a voice tag, take the voice and the always synthesizer option
            else if (qName.Equals("voice"))
            {
                string voice = "";
                string response;
                bool alwaysSynthesizer = false;

                // Pick the voice and synthesizer option
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("name"))
                        voice = entry.Value.ToString();
                    if (entry.Key.Equals("synthesizeAlways"))
                    {
                        response = entry.Value.ToString();
                        if (response.Equals("yes"))
                            alwaysSynthesizer = true;
                    }

                }
                npc.setAlwaysSynthesizer(alwaysSynthesizer);
                npc.setVoice(voice);
            }

            else if (qName.Equals("actions"))
            {
                subParser = new ActionsSubParser(chapter, npc);
                subParsing = SUBPARSING_ACTIONS;
            }

            // If it is a description tag, create the new description (with its id)
            else if (qName.Equals("description"))
            {
                description = new Description();
                subParser = new DescriptionsSubParser(description, chapter);
                subParsing = SUBPARSING_DESCRIPTION;
            }

        }

        // If a condition or action is being subparsed, spread the call
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

            // If it is a character tag, store the character in the game data
            if (qName.Equals("character"))
            {
                chapter.addCharacter(npc);
            }

            // If it is a documentation tag, hold the documentation in the character
            else if (qName.Equals("documentation"))
            {
                if (reading == READING_NONE)
                    npc.setDocumentation(currentstring.ToString().Trim());
                else if (reading == READING_CONVERSATION_REFERENCE)
                    conversationReference.setDocumentation(currentstring.ToString().Trim());
            }

            // If it is a resources tag, add the resources in the character
            else if (qName.Equals("resources"))
            {
                npc.addResources(currentResources);
                reading = READING_NONE;
            }

            // If it is a conversation reference tag, add the reference to the character
            else if (qName.Equals("conversation-ref"))
            {

                //npc.addConversationReference( conversationReference );
                Action action = new Action(Action.TALK_TO);
                action.setConditions(conversationReference.getConditions());
                action.setDocumentation(conversationReference.getDocumentation());
                TriggerConversationEffect effect = new TriggerConversationEffect(conversationReference.getTargetId());
                action.getEffects().add(effect);
                npc.addAction(action);
                reading = READING_NONE;
            }

            // Reset the current string
            currentstring = "";
        }

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {

            // Spread the end element call
            subParser.endElement(namespaceURI, sName, qName);

            // If the condition is being closed
            if (qName.Equals("condition"))
            {
                // Add the condition to the resources
                if (reading == READING_RESOURCES)
                    currentResources.setConditions(currentConditions);

                // Add the condition to the conversation reference
                if (reading == READING_CONVERSATION_REFERENCE)
                    conversationReference.setConditions(currentConditions);

                // Stop subparsing
                subParsing = SUBPARSING_NONE;
            }
        }
        else if (subParsing == SUBPARSING_ACTIONS)
        {
            subParser.endElement(namespaceURI, sName, qName);
            if (qName.Equals("actions"))
            {
                subParsing = SUBPARSING_NONE;
            }
        }

        // If it is a description tag, create the new description (with its id)
        else if (subParsing == SUBPARSING_DESCRIPTION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);
            if (qName.Equals("description"))
            {
                this.descriptions.Add(description);
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

        // If no element is being subparsed, read the characters
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If there are some kind of subparsing, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
