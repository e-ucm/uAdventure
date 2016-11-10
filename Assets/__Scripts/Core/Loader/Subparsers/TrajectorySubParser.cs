using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

public class TrajectorySubParser : SubParser {
    /* Attributes */

    /**
     * Constant for subparsing nothing.
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Stores the current element being subparsed.
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * Trajectory being parsed.
     */
    private Trajectory trajectory;

    /**
     * Subparser for effects and conditions.
     */
    private SubParser subParser;

    /**
     * Scene to add the trajectory
     */
    private Scene scene;

    /* Methods */

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public TrajectorySubParser(Chapter chapter, Scene scene):base(chapter)
    {
        this.trajectory = new Trajectory();
        //scene.setTrajectory(trajectory);
        this.scene = scene;
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
            if (qName.Equals("node"))
            {

                int x = 0, y = 0;
                float scale = 1.0f;

                string id = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("x"))
                        x = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("y"))
                        y = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("id"))
                        id = entry.Value.ToString();
                    if (entry.Key.Equals("scale"))
                    {
                        scale = float.Parse(entry.Value.ToString(), CultureInfo.InvariantCulture);
                    }
                }

                trajectory.addNode(id, x, y, scale);

            }

            // If it is a condition tag, create new conditions and switch the state
            else if (qName.Equals("side"))
            {
                string idStart = "";
                string idEnd = "";
                int length = -1;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("idStart"))
                        idStart = entry.Value.ToString();
                    if (entry.Key.Equals("idEnd"))
                        idEnd = entry.Value.ToString();
                    if (entry.Key.Equals("length"))
                        length = int.Parse(entry.Value.ToString());
                }

                trajectory.addSide(idStart, idEnd, length);
            }

            else if (qName.Equals("initialnode"))
            {
                string id = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        id = entry.Value.ToString();
                }

                trajectory.setInitial(id);
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

            // Reset the current string
            currentstring = string.Empty;
        }

        if (qName.Equals("trajectory"))
        {
            if (trajectory.getNodes().Count != 0)
            {
                trajectory.deleteUnconnectedNodes();
                scene.setTrajectory(trajectory);

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
