using System;
using UnityEngine;
using System.Collections;
using uAdventure.Runner;

namespace uAdventure.Core
{
    /**
     * This class holds the data of a non playing character (npc) in eAdventure
     */
    public class NPC : Element, ICloneable
    {

        /**
         * The tag for the standup animation
         */
        public const string RESOURCE_TYPE_STAND_UP = "standup";

        /**
         * The tag for the standdown animation
         */
        public const string RESOURCE_TYPE_STAND_DOWN = "standdown";

        /**
         * The tag for the standright animation
         */
        public const string RESOURCE_TYPE_STAND_RIGHT = "standright";

        public const string RESOURCE_TYPE_STAND_LEFT = "standleft";

        /**
         * The tag for the speakup animation
         */
        public const string RESOURCE_TYPE_SPEAK_UP = "speakup";

        /**
         * The tag for the speakdown animation
         */
        public const string RESOURCE_TYPE_SPEAK_DOWN = "speakdown";

        /**
         * The tag for the speakright animation
         */
        public const string RESOURCE_TYPE_SPEAK_RIGHT = "speakright";

        public const string RESOURCE_TYPE_SPEAK_LEFT = "speakleft";

        /**
         * The tag for the useright animation
         */
        public const string RESOURCE_TYPE_USE_RIGHT = "useright";

        public const string RESOURCE_TYPE_USE_LEFT = "useleft";

        /**
         * The tag for the walkup animation
         */
        public const string RESOURCE_TYPE_WALK_UP = "walkup";

        /**
         * The tag for the walkdown animation
         */
        public const string RESOURCE_TYPE_WALK_DOWN = "walkdown";

        /**
         * The tag for the walkright animation
         */
        public const string RESOURCE_TYPE_WALK_RIGHT = "walkright";

        public const string RESOURCE_TYPE_WALK_LEFT = "walkleft";

        /**
         * The front color of the text of the character
         */
        protected Color textFrontColor;

        /**
         * The border color of the text of the character
         */
        protected Color textBorderColor;

        protected Color bubbleBkgColor;

        protected Color bubbleBorderColor;

        protected bool showsSpeechBubbles;


        /**
         * Item's Behaviour (see {@link BehaviourTYpe}
         */
        private Item.BehaviourType behaviour;

        /**
         * The voice which the synthesizer uses to read text of the character
         */
        protected string voice;

        /**
         * Tells if it must be read by synthesizer all conversation lines
         */
        protected bool alwaysSynthesizer;

        /**
         * Creates a new NPC
         * 
         * @param id
         *            the id of the npc
         */
        public NPC(string id) : base(id)
        {
            // Default colors are white for the front color, and black for the border color
            textFrontColor = Color.white;
            textBorderColor = Color.black;
            showsSpeechBubbles = false;
            bubbleBkgColor = Color.white;
            bubbleBorderColor = Color.black;
            behaviour = Item.BehaviourType.NORMAL;
        }

        /**
         * Returns the front color of the character's text
         * 
         * @return string with the color, in format "#RRGGBB"
         */
        public Color getTextFrontColor()
        {
            return textFrontColor;
        }

        /**
         * Returns the boder color of the character's text
         * 
         * @return string with the color, in format "#RRGGBB"
         */
        public Color getTextBorderColor()
        {

            return textBorderColor;
        }

        public Color getBubbleBorderColor()
        {

            return bubbleBorderColor;
        }

        public Color getBubbleBkgColor()
        {

            return bubbleBkgColor;
        }

        public bool getShowsSpeechBubbles()
        {

            return showsSpeechBubbles;
        }

        public void setShowsSpeechBubbles(bool showsSpeechBubbles)
        {

            this.showsSpeechBubbles = showsSpeechBubbles;
        }

        /**
         * Sets the front color of the character's text
         * 
         * @param textFrontColor
         *            string with the color, in format "#RRGGBB"
         */
        public void setTextFrontColor(Color textFrontColor)
        {

            this.textFrontColor = textFrontColor;
        }

        /**
         * Sets the border color of the character's text
         * 
         * @param textBorderColor
         *            string with the color, in format "#RRGGBB"
         */
        public void setTextBorderColor(Color textBorderColor)
        {

            this.textBorderColor = textBorderColor;
        }

        public void setBubbleBorderColor(Color bubbleBorderColor)
        {

            this.bubbleBorderColor = bubbleBorderColor;
        }

        public void setBubbleBkgColor(Color bubbleBkgColor)
        {

            this.bubbleBkgColor = bubbleBkgColor;
        }

        /*
         * (non-Javadoc)
         * 
         * @see java.lang.Object#tostring()
         */
        /*
       @Override
       public string tostring()
       {

           stringBuffer sb = new stringBuffer(40);

           sb.append("\n");
           sb.append(super.tostring());

           return sb.tostring();
       }*/

        /**
         * Gets the voice of the character
         * 
         * @return string with the voice
         */
        public string getVoice()
        {

            return voice;
        }

        /**
         * Sets the voice of the character
         * 
         * @param voice
         *            string with the voice
         */
        public void setVoice(string voice)
        {

            this.voice = voice;
        }

        /**
         * Get if the conversation lines must be read by synthesizer
         * 
         * @return True, if always read by synthesizer, false, otherwise
         */
        public bool isAlwaysSynthesizer()
        {

            return alwaysSynthesizer;
        }

        /**
         * Change the possibility of read all conversation lines with the
         * synthesizer
         * 
         * @param alwaysSynthesizer
         *            the new value
         */
        public void setAlwaysSynthesizer(bool alwaysSynthesizer)
        {

            this.alwaysSynthesizer = alwaysSynthesizer;
        }


        /**
         * Returns this item's behaviour. 
         * @return Behaviour
         * see {@link BehaviourType for more info}
         */
        public Item.BehaviourType getBehaviour()
        {
            return behaviour;
        }
        //For tools
        public int getBehaviourInteger()
        {
            return (int)behaviour;
        }

        public void setBehaviour(Item.BehaviourType behaviour)
        {

            this.behaviour = behaviour;
        }

        // For tools
        public void setBehaviourInteger(int behaviour)
        {
            if (behaviour == (int)Item.BehaviourType.ATREZZO)
            {
                this.behaviour = Item.BehaviourType.ATREZZO;
            }
            else if (behaviour == (int)Item.BehaviourType.NORMAL)
            {
                this.behaviour = Item.BehaviourType.NORMAL;
            }
            else if (behaviour == (int)Item.BehaviourType.FIRST_ACTION)
            {
                this.behaviour = Item.BehaviourType.FIRST_ACTION;
            }
        }

        public override object Clone()
        {
            NPC n = (NPC)base.Clone();
            n.alwaysSynthesizer = alwaysSynthesizer;
            n.textBorderColor = textBorderColor;
            n.textFrontColor = textFrontColor;
            n.voice = voice;
            return n;
        }
    }
}