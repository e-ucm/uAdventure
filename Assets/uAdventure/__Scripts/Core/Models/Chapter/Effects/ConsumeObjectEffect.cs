using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that consumes an object in the inventory
     */
    public class ConsumeObjectEffect : AbstractEffect, HasTargetId
    {

        /**
         * Id of the item to be consumed
         */
        private string idTarget;

        /**
         * Creates a new ConsumeObjectEffect.
         * 
         * @param idTarget
         *            the id of the object to be consumed
         */
        public ConsumeObjectEffect(string idTarget) : base()
        {
            this.idTarget = idTarget;
        }

        public override EffectType getType()
        {

            return EffectType.CONSUME_OBJECT;
        }

        /**
         * Returns the idTarget
         * 
         * @return string containing the idTarget
         */
        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Sets the new idTarget
         * 
         * @param idTarget
         *            New idTarget
         */
        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ConsumeObjectEffect coe = (ConsumeObjectEffect) super.clone( );
            coe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            return coe;
        }*/
    }
}