using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BookParagraphsListDataControl : DataControl
    {
        /**
           * List of book paragraphs.
           */
        private List<BookParagraph> bookParagraphsList;

        /**
         * Book paragraph controllers.
         */
        private List<BookParagraphDataControl> bookParagraphsDataControlList;

        /**
         * Constructor.
         * 
         * @param bookParagraphsList
         *            List of book paragraphs
         */
        public BookParagraphsListDataControl(List<BookParagraph> bookParagraphsList)
        {

            this.bookParagraphsList = bookParagraphsList;

            // Create the subcontrollers
            bookParagraphsDataControlList = new List<BookParagraphDataControl>();
            foreach (BookParagraph bookParagraph in bookParagraphsList)
                bookParagraphsDataControlList.Add(new BookParagraphDataControl(bookParagraph));
        }

        /**
         * Return the list of book paragraph controllers.
         * 
         * @return Book paragraph controllers
         */
        public List<BookParagraphDataControl> getBookParagraphs()
        {

            return bookParagraphsDataControlList;
        }

        /**
         * Returns the last book paragraph controller of the list.
         * 
         * @return Last book paragraph
         */
        public BookParagraphDataControl getLastBookParagraph()
        {

            return bookParagraphsDataControlList[bookParagraphsDataControlList.Count - 1];
        }

        /**
         * Returns the info of the book paragraphs contained in the list.
         * 
         * @return Array with the information of the book paragraphs. It contains
         *         the number of each paragraph, the type of the paragraph, and the
         *         word count if applicable
         */
        public string[][] getBookParagraphsInfo()
        {

            string[][] bookParagraphsInfo = null;

            // Create the list for the book paragraphs
            bookParagraphsInfo = new string[bookParagraphsList.Count][];
            for (int i = 0; i < bookParagraphsList.Count; i++)
                bookParagraphsInfo[i] = new string[3];

            // Fill the array with the info
            for (int i = 0; i < bookParagraphsList.Count; i++)
            {
                BookParagraph bookParagraph = bookParagraphsList[i];

                if (bookParagraph.getType() == BookParagraph.TEXT)
                    bookParagraphsInfo[i][0] = TC.get("BookParagraphsList.TextParagraph", (i + 1).ToString());
                else if (bookParagraph.getType() == BookParagraph.TITLE)
                    bookParagraphsInfo[i][0] = TC.get("BookParagraphsList.TitleParagraph", (i + 1).ToString());
                else if (bookParagraph.getType() == BookParagraph.BULLET)
                    bookParagraphsInfo[i][0] = TC.get("BookParagraphsList.BulletParagraph", (i + 1).ToString());
                else if (bookParagraph.getType() == BookParagraph.IMAGE)
                    bookParagraphsInfo[i][0] = TC.get("BookParagraphsList.ImageParagraph", (i + 1).ToString());

                if (bookParagraph.getType() != BookParagraph.IMAGE)
                    bookParagraphsInfo[i][1] = TC.get("BookParagraphsList.WordCount", (bookParagraph.getContent().Split(' ').Length.ToString()));
                else
                    bookParagraphsInfo[i][1] = TC.get("BookParagraphsList.NotApplicable");
            }

            return bookParagraphsInfo;
        }


        public override System.Object getContent()
        {

            return bookParagraphsList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.BOOK_TITLE_PARAGRAPH, Controller.BOOK_TEXT_PARAGRAPH, Controller.BOOK_BULLET_PARAGRAPH, Controller.BOOK_IMAGE_PARAGRAPH };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new paragraphs
            return type == Controller.BOOK_TITLE_PARAGRAPH || type == Controller.BOOK_TEXT_PARAGRAPH || type == Controller.BOOK_BULLET_PARAGRAPH || type == Controller.BOOK_IMAGE_PARAGRAPH;
        }


        public override bool canBeDeleted()
        {

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
        }


        public override bool addElement(int type, string id)
        {

            BookParagraph newBookParagraph = null;

            if (type == Controller.BOOK_TITLE_PARAGRAPH)
                newBookParagraph = new BookParagraph(BookParagraph.TITLE);

            else if (type == Controller.BOOK_TEXT_PARAGRAPH)
                newBookParagraph = new BookParagraph(BookParagraph.TEXT);

            else if (type == Controller.BOOK_BULLET_PARAGRAPH)
                newBookParagraph = new BookParagraph(BookParagraph.BULLET);

            else if (type == Controller.BOOK_IMAGE_PARAGRAPH)
                newBookParagraph = new BookParagraph(BookParagraph.IMAGE);

            // If a paragraph was added, add the controller to the list
            if (newBookParagraph != null)
            {
                bookParagraphsList.Add(newBookParagraph);
                bookParagraphsDataControlList.Add(new BookParagraphDataControl(newBookParagraph));
                controller.DataModified();
            }

            return newBookParagraph != null;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;

            if (bookParagraphsList.Remove((BookParagraph)dataControl.getContent()))
            {
                bookParagraphsDataControlList.Remove((BookParagraphDataControl)dataControl);
                elementDeleted = true;
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = bookParagraphsList.IndexOf((BookParagraph)dataControl.getContent());

            if (elementIndex > 0)
            {
                BookParagraph e = bookParagraphsList[elementIndex];
                BookParagraphDataControl c = bookParagraphsDataControlList[elementIndex];
                bookParagraphsList.RemoveAt(elementIndex);
                bookParagraphsDataControlList.RemoveAt(elementIndex);
                bookParagraphsList.Insert(elementIndex - 1, e);
                bookParagraphsDataControlList.Insert(elementIndex - 1, c);
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = bookParagraphsList.IndexOf((BookParagraph)dataControl.getContent());

            if (elementIndex < bookParagraphsList.Count - 1)
            {
                BookParagraph e = bookParagraphsList[elementIndex];
                BookParagraphDataControl c = bookParagraphsDataControlList[elementIndex];
                bookParagraphsList.RemoveAt(elementIndex);
                bookParagraphsDataControlList.RemoveAt(elementIndex);
                bookParagraphsList.Insert(elementIndex + 1, e);
                bookParagraphsDataControlList.Insert(elementIndex + 1, c);
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Do nothing
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the paragraphs
            for (int i = 0; i < bookParagraphsDataControlList.Count; i++)
            {
                string bookParagraphPath = currentPath + " >> " + TC.get("Element.BookParagraph") + " #" + (i + 1) + " (" + TC.getElement(bookParagraphsDataControlList[i].getType()) + ")";
                valid &= bookParagraphsDataControlList[i].isValid(bookParagraphPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Spread the call to the paragraphs
            foreach (BookParagraphDataControl bookParagraphDataControl in bookParagraphsDataControlList)
                count += bookParagraphDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Spread the call to the paragraphs
            foreach (BookParagraphDataControl bookParagraphDataControl in bookParagraphsDataControlList)
                bookParagraphDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Spread the call to the paragraphs
            foreach (BookParagraphDataControl bookParagraphDataControl in bookParagraphsDataControlList)
                bookParagraphDataControl.deleteAssetReferences(assetPath);
        }


        public override int countIdentifierReferences(string id)
        {

            return 0;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // Do nothing
        }


        public override void deleteIdentifierReferences(string id)
        {

            // Do nothing
        }


        public override bool canBeDuplicated()
        {

            return false;
        }


        public override void recursiveSearch()
        {

            foreach (DataControl dc in bookParagraphsDataControlList)
                dc.recursiveSearch();
        }

        public List<BookParagraph> getBookParagraphsList()
        {

            return bookParagraphsList;
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, bookParagraphsDataControlList.Cast<Searchable>().ToList());
        }
    }
}