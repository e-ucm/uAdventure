using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

/**
 * Class to subparse conditions
 */
public class ConditionSubParser : SubParser {

    /* Attributes */

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading either tag
     */
    private const int READING_EITHER = 1;

    /**
     * Stores the current element being parsed
     */
    private int reading = READING_NONE;

    /**
     * Stores the conditions
     */
    private Conditions conditions;

    /**
     * Stores the current either conditions
     */
    private Conditions currentEitherCondition;

    /* Methods */

    /**
     * Constructor
     * 
     * @param conditions
     *            Structure in which the conditions will be placed
     * @param chapter
     *            Chapter data to store the read data
     */
    public ConditionSubParser(Conditions conditions, Chapter chapter):base(chapter)
    {
        this.conditions = conditions;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {
        // If it is an either tag, create a new either conditions and switch the state
        if (qName.Equals("either"))
        {
            currentEitherCondition = new Conditions();
            reading = READING_EITHER;
        }

        // If it is an active tag
        else if (qName.Equals("active"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("flag"))
                {

                    // Store the active flag in the conditions or either conditions
                    if (reading == READING_NONE)
                        conditions.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_ACTIVE));
                    if (reading == READING_EITHER)
                        currentEitherCondition.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_ACTIVE));

                    chapter.addFlag(entry.Value.ToString());
                }
            }
        }

        // If it is an inactive tag
        else if (qName.Equals("inactive"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("flag"))
                {
                    // Store the inactive flag in the conditions or either conditions
                    if (reading == READING_NONE)
                        conditions.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_INACTIVE));
                    if (reading == READING_EITHER)
                        currentEitherCondition.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_INACTIVE));

                    chapter.addFlag(entry.Value.ToString());
                }
            }
        }

        // If it is a greater-than tag
        else if (qName.Equals("greater-than"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
            chapter.addVar(var);
        }

        // If it is a greater-Equals-than tag
        else if (qName.Equals("greater-equals-than"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
            chapter.addVar(var);
        }

        // If it is a less-than tag
        else if (qName.Equals("less-than"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
            chapter.addVar(var);
        }

        // If it is a less-Equals-than tag
        else if (qName.Equals("less-equals-than"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
            chapter.addVar(var);
        }

        // If it is a Equals-than tag
        else if (qName.Equals("equals"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
            chapter.addVar(var);
        }

        // If it is a not-Equals-than tag
        else if (qName.Equals("not-equals"))
        {
            // The var
            string var = null;
            // The value
            int value = 0;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = int.Parse(entry.Value.ToString());
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
            chapter.addVar(var);
        }


        // If it is a global-state-reference tag
        else if (qName.Equals("global-state-ref"))
        {
            // Id
            string id = null;
            // VALUE WAS ADDED IN EAD1.4 - It allows negating a global state
            bool value = true;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                {
                    id = entry.Value.ToString();
                }
                if (entry.Key.Equals("value"))
                {
                    value = entry.Value.ToString().ToLower().Equals("true");
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                conditions.add(new GlobalStateCondition(id, value ? GlobalStateCondition.GS_SATISFIED : GlobalStateCondition.GS_NOT_SATISFIED));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new GlobalStateCondition(id, value ? GlobalStateCondition.GS_SATISFIED : GlobalStateCondition.GS_NOT_SATISFIED));
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

        // If it is an either tag
        if (qName.Equals("either"))
        {
            // Store the either condition in the condition, and switch the state back to normal
            conditions.add(currentEitherCondition);
            reading = READING_NONE;
        }
    }

}
