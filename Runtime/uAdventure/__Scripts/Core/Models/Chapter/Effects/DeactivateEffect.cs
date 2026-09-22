using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that deactivates a flag.
     */
    public class DeactivateEffect : AbstractEffect, HasTargetId
    {

        /**
         * Name of the flag to be activated
         */
        private string idFlag;

        /**
         * Creates a new DeactivateEffect.
         * 
         * @param idFlag
         *            the id of the flag to be deactivated
         */
        public DeactivateEffect(string idFlag) : base()
        {
            this.idFlag = idFlag;
        }


        public override EffectType getType()
        {

            return EffectType.DEACTIVATE;
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

            DeactivateEffect de = (DeactivateEffect) super.clone( );
            de.idFlag = ( idFlag != null ? new string(idFlag ) : null );
            return de;
        }*/
    }
}