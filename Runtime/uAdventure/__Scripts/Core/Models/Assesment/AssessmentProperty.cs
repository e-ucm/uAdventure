using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Assessment property, stores an id and a value
     */

    public class AssessmentProperty : HasId, ICloneable
    {

        /**
        * Required
        */
        private const long serialVersionUID = 1L;

        /**
         * Id of the property
         */
        private string id;

        /**
         * Value of the property
         */
        private string value;

        /**
         * If this property dependent on var/flag value, this attribute store its name 
         */
        private string varName;

        /**
         * Default constructor
         * 
         * @param id
         *            Id of the property
         * @param value
         *            Value of the property
         */

        public AssessmentProperty(string id, string value)
        {

            this.id = id;
            this.value = value;
            this.varName = null;

        }

        /**
         * Constructor for properties dependent on in-game values
         * 
         * @param id
         * @param value
         * @param varName
         */

        public AssessmentProperty(string id, string value, string varName)
        {

            this.id = id;
            this.value = value;
            this.varName = varName;

        }

        /**
         * Returns the id of the property
         * 
         * @return Id of the property
         */

        public string getId()
        {

            return id;
        }

        /**
         * Returns the value of the property
         * 
         * @return Value of the property
         */

        public string getValue()
        {

            return value;
        }

        public void setId(string id)
        {

            this.id = id;
        }

        public void setValue(string value)
        {

            this.value = value;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            AssessmentProperty ap = (AssessmentProperty) super.clone( );
            ap.id = ( id != null ? new string(id ) : null );
            ap.value = ( value != null ? new string(value ) : null );
            ap.varName = (varName != null ? new string(varName) : null);
            return ap;
        }*/


        public string getVarName()
        {

            return varName;
        }


        public void setVarName(string varName)
        {

            this.varName = varName;
        }

        public object Clone()
        {
            AssessmentProperty ap = (AssessmentProperty)this.MemberwiseClone();
            ap.id = (id != null ? id : null);
            ap.value = (value != null ? value : null);
            ap.varName = (varName != null ? varName : null);
            return ap;
        }
    }
}