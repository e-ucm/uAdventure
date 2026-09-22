using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that activates a flag
     */
    public class ActivateEffect : AbstractEffect, HasTargetId
    {

        /**
         * Name of the flag to be activated
         */
        private string idFlag;

        /**
         * Creates a new Activate effect.
         * 
         * @param idFlag
         *            the id of the flag to be activated
         */
        public ActivateEffect(string idFlag) : base()
        {
            this.idFlag = idFlag;
        }

        public override EffectType getType()
        {

            return EffectType.ACTIVATE;
        }

        /**
         * Returns the idFlag
         * 
         * @return string containing the idFlag
         */
        public string getTargetId()
        {

            return idFlag;
        }

        /**
         * Sets the new idFlag
         * 
         * @param idFlag
         *            New idFlag
         */
        public void setTargetId(string idFlag)
        {

            this.idFlag = idFlag;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ActivateEffect ae = (ActivateEffect) super.clone( );
            ae.idFlag = ( idFlag != null ? new string(idFlag ) : null );
            return ae;
        }*/
    }
}