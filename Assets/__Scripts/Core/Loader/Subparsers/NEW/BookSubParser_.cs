using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class BookSubParser_ : Subparser_
{
    private Book book;
    private ResourcesUni currentResources;
    private Conditions currentConditions;

    public BookSubParser_(Chapter chapter) : base(chapter)
    {
    }

    public override void ParseElement(XmlElement element)
    {
        XmlNodeList
            resourcess = element.SelectNodes("resources"),
            documentations = element.SelectNodes("documentations"),
            texts = element.SelectNodes("text"),
            pagess = element.SelectNodes("pages"),
            conditions,
            assets,
            pages,
            titles,
            bullets,
            imgs;

        string tmpArgVal;

        string bookId = "";
        string xPrevious = "", xNext = "", yPrevious = "", yNext = "";

        tmpArgVal = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            bookId = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("xPreviousPage");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            xPrevious = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("xNextPage");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            xNext = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("yPreviousPage");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            yPrevious = tmpArgVal;
        }
        tmpArgVal = element.GetAttribute("yNextPage");
        if (!string.IsNullOrEmpty(tmpArgVal))
        {
            yNext = tmpArgVal;
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

        foreach (XmlElement el in resourcess)
        {
            currentResources = new ResourcesUni();

            tmpArgVal = el.GetAttribute("name");
            if (!string.IsNullOrEmpty(tmpArgVal))
            {
                currentResources.setName(tmpArgVal);
            }

            assets = el.SelectNodes("asset");
            foreach (XmlElement ell in assets)
            {
                string type = "";
                string path = "";

                tmpArgVal = ell.GetAttribute("type");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    type = tmpArgVal;
                }
                tmpArgVal = ell.GetAttribute("uri");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    path = tmpArgVal;
                }
                currentResources.addAsset(type, path);
            }

            conditions = el.SelectNodes("condition");
            foreach (XmlElement ell in conditions)
            {
                currentConditions = new Conditions();
                new ConditionSubParser_(currentConditions, chapter).ParseElement(ell);
                currentResources.setConditions(currentConditions);
            }

            book.addResources(currentResources);
        }
        foreach (XmlElement el in documentations)
        {
            string currentstring = el.InnerText;
            book.setDocumentation(currentstring.ToString().Trim());
        }

        foreach (XmlElement el in texts)
        {
            book.setType(Book.TYPE_PARAGRAPHS);
            string currentstring_ = el.InnerText;
            // Add the new text paragraph
            if (currentstring_ != null &&
                currentstring_.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                book.addParagraph(new BookParagraph(BookParagraph.TEXT,
                    currentstring_.ToString().Trim().Replace("\t", "")));

            titles = el.SelectNodes("title");
            foreach (XmlElement ell in titles)
            {
                string currentstring = ell.InnerText;
                if (currentstring != null &&
                    currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                    book.addParagraph(new BookParagraph(BookParagraph.TITLE,
                        currentstring.ToString().Trim().Replace("\t", "")));
            }

            bullets = el.SelectNodes("bullet");
            foreach (XmlElement ell in bullets)
            {
                string currentstring = ell.InnerText;
                if (currentstring != null &&
                    currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                    book.addParagraph(new BookParagraph(BookParagraph.BULLET,
                        currentstring.ToString().Trim().Replace("\t", "")));
            }

            imgs = el.SelectNodes("img");
            foreach (XmlElement ell in imgs)
            {
                string currentstring = ell.InnerText;
                // Add the new text paragraph
                if (currentstring.ToString().Trim().Replace("\t", "").Replace("\n", "").Length > 0)
                {
                    book.addParagraph(new BookParagraph(BookParagraph.TEXT,
                        currentstring.ToString().Trim().Replace("\t", "")));
                    currentstring = string.Empty;
                }

                string path = "";
                tmpArgVal = ell.GetAttribute("src");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    path = tmpArgVal;
                }
                // Add the new image paragraph
                book.addParagraph(new BookParagraph(BookParagraph.IMAGE, path));
            }
        }
        foreach (XmlElement el in pagess)
        {
            book.setType(Book.TYPE_PAGES);

            pages = el.SelectNodes("page");

            foreach (XmlElement ell in pages)
            {
                string uri = "";
                int type = BookPage.TYPE_URL;
                int margin = 0;
                int marginEnd = 0;
                int marginTop = 0;
                int marginBottom = 0;
                bool scrollable = false;

                tmpArgVal = ell.GetAttribute("uri");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    uri = tmpArgVal;
                }

                tmpArgVal = ell.GetAttribute("type");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("resource"))
                        type = BookPage.TYPE_RESOURCE;
                    if (tmpArgVal.Equals("image"))
                        type = BookPage.TYPE_IMAGE;
                }

                tmpArgVal = ell.GetAttribute("type");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("resource"))
                        type = BookPage.TYPE_RESOURCE;
                    if (tmpArgVal.Equals("image"))
                        type = BookPage.TYPE_IMAGE;
                }

                tmpArgVal = ell.GetAttribute("scrollable");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    if (tmpArgVal.Equals("yes"))
                        scrollable = true;
                }

                tmpArgVal = ell.GetAttribute("margin");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    try
                    {
                        margin = int.Parse(tmpArgVal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                tmpArgVal = ell.GetAttribute("marginEnd");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    try
                    {
                        marginEnd = int.Parse(tmpArgVal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                tmpArgVal = ell.GetAttribute("marginTop");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    try
                    {
                        marginTop = int.Parse(tmpArgVal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                tmpArgVal = ell.GetAttribute("marginBottom");
                if (!string.IsNullOrEmpty(tmpArgVal))
                {
                    try
                    {
                        marginBottom = int.Parse(tmpArgVal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }


                book.addPage(uri, type, margin, marginEnd, marginTop, marginBottom, scrollable);
            }
        }


        chapter.addBook(book);
    }
}