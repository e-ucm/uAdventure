using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that raises a "bookscene".
     */
    public class TriggerBookEffect : AbstractEffect, HasTargetId
    {
        /**
         * Id of the book to be shown
         */
        private string targetBookId;

        /**
         * Creates a new TriggerBookEffect
         * 
         * @param targetBookId
         *            the id of the book to be shown
         */
        public TriggerBookEffect(string targetBookId) : base()
        {
            this.targetBookId = targetBookId;
        }

        public override EffectType getType()
        {
            return EffectType.TRIGGER_BOOK;
        }

        /**
         * Returns the targetBookId
         * 
         * @return string containing the targetBookId
         */
        public string getTargetId()
        {

            return targetBookId;
        }

        /**
         * Sets the new targetBookId
         * 
         * @param targetBookId
         *            New targetBookId
         */
        public void setTargetId(string targetBookId)
        {

            this.targetBookId = targetBookId;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TriggerBookEffect tbe = (TriggerBookEffect) super.clone( );
            tbe.targetBookId = ( targetBookId != null ? new string(targetBookId ) : null );
            return tbe;
        }*/
    }
}