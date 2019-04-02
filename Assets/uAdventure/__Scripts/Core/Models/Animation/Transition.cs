using System;

namespace uAdventure.Core
{
    /**
     * This class holds the information for an animation transition
     */
    public class Transition : Timed, ICloneable
    {

        /**
         * Time (duration) of the transition
         */
        private long time;

        /**
         * Type of the transition: {@link #TYPE_FADEIN}, {@link #TYPE_NONE},
         * {@link #TYPE_HORIZONTAL} or {@link #TYPE_VERTICAL}
         */
        private TransitionType type;

        /**
         * Creates a new empty transition
         */
        public Transition()
        {
            time = 0;
            type = TransitionType.NoTransition;
        }

        /**
         * Returns the time (duration) of the transition in milliseconds
         * 
         * @return the time (duration) of the transition in milliseconds
         */
        public long getTime()
        {
            return time;
        }

        /**
         * Sets the time (duration) of the transition in milliseconds
         * 
         * @param time
         *            the new time (duration) of the transition in milliseconds
         */
        public void setTime(long time)
        {
            this.time = time;
        }

        /**
         * Returns the type of the transition
         * 
         * @return the type of the transition
         */
        public TransitionType getType()
        {
            return type;
        }

        /**
         * Sets the type of the transition
         * 
         * @param type
         *            The new type of the transition
         */
        public void setType(TransitionType type)
        {
            this.type = type;
        }

        public object Clone()
        {
            Transition t = (Transition)this.MemberwiseClone();
            t.time = time;
            t.type = type;
            return t;
        }
    }
}