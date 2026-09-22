using System.Collections;
using System.Collections.Generic;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Core
{
    public class ChangeOrientationEffect : AbstractEffect, HasTargetId
    {

        /**
         * Id of the item to be consumed
         */
        private string idTarget;

        private Orientation orientation;

        /**
         * Creates a new ConsumeObjectEffect.
         * 
         * @param idTarget
         *            the id of the object to be consumed
         */
        public ChangeOrientationEffect(string idTarget, Orientation orientation) : base()
        {
            this.idTarget = idTarget;
            this.orientation = orientation;
        }

        public override EffectType getType()
        {

            return EffectType.CUSTOM_EFFECT;
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

        public Orientation GetOrientation()
        {
            return orientation;
        }

        public void SetOrientation(Orientation value)
        {
            orientation = value;
        }
    }
}
