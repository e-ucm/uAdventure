using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BooksListDataControl : DataControl
    {
        /**
         * List of books.
         */
        private List<Book> booksList;

        /**
         * Book controllers.
         */
        private List<BookDataControl> booksDataControlList;

        /**
         * Constructor.
         * 
         * @param booksList
         *            List of books
         */

        public BooksListDataControl(List<Book> booksList)
        {

            this.booksList = booksList;

            // Create the subcontrollers
            booksDataControlList = new List<BookDataControl>();
            foreach (Book book in booksList)
                booksDataControlList.Add(new BookDataControl(book));
        }

        /**
         * Returns the list of book controllers.
         * 
         * @return Book controllers
         */

        public List<BookDataControl> getBooks()
        {

            return booksDataControlList;
        }

        /**
         * Returns the last book controller of the list.
         * 
         * @return Last book controller
         */

        public BookDataControl getLastBook()
        {

            return booksDataControlList[booksDataControlList.Count - 1];
        }


        public override System.Object getContent()
        {

            return booksList;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.BOOK };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new scenes
            return type == Controller.BOOK;
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

        public string[] getBooksIDs()
        {
            string[] tmp = new string[booksList.Count];

            for (int i = 0; i < booksList.Count; i++)
                tmp[i] = booksList[i].getId();

            return tmp;
        }

        public override bool addElement(int type, string bookId)
        {
            bool elementAdded = false;
            
            // Show a dialog asking for the book id
            if (string.IsNullOrEmpty(bookId))
                controller.ShowInputIdDialog(TC.get("Operation.AddBookTitle"), TC.get("Operation.AddBookMessage"),
                    Controller.Instance.makeElementValid(TC.get("Operation.AddBookDefaultValue")), performAddBook);
            else
            {
                performAddBook(null, bookId);
                elementAdded = true;
            }

            return elementAdded;
        }

        private void performAddBook(object sender, string bookId)
        {
            // If some value was typed and the identifier is valid
            if (!controller.isElementIdValid(bookId))
                bookId = controller.makeElementValid(bookId);

            // Add thew new book
            Book newBook = new Book(bookId);
            newBook.setType(Book.TYPE_PARAGRAPHS);

            // Set default background
            ResourcesUni resources = new ResourcesUni();
            resources.addAsset("background", SpecialAssetPaths.ASSET_DEFAULT_BOOK_IMAGE);
            resources.addAsset("arrowLeftNormal", SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL);
            resources.addAsset("arrowLeftOver", SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER);
            newBook.addResources(resources);

            BookDataControl newDataControl = new BookDataControl(newBook);
            booksList.Add(newBook);
            booksDataControlList.Add(newDataControl);
            controller.IdentifierSummary.addId<Book>(bookId);
        }


        public override bool duplicateElement(DataControl dataControl)
        {

            if (!(dataControl is BookDataControl))
                return false;


            Book newElement = (Book)(((Book)(dataControl.getContent())).Clone());
			string id = newElement.getId();

			if (!controller.isElementIdValid(id))
				id = controller.makeElementValid(id);
			
            newElement.setId(id);
            booksList.Add(newElement);
            booksDataControlList.Add(new BookDataControl(newElement));
            controller.IdentifierSummary.addId<Book>(id);
            return true;

        }


        public override string getDefaultId(int type)
        {

            return TC.get("Operation.AddBookDefaultValue");
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            bool elementDeleted = false;
            string bookId = ((BookDataControl)dataControl).getId();
            string references = controller.countIdentifierReferences(bookId).ToString();

            // Ask for confirmation
            if (!askConfirmation || controller.ShowStrictConfirmDialog(TC.get("Operation.DeleteElementTitle"), TC.get("Operation.DeleteElementWarning", new string[] { bookId, references })))
            {
                if (booksList.Remove((Book)dataControl.getContent()))
                {
                    booksDataControlList.Remove((BookDataControl)dataControl);
                    controller.deleteIdentifierReferences(bookId);
                    controller.IdentifierSummary.deleteId<Book>(bookId);
                    controller.updateVarFlagSummary();
                    controller.DataModified();
                    elementDeleted = true;
                }
            }

            return elementDeleted;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = booksList.IndexOf((Book)dataControl.getContent());

            if (elementIndex > 0)
            {
                Book e = booksList[elementIndex];
                BookDataControl c = booksDataControlList[elementIndex];
                booksList.RemoveAt(elementIndex);
                booksDataControlList.RemoveAt(elementIndex);
                booksList.Insert(elementIndex - 1, e);
                booksDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = booksList.IndexOf((Book)dataControl.getContent());

            if (elementIndex < booksList.Count - 1)
            {
                Book e = booksList[elementIndex];
                BookDataControl c = booksDataControlList[elementIndex];
                booksList.RemoveAt(elementIndex);
                booksDataControlList.RemoveAt(elementIndex);
                booksList.Insert(elementIndex + 1, e);
                booksDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
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

            // Update the current path
            currentPath += " >> " + TC.getElement(Controller.BOOKS_LIST);

            // Iterate through the books
            foreach (BookDataControl bookDataControl in booksDataControlList)
            {
                string bookPath = currentPath + " >> " + bookDataControl.getId();
                valid &= bookDataControl.isValid(bookPath, incidences);
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through the books
            foreach (BookDataControl bookDataControl in booksDataControlList)
                count += bookDataControl.countAssetReferences(assetPath);

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through the books
            foreach (BookDataControl bookDataControl in booksDataControlList)
                bookDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through the books
            foreach (BookDataControl bookDataControl in booksDataControlList)
                bookDataControl.deleteAssetReferences(assetPath);
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

            foreach (DataControl dc in this.booksDataControlList)
                dc.recursiveSearch();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, booksDataControlList.Cast<Searchable>().ToList());
        }
    }
}