using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that makes the player to speak a line of text.
     */
    public class SpeakPlayerEffect : AbstractEffect
    {
        /**
             * Path for the audio track where the line is recorded. Its use is optional.
             */
        private string audioPath;

        /**
         * Text for the player to speak
         */
        private string line;

        /**
         * Creates a new SpeakPlayerEffect.
         * 
         * @param line
         *            the text to be spoken
         */
        public SpeakPlayerEffect(string line) : base()
        {
            this.line = line;
        }

        public override EffectType getType()
        {
            return EffectType.SPEAK_PLAYER;
        }

        /**
         * Returns the line that the player will speak
         * 
         * @return The line of the player
         */
        public string getLine()
        {

            return line;
        }

        /**
         * Sets the line that the player will speak
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

            SpeakPlayerEffect spe = (SpeakPlayerEffect) super.clone( );
            spe.line = ( line != null ? new string(line ) : null );
            spe.audioPath = ( audioPath != null ? new string(audioPath ) : null );
            return spe;
        }*/
    }
}