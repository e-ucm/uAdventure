using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * LMS property, stores an id and a value, for adaptation purposes
     */
    public class UOLProperty : ICloneable, HasId
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
         * The type of the comparison operation between LMS value of "id" and
         * attribute "value"
         */
        private string operation;

        /**
         * Default constructor
         * 
         * @param id
         *            Id of the property
         * @param value
         *            Value of the property
         * @param operation
         *            The comparison operation between LMS value of Id and Value
         */
        public UOLProperty(string id, string value, string operation)
        {

            this.id = id;
            this.value = value;
            this.operation = operation;
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

        /**
         * @return the operation
         */
        public string getOperation()
        {

            return operation;
        }

        /**
         * @param operation
         *            the operation to set
         */
        public void setOperation(string operation)
        {

            this.operation = operation;
        }

        public object Clone()
        {
            UOLProperty uolp = (UOLProperty)this.MemberwiseClone();
            uolp.id = (id != null ? id : null);
            uolp.value = (value != null ? value : null);
            uolp.operation = (operation != null ? operation : null);
            return uolp;
        }
        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       UOLProperty uolp = (UOLProperty) super.clone( );
       uolp.id = ( id != null ? new string(id ) : null );
       uolp.value = ( value != null ? new string(value ) : null );
       uolp.operation = ( operation != null ? new string(operation ) : null );
       ;
       return uolp;
    }*/
    }
}