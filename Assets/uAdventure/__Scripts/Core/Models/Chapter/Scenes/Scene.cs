using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds the data for a scene in eAdventure
     */
    [ChapterTarget]
    public class Scene : GeneralScene, Positioned, ICloneable
    {
        public override string ToString()
        {
            return getId();
        }

        /**
             * The value of player layer when hasn't layer. This may
             */
        public const int PLAYER_WITHOUT_LAYER = -1;

        /**
         * The value of player layer in 1ş person adventures
         */
        public const int PLAYER_NO_ALLOWED = -2;

        /**
         * The tag for the background image
         */
        public const string RESOURCE_TYPE_BACKGROUND = "background";

        /**
         * The tag for the foreground image
         */
        public const string RESOURCE_TYPE_FOREGROUND = "foreground";

        /**
         * The tag for the hard map image
         */
        public const string RESOURCE_TYPE_HARDMAP = "hardmap";

        /**
         * The tag for the background music
         */
        public const string RESOURCE_TYPE_MUSIC = "bgmusic";

        public const int DEFAULT_PLAYER_X = AssetsImageDimensions.BACKGROUND_MAX_WIDTH / 2;
        public const int DEFAULT_PLAYER_Y = 3 * AssetsImageDimensions.BACKGROUND_MAX_HEIGHT / 4;

        /**
         * Default X position for the player
         */
        private int defaultX;

        /**
         * Default Y position for the player
         */
        private int defaultY;

        /**
         * The position in which will be drown the player
         */
        private int playerLayer;

        /**
         * Set if it is allow to use the player layer or not. Its default value it
         * is true.
         */
        private bool allowPlayerLayer;

        /**
         * List of exits
         */
        private List<Exit> exits;

        /**
         * List of item references
         */
        private List<ElementReference> itemReferences;

        /**
         * List of atrezzo references
         */
        private List<ElementReference> atrezzoReferences;

        /**
         * List of character references
         */
        private List<ElementReference> characterReferences;

        /**
         * List of active areas
         */
        private List<ActiveArea> activeAreas;

        /**
         * List of barriers
         */
        private List<Barrier> barriers;

        private Trajectory trajectory;

        private float playerScale;

        /**
         * Creates a new Scene
         * 
         * @param id
         *            the scene's id
         */
        public Scene(string id) : base(GeneralSceneSceneType.SCENE, id)
        {
            defaultX = DEFAULT_PLAYER_X;
            defaultY = DEFAULT_PLAYER_Y;
            exits = new List<Exit>();
            itemReferences = new List<ElementReference>();
            atrezzoReferences = new List<ElementReference>();
            characterReferences = new List<ElementReference>();
            activeAreas = new List<ActiveArea>();
            barriers = new List<Barrier>();
            playerLayer = PLAYER_WITHOUT_LAYER;
            allowPlayerLayer = true;
            playerScale = 1.0f;
        }

        /**
         * @return the trajectory
         */
        public Trajectory getTrajectory()
        {

            return trajectory;
        }

        /**
         * @param trajectory
         *            the trajectory to set
         */
        public void setTrajectory(Trajectory trajectory)
        {

            this.trajectory = trajectory;
        }

        /**
         * Returns whether this scene has a default position for the player
         * 
         * @return true if this scene has a default position for the player, false
         *         otherwise
         */
        public bool hasDefaultPosition()
        {

            return (defaultX != int.MinValue) && (defaultY != int.MinValue);
        }

        /**
         * Returns the horizontal coordinate of the default position for the player
         * 
         * @return the horizontal coordinate of the default position for the player
         */
        public int getPositionX()
        {

            return defaultX;
        }

        /**
         * Returns the vertical coordinate of the default position for the player
         * 
         * @return the vertical coordinate of the default position for the player
         */
        public int getPositionY()
        {

            return defaultY;
        }

        /**
         * Returns the list of exits
         * 
         * @return the list of exits
         */
        public List<Exit> getExits()
        {

            return exits;
        }

        /**
         * Returns the list of item references
         * 
         * @return the list of item references
         */
        public List<ElementReference> getItemReferences()
        {

            return itemReferences;
        }

        /**
         * Returns the list of atrezzo item references
         * 
         * @return the list of atrezzo item references
         */
        public List<ElementReference> getAtrezzoReferences()
        {

            return atrezzoReferences;
        }

        /**
         * Returns the list of character references
         * 
         * @return the list of character references
         */
        public List<ElementReference> getCharacterReferences()
        {

            return characterReferences;
        }

        /**
         * Changes the player's default position
         * 
         * @param defaultX
         *            the horizontal coordinate
         * @param defaultY
         *            the vertical coordinate
         */
        public void setDefaultPosition(int defaultX, int defaultY)
        {

            this.defaultX = defaultX;
            this.defaultY = defaultY;
        }

        /**
         * Adds an exit to the list of exits
         * 
         * @param exit
         *            the exit to add
         */
        public void addExit(Exit exit)
        {

            exits.Add(exit);
        }

        /**
         * Adds an item reference to the list of item references
         * 
         * @param itemReference
         *            the item reference to add
         */
        public void addItemReference(ElementReference itemReference)
        {

            itemReferences.Add(itemReference);
        }

        /**
         * Adds an atrezzo item reference to the list of atrezzo item references
         * 
         * @param atrezzoReference
         *            the atrezzo item reference to add
         */
        public void addAtrezzoReference(ElementReference itemReference)
        {

            atrezzoReferences.Add(itemReference);
        }

        /**
         * Adds a character reference to the list of character references
         * 
         * @param characterReference
         *            the character reference to add
         */
        public void addCharacterReference(ElementReference characterReference)
        {

            characterReferences.Add(characterReference);
        }

        /**
         * Adds an active area
         * 
         * @param activeArea
         *            the active area to add
         */
        public void addActiveArea(ActiveArea activeArea)
        {

            activeAreas.Add(activeArea);
        }

        /**
         * @return the activeAreas
         */
        public List<ActiveArea> getActiveAreas()
        {

            return activeAreas;
        }

        /**
         * Adds a new barrier
         * 
         * @param barrier
         *            the barrier to add
         */
        public void addBarrier(Barrier barrier)
        {

            barriers.Add(barrier);
        }

        /**
         * @return the barriers
         */
        public List<Barrier> getBarriers()
        {

            return barriers;
        }

        public bool isForcedPlayerLayer()
        {
            return playerLayer > PLAYER_WITHOUT_LAYER;
        }

        public bool isAllowPlayerLayer()
        {

            return playerLayer != PLAYER_NO_ALLOWED;
        }

        /**
         * Change if player layer can be used. If it can not, change the player
         * layer value to PLAYER_NO_ALLOWED
         * 
         * @param allowPlayerLayer
         * 
         */
        public void setAllowPlayerLayer( bool allowPlayerLayer ) {

            this.allowPlayerLayer = allowPlayerLayer;
            if (!allowPlayerLayer)
                playerLayer = PLAYER_NO_ALLOWED;
        }

        /**
         * Returns the player layer
         * 
         * @return current player layer
         */
        public int getPlayerLayer()
        {

            return playerLayer;
        }

        /**
         * Change the current player layer if it is allow to do that.
         * 
         * @param playerLayer
         *            the current player layer
         */
        public void setPlayerLayer(int playerLayer)
        {

            this.playerLayer = playerLayer;
            if( playerLayer == PLAYER_NO_ALLOWED )
                allowPlayerLayer = false;

        }

        public void setPlayerScale(float scale)
        {

            this.playerScale = scale;
        }

        public float getPlayerScale()
        {

            return playerScale;
        }

        public void setPositionX(int newX)
        {

            this.defaultX = newX;
        }

        public void setPositionY(int newY)
        {

            this.defaultY = newY;
        }

        public override object Clone()
        {
            Scene s = (Scene)base.Clone();
            if (activeAreas != null)
            {
                s.activeAreas = new List<ActiveArea>();
                foreach (ActiveArea aa in activeAreas)
                    s.activeAreas.Add((ActiveArea)aa.Clone());
            }
            s.allowPlayerLayer = allowPlayerLayer;
            if (atrezzoReferences != null)
            {
                s.atrezzoReferences = new List<ElementReference>();
                foreach (ElementReference er in atrezzoReferences)
                    s.atrezzoReferences.Add((ElementReference)er.Clone());
            }
            if (barriers != null)
            {
                s.barriers = new List<Barrier>();
                foreach (Barrier b in barriers)
                    s.barriers.Add((Barrier)b.Clone());
            }
            if (characterReferences != null)
            {
                s.characterReferences = new List<ElementReference>();
                foreach (ElementReference er in characterReferences)
                    s.characterReferences.Add((ElementReference)er.Clone());
            }
            s.defaultX = defaultX;
            s.defaultY = defaultY;
            if (exits != null)
            {
                s.exits = new List<Exit>();
                foreach (Exit e in exits)
                    s.exits.Add((Exit)e.Clone());
            }
            if (itemReferences != null)
            {
                s.itemReferences = new List<ElementReference>();
                foreach (ElementReference er in itemReferences)
                    s.itemReferences.Add((ElementReference)er.Clone());
            }
            s.playerLayer = playerLayer;
            s.playerScale = playerScale;
            s.trajectory = (trajectory != null ? (Trajectory)trajectory.Clone() : null);
            return s;
        }
    }
}