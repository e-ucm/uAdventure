using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that triggers a conversation.
     */
    public class TriggerConversationEffect : AbstractEffect, HasTargetId
    {
        /**
        * Id of the conversation to be played
        */
        private string targetConversationId;

        /**
         * Creates a new TriggerConversationEffect.
         * 
         * @param targetConversationId
         *            the id of the conversation to be triggered
         */
        public TriggerConversationEffect(string targetConversationId) : base()
        {
            this.targetConversationId = targetConversationId;
        }

        public override EffectType getType()
        {
            return EffectType.TRIGGER_CONVERSATION;
        }

        /**
         * Returns the targetConversationId
         * 
         * @return string containing the targetConversationId
         */
        public string getTargetId()
        {

            return targetConversationId;
        }

        /**
         * Sets the new targetConversationId
         * 
         * @param targetConversationId
         *            New targetConversationId
         */
        public void setTargetId(string targetConversationId)
        {

            this.targetConversationId = targetConversationId;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TriggerConversationEffect tce = (TriggerConversationEffect) super.clone( );
            tce.targetConversationId = ( targetConversationId != null ? new string(targetConversationId ) : null );
            return tce;
        }*/
    }
}