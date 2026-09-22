using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that triggers a scene
     */
    public class TriggerSceneEffect : AbstractEffect, HasTargetId
    {
        /**
         * Id of the cutscene to be played
         */
        private string targetSceneId;

        /**
         * X position of the player in the scene.
         */
        private int x;

        /**
         * Y position of the player in the scene.
         */
        private int y;

        /**
         * Transition duration
         */
        private int transitionTime = 0;

        /**
         * Type of the transition
         */
        private TransitionType transitionType = 0;

        private float destinyScale;

        public float DestinyScale
        {
            get
            {
                return destinyScale;
            }

            set
            {
                destinyScale = value;
            }
        }

        /**
         * Creates a new TriggerSceneEffect
         * 
         * @param targetSceneId
         *            the id of the cutscene to be triggered
         * @param x
         *            X position of the player in the new scene
         * @param y
         *            Y position of the player in the new scene
         */
        public TriggerSceneEffect(string targetSceneId, int x, int y, float destinyScale = float.MinValue, int transitionTime = 0, int transitionType = 0) : base()
        {
            this.targetSceneId = targetSceneId;
            this.x = x;
            this.y = y;
            this.DestinyScale = destinyScale;
            this.transitionTime = transitionTime;
            this.transitionType = (TransitionType)transitionType;
        }

        public override EffectType getType()
        {

            return EffectType.TRIGGER_SCENE;
        }

        /**
         * Returns the targetSceneId
         * 
         * @return string containing the targetSceneId
         */
        public string getTargetId()
        {

            return targetSceneId;
        }

        /**
         * Sets the new targetSceneId
         * 
         * @param targetSceneId
         *            New targetSceneId
         */
        public void setTargetId(string targetSceneId)
        {

            this.targetSceneId = targetSceneId;
        }

        /**
         * Returns the X destiny position of the player.
         * 
         * @return X destiny position
         */
        public int getX()
        {

            return x;
        }

        /**
         * Returns the Y destiny position of the player.
         * 
         * @return Y destiny position
         */
        public int getY()
        {

            return y;
        }

        /**
         * Sets the new insertion position for the player
         * 
         * @param x
         *            X coord of the position
         * @param y
         *            Y coord of the position
         */
        public void setPosition(int x, int y)
        {

            this.x = x;
            this.y = y;
        }

        public int getTransitionTime()
        {

            return transitionTime;
        }
        public TransitionType getTransitionType()
        {

            return transitionType;
        }

        public void setTransitionTime(int transitionTime)
        {
            this.transitionTime = transitionTime;
        }
        public void setTransitionType(TransitionType transitionType)
        {
            this.transitionType = transitionType;
        }
    }
}