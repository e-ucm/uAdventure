using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class ExitLook : HasSound, ICloneable
    {

        private string exitText;

        private string cursorPath;

        //eAd1.4
        // Added for accessibility purposes. When the mouse hovers the exit, a sound is played (only once)
        private string soundPath;

        public ExitLook()
        {

            exitText = "";
            cursorPath = null;
        }

        /**
         * @return the exitText
         */
        public string getExitText()
        {

            return exitText;
        }

        /**
         * @param exitText
         *            the exitText to set
         */
        public void setExitText(string exitText)
        {

            this.exitText = exitText;
        }

        /**
         * @return the cursorPath
         */
        public string getCursorPath()
        {

            return cursorPath;
        }

        /**
         * @param cursorPath
         *            the cursorPath to set
         */
        public void setCursorPath(string cursorPath)
        {

            this.cursorPath = cursorPath;
            if (cursorPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }
        }

        /**
         * Added for v1.4 - soundPath for accessibility purposes
         * @return
         */
        public string getSoundPath()
        {

            return soundPath;
        }

        /**
         * Added for v1.4 - soundPath for accessibility purposes
         * @return
         */
        public void setSoundPath(string soundPath)
        {

            this.soundPath = soundPath;
            if (soundPath != null)
            {
                AllElementsWithAssets.addAsset(this);
            }
        }

        public object Clone()
        {
            ExitLook el = (ExitLook)this.MemberwiseClone();
            el.cursorPath = (cursorPath != null ? cursorPath : null);
            el.exitText = (exitText != null ? exitText : null);
            el.soundPath = (soundPath != null ? soundPath : null);
            return el;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ExitLook el = (ExitLook) super.clone( );
            el.cursorPath = ( cursorPath != null ? new String(cursorPath ) : null );
            el.exitText = ( exitText != null ? new String(exitText ) : null );
            el.soundPath = ( soundPath != null ? new String(soundPath ) : null );
            return el;
        }*/

    }
}