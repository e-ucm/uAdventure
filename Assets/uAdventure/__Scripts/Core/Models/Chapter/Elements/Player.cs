using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class holds the data for the player in eAdventure
     */
    public class Player : NPC, ICloneable
    {
        /**
         * Constant identifier of the player (used with conversation lines, also).
         */
        public const string IDENTIFIER = "Player";

        /**
         * Creates a new player
         */
        public Player() : base(IDENTIFIER)
        {
        }

        public override object Clone()
        {

            Player p = (Player)base.Clone();
            return p;
        }
    }
}