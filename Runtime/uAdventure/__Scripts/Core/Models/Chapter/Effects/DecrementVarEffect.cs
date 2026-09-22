using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that decrements a var according to a given value
     */
    public class DecrementVarEffect : AbstractEffect, HasTargetId
    {
        /**
            * Name of the var
            */
        private string idVar;

        /**
         * Value to be decremented
         */
        private int value;

        /**
         * Creates a new Activate effect.
         * 
         * @param idVar
         *            the id of the var to be activated
         */
        public DecrementVarEffect(string idVar, int value) : base()
        {
            this.idVar = idVar;
            this.value = value;
        }

        public override EffectType getType()
        {

            return EffectType.DECREMENT_VAR;
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
        public int getDecrement()
        {

            return value;
        }

        /**
         * @param value
         *            the value to set
         */
        public void setDecrement(int value)
        {

            this.value = value;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            DecrementVarEffect dve = (DecrementVarEffect) super.clone( );
            dve.idVar = ( idVar != null ? new string(idVar ) : null );
            dve.value = value;
            return dve;
        }*/
    }
}