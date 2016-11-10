using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class for subparsing set of descriptions, it means, parse all the set of name, description and detailed descriptions with their associated
 * soundPaths and the conditions for each description.
 * 
 */
public class DescriptionsSubParser :SubParser {


    private Description description;

    private Conditions currentConditions;

    /**
     * Constant for subparsing nothing.
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag.
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Subparser for  conditions.
     */
    private SubParser subParser;

    /**
     * Stores the current element being subparsed.
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * Constructor 
     * 
     * @param description
     *          the description to be parsed
     * @param chapter
     *          Chapter data to store the read data
     */
    public DescriptionsSubParser(Description description, Chapter chapter):base(chapter)
    {
        this.description = description;
    }


    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {


            // If it is a name tag, store the name 
            if (qName.Equals("name"))
            {
                string soundPath = "";

                // if name tag has soundPath attribute, add it to the active area data model
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("soundPath"))
                        soundPath = entry.Value.ToString();
                }


                description.setNameSoundPath(soundPath);

            }

            // If it is a brief tag, store the brief description 
            else if (qName.Equals("brief"))
            {

                string soundPath = "";

                // if brief tag has soundPath attribute, add it to the data model
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("soundPath"))
                        soundPath = entry.Value.ToString();
                }

                description.setDescriptionSoundPath(soundPath);
            }

            // If it is a detailed tag, store the detailed description 
            else if (qName.Equals("detailed"))
            {

                string soundPath = "";

                // if detailed tag has soundPath attribute, add it to the data model
                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("soundPath"))
                        soundPath = entry.Value.ToString();
                }

                description.setDetailedDescriptionSoundPath(soundPath);
            }

            // If it is a condition tag, create new conditions and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }

        }// end if subparsing none

        // If it is reading an effect or a condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            subParser.startElement(namespaceURI, sName, qName, attrs);
        }

    }


    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {


            // If it is a name tag, store the name in the active area
            if (qName.Equals("name"))
            {
                description.setName(currentstring.ToString().Trim());
            }
            // If it is a brief tag, store the brief description in the active area
            else if (qName.Equals("brief"))
            {
                description.setDescription(currentstring.ToString().Trim());
            }
            // If it is a detailed tag, store the detailed description in the active area
            else if (qName.Equals("detailed"))
            {
                description.setDetailedDescription(currentstring.ToString().Trim());
            }

            // Reset the current string
            currentstring = string.Empty;

        }// end if subparsing none

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {
            // Spread the call
            subParser.endElement(namespaceURI, sName, qName);

            // If the condition tag is being closed
            if (qName.Equals("condition"))
            {
                this.description.setConditions(currentConditions);

                // Switch state
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

        // If a condition is being subparsed, spread the call
        else if (subParsing == SUBPARSING_CONDITION)
            subParser.characters(buf, offset, len);

    }
}
