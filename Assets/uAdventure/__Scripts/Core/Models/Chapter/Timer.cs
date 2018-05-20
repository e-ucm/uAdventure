using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class Timer : Documented, ICloneable
    {

        public const long DEFAULT_SECONDS = 60L;

        private long seconds;

        private Conditions initCond;

        private Conditions endCond;

        private Effects effect;

        private Effects postEffect;

        private string documentation;

        private bool usesEndCondition;

        private bool runsInLoop;

        private bool multipleStarts;

        private bool showTime;

        private string displayName;

        private bool countDown;

        private bool showWhenStopped;

        private Color fontColor = Color.black;

        private Color borderColor = Color.white;

        public Timer(long time, Conditions init, Conditions end, Effects effect, Effects postEffect)
        {

            this.seconds = time;
            this.initCond = init;
            this.endCond = end;
            this.effect = effect;
            this.postEffect = postEffect;
            usesEndCondition = true;
            runsInLoop = true;
            multipleStarts = true;

            showTime = false;
            displayName = "timer";
            countDown = true;
            showWhenStopped = false;
        }

        public Timer(long time) : this(time, new Conditions(), new Conditions(), new Effects(), new Effects())
        {
        }

        public Timer() : this(DEFAULT_SECONDS)
        {
        }

        /**
         * @return the seconds
         */
        public long getTime()
        {

            return seconds;
        }

        /**
         * @param seconds
         *            the seconds to set
         */
        public void setTime(long seconds)
        {

            this.seconds = seconds;
        }

        /**
         * @return the initCond
         */
        public Conditions getInitCond()
        {

            return initCond;
        }

        /**
         * @param initCond
         *            the initCond to set
         */
        public void setInitCond(Conditions initCond)
        {

            this.initCond = initCond;
        }

        /**
         * @return the endCond
         */
        public Conditions getEndCond()
        {

            return endCond;
        }

        /**
         * @param endCond
         *            the endCond to set
         */
        public void setEndCond(Conditions endCond)
        {

            this.endCond = endCond;
        }

        /**
         * @return the effect
         */
        public Effects getEffects()
        {

            return effect;
        }

        /**
         * @param effect
         *            the effect to set
         */
        public void setEffects(Effects effect)
        {

            this.effect = effect;
        }

        /**
         * @return the postEffect
         */
        public Effects getPostEffects()
        {

            return postEffect;
        }

        /**
         * @param postEffect
         *            the postEffect to set
         */
        public void setPostEffects(Effects postEffect)
        {

            this.postEffect = postEffect;
        }

        /**
         * @return the documentation
         */
        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * @param documentation
         *            the documentation to set
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * @return the usesEndCondition
         */
        public bool isUsesEndCondition()
        {

            return usesEndCondition;
        }

        /**
         * @param usesEndCondition
         *            the usesEndCondition to set
         */
        public void setUsesEndCondition(bool usesEndCondition)
        {

            this.usesEndCondition = usesEndCondition;
        }

        /**
         * @return the runsInLoop
         */
        public bool isRunsInLoop()
        {

            return runsInLoop;
        }

        /**
         * @param runsInLoop
         *            the runsInLoop to set
         */
        public void setRunsInLoop(bool runsInLoop)
        {

            this.runsInLoop = runsInLoop;
        }

        /**
         * @return the multipleStarts
         */
        public bool isMultipleStarts()
        {

            return multipleStarts;
        }

        /**
         * @param multipleStarts
         *            the multipleStarts to set
         */
        public void setMultipleStarts(bool multipleStarts)
        {

            this.multipleStarts = multipleStarts;
        }

        /**
         * @return the countDown
         */
        public bool isCountDown()
        {

            return countDown;
        }

        /**
         * @param countDown
         *            the countDown to set
         */
        public void setCountDown(bool countDown)
        {

            this.countDown = countDown;
        }

        /**
         * @return the showWhenStopped
         */
        public bool isShowWhenStopped()
        {

            return showWhenStopped;
        }

        /**
         * @param showWhenStopped
         *            the showWhenStopped to set
         */
        public void setShowWhenStopped(bool showWhenStopped)
        {

            this.showWhenStopped = showWhenStopped;
        }

        /**
         * @return the fontColor
         */
        public Color getFontColor()
        {

            return fontColor;
        }

        /**
         * @param fontColor
         *            the fontColor to set
         */
        public void setFontColor(Color fontColor)
        {

            this.fontColor = fontColor;
        }

        /**
         * @return the borderColor
         */
        public Color getBorderColor()
        {

            return borderColor;
        }

        /**
         * @param borderColor
         *            the borderColor to set
         */
        public void setBorderColor(Color borderColor)
        {

            this.borderColor = borderColor;
        }

        public string getDisplayName()
        {

            return displayName;
        }

        public void setDisplayName(string displayName)
        {

            this.displayName = displayName;
        }

        public bool isShowTime()
        {

            return showTime;
        }

        public void setShowTime(bool showTime)
        {

            this.showTime = showTime;
        }

        public object Clone()
        {
            Timer t = (Timer)this.MemberwiseClone();
            t.documentation = (documentation != null ? documentation : null);
            t.effect = (effect != null ? (Effects)effect.Clone() : null);
            t.endCond = (endCond != null ? (Conditions)endCond.Clone() : null);
            t.initCond = (initCond != null ? (Conditions)initCond.Clone() : null);
            t.postEffect = (postEffect != null ? (Effects)postEffect.Clone() : null);
            t.seconds = seconds;
            t.runsInLoop = runsInLoop;
            t.multipleStarts = multipleStarts;
            t.usesEndCondition = usesEndCondition;
            return t;
        }
        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       Timer t = (Timer) super.clone( );
       t.documentation = ( documentation != null ? new string(documentation ) : null );
       t.effect = ( effect != null ? (Effects) effect.clone( ) : null );
       t.endCond = ( endCond != null ? (Conditions) endCond.clone( ) : null );
       t.initCond = ( initCond != null ? (Conditions) initCond.clone( ) : null );
       t.postEffect = ( postEffect != null ? (Effects) postEffect.clone( ) : null );
       t.seconds = seconds;
       t.runsInLoop = runsInLoop;
       t.multipleStarts = multipleStarts;
       t.usesEndCondition = usesEndCondition;
       return t;
    }*/

    }
}