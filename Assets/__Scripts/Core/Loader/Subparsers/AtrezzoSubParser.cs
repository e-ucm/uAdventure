using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * 
 * Class to subperse atrezzo items
 * 
 */
public class AtrezzoSubParser : SubParser
{

    /* Attributes */

    /**
     * Constant for reading nothing.
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading resources tag.
     */
    private const int READING_RESOURCES = 1;

    /**
     * Constant for subparsing nothing.
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag.
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Constant for subparsing effect tag.
     */
    private const int SUBPARSING_EFFECT = 2;

    /**
     * Constant for subparsing description tag.
     */
    private const int SUBPARSING_DESCRIPTION = 3;

    /**
     * Store the current element being parsed.
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed.
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * Atrezzo object being parsed.
     */
    private Atrezzo atrezzo;

    /**
     * Current resources being parsed.
     */
    private ResourcesUni currentResources;

    /**
     * Current conditions being parsed.
     */
    private Conditions currentConditions;

    /**
     * Current effects being parsed.
     */
    private Effects currentEffects;

    /**
     * Subparser for effects and conditions.
     */
    private SubParser subParser;


    private List<Description> descriptions;

    private Description description;

    /* Methods */

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public AtrezzoSubParser(Chapter chapter):base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.cargador.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {
            // If it is a object tag, create the new object (with its id)
            if (qName.Equals("atrezzoobject"))
            {
                string atrezzoId = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                    if (entry.Key.Equals("id"))
                        atrezzoId = entry.Value.ToString();

                atrezzo = new Atrezzo(atrezzoId);

                descriptions = new List<Description>();
                atrezzo.setDescriptions(descriptions);
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

            // If it is a description tag, create the new description (with its id)
            else if (qName.Equals("description"))
            {
                description = new Description();
                subParser = new DescriptionsSubParser(description, chapter);
                subParsing = SUBPARSING_DESCRIPTION;
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
        }

        // If it is reading an effect or a condition, spread the call
        if (subParsing != SUBPARSING_NONE)
        {
            //string id = this.atrezzo.getId( );
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

            // If it is an object tag, store the object in the game data
            if (qName.Equals("atrezzoobject"))
            {
                chapter.addAtrezzo(atrezzo);
            }

            // If it is a resources tag, add it to the object
            else if (qName.Equals("resources"))
            {
                atrezzo.addResources(currentResources);
                reading = READING_NONE;
            }

            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                if (reading == READING_NONE)
                    atrezzo.setDocumentation(currentstring.ToString().Trim());

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

        // If it is reading an effect or a condition, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
