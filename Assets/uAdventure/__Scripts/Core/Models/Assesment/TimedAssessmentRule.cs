using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * Timed Rule for the assesment engine
     */
    public class TimedAssessmentRule : AssessmentRule, ICloneable
    {

        /**
             * End conditions
             */
        protected Conditions endConditions;

        /**
         * List of timed effects
         */
        protected List<TimedAssessmentEffect> effects;

        /**
         * For loading purpose, only
         */
        protected int effectIndex;

        /**
         * True if the assessmentRule is Done
         */
        protected bool isDone;

        /**
         * Time the rule took
         */
        protected long elapsedTime;

        protected bool usesEndConditions;

        private long startTime;

        /**
         * Default constructor
         * 
         * @param id
         *            Id of the rule
         * @param importance
         *            Importance of the rule
         */

        public TimedAssessmentRule(string id, int importance, bool repeatRule) : base(id, importance, repeatRule)
        {
            effects = new List<TimedAssessmentEffect>();
            this.endConditions = new Conditions();
            usesEndConditions = true;
            effectIndex = -1;
            elapsedTime = 0;
            isDone = false;
        }

        /**
         * Sets the conditions of the rule
         * 
         * @param conditions
         *            Conditions of the rule
         */

        public void setInitConditions(Conditions initConditions)
        {

            this.conditions = initConditions;
        }

        /**
         * Sets the text of the rule
         * 
         * @param text
         *            Text of the rule
         */

        public void setText(string text, int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                effects[effectBlock].setText(text);
        }

        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */

        public void addProperty(AssessmentProperty property, int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                effects[effectBlock].getAssessmentProperties().Add(property);
        }

        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */

        public AssessmentProperty getProperty(int property, int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                return effects[effectBlock].getAssessmentProperties()[property];
            return null;
        }

        public List<AssessmentProperty> getAssessmentProperties(int effect)
        {

            return effects[effect].getAssessmentProperties();
        }

        public void setMinTime(int time, int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                effects[effectBlock].setMinTime(time);
        }

        public void setMaxTime(int time, int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                effects[effectBlock].setMaxTime(time);
        }

        public int getMinTime(int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                return effects[effectBlock].getMinTime();
            return int.MinValue;
        }

        public int getMaxTime(int effectBlock)
        {

            if (effectBlock >= 0 && effectBlock < effects.Count)
                return effects[effectBlock].getMaxTime();
            return int.MaxValue;
        }

        public int getEffectsCount()
        {

            return this.effects.Count;
        }

        /**
         * @return the conditions
         */

        public Conditions getInitConditions()
        {

            return conditions;
        }

        /**
         * @return the endConditions
         */

        public Conditions getEndConditions()
        {

            return endConditions;
        }

        /**
         * @param endConditions
         *            the endConditions to set
         */

        public void setEndConditions(Conditions endConditions)
        {

            this.endConditions = endConditions;
        }

        /**
         * @return the effects
         */

        public List<TimedAssessmentEffect> getEffects()
        {

            return effects;
        }

        /**
         * @param effects
         *            the effects to set
         */

        public void setEffects(List<TimedAssessmentEffect> effects)
        {

            this.effects = effects;
        }

        public void addEffect()
        {

            this.effectIndex++;
            effects.Add(new TimedAssessmentEffect());
        }

        public TimedAssessmentEffect getLastEffect()
        {

            return effects[effects.Count - 1];
        }

        public void addEffect(int min, int max)
        {

            this.effectIndex++;
            TimedAssessmentEffect newEffect = new TimedAssessmentEffect();
            newEffect.setMinTime(min);
            newEffect.setMaxTime(max);
            effects.Add(newEffect);
        }

        /**
         * Sets the text of the rule
         * 
         * @param text
         *            Text of the rule
         */

        public override void setText(string text)
        {

            setText(text, effectIndex);
        }

        /**
         * Adds a new assessment property
         * 
         * @param property
         *            Assessment property to be added
         */

        public override void addProperty(AssessmentProperty property)
        {

            addProperty(property, effectIndex);
        }

        /**
         * Returns true if the rule is active
         * 
         * @return True if the rule is active, false otherwise
         */

        public bool isActive()
        {

            return isDone;
        }

        public void ruleStarted(long currentTime)
        {

            isDone = false;
            this.startTime = currentTime;
        }

        public void ruleDone(long currentTime)
        {

            this.isDone = true;
            this.elapsedTime = currentTime - this.startTime;

            // Evaluate the rule
            foreach (TimedAssessmentEffect effect in this.effects)
            {
                if (elapsedTime >= effect.getMinTime() && elapsedTime <= effect.getMaxTime())
                {
                    this.effect.SetAssesmentPropertiesList(effect.getAssessmentProperties());
                    this.effect.setText("[ELAPSED TIME = " + getTimeHhMmSs() + " ] " + effect.getText());
                    //System.out.println( "[RULE EVALUATION] "+text );
                    break;
                }
            }

        }

        /**
         * Returns the time of the timer represented as hours:minutes:seconds. The
         * string returned will look like: HHh:MMm:SSs
         * 
         * @return The time as HHh:MMm:SSs
         */

        private string getTimeHhMmSs()
        {

            string time = "";

            // Less than 60 seconds
            if (elapsedTime < 60 && elapsedTime >= 0)
            {
                time = elapsedTime + "s";
            }

            // Between 1 minute and 60 minutes
            else if (elapsedTime < 3600 && elapsedTime >= 60)
            {
                long minutes = elapsedTime / 60;
                long lastSeconds = elapsedTime % 60;
                time = minutes + "m:" + lastSeconds + "s";
            }

            // One hour or more
            else if (elapsedTime >= 3600)
            {
                long hours = elapsedTime / 3600;
                long minutes = (elapsedTime % 3600) / 60;
                long lastSeconds = (elapsedTime % 3600) % 60;
                time = hours + "h:" + minutes + "m:" + lastSeconds + "s";
            }

            return time;
        }

        public void setStartTime(long startTime)
        {

            this.startTime = startTime;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TimedAssessmentRule tar = (TimedAssessmentRule) super.clone( );
            tar.effectIndex = effectIndex;
            if( effects != null ) {
                tar.effects = new List<TimedAssessmentEffect>();
                for (TimedAssessmentEffect tae : effects)
                    tar.effects.add((TimedAssessmentEffect)tae.clone());
            }
            tar.startTime = startTime;
            tar.elapsedTime = elapsedTime;
            tar.endConditions = ( endConditions != null ? (Conditions) endConditions.clone( ) : null );
            tar.isDone = isDone;
            tar.usesEndConditions = usesEndConditions;
            return tar;
        }*/

        public override object Clone()
        {
            TimedAssessmentRule tar = (TimedAssessmentRule)base.Clone();
            tar.effectIndex = effectIndex;
            if (effects != null)
            {
                tar.effects = new List<TimedAssessmentEffect>();
                foreach (TimedAssessmentEffect tae in effects)
                    tar.effects.Add((TimedAssessmentEffect)tae.Clone());
            }
            tar.startTime = startTime;
            tar.elapsedTime = elapsedTime;
            tar.endConditions = (endConditions != null ? (Conditions)endConditions.Clone() : null);
            tar.isDone = isDone;
            tar.usesEndConditions = usesEndConditions;
            return tar;
        }

        public void setUsesEndConditions(bool b)
        {

            this.usesEndConditions = b;
        }

        public bool isUsesEndConditions()
        {

            return usesEndConditions;
        }
    }
}