using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This handler reads the initial values for the adaptation engine. This class
 * is only used for specific xml adaptation files, that is a past
 * characteristic, to preserve past game version. In new versions, the
 * adaptation info is in chapter.xml file.For this reason the parsing of
 * assessment is now in chapter parsing (ChapterHandler)
 */
public class AdaptationHandler : XMLHandler
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
     * List of flags involved in this assessment script
     */
    private List<string> flags;

    /**
     * List of vars involved in this assessment script
     */
    private List<string> vars;

    /**
     * string to store the current string in the XML file
     */
    private string currentstring;

    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    /**
     * bool to check if it is an scorm 1.2 profile
     */
    private bool scorm12;

    /**
     * bool to check if it is an scorm 2004 profile
     */
    private bool scorm2004;

    /**
     * Default constructor
     */
    public AdaptationHandler(InputStreamCreator isCreator, List<AdaptationRule> rules, AdaptedState iState)
    {

        initialState = iState;
        if (rules == null)
            externalRules = new List<AdaptationRule>();
        else
            externalRules = rules;
        currentstring = string.Empty;
        vars = new List<string>();
        flags = new List<string>();
        this.isCreator = isCreator;
    }

    private void addFlag(string flag)
    {

        if (!flags.Contains(flag))
        {
            flags.Add(flag);
        }
    }

    private void addVar(string var)
    {

        if (!vars.Contains(var))
        {
            vars.Add(var);
        }
    }

    /**
     * Returns the initial state
     * 
     * @return initial state
     */
    public AdaptedState getInitialState()
    {

        return initialState;
    }

    /**
     * Returns a list of adaptation rules
     * 
     * @return adaptation rules
     */
    public List<AdaptationRule> getAdaptationRules()
    {

        return externalRules;
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
                    scorm12 = entry.Value.ToString().Equals("yes");
                }
                if (entry.Key.Equals("scorm2004"))
                {
                    scorm2004 = entry.Value.ToString().Equals("yes");
                }
            }
        }

        //Start parsing the initial state
        if (qName.Equals("initial-state"))
        {
            parsing = INITIAL_STATE;
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
                        initialState.setTargetId(entry.Value.ToString().ToString());
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
                    addFlag(entry.Value.ToString());
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
                    addFlag(entry.Value.ToString());
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
            addVar(var);

        }

        // If the tag increment a var
        else if (qName.Equals("increment"))
        {
            string var = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }

            }

            if (parsing == INITIAL_STATE)
            {
                initialState.addVarValue(var, AdaptedState.INCREMENT);
            }
            else {
                rule_temp.addVarValue(var, AdaptedState.INCREMENT);
            }
            addVar(var);

        }

        // If the tag decrement a var
        else if (qName.Equals("decrement"))
        {
            string var = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("var"))
                {
                    var = entry.Value.ToString();
                }

            }

            if (parsing == INITIAL_STATE)
            {
                initialState.addVarValue(var, AdaptedState.DECREMENT);
            }
            else {
                rule_temp.addVarValue(var, AdaptedState.DECREMENT);
            }
            addVar(var);

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
        if (qName.Equals("initial-state"))
        {
            parsing = NONE;
        }

        else if (qName.Equals("description"))
        {
            this.rule_temp.setDescription(currentstring.ToString().Trim());
        }

        //Finish parsing an adaptation rule
        else if (qName.Equals("adaptation-rule"))
        {
            parsing = NONE;
            externalRules.Add(rule_temp);
        }

        // Reset the current string
        currentstring = string.Empty;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ErrorHandler#error(org.xml.sax.SAXParseException)
    // */
    //@Override
    //    public void error(SAXParseException exception) throws SAXParseException
    //{

    //        throw exception;
    //}

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // Append the new characters
        currentstring += new string(buf, offset, len);

    }

    /**
     * @return the flags
     */
    public List<string> getFlags()
    {

        return flags;
    }

    /**
     * @return the vars
     */
    public List<string> getVars()
    {

        return vars;
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.EntityResolver#resolveEntity(java.lang.string, java.lang.string)
     */
    //@Override
    //    public InputSource resolveEntity(string publicId, string systemId)
    //{

    //    int startFilename = systemId.lastIndexOf("/") + 1;
    //    string filename = systemId.substring(startFilename, systemId.length());
    //    InputStream inputStream = isCreator.buildInputStream(filename);
    //    return new InputSource(inputStream);

    //}

    /**
     * @return the scorm12
     */
    public bool isScorm12()
    {

        return scorm12;
    }

    /**
     * @return the scorm2004
     */
    public bool isScorm2004()
    {

        return scorm2004;
    }
}
