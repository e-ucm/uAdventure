using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class is the handler to parse the assesment rules file of the adventure.
 * This class is only used for specific xml assessment files, that is a past
 * characteristic, to preserve past game version. In new versions, the
 * assessment info is in chapter.xml file. For this reason the parsing of
 * assessment is now in chapter parsing (ChapterHandler)
 */
public class AssessmentHandler : XMLHandler
{
    /* Attributes */

    /**
     * string to store the current string in the XML file
     */
    private string currentstring;

    /**
     * Constant for reading nothing
     */
    private const int READING_NONE = 0;

    /**
     * Constant for reading either tag
     */
    private const int READING_EITHER = 1;

    /**
     * Stores the current element being read
     */
    private int reading = READING_NONE;

    /**
     * Array of assessment rules
     */
    private List<AssessmentRule> assessmentRules;

    /**
     * List of flags involved in this assessment script
     */
    private List<string> flags;

    /**
     * List of vars involved in this assessment script
     */
    private List<string> vars;

    /**
     * Assessment rule currently being read
     */
    private AssessmentRule currentAssessmentRule;

    /**
     * Set of conditions being read
     */
    private Conditions currentConditions;

    /**
     * Set of either conditions being read
     */
    private Conditions currentEitherCondition;

    /**
     * InputStreamCreator used in resolveEntity to find dtds (only required in
     * Applet mode)
     */
    private InputStreamCreator isCreator;

    /**
     * The assessment profile
     */
    private AssessmentProfile profile;

    /* Methods */

