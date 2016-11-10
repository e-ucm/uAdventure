using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse items.
 */
public class BarrierSubParser : SubParser {


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
     * Store the current element being parsed.
     */
    private int reading = READING_NONE;

    /**
     * Stores the current element being subparsed.
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * Barrier being parsed.
     */
    private Barrier barrier;

    /**
     * Current conditions being parsed.
     */
    private Conditions currentConditions;

    /**
     * Subparser for effects and conditions.
     */
    private SubParser subParser;

    /**
     * Stores the scene where the area should be attached
     */
    private Scene scene;

    private int nBarriers;

    /* Methods */

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public BarrierSubParser(Chapter chapter, Scene scene, int nBarriers):base(chapter)
    {
        this.nBarriers = nBarriers;
        this.scene = scene;
    }

    private string generateId()
    {

        return (nBarriers + 1).ToString();
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
            if (qName.Equals("barrier"))
            {

                int x = 0, y = 0, width = 0, height = 0;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("width"))
                        width = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("height"))
                        height = int.Parse(entry.Value.ToString());
                }

                barrier = new Barrier(generateId(), x, y, width, height);
            }

            // If it is a condition tag, create new conditions and switch the state
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                subParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
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
            if (qName.Equals("barrier"))
            {
                scene.addBarrier(barrier);
            }

            // If it is a documentation tag, hold the documentation in the current element
            else if (qName.Equals("documentation"))
            {
                if (reading == READING_NONE)
                    barrier.setDocumentation(currentstring.ToString().Trim());
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
                    this.barrier.setConditions(currentConditions);
                }
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

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If it is reading an effect or a condition, spread the call
        else
            subParser.characters(buf, offset, len);
    }
}
