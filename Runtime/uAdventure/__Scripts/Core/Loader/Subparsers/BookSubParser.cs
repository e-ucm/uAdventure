using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace uAdventure.Core
{
	[DOMParser(typeof(Book))]
	[DOMParser("book")]
	public class BookSubParser : IDOMParser
    {

		public object DOMParse(XmlElement element, params object[] parameters)
		{
            XmlNodeList
                resourcess = element.SelectNodes("resources"),
                documentations = element.SelectNodes("documentations"),
                texts = element.SelectNodes("text"),
                pagess = element.SelectNodes("pages"),
                pages;

            string bookId = "";
            string xPrevious = "", xNext = "", yPrevious = "", yNext = "";

            bookId 		= element.GetAttribute("id");
			xPrevious 	= element.GetAttribute ("xPreviousPage");
			xNext 		= element.GetAttribute ("xNextPage");
			yPrevious 	= element.GetAttribute ("yPreviousPage");
			yNext 		= element.GetAttribute ("yNextPage");

            Book book = new Book(bookId);

            if (xPrevious != "" && yPrevious != "")
            {
                try
                {
                    float x = float.Parse(xPrevious);
                    float y = float.Parse(yPrevious);
                    book.setPreviousPageVector2(new Vector2(x, y));
                }
                catch
                {
                    // Number in XML is wrong -> Do nothing
                }
            }
            if (xNext != "" && yNext != "")
            {
                try
                {
                    float x = float.Parse(xNext);
                    float y = float.Parse(yNext);
                    book.setNextPageVector2(new Vector2(x, y));
                }
                catch
                {
                    // Number in XML is wrong -> Do nothing
                }
            }


            // RESOURCES
            foreach (var res in DOMParserUtility.DOMParse<ResourcesUni>(resourcess, parameters))
                book.addResources(res);


            foreach (XmlElement el in documentations)
            {
                string currentstring = el.InnerText;
                book.setDocumentation(currentstring.ToString().Trim());
            }

            foreach (XmlElement text in texts)
            {
                book.setType(Book.TYPE_PARAGRAPHS);
                foreach (XmlNode child in text.ChildNodes)
                {
                    if (child is XmlText)
                    {
                        var childText = child.InnerText;
                        if (!string.IsNullOrEmpty(childText.Replace("\t\n", "")))
                        {
                            book.addParagraph(new BookParagraph(BookParagraph.TEXT, childText.Trim().Replace("\t", "")));
                        }
                    }
                    else if (child is XmlElement)
                    {
                        var paragraph = DOMParserUtility.DOMParse(child, parameters) as BookParagraph;
                        if (paragraph != null)
                        {
                            book.addParagraph(paragraph);
                        }
                    }
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

					uri = ell.GetAttribute("uri");

					switch(ell.GetAttribute("type")){
					case "resource" : type = BookPage.TYPE_RESOURCE; break;
					case "image" : type = BookPage.TYPE_IMAGE; break;
					}

					scrollable = ExString.EqualsDefault(ell.GetAttribute("scrollable"), "yes", false);
					margin = ExParsers.ParseDefault(ell.GetAttribute("margin"), 0);
					marginEnd = ExParsers.ParseDefault(ell.GetAttribute("marginEnd"), 0);
					marginTop = ExParsers.ParseDefault(ell.GetAttribute("marginTop"), 0);
					marginBottom = ExParsers.ParseDefault(ell.GetAttribute("marginBottom"), 0);

                    book.addPage(uri, type, margin, marginEnd, marginTop, marginBottom, scrollable);
                }
            }

			return book;
        }
    }

    [DOMParser("title")]
    public class TitleSubParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            return new BookParagraph(BookParagraph.TITLE, element.InnerText.Trim().Replace("\t", ""));
        }
    }

    [DOMParser("bullet")]
    public class BulletSubParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            return new BookParagraph(BookParagraph.BULLET, element.InnerText.Trim().Replace("\t", ""));
        }
    }

    [DOMParser("img")]
    public class ImageSubParser : IDOMParser
    {
        public object DOMParse(XmlElement element, params object[] parameters)
        {
            string path = element.GetAttribute("src");
            return new BookParagraph(BookParagraph.IMAGE, path);
        }
    }
}