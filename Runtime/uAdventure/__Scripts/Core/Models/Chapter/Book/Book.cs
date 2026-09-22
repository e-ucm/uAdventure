using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class holds a "bookscene" data
     */
    public class Book : Documented, HasId, ICloneable
    {

        /**
         * The xml tag for the background image of the bookscene
         */
        public const string RESOURCE_TYPE_BACKGROUND = "background";

        /**
         * XML tags for the arrows image for the book
         */
        public const string RESOURCE_TYPE_ARROW_LEFT_NORMAL = "arrowLeftNormal";
        public const string RESOURCE_TYPE_ARROW_RIGHT_NORMAL = "arrowRightNormal";
        public const string RESOURCE_TYPE_ARROW_LEFT_OVER = "arrowLeftOver";
        public const string RESOURCE_TYPE_ARROW_RIGHT_OVER = "arrowRightOver";


        public const int TYPE_PARAGRAPHS = 0;

        public const int TYPE_PAGES = 1;

        /**
         * Id of the book
         */
        private string id;

        /**
         * Documentation of the book.
         */
        private string documentation;

        /**
         * Set of resources for the book
         */
        private List<ResourcesUni> resources;

        /**
         * Paragraphs of the book
         */
        private List<BookParagraph> paragraphs;

        /**
         * Pages of the book: Used in case type is {@value #TYPE_PAGES} or
         * {@value #TYPE_SCROLL_PAGE}.
         */
        private List<BookPage> pages;

        /**
         * Previous page Vector2
         */
        private Vector2 previousPageVector2;

        /**
         * Next page Vector2
         */
        private Vector2 nextPageVector2;


        public Vector2 getPreviousPageVector2()
        {

            return previousPageVector2;
        }


        public void setPreviousPageVector2(Vector2 previousPageVector2)
        {
            this.previousPageVector2 = previousPageVector2;
        }


        public Vector2 getNextPageVector2()
        {
            return nextPageVector2;
        }


        public void setNextPageVector2(Vector2 nextPageVector2)
        {
            this.nextPageVector2 = nextPageVector2;
        }

        /**
         * {@link #TYPE_PAGES}, {@link #TYPE_PARAGRAPHS}
         */
        private int type;

        /**
         * Creates a new Book
         * 
         * @param id
         *            the id of the book
         */
        public Book(string id)
        {
            this.id = id;
            this.type = TYPE_PARAGRAPHS;
            resources = new List<ResourcesUni>();
            paragraphs = new List<BookParagraph>();
            pages = new List<BookPage>();
            previousPageVector2 = new Vector2(20, 507);
            nextPageVector2 = new Vector2(707, 507);
        }

        /**
         * Returns the book's id
         * 
         * @return the book's id
         */
        public string getId()
        {

            return id;
        }

        /**
         * Returns the documentation of the book.
         * 
         * @return the documentation of the book
         */
        public string getDocumentation()
        {

            return documentation;
        }

        /**
         * Returns the book's list of resources
         * 
         * @return the book's list of resources
         */
        public List<ResourcesUni> getResources()
        {

            return resources;
        }

        /**
         * Adds some resources to the list of resources
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Returns the book's set of paragraphs
         * 
         * @return The book's set of paragraphs
         */
        public List<BookParagraph> getParagraphs()
        {

            return paragraphs;
        }

        /**
         * Adds a page to the book with margin
         * 
         * @param page
         *            New page (url) to be added
         */

        public void addPage(string uri, int type, int margin, bool scrollable)
        {

            BookPage page = new BookPage(uri, type, margin, scrollable);
            pages.Add(page);
            // add the page to the structure that gather all elements with assets (for chapter importation)
            if (uri != null && !uri.Equals(""))
                AllElementsWithAssets.addAsset(page);
        }

        /**
         * Adds a page to the book with margin
         * 
         * @param page
         *            New page (url) to be added
         */

        public void addPage(string uri, int type, int margin, int marginEnd, int marginTop, int marginBottom, bool scrollable)
        {

            BookPage page = new BookPage(uri, type, margin, marginEnd, marginTop, marginBottom, scrollable);
            pages.Add(page);
            // add the page to the structure that gather all elements with assets (for chapter importation)
            if (type != BookPage.TYPE_URL && uri != null && !uri.Equals(""))
                AllElementsWithAssets.addAsset(page);
        }

        /**
         * Adds a page to the book
         * 
         * @param page
         *            New page (url) to be added
         */

        public void addPage(string uri, int type)
        {

            BookPage page = new BookPage(uri, type);
            pages.Add(page);
            // add the page to the structure that gather all elements with assets (for chapter importation)
            if (uri != null && !uri.Equals(""))
                AllElementsWithAssets.addAsset(page);
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
        }

        /**
         * @return the pageURLs
         */
        public List<BookPage> getPageURLs()
        {

            return pages;
        }

        /**
         * Sets the a new identifier for the book.
         * 
         * @param id
         *            New identifier
         */
        public void setId(string id)
        {

            this.id = id;
        }

        /**
         * Changes the documentation of this book.
         * 
         * @param documentation
         *            The new documentation
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Adds a paragraph to the book
         * 
         * @param paragraph
         *            New paragraph to be added
         */
        public void addParagraph(BookParagraph paragraph)
        {

            paragraphs.Add(paragraph);
            // add the page to the structure that gather all elements with assets (for chapter importation)
            if (paragraph.getType() == BookParagraph.IMAGE)
                AllElementsWithAssets.addAsset(paragraph);
        }

        public object Clone()
        {
            Book b = (Book)this.MemberwiseClone();
            b.documentation = (documentation != null ? documentation : null);
            b.id = (id != null ? id : null);
            if (pages != null)
            {
                b.pages = new List<BookPage>();
                foreach (BookPage bp in pages)
                    b.pages.Add((BookPage)bp.Clone());
            }
            if (paragraphs != null)
            {
                b.paragraphs = new List<BookParagraph>();
                foreach (BookParagraph bp in paragraphs)
                    b.paragraphs.Add((BookParagraph)bp.Clone());
            }
            if (resources != null)
            {
                b.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                    b.resources.Add((ResourcesUni)r.Clone());
            }

            // TODO check this Zero comparision
            //if (nextPageVector2 != Vector2.zero)
            b.nextPageVector2 = nextPageVector2;

            //if (previousPageVector2 != Vector2.zero)
            b.previousPageVector2 = previousPageVector2;

            b.type = type;
            return b;
        }
        /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

       Book b = (Book) super.clone( );
       b.documentation = ( documentation != null ? new string(documentation ) : null );
       b.id = ( id != null ? new string(id ) : null );
       if( pages != null ) {
           b.pages = new List<BookPage>( );
           for( BookPage bp : pages )
               b.pages.add( (BookPage) bp.clone( ) );
       }
       if( paragraphs != null ) {
           b.paragraphs = new List<BookParagraph>( );
           for( BookParagraph bp : paragraphs )
               b.paragraphs.add( (BookParagraph) bp.clone( ) );
       }
       if( resources != null ) {
           b.resources = new List<Resources>( );
           for( Resources r : resources )
               b.resources.add( (Resources) r.clone( ) );
       }

       if ( nextPageVector2 != null )
           b.nextPageVector2 = (Vector2) this.nextPageVector2.clone( );

       if ( previousPageVector2 != null)
           b.previousPageVector2 = (Vector2) this.previousPageVector2.clone( );

    b.type = type;
       return b;
    }*/
    }
}