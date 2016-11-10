using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;

/**
 * Class to sub-parse books
 */
public class BookSubParser : SubParser
{


    /* Attributes */

    /**
     * Constant for subparsing nothing
     */
    private const int SUBPARSING_NONE = 0;

    /**
     * Constant for subparsing condition tag
     */
    private const int SUBPARSING_CONDITION = 1;

    /**
     * Stores the current element being subparsed
     */
    private int subParsing = SUBPARSING_NONE;

    /**
     * The book being read
     */
    private Book book;

    /**
     * Current resources being read
     */
    private ResourcesUni currentResources;

    /**
     * Current conditions being read
     */
    private Conditions currentConditions;

    /**
     * Subparser for the conditions
     */
    private SubParser conditionSubParser;

    /* Methods */

    /**
     * Constructor
     * 
     * @param chapter
     *            Chapter data to store the read data
     */
    public BookSubParser(Chapter chapter) : base(chapter)
    {
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#startElement(java.lang.string, java.lang.string,
     *      java.lang.string, org.xml.sax.Attributes)
     */
    public override void startElement(string namespaceURI, string sName, string qName, Dictionary<string, string> attrs)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a book tag, store the id of the book
            if (qName.Equals("book"))
            {
                string bookId = "";
                string xPrevious = "", xNext = "", yPrevious = "", yNext = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("id"))
                        bookId = entry.Value.ToString();
                    else if (entry.Key.Equals("xPreviousPage"))
                        xPrevious = entry.Value.ToString();
                    else if (entry.Key.Equals("xNextPage"))
                        xNext = entry.Value.ToString();
                    else if (entry.Key.Equals("yPreviousPage"))
                        yPrevious = entry.Value.ToString();
                    else if (entry.Key.Equals("yNextPage"))
                        yNext = entry.Value.ToString();
                }

                book = new Book(bookId);

                if (xPrevious != "" && yPrevious != "")
                {
                    try
                    {
                        int x = int.Parse(xPrevious);
                        int y = int.Parse(yPrevious);
                        book.setPreviousPageVector2(new Vector2(x, y));
                    }
                    catch (Exception e)
                    {
                        // Number in XML is wrong -> Do nothing
                    }
                }

                if (xNext != "" && yNext != "")
                {
                    try
                    {
                        int x = int.Parse(xNext);
                        int y = int.Parse(yNext);
                        book.setNextPageVector2(new Vector2(x, y));
                    }
                    catch (Exception e)
                    {
                        // Number in XML is wrong -> Do nothing
                    }
                }
            }

            // If it is a resources tag, create the new resources
            else if (qName.Equals("resources"))
            {
                currentResources = new ResourcesUni();

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("name"))
                        currentResources.setName(entry.Value.ToString());
                }

            }

            // If it is a documentation tag, hold the documentation in the book
            else if (qName.Equals("documentation"))
            {
                currentstring = string.Empty;
            }

            // If it is a condition tag, create a new subparser
            else if (qName.Equals("condition"))
            {
                currentConditions = new Conditions();
                conditionSubParser = new ConditionSubParser(currentConditions, chapter);
                subParsing = SUBPARSING_CONDITION;
            }

            // If it is an asset tag, read it and add it to the current resources
            else if (qName.Equals("asset"))
            {
                string type = "";
                string path = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("type"))
                        type = entry.Value.ToString();
                    if (entry.Key.Equals("uri"))
                        path = entry.Value.ToString();
                }

                currentResources.addAsset(type, path);
            }

            else if (qName.Equals("text"))
            {
                book.setType(Book.TYPE_PARAGRAPHS);
            }

            else if (qName.Equals("pages"))
            {
                book.setType(Book.TYPE_PAGES);
            }

            else if (qName.Equals("page"))
            {
                string uri = "";
                int type = BookPage.TYPE_URL;
                int margin = 0;
                int marginEnd = 0;
                int marginTop = 0;
                int marginBottom = 0;
                bool scrollable = false;

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("uri"))
                        uri = entry.Value.ToString();

                    if (entry.Key.Equals("type"))
                    {
                        if (entry.Value.ToString().Equals("resource"))
                            type = BookPage.TYPE_RESOURCE;
                        if (entry.Value.ToString().Equals("image"))
                            type = BookPage.TYPE_IMAGE;
                    }

                    if (entry.Key.Equals("scrollable"))
                        if (entry.Value.ToString().Equals("yes"))
                            scrollable = true;

                    if (entry.Key.Equals("margin"))
                    {
                        try
                        {
                            margin = int.Parse(entry.Value.ToString());
                        }
                        catch (Exception e) { Debug.LogError(e); }
                    }

                    if (entry.Key.Equals("marginEnd"))
                    {
                        try
                        {
                            marginEnd = int.Parse(entry.Value.ToString());
                        }
                        catch (Exception e) { Debug.LogError(e); }
                    }

                    if (entry.Key.Equals("marginTop"))
                    {
                        try
                        {
                            marginTop = int.Parse(entry.Value.ToString());
                        }
                        catch (Exception e) { Debug.LogError(e); }
                    }

                    if (entry.Key.Equals("marginBottom"))
                    {
                        try
                        {
                            marginBottom = int.Parse(entry.Value.ToString());
                        }
                        catch (Exception e) { Debug.LogError(e); }
                    }

                }
                book.addPage(uri, type, margin, marginEnd, marginTop, marginBottom, scrollable);

            }

            // If it is a title or bullet tag, store the previous text in the book
            else if (qName.Equals("title") || qName.Equals("bullet"))
            {
                // Add the new text paragraph
                if (currentstring != null && currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                    book.addParagraph(new BookParagraph(BookParagraph.TEXT, currentstring.ToString().Trim().Replace("\t", "")));
                currentstring = string.Empty;
            }

            // If it is an image tag, store the image in the book
            else if (qName.Equals("img"))
            {

                // Add the new text paragraph
                if (currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                {
                    book.addParagraph(new BookParagraph(BookParagraph.TEXT, currentstring.ToString().Trim().Replace("\t", "")));
                    currentstring = string.Empty;
                }

                string path = "";

                foreach (KeyValuePair<string, string> entry in attrs)
                {
                    if (entry.Key.Equals("src"))
                        path = entry.Value.ToString();
                }

                // Add the new image paragraph
                book.addParagraph(new BookParagraph(BookParagraph.IMAGE, path));
            }
        }

        // If a condition is being subparsed, spread the call
        if (subParsing == SUBPARSING_CONDITION)
        {
            conditionSubParser.startElement(namespaceURI, sName, qName, attrs);
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#endElement(java.lang.string, java.lang.string,
     *      java.lang.string)
     */
    public override void endElement(string namespaceURI, string sName, string qName)
    {

        // If no element is being subparsed
        if (subParsing == SUBPARSING_NONE)
        {

            // If it is a book tag, add the book to the game data
            if (qName.Equals("book"))
            {
                chapter.addBook(book);
            }

            // If it is a resources tag, add the resources to the book
            else if (qName.Equals("resources"))
            {
                book.addResources(currentResources);
            }

            // If it is a documentation tag, hold the documentation in the book
            else if (qName.Equals("documentation"))
            {
                book.setDocumentation(currentstring.ToString().Trim());
            }

            // If it is a text tag, add the text to the book
            else if (qName.Equals("text"))
            {
                // Add the new text paragraph
                if (currentstring != null && currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                    book.addParagraph(new BookParagraph(BookParagraph.TEXT, currentstring.ToString().Trim().Replace("\t", "")));
            }

            // If it is a title tag, add the text to the book
            else if (qName.Equals("title"))
            {
                // Add the new title paragraph
                if (currentstring != null)
                    book.addParagraph(new BookParagraph(BookParagraph.TITLE, currentstring.ToString().Trim().Replace("\t", "")));
            }

            else if (qName.Equals("bullet"))
            {
                // Add the new bullet paragraph
                if (currentstring != null)
                    book.addParagraph(new BookParagraph(BookParagraph.BULLET, currentstring.ToString().Trim().Replace("\t", "")));
            }

            // Reset the current string
            currentstring = string.Empty;
        }

        // If a condition is being subparsed
        else if (subParsing == SUBPARSING_CONDITION)
        {

            // Spread the end element call
            conditionSubParser.endElement(namespaceURI, sName, qName);

            // If the condition is being closed, add the conditions to the resources
            if (qName.Equals("condition"))
            {
                currentResources.setConditions(currentConditions);
                subParsing = SUBPARSING_NONE;
            }
        }
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.engine.loader.subparsers.SubParser#characters(char[], int, int)
     */
    public override void characters(char[] buf, int offset, int len)
    {

        // If no element is being subparsed, read the characters
        if (subParsing == SUBPARSING_NONE)
            base.characters(buf, offset, len);

        // If a condition is being subparsed, spread the call
        else if (subParsing == SUBPARSING_CONDITION)
            conditionSubParser.characters(buf, offset, len);
    }
}