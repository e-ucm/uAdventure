using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace uAdventure.Core
{
    /**
     * Stores an adaptation profile. Each profile contains the path of the xml file
     * where it is stored (relative to the zip file), along with the list of
     * adaptation rules and initial state defined in the profile
     * 
     * @author Javier
     * 
     */

    public class AdaptationProfile : ContainsAdaptedState, ICloneable
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
         * The Name of the adaptation profile
         */
        // Also is the path of the assessment profile for old game version. In new game version, there arent separate files for assessment,
        // the assessment info is in chapter.xml
        private string name;

        /**
         * The list of adaptation rules
         */
        private List<AdaptationRule> rules;

        /**
         * Initial state defined in the profile
         */
        private AdaptedState initialState;

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

        /**
         * @param name
         * @param rules
         * @param initialState
         * @param scorm12
         * @param scorm2004
         */

        public AdaptationProfile(List<AdaptationRule> rules, AdaptedState initialState, string name, bool scorm12,
            bool scorm2004)
        {

            this.name = name;
            this.rules = rules;
            if (initialState == null)
                this.initialState = new AdaptedState();
            else
                this.initialState = initialState;
            flags = new List<string>();
            vars = new List<string>();
            this.scorm2004 = scorm2004;
            this.scorm12 = scorm12;
        }

        /**
         * Empty constructor
         */

        public AdaptationProfile()
        {

            name = null;
            rules = new List<AdaptationRule>();
            scorm2004 = false;
            scorm12 = false;
            flags = new List<string>();
            vars = new List<string>();
            initialState = new AdaptedState();
        }

        /**
         * @param path
         */

        public AdaptationProfile(string name) : this()
        {
            this.name = name;
        }

        /**
         * @return the name
         */

        public string getName()
        {

            return name;
        }

        /**
         * @param name
         *            the name to set
         */

        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * @return the rules
         */

        public List<AdaptationRule> getRules()
        {

            return rules;
        }

        /**
         * Adds a new rule to the structure
         */

        public void addRule(AdaptationRule rule)
        {

            this.rules.Add(rule);
        }

        /**
         * Adds a new rule to the structure
         */

        public void addRule(AdaptationRule rule, int index)
        {

            this.rules.Insert(index, rule);
        }

        /**
         * @return the initialState
         */

        public AdaptedState getAdaptedState()
        {

            return initialState;
        }

        /**
         * @param initialState
         *            the initialState to set
         */

        public void setAdaptedState(AdaptedState initialState)
        {

            this.initialState = initialState;
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

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AdaptationProfile ap = (AdaptationProfile) super.clone( );
            ap.flags = new List<string>( );
            for( string s : flags )
                ap.flags.add( ( s != null ? new string(s ) : null ) );
            ap.initialState = (AdaptedState) initialState.clone( );
            ap.name = ( name != null ? new string(name ) : null );
            ap.rules = new List<AdaptationRule>( );
            for( AdaptationRule ar : rules )
                ap.rules.add( (AdaptationRule) ar.clone( ) );
            ap.vars = new List<string>( );
            for( string s : vars )
                ap.vars.add( ( s != null ? new string(s ) : null ) );
            ap.scorm12 = scorm12;
            ap.scorm2004 = scorm2004;
            return ap;
        }
        */
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
         * @param scorm2004
         *            the scorm2004 to set
         */

        public void setScorm2004(bool scorm2004)
        {

            this.scorm2004 = scorm2004;
        }

        /**
         * @param scorm12
         *            the scorm12 to set
         */

        public void setScorm12(bool scorm12)
        {

            this.scorm12 = scorm12;
        }

        /**
         * @param initialState
         *            the initialState to set
         */

        public void setInitialState(AdaptedState initialState)
        {

            this.initialState = initialState;
        }

        /**
         * @return the initialState
         */

        public AdaptedState getInitialState()
        {

            return initialState;
        }

        /**
         * @param rules
         *            the rules to set
         */

        public void setRules(List<AdaptationRule> rules)
        {

            this.rules = rules;
        }

        public object Clone()
        {
            AdaptationProfile ap = (AdaptationProfile)this.MemberwiseClone();
            ap.flags = new List<string>();
            foreach (string s in flags)
                ap.flags.Add((s != null ? s : null));
            ap.initialState = (AdaptedState)initialState.Clone();
            ap.name = (name != null ? name : null);
            ap.rules = new List<AdaptationRule>();
            foreach (AdaptationRule ar in rules)
                ap.rules.Add((AdaptationRule)ar.Clone());
            ap.vars = new List<string>();
            foreach (string s in vars)
                ap.vars.Add((s != null ? s : null));
            ap.scorm12 = scorm12;
            ap.scorm2004 = scorm2004;
            return ap;
        }
    }
}