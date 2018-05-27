using UnityEngine;
using System.Collections;
using System.Xml;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [DOMWriter(typeof(Book))]
    public class BookDOMWriter : ParametrizedDOMWriter
    {
        public BookDOMWriter()
        {

        }

        protected override string GetElementNameFor(object target)
        {
            return "book";
        }

        protected override void FillNode(XmlNode node, object target, params IDOMWriterParam[] options)
        {
            var book = target as Book;
            XmlElement bookElement = node as XmlElement;

            // Create the necessary elements to create the DOM
            XmlDocument doc = Writer.GetDoc();

            // Add attributes to root node
            bookElement.SetAttribute("id", book.getId());

            // Adding next page position
            if (book.getNextPageVector2() != null)
            {
                bookElement.SetAttribute("xNextPage", book.getNextPageVector2().x + "");
                bookElement.SetAttribute("yNextPage", book.getNextPageVector2().y + "");
            }

            // Adding previous page position
            if (book.getPreviousPageVector2() != null)
            {
                bookElement.SetAttribute("xPreviousPage", book.getPreviousPageVector2().x + "");
                bookElement.SetAttribute("yPreviousPage", book.getPreviousPageVector2().y + "");
            }

            // Append the documentation (if avalaible)
            if (book.getDocumentation() != null)
            {
                XmlNode bookDocumentationNode = doc.CreateElement("documentation");
                bookDocumentationNode.AppendChild(doc.CreateTextNode(book.getDocumentation()));
                bookElement.AppendChild(bookDocumentationNode);
            }

            // Append the resources
            foreach (ResourcesUni resources in book.getResources())
            {
                XmlNode resourcesNode = ResourcesDOMWriter.buildDOM(resources, ResourcesDOMWriter.RESOURCES_BOOK);
                doc.ImportNode(resourcesNode, true);
                bookElement.AppendChild(resourcesNode);
            }

            // Create the text/pages element

            XmlElement textPagesElement = null;
            if (book.getType() == Book.TYPE_PARAGRAPHS)
            {
                textPagesElement = doc.CreateElement("text");

                // Create and append the paragraphs
                foreach (BookParagraph bookParagraph in book.getParagraphs())
                {
                    XmlNode paragraphNode = null;

                    // If it is a text paragraph
                    if (bookParagraph.getType() == BookParagraph.TEXT)
                    {
                        paragraphNode = doc.CreateTextNode(bookParagraph.getContent());
                    }

                    // If it is a text paragraph
                    if (bookParagraph.getType() == BookParagraph.TITLE)
                    {
                        paragraphNode = doc.CreateElement("title");
                        paragraphNode.AppendChild(doc.CreateTextNode(bookParagraph.getContent()));
                    }

                    // If it is a bullet paragraph
                    else if (bookParagraph.getType() == BookParagraph.BULLET)
                    {
                        paragraphNode = doc.CreateElement("bullet");
                        paragraphNode.AppendChild(doc.CreateTextNode(bookParagraph.getContent()));
                    }

                    // If it is an image paragraph
                    else if (bookParagraph.getType() == BookParagraph.IMAGE)
                    {
                        XmlElement imageParagraphElement = doc.CreateElement("img");
                        imageParagraphElement.SetAttribute("src", bookParagraph.getContent());
                        paragraphNode = imageParagraphElement;
                    }

                    // Append the created paragraph
                    textPagesElement.AppendChild(paragraphNode);
                }
            }
            else if (book.getType() == Book.TYPE_PAGES)
            {
                textPagesElement = doc.CreateElement("pages");
                foreach (BookPage page in book.getPageURLs())
                {
                    if (page.getUri().Length > 0)
                    {
                        // Create the node for the page
                        XmlElement pageElement = doc.CreateElement("page");

                        //Attributes: uri, type, margin, scrollable
                        pageElement.SetAttribute("scrollable", (page.getScrollable() ? "yes" : "no"));
                        pageElement.SetAttribute("margin", page.getMargin().ToString());
                        if (page.getMarginEnd() != 0)
                            pageElement.SetAttribute("marginEnd", page.getMarginEnd().ToString());
                        if (page.getMarginTop() != 0)
                            pageElement.SetAttribute("marginTop", page.getMarginTop().ToString());
                        if (page.getMarginBottom() != 0)
                            pageElement.SetAttribute("marginBottom", page.getMarginBottom().ToString());
                        pageElement.SetAttribute("uri", page.getUri());
                        switch (page.getType())
                        {
                            case BookPage.TYPE_RESOURCE:
                                pageElement.SetAttribute("type", "resource");
                                break;
                            case BookPage.TYPE_IMAGE:
                                pageElement.SetAttribute("type", "image");
                                break;
                            case BookPage.TYPE_URL:
                                pageElement.SetAttribute("type", "url");
                                break;
                            default:
                                pageElement.SetAttribute("type", "url");
                                break;
                        }

                        textPagesElement.AppendChild(pageElement);
                    }
                }
            }
            // Append the text element
            bookElement.AppendChild(textPagesElement);
        }

    }
}