using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Main class for the Page (HTML or RTF document) of a book
     * 
     * @author Javier Torrente
     * 
     */

    public class BookPage : ICloneable
    {
        /**
         * The page takes the document from a url, which can be on the internet
         */
        public const int TYPE_URL = 0;

        /**
         * The page takes the document as a resource, from the zip file
         */
        public const int TYPE_RESOURCE = 1;

        /**
         * The page takes the document as an image from the zip file
         */
        public const int TYPE_IMAGE = 2;

        /**
         * string to add at the beggining of all images names to avoid problems of
         * overwriting
         */
        private const string IMAGE_FILE_STARTER = "eAdventure_styled_text_image";

        /**
         * The url/resource path
         */
        private string uri;

        /**
         * The type of the page: {@link #TYPE_URL} or {@link #TYPE_RESOURCE}
         */
        private int type;

        /**
         * Space to be left between the border of the background image and the book
         * page
         */
        private int margin;

        private int marginEnd;

        private int marginTop;

        private int marginBottom;

        /**
         * Determines whether the page must be in a scroll pane (Scroll bars shown)
         */
        private bool scrollable;

        /**
         * Determines whether the page have changed and it hasn't been exported to
         * its image
         */
        private bool changed = false;

        /**
         * 
         * @param changed
         *            set the changed
         */

        public void setChanged(bool changed)
        {

            this.changed = changed;
        }

        /**
         * @return if the page has changed since last time was saved
         */

        public bool isChanged()
        {

            return changed;
        }

        /**
         * 
         * @param withPath Determines if name must have relative path 
         * @return the name for the image representing this bookPage
         */

        public string getImageName()
        {

            try
            {
                if (uri != null)
                {
                    string fileName = "";
                    /*if ( withPath ){
                        fileName += AssetsController.getCategoryFolder( AssetsController.CATEGORY_IMAGE ) + "/";
                    }*/
                    fileName += IMAGE_FILE_STARTER + uri.Substring(uri.LastIndexOf('/') + 1, uri.LastIndexOf('.'));

                    /*if ( withPath ){
                        fileName += ".png";
                    }*/
                    return fileName;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            return null;
        }

        /**
         * @return the uri
         */

        public string getUri()
        {

            return uri;
        }

        /**
         * @param uri
         *            the uri to set
         */

        public void setUri(string uri)
        {

            this.uri = uri;
            setChanged(true);
        }

        /**
         * @return the type
         */

        public int getType()
        {

            return type;
        }

        /**
         * @param type
         *            the type to set
         */

        public void setType(int type)
        {

            this.type = type;
            setChanged(true);
        }

        public BookPage(string uri, int type, int margin, bool scrollable)
        {

            this.uri = uri;
            this.type = type;
            this.margin = margin;
            this.scrollable = scrollable;
        }

        public BookPage(string uri, int type, int margin, int marginEnd, int marginTop, int marginBottom, bool scrollable)
        {

            this.uri = uri;
            this.type = type;
            this.margin = margin;
            this.marginEnd = marginEnd;
            this.marginTop = marginTop;
            this.marginBottom = marginBottom;
            this.scrollable = scrollable;
        }

        /**
         * @param uri
         * @param type
         */

        public BookPage(string uri, int type, int margin) : this(uri, type, margin, false)
        {
        }

        public BookPage(string uri, int type) : this(uri, type, 0)
        {
        }

        /**
         * @return the margin
         */

        public int getMargin()
        {

            return margin;
        }

        /**
         * @param margin
         *            the margin to set
         */

        public void setMargin(int margin)
        {

            this.margin = margin;
            setChanged(true);
        }

        /**
         * @return the margin
         */

        public int getMarginStart()
        {

            return margin;
        }

        /**
         * @param margin
         *            the margin to set
         */

        public void setMarginStart(int margin)
        {

            this.margin = margin;
            setChanged(true);
        }

        /**
         * @return the marginEnd
         */

        public int getMarginEnd()
        {

            return marginEnd;
        }

        /**
         * @param marginEnd
         *            the marginEnd to set
         */

        public void setMarginEnd(int marginEnd)
        {

            this.marginEnd = marginEnd;
            setChanged(true);
        }

        /**
         * @return the marginTop
         */

        public int getMarginTop()
        {

            return marginTop;
        }

        /**
         * @param marginTop
         *            the marginTop to set
         */

        public void setMarginTop(int marginTop)
        {

            this.marginTop = marginTop;
            setChanged(true);
        }

        /**
         * @return the marginBottom
         */

        public int getMarginBottom()
        {

            return marginBottom;
        }

        /**
         * @param marginBottom
         *            the marginBottom to set
         */

        public void setMarginBottom(int marginBottom)
        {

            this.marginBottom = marginBottom;
            setChanged(true);
        }

        public bool getScrollable()
        {

            return scrollable;
        }

        public void setScrollable(bool scrollable)
        {

            this.scrollable = scrollable;
        }

        public object Clone()
        {
            BookPage bp = (BookPage)this.MemberwiseClone();
            bp.margin = margin;
            bp.marginBottom = marginBottom;
            bp.marginEnd = marginEnd;
            bp.marginTop = marginTop;
            bp.scrollable = scrollable;
            bp.type = type;
            bp.uri = (uri != null ? uri : null);
            return bp;
        }

        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       BookPage bp = (BookPage) super.clone( );
       bp.margin = margin;
       bp.marginBottom = marginBottom;
       bp.marginEnd = marginEnd;
       bp.marginTop = marginTop;
       bp.scrollable = scrollable;
       bp.type = type;
       bp.uri = ( uri != null ? new string(uri ) : null );
       return bp;
    }*/
    }
}