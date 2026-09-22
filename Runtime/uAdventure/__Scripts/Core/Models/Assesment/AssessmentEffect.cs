using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class AssessmentEffect : ICloneable
    {

        /**
             * Text of the effect of the rule, if present (null if not)
             */
        protected string text;

        /**
         * List of properties to be set
         */
        protected List<AssessmentProperty> properties;

        public AssessmentEffect()
        {
            text = null;
            properties = new List<AssessmentProperty>();
        }

        /**
         * Sets the text of the rule
         * 
         * @param text
         *            Text of the rule
         */
        public void setText(string text)
        {

            this.text = text;
        }

        public void SetAssesmentPropertiesList(List<AssessmentProperty> list)
        {
            this.properties = list;
        }
        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */
        public void addProperty(AssessmentProperty property)
        {

            properties.Add(property);
        }

        /**
         * Returns the rule's text (if present)
         * 
         * @return Text of the rule if present, null otherwise
         */
        public string getText()
        {
            return text;
        }

        public virtual List<AssessmentProperty> getAssessmentProperties()
        {

            return properties;
        }

        public virtual object Clone()
        {
            AssessmentEffect ae = (AssessmentEffect)this.MemberwiseClone();
            if (properties != null)
            {
                ae.properties = new List<AssessmentProperty>();
                foreach (AssessmentProperty ap in properties)
                    ae.properties.Add((AssessmentProperty)ap.Clone());
            }
            ae.text = (text != null ? text : null);
            return ae;
        }
        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       AssessmentEffect ae = (AssessmentEffect) super.clone( );
       if( properties != null ) {
           ae.properties = new List<AssessmentProperty>();
           for (AssessmentProperty ap : properties)
               ae.properties.add((AssessmentProperty)ap.clone());
       }
       ae.text = ( text != null ? new String(text ) : null );
       return ae;

    }*/
    }
}