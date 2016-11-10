using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse objetcs
 */
public class PlayerSubParser : SubParser {

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
     * Constant for subparsing description tag.
     */
    private const int SUBPARSING_DESCRIPTION = 2;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * Player being parsed
     */
    private Player player;

    /**
     * Current resources being parsed
     */
    private ResourcesUni currentResources;

    /**
     * Current conditions being parsed
     */
    private Conditions currentConditions;

    /**
     * Subparser for conditions
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
    public PlayerSubParser(Chapter chapter):base(chapter)
    {
        descriptions = new List<Description>();
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

            // If it is a player tag, create the player
            if (qName.Equals("player"))
            {
                player = new Player();
                descriptions = new List<Description>();
                player.setDescriptions(descriptions);
            }

            // If it is a resources tag, create new resources
            else if (qName.Equals("resources"))
            {
                currentResources = new ResourcesUni();

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("name"))
                        currentResources.setName(entry.Value.ToString());
                }

            }

            // If it is a condition tag, create new conditions, new subparser and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
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

                // Set the color in the player
                if (qName.Equals("frontcolor"))
                    player.setTextFrontColor(color);
                if (qName.Equals("bordercolor"))
                    player.setTextBorderColor(color);
            }

            else if (qName.Equals("textcolor"))
            {
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("showsSpeechBubble"))
                        player.setShowsSpeechBubbles(entry.Value.ToString().Equals("yes"));
                    if (entry.Key.Equals("bubbleBkgColor"))
                        player.setBubbleBkgColor(entry.Value.ToString());
                    if (entry.Key.Equals("bubbleBorderColor"))
                        player.setBubbleBorderColor(entry.Value.ToString());
                }
            }

            // If it is a voice tag, take the voice and the always synthesizer option
            else if (qName.Equals("voice"))
            {
                string voice = string.Empty;
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
                player.setAlwaysSynthesizer(alwaysSynthesizer);
                player.setVoice(voice);

            }

            // If it is a description tag, create the new description (with its id)
            else if (qName.Equals("description"))
            {
                description = new Description();
                subParser = new DescriptionsSubParser(description, chapter);
                subParsing = SUBPARSING_DESCRIPTION;
            }

        }

        // If a condition is being subparsed, spread the call
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

            // If it is a player tag, store the player in the game data
            if (qName.Equals("player"))
            {
                chapter.setPlayer(player);
            }

            // If it is a documentation tag, hold the documentation in the player
            else if (qName.Equals("documentation"))
            {
                player.setDocumentation(currentstring.ToString().Trim());
            }

            // If it is a resources tag, add the resources to the player
            else if (qName.Equals("resources"))
            {
                player.addResources(currentResources);
            }
            // Reset the current string
            currentstring = string.Empty;
        }

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the condition tag is being closed, add the condition to the resources, and switch the state
            if (qName.Equals("condition"))
            {
                currentResources.setConditions(currentConditions);
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

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        /// If there are some kind of subparsing, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
