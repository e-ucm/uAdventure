using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class TimedAssessmentEffect : AssessmentEffect, ICloneable
    {
        protected int minTime;

        protected int maxTime;

        public TimedAssessmentEffect() : base()
        {
            minTime = 0;
            maxTime = 120;
        }

        public override List<AssessmentProperty> getAssessmentProperties()
        {

            return properties;
        }

        /**
         * @return the minTime
         */
        public int getMinTime()
        {

            return minTime;
        }

        /**
         * @param minTime
         *            the minTime to set
         */
        public void setMinTime(int minTime)
        {

            this.minTime = minTime;
        }

        /**
         * @return the maxTime
         */
        public int getMaxTime()
        {

            return maxTime;
        }

        /**
         * @param maxTime
         *            the maxTime to set
         */
        public void setMaxTime(int maxTime)
        {

            this.maxTime = maxTime;
        }

        public bool isMinTimeSet()
        {

            return this.minTime != int.MinValue;
        }

        public bool isMaxTimeSet()
        {

            return this.maxTime != int.MaxValue;
        }
        public override object Clone()
        {
            TimedAssessmentEffect tae = (TimedAssessmentEffect)base.Clone();
            tae.maxTime = maxTime;
            tae.minTime = minTime;
            return tae;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TimedAssessmentEffect tae = (TimedAssessmentEffect) super.clone( );
            tae.maxTime = maxTime;
            tae.minTime = minTime;
            return tae;
        }*/
    }
}