using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

/**
 * Class to subparse items.
 */
public class ItemSubParser : SubParser {

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

    private const int SUBPARSING_ACTIONS = 3;

    /**
     * Constant for subparsing description tag.
     */
    private const int SUBPARSING_DESCRIPTION = 4;

    /**
     * Store the current element being parsed.
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed.
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * parsedObject being parsed.
     */
    private Item parsedObject;

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
    public ItemSubParser(Chapter chapter):base(chapter)
    {
    }

    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {
        // If no element is being subparsed
       // Debug.Log(namespaceURI + " " + sName + " " + qName + "\nAttrs:\n" + CollectionPrinter.PrintCollection(attrs));
        if (subParsing == SUBPARSING_NONE)
        {
            // If it is a object tag, create the new parsedObject (with its id)
            if (qName.Equals("object"))
            {
                string parsedObjectId = "";
                bool returnsWhenDragged = true;

                //Two lines added:v1.4
                Item.BehaviourType behaviour = Item.BehaviourType.NORMAL;
                long resourceTransition = 0;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        parsedObjectId = entry.Value.ToString();
                    if (entry.Key.Equals("returnsWhenDragged"))
                        returnsWhenDragged = (entry.Value.ToString().Equals("yes") ? true : false);
                    if (entry.Key.Equals("behaviour"))
                    {
                        if (entry.Value.ToString().Equals("normal"))
                        {
                            behaviour = Item.BehaviourType.NORMAL;
                        }
                        else if (entry.Value.ToString().Equals("atrezzo"))
                        {
                            behaviour = Item.BehaviourType.ATREZZO;
                        }
                        else if (entry.Value.ToString().Equals("first-action"))
                        {
                            behaviour = Item.BehaviourType.FIRST_ACTION;
                        }
                    }
                    if (entry.Key.Equals("resources-transition-time"))
                        resourceTransition = long.Parse(entry.Value.ToString());

                }

                parsedObject = new Item(parsedObjectId);
                parsedObject.setReturnsWhenDragged(returnsWhenDragged);

                parsedObject.setResourcesTransitionTime(resourceTransition);
                parsedObject.setBehaviour(behaviour);

                descriptions = new List<Description>();
                parsedObject.setDescriptions(descriptions);

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

                //Debug.Log("RESOURCES, PARSUJE: " + parsedObject);
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


            else if (qName.Equals("actions"))
            {
                subParser = new ActionsSubParser(chapter, parsedObject);
                subParsing = SUBPARSING_ACTIONS;
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
        //Debug.Log(namespaceURI + " " + sName + " " + qName );
        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is an parsedObject tag, store the parsedObject in the game data
            if (qName.Equals("object"))
            {
                //Debug.Log(currentResources);
                //Debug.Log(parsedObject);
                chapter.addItem(parsedObject);
            }

            // If it is a resources tag, add it to the parsedObject
            else if (qName.Equals("resources"))
            {
                //Debug.Log("END, PARSUJE: " + parsedObject);
                //Debug.Log(currentResources);
                //Debug.Log(parsedObject);
                parsedObject.addResources(currentResources);
                reading = READING_NONE;
            }



            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                //Debug.Log(currentResources);
                //Debug.Log(parsedObject);
                if (reading == READING_NONE)
                    parsedObject.setDocumentation(currentstring.ToString().Trim());
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

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If it is reading an effect or a condition, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
