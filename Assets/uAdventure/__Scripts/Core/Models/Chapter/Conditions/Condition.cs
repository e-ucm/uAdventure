using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class manages a condition in eAdventure
     */
    [System.Serializable]
    public abstract class Condition : HasId, ICloneable
    {

        /**
         * Constant for state not set.
         */
        public const int NO_STATE = -1;

        /**
         * Condition based on var
         */
        public const int VAR_CONDITION = 0;

        /**
         * Condition based on flag
         */
        public const int FLAG_CONDITION = 1;

        /**
         * Condition based on condition group
         */
        public const int GLOBAL_STATE_CONDITION = 2;

        /**
         * Name of the flag to be checked
         */
        [SerializeField]
        protected string id;

        /**
         * Stores if the flag must be active or inactive
         */
        [SerializeField]
        protected int state;

        /**
         * Type of the condition ({@link #VAR_CONDITION}, {@link #FLAG_CONDITION}
         * or {@link #GLOBAL_STATE_CONDITION}
         */
        [SerializeField]
        protected int type;

        /**
         * Creates a new condition
         * 
         * @param flagVar
         *            Flag/Var of the condition
         * @param state
         *            Determines the state: {@link #FLAG_ACTIVE}
         *            {@link #FLAG_INACTIVE} {@link #NO_STATE} {@link #VAR_EQUALS} {@link #VAR_NOT_EQUALS}
         *            {@link #VAR_GREATER_EQUALS_THAN} {@link #VAR_GREATER_THAN}
         *            {@link #VAR_LESS_EQUALS_THAN} {@link #VAR_LESS_THAN}
         */
        public Condition(int type, string flagVar, int state)
        {

            this.type = type;
            this.id = flagVar;
            this.state = state;
        }

        /**
         * Returns the flag/Var of the condition
         * 
         * @return The flag/Var of the condition
         */
        public string getId()
        {

            return id;
        }

        /**
         * Returns whether the flag/Var must be activated or deactivated for this
         * condition to be satisfied
         * 
         * @return the state {@link #FLAG_ACTIVE} {@link #FLAG_INACTIVE}
         *         {@link #NO_STATE} {@link #VAR_EQUALS} {@link #VAR_NOT_EQUALS}
         *         {@link #VAR_GREATER_EQUALS_THAN} {@link #VAR_GREATER_THAN}
         *         {@link #VAR_LESS_EQUALS_THAN} {@link #VAR_LESS_THAN}
         */
        public int getState()
        {

            return state;
        }

        /**
         * Sets a new flag for this condition
         * 
         * @param flagVar
         *            New condition flag/Var
         */
        public void setId(string flagVar)
        {

            this.id = flagVar;
        }

        /**
         * Sets a new active or inactive state for the flag/Var.
         * 
         * @param state
         *            New state {@link #FLAG_ACTIVE} {@link #FLAG_INACTIVE}
         *            {@link #NO_STATE} {@link #VAR_EQUALS} {@link #VAR_NOT_EQUALS}
         *            {@link #VAR_GREATER_EQUALS_THAN} {@link #VAR_GREATER_THAN}
         *            {@link #VAR_LESS_EQUALS_THAN} {@link #VAR_LESS_THAN}
         */
        public void setState(int state)
        {

            this.state = state;
        }

        /**
         * @return the type
         */
        public int getType()
        {

            return type;
        }

        public virtual object Clone()
        {
            Condition c = (Condition)this.MemberwiseClone();
            c.id = id;
            c.state = state;
            c.type = type;
            return c;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       Condition c = (Condition) super.clone( );
       c.id = ( id != null ? new string(id ) : null );
       c.state = state;
       c.type = type;
       return c;
    }*/
    }
}