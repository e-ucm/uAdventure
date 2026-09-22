using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * A condition based on a reference to a global state
     * 
     * @author Javier
     * 
     */
    [Serializable]
    public class FlagCondition : Condition, ICloneable
    {
        /**
           * Constant for active flag.
           */
        public const int FLAG_ACTIVE = 0;

        /**
         * Constant for inactive flag.
         */
        public const int FLAG_INACTIVE = 1;

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */
        public FlagCondition(string id) : base(Condition.FLAG_CONDITION, id, FlagCondition.FLAG_ACTIVE)
        {
        }

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */
        public FlagCondition(string id, int state) : base(Condition.FLAG_CONDITION, id, state)
        {
            if (state != FLAG_ACTIVE && state != FLAG_INACTIVE)
            {
                state = FLAG_ACTIVE;
            }
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            FlagCondition gsr = (FlagCondition) super.clone( );
            return gsr;
        }*/

        /**
         * Returns true if the state is FLAG_ACTIVE
         * 
         * @return
         */
        public bool isActiveState()
        {

            return state == FLAG_ACTIVE;
        }

        /**
         * Returns true if the state is FLAG_INACTIVE
         * 
         * @return
         */
        public bool isInactiveState()
        {

            return state == FLAG_INACTIVE;
        }

        public override string ToString()
        {
            return state == FLAG_ACTIVE ? getId() : "not " + getId(); 
        }

        public override object Clone()
        {
            FlagCondition gsr = (FlagCondition)base.Clone();
            return gsr;
        }
    }
}