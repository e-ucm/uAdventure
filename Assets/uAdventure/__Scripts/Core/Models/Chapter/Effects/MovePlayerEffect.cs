using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that makes the player to move to the given position.
     */
    public class MovePlayerEffect : AbstractEffect
    {

        /**
        * The destination of the player
        */
        private int x;

        private int y;

        /**
         * Creates a new MovePlayerEffect.
         * 
         * @param x
         *            X final position for the player
         * @param y
         *            Y final position for the player
         */
        public MovePlayerEffect(int x, int y) : base()
        {
            this.x = x;
            this.y = y;
        }

        public override EffectType getType()
        {

            return EffectType.MOVE_PLAYER;
        }

        /**
         * Returns the destiny x position.
         * 
         * @return Destiny x coord
         */
        public int getX()
        {

            return x;
        }

        /**
         * Returns the destiny y position.
         * 
         * @return Destiny y coord
         */
        public int getY()
        {

            return y;
        }

        /**
         * Sets the new destiny position
         * 
         * @param x
         *            New destiny X coordinate
         * @param y
         *            New destiny Y coordinate
         */
        public void setDestiny(int x, int y)
        {

            this.x = x;
            this.y = y;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            MovePlayerEffect mpe = (MovePlayerEffect) super.clone( );
            mpe.x = x;
            mpe.y = y;
            return mpe;
        }*/
    }
}