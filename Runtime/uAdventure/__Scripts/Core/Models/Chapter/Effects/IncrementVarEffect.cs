using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that increments a var according to a given value
     */
    public class IncrementVarEffect : AbstractEffect, HasTargetId
    {
        /**
            * Name of the var
            */
        private string idVar;

        /**
         * Value to be incremented
         */
        private int value;

        /**
         * Creates a new Activate effect.
         * 
         * @param idVar
         *            the id of the var to be activated
         */
        public IncrementVarEffect(string idVar, int value) : base()
        {
            this.idVar = idVar;
            this.value = value;
        }


        public override EffectType getType()
        {

            return EffectType.INCREMENT_VAR;
        }

        /**
         * Returns the idVar
         * 
         * @return string containing the idVar
         */
        public string getTargetId()
        {

            return idVar;
        }

        /**
         * Sets the new idVar
         * 
         * @param idVar
         *            New idVar
         */
        public void setTargetId(string idVar)
        {

            this.idVar = idVar;
        }

        /**
         * @return the value
         */
        public int getIncrement()
        {

            return value;
        }

        /**
         * @param value
         *            the value to set
         */
        public void setIncrement(int value)
        {

            this.value = value;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            IncrementVarEffect ive = (IncrementVarEffect) super.clone( );
            ive.idVar = ( idVar != null ? new string(idVar ) : null );
            ive.value = value;
            return ive;
        }*/
    }
}