    /**
     * Default constructor
     */
    public AssessmentHandler(InputStreamCreator isCreator, AssessmentProfile profile)
    {

        this.profile = profile;
        if (profile.getRules() == null)
            assessmentRules = new List<AssessmentRule>();
        else
            assessmentRules = profile.getRules();
        currentAssessmentRule = null;
        currentstring = string.Empty;
        vars = new List<string>();
        flags = new List<string>();
        profile.setFlags(flags);
        profile.setVars(vars);
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

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#startElement(java.lang.string, java.lang.string, java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        if (qName.Equals("assessment-rules"))
        {

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("show-report-at-end"))
                {
                    profile.setShowReportAtEnd(entry.Value.ToString().Equals("yes"));
                }
                if (entry.Key.Equals("send-to-email"))
                {
                    if (entry.Value.ToString() == null || entry.Value.ToString().ToString().Length < 1)
                    {
                        profile.setEmail("");
                        profile.setSendByEmail(false);
                    }
                    else {
                        profile.setEmail(entry.Value.ToString());
                        profile.setSendByEmail(true);
                    }
                }
                if (entry.Key.Equals("scorm12"))
                {
                    profile.setScorm12(entry.Value.ToString().Equals("yes"));
                }
                if (entry.Key.Equals("scorm2004"))
                {
                    profile.setScorm2004(entry.Value.ToString().Equals("yes"));
                }
            }

        }
        else if (qName.Equals("smtp-config"))
        {
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("smtp-ssl"))
                    profile.setSmtpSSL(entry.Value.ToString().Equals("yes"));
                if (entry.Key.Equals("smtp-server"))
                    profile.setSmtpServer(entry.Value.ToString());
                if (entry.Key.Equals("smtp-port"))
                    profile.setSmtpPort(entry.Value.ToString());
                if (entry.Key.Equals("smtp-user"))
                    profile.setSmtpUser(entry.Value.ToString());
                if (entry.Key.Equals("smtp-pwd"))
                    profile.setSmtpPwd(entry.Value.ToString());
            }
        }

        else if (qName.Equals("assessment-rule"))
        {

            string id = null;
            int importance = 0;
            bool repeatRule = false;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                    id = entry.Value.ToString();
                if (entry.Key.Equals("importance"))
                {
                    for (int j = 0; j < AssessmentRule.N_IMPORTANCE_VALUES; j++)
                        if (entry.Value.ToString().Equals(AssessmentRule.IMPORTANCE_VALUES[j]))
                            importance = j;
                }
                if (entry.Key.Equals("repeatRule"))
                    repeatRule = entry.Value.ToString().Equals("yes");
            }

            currentAssessmentRule = new AssessmentRule(id, importance, repeatRule);
        }

        else if (qName.Equals("timed-assessment-rule"))
        {

            string id = null;
            int importance = 0;
            bool usesEndConditions = false;
            bool has = false;
            bool repeatRule = false;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                    id = entry.Value.ToString();
                if (entry.Key.Equals("importance"))
                {
                    for (int j = 0; j < AssessmentRule.N_IMPORTANCE_VALUES; j++)
                        if (entry.Value.ToString().Equals(AssessmentRule.IMPORTANCE_VALUES[j]))
                            importance = j;
                }
                if (entry.Key.Equals("usesEndConditions"))
                {
                    has = true;
                    usesEndConditions = entry.Value.ToString().Equals("yes");
                }
                if (entry.Key.Equals("repeatRule"))
                    repeatRule = entry.Value.ToString().Equals("yes");
            }

            currentAssessmentRule = new TimedAssessmentRule(id, importance, repeatRule);
            if (has)
                ((TimedAssessmentRule)currentAssessmentRule).setUsesEndConditions(usesEndConditions);
        }

        else if (qName.Equals("condition") || qName.Equals("init-condition") || qName.Equals("end-condition"))
        {
            currentConditions = new Conditions();
        }

        // If it is an either tag, create a new either conditions and switch the state
        else if (qName.Equals("either"))
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
                        currentConditions.add(new FlagCondition(entry.Value.ToString()));
                    if (reading == READING_EITHER)
                        currentEitherCondition.add(new FlagCondition(entry.Value.ToString()));
                    addFlag(entry.Value.ToString());
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
                        currentConditions.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_INACTIVE));
                    if (reading == READING_EITHER)
                        currentEitherCondition.add(new FlagCondition(entry.Value.ToString(), FlagCondition.FLAG_INACTIVE));
                    addFlag(entry.Value.ToString());
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_THAN, value));
            addVar(var);
        }

        // If it is a greater-equals-than tag
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_GREATER_EQUALS_THAN, value));
            addVar(var);
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_THAN, value));
            addVar(var);
        }

        // If it is a less-equals-than tag
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_LESS_EQUALS_THAN, value));
            addVar(var);
        }

        // If it is a equals-than tag
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_EQUALS, value));
            addVar(var);
        }

        // If it is a not-equals-than tag
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
                currentConditions.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new VarCondition(var, VarCondition.VAR_NOT_EQUALS, value));
            addVar(var);
        }


        // If it is a global-state-reference tag
        else if (qName.Equals("global-state-ref"))
        {
            // Id
            string id = null;
            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                {
                    id = entry.Value.ToString();
                }
            }
            // Store the inactive flag in the conditions or either conditions
            if (reading == READING_NONE)
                currentConditions.add(new GlobalStateCondition(id));
            if (reading == READING_EITHER)
                currentEitherCondition.add(new GlobalStateCondition(id));
        }

        else if (qName.Equals("set-property"))
        {
            string id = null;
            string value = null;

            foreach (KeyValuePair<string, string> entry in attrs)
            {
                if (entry.Key.Equals("id"))
                    id = entry.Value.ToString();
                if (entry.Key.Equals("value"))
                    value = entry.Value.ToString();

            }

            currentAssessmentRule.addProperty(new AssessmentProperty(id, value));
        }

        else if (qName.Equals("effect"))
        {
            if (currentAssessmentRule is TimedAssessmentRule)
            {
                int timeMin = int.MinValue;
                int timeMax = int.MinValue;
                foreach (KeyValuePair<string, string> entry in attrs)
                {

                    if (entry.Key.Equals("time-min"))
                        timeMin = int.Parse(entry.Value.ToString());
                    if (entry.Key.Equals("time-max"))
                        timeMax = int.Parse(entry.Value.ToString());
                }

                TimedAssessmentRule tRule = (TimedAssessmentRule)currentAssessmentRule;
                if (timeMin != int.MinValue && timeMax != int.MaxValue)
                {
                    tRule.addEffect(timeMin, timeMax);
                }
                else {
                    tRule.addEffect();
                }
            }
        }
    }

    /*  
     *  (non-Javadoc)
     * @see org.xml.sax.ContentHandler#endElement(java.lang.string, java.lang.string, java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        if (qName.Equals("assessment-rule") || qName.Equals("timed-assessment-rule"))
        {
            assessmentRules.Add(currentAssessmentRule);
        }

        else if (qName.Equals("concept"))
        {
            currentAssessmentRule.setConcept(currentstring.ToString().Trim());
        }

        else if (qName.Equals("condition"))
        {
            currentAssessmentRule.setConditions(currentConditions);
        }

        else if (qName.Equals("init-condition"))
        {
            ((TimedAssessmentRule)currentAssessmentRule).setInitConditions(currentConditions);
        }

        else if (qName.Equals("end-condition"))
        {
            ((TimedAssessmentRule)currentAssessmentRule).setEndConditions(currentConditions);
        }

        // If it is an either tag
        else if (qName.Equals("either"))
        {
            // Store the either condition in the condition, and switch the state back to normal
            currentConditions.add(currentEitherCondition);
            reading = READING_NONE;
        }

        else if (qName.Equals("set-text"))
        {
            currentAssessmentRule.setText(currentstring.ToString().Trim());
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
        currentstring += new string(buf, offset, len);
    }

    /*
     *  (non-Javadoc)
     * @see org.xml.sax.ErrorHandler#error(org.xml.sax.SAXParseException)
     */
    //    @Override
    //    public void error(SAXParseException exception) throws SAXParseException
    //{

    //    // On validation, propagate exception
    //    exception.printStackTrace( );
    //        throw exception;
    //}

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
}