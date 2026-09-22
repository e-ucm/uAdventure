using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect to wait some time without do nothing
     */
    public class WaitTimeEffect : AbstractEffect
    {
        /**
             * The time to wait without do nothing
             */
        private int time;

        /**
         * Constructor
         * 
         * @param time
         */
        public WaitTimeEffect(int time) : base()
        {
            this.time = time;
        }

        /**
         * @return the time
         */
        public int getTime()
        {

            return time;
        }

        /**
         * @param time
         *            the time to set
         */
        public void setTime(int time)
        {

            this.time = time;
        }

        /**
         * Return the effect type
         */
        public override EffectType getType()
        {
            return EffectType.WAIT_TIME;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            WaitTimeEffect wte = (WaitTimeEffect) super.clone( );
            wte.time = time;
            return wte;
        }*/
    }
}