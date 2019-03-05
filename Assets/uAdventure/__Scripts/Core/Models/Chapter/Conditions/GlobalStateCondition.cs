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
    public class GlobalStateCondition : Condition, ICloneable
    {

        public const int GS_SATISFIED = 8;

        public const int GS_NOT_SATISFIED = 9;

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */
        public GlobalStateCondition(string id) : base(Condition.GLOBAL_STATE_CONDITION, id, GS_SATISFIED)
        {
        }

        /**
         * Constructor
         * 
         * @param flagVar
         * @param state
         */
        public GlobalStateCondition(string id, int state) : base(Condition.GLOBAL_STATE_CONDITION, id, (state != GS_SATISFIED && state != GS_NOT_SATISFIED ? GS_SATISFIED : state))
        {

        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            GlobalStateCondition gsr = (GlobalStateCondition) super.clone( );
            return gsr;
        }*/

        public override object Clone()
        {
            GlobalStateCondition gsr = (GlobalStateCondition)base.Clone();
            return gsr;
        }
    }
}