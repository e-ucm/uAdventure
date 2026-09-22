using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that makes a character to walk to a given position.
     */
    public class MoveNPCEffect : AbstractEffect, HasTargetId
    {

        /**
         * Id of the npc who will walk
         */
        private string idTarget;

        /**
         * The destination of the npc
         */
        private int x;

        private int y;

        /**
         * Creates a new FunctionalMoveNPCEffect.
         * 
         * @param idTarget
         *            the id of the character who will walk
         * @param x
         *            X final position for the NPC
         * @param y
         *            Y final position for the NPC
         */
        public MoveNPCEffect(string idTarget, int x, int y) : base()
        {
            this.idTarget = idTarget;
            this.x = x;
            this.y = y;
        }

        public override EffectType getType()
        {

            return EffectType.MOVE_NPC;
        }

        /**
         * Returns the id target.
         * 
         * @return Id target
         */
        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Sets the new id target.
         * 
         * @param idTarget
         *            New id target
         */
        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
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

            MoveNPCEffect npe = (MoveNPCEffect) super.clone( );
            npe.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            npe.x = x;
            npe.y = y;
            return npe;
        }*/
    }
}