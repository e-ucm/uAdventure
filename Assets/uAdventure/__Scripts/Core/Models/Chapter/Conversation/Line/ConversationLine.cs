using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * This class stores a single conversation line, along with the name of the
     * speaker character.
     */

    public class ConversationLine : Named, ICloneable
    {

        public enum Type
        {
            NORMAL,
            WHISPER,
            THOUGHT,
            YELL
        };

        /*
                public override string Tostring()
            {

                switch (this)
                {
                    case WHISPER:
                        return "#:*";
                    case THOUGHT:
                        return "#O";
                    case YELL:
                        return "#!";
                    default:
                        return "";
                }
            public string getName()
            {

                string name;
                switch (this)
                {
                    case Type.WHISPER:
                        name = Language.GetText("ConversationLine.Type.Whisper");
                        break;
                    case Type.THOUGHT:
                        name = Language.GetText("ConversationLine.Type.Thought");
                        break;
                    case Type.YELL:
                        name = Language.GetText("ConversationLine.Type.Yell");
                        break;
                    default:
                        name = Language.GetText("ConversationLine.Type.Normal");
                        break;
                }
                return name + " " + this.ToString();
            }
        public static Type getType(string s)
        {

            if (s.Equals(Type.WHISPER.ToString()))
            {
                return Type.WHISPER;
            }
            else if (s.Equals(Type.THOUGHT.ToString()))
            {
                return Type.THOUGHT;
            }
            else if (s.Equals(Type.YELL.ToString()))
            {
                return Type.YELL;
            }
            return Type.NORMAL;
        }
    }

            */
        /**
         * Constant for the player identifier for the lines.
         */
        public const string PLAYER = Player.IDENTIFIER;

        /**
         * xapi is correct
         */
        public bool xapiCorrect = false;

        /**
         * string that holds the name of the character.
         */
        private string name;

        /**
         * Sentence said by the character.
         */
        private string text;

        /**
         * Path for the audio track where the line is recorded. Its use is optional.
         */
        private string audioPath;

        /**
         * Tell if the line has to be read by synthesizer
         */
        private bool synthesizerVoice;

        /**
         * Conditions associated to this line
         */
        private Conditions conditions;

        /**
         * Keep line showing until user interacts
         */
        private bool keepShowing;

        /**
         * Constructor.
         * 
         * @param name
         *            Name of the character
         * @param text
         *            Sentence
         */

        public ConversationLine(string name, string text)
        {

            this.name = name;
            this.text = text;
            this.synthesizerVoice = false;
            this.keepShowing = false;
            conditions = new Conditions();
        }

        /**
         * Returns true if the xapi question is a correct option
         * 
         * @return true if the xapi question is a correct option
         */

        public bool getXApiCorrect()
        {
            return xapiCorrect;
        }

        /**
         * Returns the name of the character.
         * 
         * @return The name of the character
         */

        public string getName()
        {

            return name;
        }

        /**
         * Returns the text of the converstational line.
         * 
         * @return The text of the conversational line
         */

        public string getText()
        {

            return text;
        }

        /**
         * Returns if the line belongs to the player.
         * 
         * @return True if the line belongs to the player, false otherwise
         */

        public bool isPlayerLine()
        {

            return name.Equals(PLAYER);
        }

        /**
         * Sets if the option is correct
         * 
         * @param if the option is correct
         */

        public void setXApiCorrect(bool xapiCorrect)
        {

            this.xapiCorrect = xapiCorrect;
        }

        /**
         * Sets the new name of the line.
         * 
         * @param name
         *            New name
         */

        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * Sets the new text of the line.
         * 
         * @param text
         *            New text
         */

        public void setText(string text)
        {

            this.text = text;
        }

        /**
         * @return the audioPath
         */

        public string getAudioPath()
        {

            return audioPath;
        }

        /**
         * @param audioPath
         *            the audioPath to set
         */

        public void setAudioPath(string audioPath)
        {

            this.audioPath = audioPath;

            //if audioPath is not null, store the conversation line 
            if (audioPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }
        }

        /**
         * Returns true if the audio path is valid. That is when it is not null and
         * different to ""
         */

        public bool isValidAudio()
        {

            return audioPath != null && !audioPath.Equals("");
        }

        /**
         * Returns if the line has to be read by synthesizer
         * 
         * @return if this line has to be read by synthesizer
         */

        public bool getSynthesizerVoice()
        {

            return synthesizerVoice;
        }

        /**
         * Set if the line to be read by synthesizer
         * 
         * @param synthesizerVoice
         *            true for to be read by synthesizer
         */

        public void setSynthesizerVoice(bool synthesizerVoice)
        {

            this.synthesizerVoice = synthesizerVoice;
        }

        /**
         * @return the conditions
         */

        public Conditions getConditions()
        {

            return conditions;
        }

        /**
         * @param conditions
         *            the conditions to set
         */

        public void setConditions(Conditions conditions)
        {

            this.conditions = conditions;
        }

        public bool isKeepShowing()
        {

            return keepShowing;
        }

        public void setKeepShowing(bool keepShowing)
        {

            this.keepShowing = keepShowing;
        }

        public object Clone()
        {
            ConversationLine cl = (ConversationLine)this.MemberwiseClone();
            cl.audioPath = (audioPath != null ? audioPath : null);
            cl.name = (name != null ? name : null);
            cl.synthesizerVoice = synthesizerVoice;
            cl.text = (text != null ? text : null);
            cl.conditions = (conditions != null ? (Conditions)conditions.Clone() : null);
            cl.keepShowing = keepShowing;
            return cl;
        }
    }
}