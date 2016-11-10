using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse items.
 */
public class ActiveAreaSubParser : SubParser {


    /* Attributes */

    /**
     * Constant for reading nothing.
     */
    private const int READING_NONE = 0;

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
     * ActiveArea being parsed.
     */
    private ActiveArea activeArea;

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

    /**
     * Stores the scene where the area should be attached
     */
    private Scene scene;

    private int nAreas;


    private List<Description> descriptions;

    private Description description;

    /* Methods */

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public ActiveAreaSubParser(Chapter chapter, Scene scene, int nAreas): base(chapter)
    {
        this.nAreas = nAreas;
        this.scene = scene;

    }

    private string generateId()
    {

        return "area" + (nAreas + 1) + "scene" + scene.getId();
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
            if (qName.Equals("active-area"))
            {

                int x = 0, y = 0, width = 0, height = 0;
                string id = null;
                bool rectangular = true;
                int influenceX = 0, influenceY = 0, influenceWidth = 0, influenceHeight = 0;
                bool hasInfluence = false;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("rectangular"))
                        rectangular = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("width"))
                        width = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("height"))
                        height = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("id"))
                        id = entry.Value.ToString();
                    if (entry.Key.Equals("hasInfluenceArea"))
                        hasInfluence = entry.Value.ToString().Equals("yes");
                    if (entry.Key.Equals("influenceX"))
                        influenceX = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceY"))
                        influenceY = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceWidth"))
                        influenceWidth = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("influenceHeight"))
                        influenceHeight = int.Parse(entry.Value.ToString());

                }

                activeArea = new ActiveArea((id == null ? generateId() : id), rectangular, x, y, width, height);
                if (hasInfluence)
                {
                    InfluenceArea influenceArea = new InfluenceArea(influenceX, influenceY, influenceWidth, influenceHeight);
                    activeArea.setInfluenceArea(influenceArea);
                }
                descriptions = new List<Description>();
                activeArea.setDescriptions(descriptions);
            }

            else if (qName.Equals("point"))
            {
                if (activeArea != null)
                {
                    int x = 0, y = 0;

                    foreach (KeyValuePair<string, string> entry in attrs)
                    {
                        if (entry.Key.Equals("x"))
                            x = int.Parse(entry.Value.ToString());
                        if (entry.Key.Equals("y"))
                            y = int.Parse(entry.Value.ToString());
                    }

                    Vector2 point = new Vector2(x, y);
                    activeArea.addVector2(point);
                }
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
                subParser = new ActionsSubParser(chapter, activeArea);
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

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is an object tag, store the object in the game data
            if (qName.Equals("active-area"))
            {
                scene.addActiveArea(activeArea);
            }

            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                activeArea.setDocumentation(currentstring.ToString().Trim());
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
                if (reading == READING_NONE)
                {
                    this.activeArea.setConditions(currentConditions);
                }
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
