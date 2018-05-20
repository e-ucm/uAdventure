using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that sets a var with a given value
     */
    public class SetValueEffect : AbstractEffect, HasTargetId
    {/**
     * Name of the var to be activated
     */
        private string idVar;

        /**
         * Value to be set
         */
        private int value;

        /**
         * Creates a new Activate effect.
         * 
         * @param idVar
         *            the id of the var to be activated
         */
        public SetValueEffect(string idVar, int value) : base()
        {
            this.idVar = idVar;
            this.value = value;
        }

        public override EffectType getType()
        {

            return EffectType.SET_VALUE;
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
        public int getValue()
        {

            return value;
        }

        /**
         * @param value
         *            the value to set
         */
        public void setValue(int value)
        {

            this.value = value;
        }
        /*
            @Override
            public Object clone() throws CloneNotSupportedException
            {

                SetValueEffect sve = (SetValueEffect) super.clone( );
                sve.idVar = ( idVar != null ? new string(idVar ) : null );
                sve.value = value;
                return sve;
            }*/
    }
}