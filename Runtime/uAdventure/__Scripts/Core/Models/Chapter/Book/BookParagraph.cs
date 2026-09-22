using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * Main class of the paragraphs of one book
     */

    public class BookParagraph : ICloneable
    {
        /**
         * Type for bullet paragraph.
         */
        public const int BULLET = 0;

        /**
         * Type for image paragraph.
         */
        public const int IMAGE = 1;

        /**
         * Type for text paragraph.
         */
        public const int TEXT = 2;

        /**
         * Type for title paragraph.
         */
        public const int TITLE = 3;

        /**
         * Type of the paragraph.
         */
        private int type;

        /**
         * Text content of the paragraph.
         */
        private string content;

        /**
         * Constructor.
         * 
         * @param type
         *            The type of the paragraph
         */

        public BookParagraph(int type)
        {

            this.type = type;
            this.content = "";
        }

        /**
         * Constructor.
         * 
         * @param type
         *            The type of the paragraph
         * @param content
         *            Content of the paragraph
         */

        public BookParagraph(int type, string content)
        {

            this.type = type;
            this.content = content;
        }

        /**
         * Set the new content of the paragraph.
         * 
         * @param content
         *            New content
         */

        public void setContent(string content)
        {

            this.content = content;
        }

        /**
         * Returns the type of the paragraph
         * 
         * @return Paragraph's type
         */

        public int getType()
        {

            return type;
        }

        /**
         * Returns the content of the paragraph.
         * 
         * @return Content of the paragraph. It will be text if it is a text or
         *         bullet paragraph, or a path if it is an image paragraph
         */

        public string getContent()
        {

            return content;
        }

        public object Clone()
        {
            BookParagraph bp = (BookParagraph)this.MemberwiseClone();
            bp.content = (content != null ? content : null);
            bp.type = type;
            return bp;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       BookParagraph bp = (BookParagraph) super.clone( );
       bp.content = ( content != null ? new string(content ) : null );
       bp.type = type;
       return bp;
    }*/
    }
}