using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that triggers a cutscene
     */
    public class TriggerCutsceneEffect : AbstractEffect, HasTargetId
    {
        /**
             * Id of the cutscene to be played
             */
        private string targetCutsceneId;

        /**
         * Creates a new TriggerCutsceneEffect
         * 
         * @param targetCutsceneId
         *            the id of the cutscene to be triggered
         */
        public TriggerCutsceneEffect(string targetCutsceneId) : base()
        {
            this.targetCutsceneId = targetCutsceneId;
        }

        public override EffectType getType()
        {

            return EffectType.TRIGGER_CUTSCENE;
        }

        /**
         * Returns the targetCutsceneId
         * 
         * @return string containing the targetCutsceneId
         */
        public string getTargetId()
        {

            return targetCutsceneId;
        }

        /**
         * Sets the new targetCutsceneId
         * 
         * @param targetCutsceneId
         *            New targetCutsceneId
         */
        public void setTargetId(string targetCutsceneId)
        {

            this.targetCutsceneId = targetCutsceneId;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TriggerCutsceneEffect tce = (TriggerCutsceneEffect) super.clone( );
            tce.targetCutsceneId = ( targetCutsceneId != null ? new string(targetCutsceneId ) : null );
            return tce;
        }*/
    }
}