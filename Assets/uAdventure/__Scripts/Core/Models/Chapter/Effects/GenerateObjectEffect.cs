using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that generates an object in the inventory.
     */
    public class GenerateObjectEffect : AbstractEffect, HasTargetId
    {
        /**
         * Id of the item to be generated
         */
        private string idTarget;

        /**
         * Creates a new GenerateObjectEffect.
         * 
         * @param idTarget
         *            the id of the object to be generated
         */
        public GenerateObjectEffect(string idTarget) : base()
        {
            this.idTarget = idTarget;
        }

        public override EffectType getType()
        {
            return EffectType.GENERATE_OBJECT;
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

            GenerateObjectEffect goe = (GenerateObjectEffect) super.clone( );
            goe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            return goe;
        }*/
    }
}