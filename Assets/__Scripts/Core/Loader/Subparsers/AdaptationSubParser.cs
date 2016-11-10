using UnityEngine;
using System.Collections;
using System.Xml;
using System.Collections.Generic;

public class AdaptationSubParser : SubParser
{
    /* Constants */
    private const int NONE = 0;

    private const int INITIAL_STATE = 1;

    private const int ADAPTATION_RULE = 2;

    private int parsing = NONE;

    /**
     * Adaptation data
     */
    private AdaptedState initialState;

    private List<AdaptationRule> externalRules;

    private AdaptationRule rule_temp;

    /**
     * string to store the current string in the XML file
     */
    private string currentstring;

    /**
     * The adaptation profile to fill
     */
    private AdaptationProfile profile;

    /**
     * Default constructor
     */
    public AdaptationSubParser(Chapter chapter):base(chapter)
    {
        profile = new AdaptationProfile();
        currentstring = string.Empty;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#startElement(java.lang.string, java.lang.string, java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // Check if it is an scorm adaptation profile
        if (qName.Equals("adaptation"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("scorm12"))
                {
                    profile.setScorm12(entry.Value.ToString().Equals("yes"));
                }
                if (entry.Key.Equals("scorm2004"))
                {
                    profile.setScorm2004(entry.Value.ToString().Equals("yes"));
                }
                if (entry.Key.Equals("name"))
                {
                    profile.setName(entry.Value.ToString());
                }
            }
        }

        //Start parsing the initial state
        if (qName.Equals("initial-state"))
        {
            parsing = INITIAL_STATE;
            initialState = new AdaptedState();
        }

        //Start parsing an adaptation rule
        else if (qName.Equals("adaptation-rule"))
        {
            parsing = ADAPTATION_RULE;
            rule_temp = new AdaptationRule();
        }

        //Initial scene
        else if (qName.Equals("initial-scene"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("idTarget"))
                {
                    if (parsing == INITIAL_STATE)
                    {
                        initialState.setTargetId(entry.Value.ToString());
                    }
                    else {
                        rule_temp.setInitialScene(entry.Value.ToString());
                    }
                }
            }
        }

        // If the tag activates a flag
        else if (qName.Equals("activate"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("flag"))
                {
                    if (parsing == INITIAL_STATE)
                    {
                        initialState.addActivatedFlag(entry.Value.ToString());
                    }
                    else {
                        rule_temp.addActivatedFlag(entry.Value.ToString());
                    }
                    profile.addFlag(entry.Value.ToString());
                }
            }
        }

        // If the tag deactivates a flag
        else if (qName.Equals("deactivate"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("flag"))
                {
                    if (parsing == INITIAL_STATE)
                    {
                        initialState.addDeactivatedFlag(entry.Value.ToString());
                    }
                    else {
                        rule_temp.addDeactivatedFlag(entry.Value.ToString());
                    }
                    profile.addFlag(entry.Value.ToString());
                }
            }
        }

        // If the tag set-value a var
        else if (qName.Equals("set-value"))
        {
            string var = null;
            string value = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = entry.Value.ToString();
                }
            }

            if (parsing == INITIAL_STATE)
            {
                initialState.addVarValue(var, AdaptedState.VALUE + " " + value);
            }
            else {
                rule_temp.addVarValue(var, AdaptedState.VALUE + " " + value);
            }
            profile.addVar(var);

        }

        // If the tag increment a var
        else if (qName.Equals("increment"))
        {
            string var = null;
            string value = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = entry.Value.ToString();
                }

            }

            if (parsing == INITIAL_STATE)
            {
                initialState.addVarValue(var, AdaptedState.INCREMENT + " " + value);
            }
            else {
                rule_temp.addVarValue(var, AdaptedState.INCREMENT + " " + value);
            }
            profile.addVar(var);

        }

        // If the tag decrement a var
        else if (qName.Equals("decrement"))
        {
            string var = null;
            string value = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = entry.Value.ToString();
                }

            }

            if (parsing == INITIAL_STATE)
            {
                initialState.addVarValue(var, AdaptedState.DECREMENT + " " + value);
            }
            else {
                rule_temp.addVarValue(var, AdaptedState.DECREMENT + " " + value);
            }
            profile.addVar(var);

        }

        //Property from the UoL
        else if (qName.Equals("property"))
        {
            string id = null;
            string value = null;
            string op = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                {
                    id = entry.Value.ToString();
                }
                else if (entry.Key.Equals("value"))
                {
                    value = entry.Value.ToString();
                }
                else if (entry.Key.Equals("operation"))
                {
                    op = entry.Value.ToString();
                }
            }
            rule_temp.addUOLProperty(new UOLProperty(id, value, op));
        }

    }

    public override void endElement(string namespaceURI, string localName, string qName)
    {

        //Finish parsing the initial state
        if (qName.Equals("adaptation"))
        {
            chapter.addAdaptationProfile(profile);
        }
        else if (qName.Equals("initial-state"))
        {
            parsing = NONE;
            profile.setInitialState(initialState);
        }

        else if (qName.Equals("ruleDescription"))
        {
            this.rule_temp.setDescription(currentstring.ToString().Trim());
        }

        //Finish parsing an adaptation rule
        else if (qName.Equals("adaptation-rule"))
        {
            parsing = NONE;
            profile.addRule(rule_temp);
        }

        // Reset the current string
        currentstring = string.Empty;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {
        // Append the new characters
        currentstring+= new string(buf, offset, len);
    }
}
