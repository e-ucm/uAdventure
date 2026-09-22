using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * An effect that show text in an specific zone in scene
     */
    public class ShowTextEffect : AbstractEffect
    {
        /**
             * Path for the audio track where the line is recorded. Its use is optional.
             */
        private string audioPath;

        /**
         * The text which will be showed
         */
        private string text;

        /**
         * The x position in scene
         */
        private int x;

        /**
         * The y position in scene
         */
        private int y;

        /**
         * The text front color in RGB format
         */
        private Color rgbFrontColor;

        /**
         * The text border color in RGB fotrmat
         */
        private Color rgbBorderColor;

        /**
         * Constructor
         * 
         * @param text
         * @param x
         * @param y
         * @param front
         * @param border
         */
        public ShowTextEffect(string text, int x, int y, Color front, Color border) : base()
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.rgbFrontColor = front;
            this.rgbBorderColor = border;
        }

        /**
         * @return the text
         */
        public string getText()
        {

            return text;
        }

        /**
         * @param text
         *            the text to set
         */
        public void setText(string text)
        {

            this.text = text;
        }

        /**
         * @return the x
         */
        public int getX()
        {

            return x;
        }

        /**
         * @return the y
         */
        public int getY()
        {

            return y;
        }

        /**
         * Sets the new text position
         * 
         * @param x
         *            New text position X coordinate
         * @param y
         *            New text position Y coordinate
         */
        public void setTextPosition(int x, int y)
        {

            this.x = x;
            this.y = y;
        }

        /**
         * Return the effect type
         */
        public override EffectType getType()
        {
            return EffectType.SHOW_TEXT;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            ShowTextEffect ste = (ShowTextEffect) super.clone( );
            ste.text = ( text != null ? new string(text ) : null );
            ste.x = x;
            ste.y = y;
            ste.rgbBorderColor = rgbBorderColor;
            ste.rgbFrontColor = rgbFrontColor;
            ste.audioPath = ( audioPath != null ? new string(audioPath ) : null );
            return ste;
        }*/

        /**
         * @return the rgbFrontColor
         */
        public Color getRgbFrontColor()
        {

            return rgbFrontColor;
        }

        /**
         * @param rgbFrontColor
         *            the rgbFrontColor to set
         */
        public void setRgbFrontColor(Color rgbFrontColor)
        {

            this.rgbFrontColor = rgbFrontColor;
        }

        /**
         * @return the rgbBorderColor
         */
        public Color getRgbBorderColor()
        {

            return rgbBorderColor;
        }

        /**
         * @param rgbBorderColor
         *            the rgbBorderColor to set
         */
        public void setRgbBorderColor(Color rgbBorderColor)
        {

            this.rgbBorderColor = rgbBorderColor;
        }


        public string getAudioPath()
        {

            return audioPath;
        }


        public void setAudioPath(string audioPath)
        {

            this.audioPath = audioPath;
        }
    }
}