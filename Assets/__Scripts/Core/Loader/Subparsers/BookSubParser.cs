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
                pages,
                titles,
                bullets,
                imgs;

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
                    int x = int.Parse(xPrevious);
                    int y = int.Parse(yPrevious);
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
                    int x = int.Parse(xNext);
                    int y = int.Parse(yNext);
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

					string path = ell.GetAttribute("src");
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
}