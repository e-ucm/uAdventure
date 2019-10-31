using System;

namespace uAdventure.Core
{
    [GroupableType]
    public abstract class Cutscene : GeneralScene, HasTargetId, Positioned
    {
        /**
            * The tag for the video
            */
        public const string RESOURCE_TYPE_VIDEO = "video";

        /**
         * The tag for the slides
         */
        public const string RESOURCE_TYPE_SLIDES = "slides";

        /**
         * The tag for the background music
         */
        public const string RESOURCE_TYPE_MUSIC = "bgmusic";

        public const int GOBACK = 0;

        public const int ENDCHAPTER = 1;

        public const int NEWSCENE = 2;

        private int next;

        private int transitionTime;

        private TransitionType transitionType;

        private int destinyX;

        private int destinyY;

        private string idTarget;

        private Effects effects;

        /**
         * Creates a new cutscene
         * 
         * @param type
         *            The type of the scene
         * @param id
         *            The id of the scene
         */
        protected Cutscene(GeneralSceneSceneType type, string id) : base(type, id)
        {
            //## xapi def ##
            this.xapiClass = "accesible";
            this.xapiType = "cutscene";

            effects = new Effects();
            destinyX = int.MinValue;
            destinyY = int.MaxValue;
            transitionType = TransitionType.NoTransition;
            transitionTime = 0;
            next = GOBACK;
            HideInventory = true;
        }

        /**
         * Adds a next scene to the list of next scenes
         * 
         * @param nextScene
         *            the next scene to add
         */
        public void addNextScene(NextScene nextScene)
        {

            next = NEWSCENE;
            idTarget = nextScene.getTargetId();
            transitionTime = nextScene.getTransitionTime();
            transitionType = nextScene.getTransitionType();
            destinyX = nextScene.getPositionX();
            destinyY = nextScene.getPositionY();
            foreach (var effect in nextScene.getEffects())
            {
                effects.Add(effect);
            }
            foreach (var effect in nextScene.getPostEffects())
            {
                effects.Add(effect);
            }
        }

        /**
         * Returns whether the cutscene ends the chapter.
         * 
         * @return True if the cutscene ends the chapter, false otherwise
         */
        public bool isEndScene()
        {
            return next == ENDCHAPTER;
        }

        /**
         * Changes whether the cutscene ends the chapter.
         * 
         * @param endScene
         *            True if the cutscene ends the chapter, false otherwise
         */
        public void setEndScene(bool endScene)
        {
            this.next = endScene ? ENDCHAPTER : GOBACK;
        }

        /**
         * Returns whether this scene has been assigned a position for a player that
         * just came in
         * 
         * @return true if this scene has a position assigned, false otherwise
         */
        public bool hasPlayerPosition()
        {
            return (destinyX != int.MinValue) && (destinyY != int.MinValue);
        }

        /**
         * Returns the horizontal position of the player when he/she gets into this
         * scene
         * 
         * @return the horizontal position of the player when he/she gets into this
         *         scene
         */
        public int getPositionX()
        {
            return destinyX;
        }

        /**
         * Returns the vertical position of the player when he/she gets into this
         * scene
         * 
         * @return the verticalal position of the player when he/she gets into this
         *         scene
         */
        public int getPositionY()
        {
            return destinyY;
        }

        /**
         * Returns the effects for this next scene
         * 
         * @return the effects for this next scene
         */
        public Effects getEffects()
        {
            return effects;
        }

        /**
         * Sets a new next scene id.
         * 
         * @param nextSceneId
         *            New next scene id
         */
        public void setTargetId(string nextSceneId)
        {
            this.idTarget = nextSceneId;
        }

        public string getTargetId()
        {
            return idTarget;
        }

        /**
         * Sets the new destiny position for the next scene.
         * 
         * @param positionX
         *            X coordinate of the destiny position
         * @param positionY
         *            Y coordinate of the destiny position
         */
        public void setDestinyPosition(int positionX, int positionY)
        {
            setPositionX(positionX);
            setPositionY(positionY);
        }

        /**
         * Sets the new destiny position X for the next scene.
         * 
         * @param positionX
         *            X coordinate of the destiny position
         */
        public void setPositionX(int positionX)
        {
            this.destinyX = positionX;
        }

        /**
         * Sets the new destiny position Y for the next scene.
         * 
         * @param positionY
         *            Y coordinate of the destiny position
         */
        public void setPositionY(int positionY)
        {
            this.destinyY = positionY;
        }

        /**
         * Changes the effects for this next scene
         * 
         * @param effects
         *            The new effects
         */
        public void setEffects(Effects effects)
        {
            this.effects = effects;
        }

        public TransitionType getTransitionType()
        {
            return transitionType;
        }

        public int getTransitionTime()
        {
            return transitionTime;
        }

        public void setTransitionType(TransitionType transitionType)
        {
            this.transitionType = transitionType;
        }

        public void setTransitionTime(int transitionTime)
        {
            this.transitionTime = transitionTime;
        }

        public void setNext(int next)
        {
            this.next = next;
        }

        public int getNext()
        {
            return next;
        }

        public override object Clone()
        {
            var c = (Cutscene)base.Clone();
            c.next = next;
            c.destinyX = destinyX;
            c.destinyY = destinyY;
            c.effects = (effects != null ? (Effects)effects.Clone() : null);
            c.idTarget = idTarget;
            return c;
        }
    }
}