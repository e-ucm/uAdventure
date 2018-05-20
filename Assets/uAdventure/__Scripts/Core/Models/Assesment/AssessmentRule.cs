using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * Rule for the assesment engine
     */

    public class AssessmentRule : HasId, ICloneable
    {
        /**
            * Number of different importance values
            */
        public const int N_IMPORTANCE_VALUES = 5;

        public static int GetN_IMPORTANCE_VALUES()
        {
            return N_IMPORTANCE_VALUES;
        }

        /**
         * Importance value for very low
         */
        public const int IMPORTANCE_VERY_LOW = 0;

        public static int GetIMPORTANCE_VERY_LOW()
        {
            return IMPORTANCE_VERY_LOW;
        }

        /**
         * Importance value for low
         */
        public const int IMPORTANCE_LOW = 1;

        public static int GetIMPORTANCE_LOW()
        {
            return IMPORTANCE_LOW;
        }

        /**
         * Importance value for normal
         */
        public const int IMPORTANCE_NORMAL = 2;

        public static int GetIMPORTANCE_NORMAL()
        {
            return IMPORTANCE_NORMAL;
        }

        /**
         * Importance value for high
         */
        public const int IMPORTANCE_HIGH = 3;

        public static int GetIMPORTANCE_HIGHS()
        {
            return IMPORTANCE_HIGH;
        }

        /**
         * Importance value for very high
         */
        public const int IMPORTANCE_VERY_HIGH = 4;

        public static int GetIMPORTANCE_VERY_HIGH()
        {
            return IMPORTANCE_VERY_HIGH;
        }

        /**
         * string values for the different importance values
         */
        public static readonly string[] IMPORTANCE_VALUES = { "verylow", "low", "normal", "high", "veryhigh" };

        /**
         * Id of the rule
         */
        protected string id;

        /**
         * Importance of the rule, stored as an int
         */
        protected int importance;

        /**
         * Concept of the rule
         */
        protected string concept;

        /**
         * Conditions for the rule to trigger
         */
        protected Conditions conditions;

        /**
         * The effect of the rule
         */
        protected AssessmentEffect effect;

        /**
         * If it is active, the assessment rule can be executed more than 1 times
         */
        protected bool repeatRule;

        /**
         * Default constructor
         * 
         * @param id
         *            Id of the rule
         * @param importance
         *            Importance of the rule
         */

        public AssessmentRule(string id, int importance, bool repeatRule)
        {

            this.id = id;
            this.importance = importance;
            concept = null;
            conditions = new Conditions();
            effect = new AssessmentEffect();
            this.repeatRule = repeatRule;
        }

        /**
         * Sets the concept of the rule
         * 
         * @param concept
         *            Concept of the rule
         */

        public void setConcept(string concept)
        {

            this.concept = concept;
        }

        /**
         * Sets the conditions of the rule
         * 
         * @param conditions
         *            Conditions of the rule
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        /**
         * Sets the text of the rule
         * 
         * @param text
         *            Text of the rule
         */

        public virtual void setText(string text)
        {

            effect.setText(text);
        }

        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */

        public virtual void addProperty(AssessmentProperty property)
        {

            effect.getAssessmentProperties().Add(property);
        }

        /**
         * Return the rule's id
         * 
         * @return Id of the rule
         */

        public string getId()
        {

            return id;
        }

        /**
         * Return the rule's importance
         * 
         * @return Importance of the rule
         */

        public int getImportance()
        {

            return importance;
        }

        /**
         * Returns the rule's concept
         * 
         * @return Concept of the rule
         */

        public string getConcept()
        {

            return concept;
        }

        /**
         * Returns the rule's text (if present)
         * 
         * @return Text of the rule if present, null otherwise
         */

        public string getText()
        {

            return effect.getText();
        }

        public List<AssessmentProperty> getAssessmentProperties()
        {
            return effect.getAssessmentProperties();
        }

        /**
         * @param importance
         *            the importance to set
         */

        public void setImportance(int importance)
        {

            this.importance = importance;
        }

        /**
         * @return the conditions
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        public void setId(string assRuleId)
        {

            this.id = assRuleId;
        }

        /**
         * 
         * @return if the rule can be executed one or more times
         */

        public bool isRepeatRule()
        {

            return repeatRule;
        }


        public void setRepeatRule(bool repeatRule)
        {

            this.repeatRule = repeatRule;
        }

        public virtual object Clone()
        {
            AssessmentRule ar = (AssessmentRule)this.MemberwiseClone();
            ar.concept = (concept != null ? concept : null);
            if (conditions != null)
                ar.conditions = (Conditions)conditions.Clone();
            if (effect != null)
                ar.effect = (AssessmentEffect)effect.Clone();
            ar.id = (id != null ? id : null);
            ar.importance = importance;
            ar.repeatRule = repeatRule;
            return ar;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       AssessmentRule ar = (AssessmentRule) super.clone( );
       ar.concept = ( concept != null ? new string(concept ) : null );
       if( conditions != null )
           ar.conditions = (Conditions) conditions.clone( );
       if( effect != null )
           ar.effect = (AssessmentEffect) effect.clone( );
       ar.id = ( id != null ? new string(id ) : null );
       ar.importance = importance;
       ar.repeatRule = repeatRule;
       return ar;
    }*/
    }
}