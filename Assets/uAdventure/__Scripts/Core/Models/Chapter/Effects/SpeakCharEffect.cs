using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that makes a character to speak a line of text.
     */
    public class SpeakCharEffect : AbstractEffect, HasTargetId
    {
        /**
             * Path for the audio track where the line is recorded. Its use is optional.
             */
        private string audioPath;


        /**
         * Id of the character who will talk
         */
        private string idTarget;

        /**
         * Text for the character to speak
         */
        private string line;

        /**
         * Creates a new SpeakCharEffect.
         * 
         * @param idTarget
         *            the id of the character who will speak
         * @param line
         *            the text to be spoken
         */
        public SpeakCharEffect(string idTarget, string line) : base()
        {
            this.idTarget = idTarget;
            this.line = line;
        }


        public override EffectType getType()
        {

            return EffectType.SPEAK_CHAR;
        }

        /**
         * Returns the idTarget
         * 
         * @return string containing the idTarget
         */
        public string getTargetId()
        {

            return idTarget;
        }

        /**
         * Sets the new idTarget
         * 
         * @param idTarget
         *            New idTarget
         */
        public void setTargetId(string idTarget)
        {

            this.idTarget = idTarget;
        }

        /**
         * Returns the line that the character will speak
         * 
         * @return The line of the character
         */
        public string getLine()
        {

            return line;
        }

        /**
         * Sets the line that the character will speak
         * 
         * @param line
         *            New line
         */
        public void setLine(string line)
        {

            this.line = line;
        }


        public string getAudioPath()
        {

            return audioPath;
        }


        public void setAudioPath(string audioPath)
        {

            this.audioPath = audioPath;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            SpeakCharEffect sce = (SpeakCharEffect) super.clone( );
            sce.idTarget = ( idTarget != null ? new string(idTarget ) : null );
            sce.line = ( line != null ? new string(line ) : null );
            sce.audioPath = ( audioPath != null ? new string(audioPath ) : null );
            return sce;
        }*/
    }
}