using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that plays a sound
     */
    public class PlaySoundEffect : AbstractEffect
    {

        /**
         * Whether the sound must be played in background
         */
        private bool background;

        /**
         * The path to the sound file
         */
        private string path;

        /**
         * Creates a new PlaySoundEffect
         * 
         * @param background
         *            whether to play the sound in background
         * @param path
         *            path to the sound file
         */
        public PlaySoundEffect(bool background, string path) : base()
        {
            this.background = background;
            this.path = path;
        }

        public override EffectType getType()
        {
            return EffectType.PLAY_SOUND;
        }

        /**
         * Returns whether the sound must be played in background
         * 
         * @return True if the sound must be played in background, false otherwise
         */
        public bool isBackground()
        {

            return background;
        }

        /**
         * Sets the value which tells if the sound must be played in background
         * 
         * @param background
         *            New value for background
         */
        public void setBackground(bool background)
        {

            this.background = background;
        }

        /**
         * Returns the path of the sound
         * 
         * @return Path of the sound
         */
        public string getPath()
        {

            return path;
        }

        /**
         * Sets the new path for the sound to be played
         * 
         * @param path
         *            New path of the sound
         */
        public void setPath(string path)
        {

            this.path = path;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            PlaySoundEffect pse = (PlaySoundEffect) super.clone( );
            pse.background = background;
            pse.path = ( path != null ? new string(path ) : null );
            return pse;
        }*/
    }
}