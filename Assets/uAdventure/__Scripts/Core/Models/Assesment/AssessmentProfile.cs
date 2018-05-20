using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace uAdventure.Core
{
    /**
     * Stores an assessment profile. Each profile contains the path of the xml file
     * where it is stored (relative to the zip file), along with the list of
     * assessment rules defined in the profile
     * 
     * @author Javier
     * 
     */
    public class AssessmentProfile : ICloneable
    {
        /**
            * Constants to identify the comparison operations
            */
        public const string EQUALS = "eq";

        public const string GRATER = "gt";

        public const string LESS = "lt";

        public const string GRATER_EQ = "ge";

        public const string LESS_EQ = "le";

        /**
         * the name of assessment profile
         */
        // Also is the path of the assessment profile for old game version. In new game version, there arent separate files for assessment,
        // the assessment info is in chapter.xml
        private string name;

        /**
         * The list of assessment rules
         */
        private List<AssessmentRule> rules;

        /**
         * List of referenced flags
         */
        private List<string> flags;

        /**
         * List of referenced vars
         */
        private List<string> vars;

        /**
         * Store if adaptation profile is for scorm2004
         */
        private bool scorm2004;

        /**
         * Store if adaptation profile is for scorm 1.2
         */
        private bool scorm12;

        ////////FEEDBACK
        /**
         * If true, the assessment report is shown to the student at the end of the
         * chapter
         */
        private bool showReportAtEnd;

        /**
         * If true, the student will be asked to send an email with the report
         */
        private bool sendByEmail;

        /**
         * The email where the student's report must be sent
         */
        private string email;

        private bool smtpSSL;

        private string smtpServer;

        private string smtpPort;

        private string smtpUser;

        private string smtpPwd;

        /**
         * Empty constructor
         */
        public AssessmentProfile() : this(new List<AssessmentRule>(), null)
        {
        }

        /**
         * @param path
         */
        public AssessmentProfile(string path) : this(new List<AssessmentRule>(), path)
        {
        }

        public AssessmentProfile(List<AssessmentRule> assessmentRules, string name)
        {

            rules = assessmentRules;
            this.name = name;
            flags = new List<string>();
            vars = new List<string>();
            sendByEmail = false;
            email = "";
        }

        /**
         * @return the name
         */
        public string getName()
        {

            return name;
        }

        /**
         * @param path
         *            the name to set
         */
        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * @return the rules
         */
        public List<AssessmentRule> getRules()
        {

            return rules;
        }

        /**
         * Adds a new rule to the structure
         */
        public void addRule(AssessmentRule rule)
        {

            this.rules.Add(rule);
        }

        /**
         * Adds a new rule to the structure in the given position
         */
        public void addRule(AssessmentRule rule, int index)
        {
            //????
            this.rules.Insert(index, rule);
        }

        /**
         * Set all the rules
         * 
         * @param assessmentRules
         */
        public void setRules(List<AssessmentRule> assessmentRules)
        {

            this.rules = assessmentRules;
        }

        /**
         * Adds a new flag
         * 
         * @param flag
         */
        public void addFlag(string flag)
        {

            if (!flags.Contains(flag))
            {
                flags.Add(flag);
            }
        }

        /**
         * Adds a new var
         * 
         * @param var
         */
        public void addVar(string var)
        {

            if (!vars.Contains(var))
            {
                vars.Add(var);
            }
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

        /**
         * @param flags
         *            the flags to set
         */
        public void setFlags(List<string> flags)
        {

            this.flags = flags;
        }

        /**
         * @param vars
         *            the vars to set
         */
        public void setVars(List<string> vars)
        {

            this.vars = vars;
        }

        /**
         * @return the showReportAtEnd
         */
        public bool isShowReportAtEnd()
        {

            return showReportAtEnd;
        }

        /**
         * @param showReportAtEnd
         *            the showReportAtEnd to set
         */
        public void setShowReportAtEnd(bool showReportAtEnd)
        {

            this.showReportAtEnd = showReportAtEnd;
        }

        /**
         * @return the sendByEmail
         */
        public bool isSendByEmail()
        {

            return sendByEmail;
        }

        /**
         * @param sendByEmail
         *            the sendByEmail to set
         */
        public void setSendByEmail(bool sendByEmail)
        {

            this.sendByEmail = sendByEmail;
        }

        /**
         * @return the email
         */
        public string getEmail()
        {

            return email;
        }

        /**
         * @param email
         *            the email to set
         */
        public void setEmail(string email)
        {

            this.email = email;
        }

        public void setSmtpSSL(bool equals)
        {

            smtpSSL = equals;
        }

        public void setSmtpPort(string value)
        {

            smtpPort = value;
        }

        public void setSmtpUser(string value)
        {

            smtpUser = value;
        }

        public void setSmtpPwd(string value)
        {

            smtpPwd = value;
        }

        /**
         * @return the smtpSSL
         */
        public bool isSmtpSSL()
        {

            return smtpSSL;
        }

        /**
         * @return the smtpPort
         */
        public string getSmtpPort()
        {

            return smtpPort;
        }

        /**
         * @return the smtpUser
         */
        public string getSmtpUser()
        {

            return smtpUser;
        }

        /**
         * @return the smtpPwd
         */
        public string getSmtpPwd()
        {

            return smtpPwd;
        }

        public void setSmtpServer(string value)
        {

            this.smtpServer = value;
        }

        public string getSmtpServer()
        {

            return smtpServer;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AssessmentProfile ap = (AssessmentProfile) super.clone( );
            ap.email = ( email != null ? new string(email ) : null );
            if( flags != null ) {
                ap.flags = new List<string>( );
                for( string s : flags )
                    ap.flags.add( ( s != null ? new string(s ) : null ) );
            }
    ap.name = ( name != null ? new string(name ) : null );
            if( rules != null ) {
                ap.rules = new List<AssessmentRule>( );
                for( AssessmentRule ar : rules )
                    ap.rules.add( (AssessmentRule) ar.clone( ) );
            }
            ap.sendByEmail = sendByEmail;
            ap.showReportAtEnd = showReportAtEnd;
            ap.smtpPort = ( smtpPort != null ? new string(smtpPort ) : null );
            ap.smtpPwd = ( smtpPwd != null ? new string(smtpPwd ) : null );
            ap.smtpServer = ( smtpServer != null ? new string(smtpServer ) : null );
            ap.smtpSSL = smtpSSL;
            ap.smtpUser = ( smtpUser != null ? new string(smtpUser ) : null );
            if( vars != null ) {
                ap.vars = new List<string>( );
                for( string s : vars ) {
                    ap.vars.add( ( s != null ? new string(s ) : null ) );
                }
            }
            ap.scorm12 = scorm12;
            ap.scorm2004 = scorm2004;
            return ap;
        }*/

        /**
         * @return the scorm2004
         */
        public bool isScorm2004()
        {

            return scorm2004;
        }

        /**
         * Changes to scorm2004 profile
         */
        public void changeToScorm2004Profile()
        {

            scorm2004 = true;
            scorm12 = false;
        }

        /**
         * @return the scorm12
         */
        public bool isScorm12()
        {

            return scorm12;
        }

        /**
         * Changes to scorm12 profile
         */
        public void changeToScorm12Profile()
        {

            scorm2004 = false;
            scorm12 = true;
        }

        /**
         * Changes to scorm2004 profile
         */
        public void changeToNormalProfile()
        {

            scorm2004 = false;
            scorm12 = false;
        }

        /**
         * Returns all operation representation
         * 
         * @return
         */
        public static string[] getOperations()
        {

            return new string[] { "=", ">", "<", ">=", "<=" };
        }

        /**
         * Gets the operation name from an operation representation
         * 
         * @param ope
         *            the representation of the operation
         * @return the name of the operation
         */
        public static string getOpName(Object ope)
        {

            string op = string.Empty;
            if (ope.Equals("="))
            {
                op = EQUALS;
            }
            else if (ope.Equals(">"))
            {
                op = GRATER;
            }
            else if (ope.Equals("<"))
            {
                op = LESS;
            }
            else if (ope.Equals(">="))
            {
                op = GRATER_EQ;
            }
            else if (ope.Equals("<="))
            {
                op = LESS_EQ;
            }
            return op;
        }

        /**
         * Gets the operation representation from an operation name
         * 
         * @param ope
         *            the name of the operation
         * @return the representation of the operation
         */
        public static string getOpRepresentation(Object ope)
        {

            string op = string.Empty;
            if (ope.Equals(EQUALS))
            {
                op = "=";
            }
            else if (ope.Equals(GRATER))
            {
                op = ">";
            }
            else if (ope.Equals(LESS))
            {
                op = "<";
            }
            else if (ope.Equals(GRATER_EQ))
            {
                op = ">=";
            }
            else if (ope.Equals(LESS_EQ))
            {
                op = "<=";
            }
            return op;
        }

        /**
         * Don´t use this method in the editor
         * 
         * @param scorm2004
         *            the scorm2004 to set
         */
        // It is only use in AssessmentHandler to set the value during the parsing
        public void setScorm2004(bool scorm2004)
        {

            this.scorm2004 = scorm2004;
        }

        /**
         * Don´t use this method in the editor
         * 
         * @param scorm12
         *            the scorm12 to set
         */
        // It is only use in AssessmentHandler to set the value during the parsing
        public void setScorm12(bool scorm12)
        {

            this.scorm12 = scorm12;
        }

        public object Clone()
        {
            AssessmentProfile ap = (AssessmentProfile)this.MemberwiseClone();
            ap.email = (email != null ? email : null);
            if (flags != null)
            {
                ap.flags = new List<string>();
                foreach (string s in flags)
                    ap.flags.Add((s != null ? s : null));
            }
            ap.name = (name != null ? name : null);
            if (rules != null)
            {
                ap.rules = new List<AssessmentRule>();
                foreach (AssessmentRule ar in rules)
                    ap.rules.Add((AssessmentRule)ar.Clone());
            }
            ap.sendByEmail = sendByEmail;
            ap.showReportAtEnd = showReportAtEnd;
            ap.smtpPort = (smtpPort != null ? smtpPort : null);
            ap.smtpPwd = (smtpPwd != null ? smtpPwd : null);
            ap.smtpServer = (smtpServer != null ? smtpServer : null);
            ap.smtpSSL = smtpSSL;
            ap.smtpUser = (smtpUser != null ? smtpUser : null);
            if (vars != null)
            {
                ap.vars = new List<string>();
                foreach (string s in vars)
                {
                    ap.vars.Add((s != null ? s : null));
                }
            }
            ap.scorm12 = scorm12;
            ap.scorm2004 = scorm2004;
            return ap;
        }
    }
}