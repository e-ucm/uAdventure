using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BookDataControl : DataControlWithResources
    {

        /**
         * Contained book.
         */
        private Book book;

        /**
         * Book paragraphs list controller.
         */
        private BookParagraphsListDataControl bookParagraphsListDataControl;

        /**
         * Book pages list controller.
         */
        private BookPagesListDataControl bookPagesListDataControl;

        /**
         * Constructor.
         * 
         * @param book
         *            Contained book
         */
        public BookDataControl(Book book)
        {

            this.book = book;
            this.resourcesList = book.getResources();

            selectedResources = 0;

            // Add a new resource if the list is empty
            if (resourcesList.Count == 0)
                resourcesList.Add(new ResourcesUni());

            // Create the subcontrollers
            resourcesDataControlList = new List<ResourcesDataControl>();
            foreach (ResourcesUni resources in resourcesList)
                resourcesDataControlList.Add(new ResourcesDataControl(resources, Controller.BOOK));

            if (book.getType() == Book.TYPE_PARAGRAPHS)
                bookParagraphsListDataControl = new BookParagraphsListDataControl(book.getParagraphs());
            else if (book.getType() == Book.TYPE_PAGES)
                bookPagesListDataControl = new BookPagesListDataControl(book.getPageURLs(), this);
        }

        /**
         * Returns the book paragraphs list controller.
         * 
         * @return Book paragraphs list controller
         */
        public BookParagraphsListDataControl getBookParagraphsList()
        {

            return bookParagraphsListDataControl;
        }

        /**
         * Returns the book paragraphs list controller.
         * 
         * @return Book paragraphs list controller
         */
        public BookPagesListDataControl getBookPagesList()
        {

            return bookPagesListDataControl;
        }

        /**
         * Returns the id of the book.
         * 
         * @return Book's id
         */
        public string getId()
        {

            return book.getId();
        }

        /**
         * Returns the documentation of the book.
         * 
         * @return Book's documentation
         */
        public string getDocumentation()
        {

            return book.getDocumentation();
        }

        /**
         * Returns the path to the selected preview image.
         * 
         * @return Path to the image, null if not present
         */
        public string getPreviewImage()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("background");
        }
        public void setPreviewImage(string path)
        {
            resourcesDataControlList[selectedResources].addAsset("background", path);
        }

        public const int ARROW_LEFT = 0,
                          ARROW_RIGHT = 1,
                          ARROW_NORMAL = 0,
                          ARROW_OVER = 1;
        /**
         * Get the path of image for the specified arrow.
         * @param arrowOrientation Arrow orientation. It can be <i>ARROW_LEFT</i> or <i>ARROW_RIGHT</i>
         * @param arrowState Arrow state. It can be <i>ARROW_NORMAL</i> or <i>ARROW_OVER<i>
         * @return
         */
        public string getArrowImagePath(int arrowOrientation, int arrowState)
        {

            string asset = "";
            switch (arrowOrientation)
            {
                case ARROW_LEFT:
                    asset += "arrowLeft";
                    break;
                case ARROW_RIGHT:
                    asset += "arrowRight";
                    break;
            }

            switch (arrowState)
            {
                case ARROW_NORMAL:
                    asset += "Normal";
                    break;
                case ARROW_OVER:
                    asset += "Over";
                    break;
            }

            return resourcesDataControlList[selectedResources].getAssetPath(asset);
        }

        public void setArrowImagePath(int arrowOrientation, int arrowState, string path)
        {

            string asset = "";
            switch (arrowOrientation)
            {
                case ARROW_LEFT:
                    asset += "arrowLeft";
                    break;
                case ARROW_RIGHT:
                    asset += "arrowRight";
                    break;
            }

            switch (arrowState)
            {
                case ARROW_NORMAL:
                    asset += "Normal";
                    break;
                case ARROW_OVER:
                    asset += "Over";
                    break;
            }

            resourcesDataControlList[selectedResources].addAsset(asset, path);
        }
        /**
         * Get the path of image for the specified arrow.
         * @param arrowOrientation Arrow orientation. It can be <i>ARROW_LEFT</i> or <i>ARROW_RIGHT</i>
         * @param arrowState Arrow state. It can be <i>ARROW_NORMAL</i> or <i>ARROW_OVER<i>
         * @return
         */
        public string getArrowImagePath_WithDefault(int arrowOrientation, int arrowState)
        {

            string asset = "";
            string retVal = "";
            switch (arrowOrientation)
            {
                case ARROW_LEFT:
                    asset += "arrowLeft";
                    break;
                case ARROW_RIGHT:
                    asset += "arrowRight";
                    break;
            }

            switch (arrowState)
            {
                case ARROW_NORMAL:
                    asset += "Normal";
                    break;
                case ARROW_OVER:
                    asset += "Over";
                    break;
            }

            retVal = resourcesDataControlList[selectedResources].getAssetPath(asset);
            if (retVal == null || retVal.Equals(""))
            {
                if (arrowOrientation == ARROW_LEFT)
                {
                    if (arrowState == ARROW_NORMAL)
                    {
                        retVal = SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL;
                    }
                    else
                    {
                        retVal = SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER;
                    }
                }
                else
                {
                    if (arrowState == ARROW_NORMAL)
                    {
                        retVal = SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL_RIGHT;
                    }
                    else
                    {
                        retVal = SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER_RIGHT;
                    }
                }
            }

            return retVal;
        }

        /**
         * Sets the new documentation of the book.
         * 
         * @param documentation
         *            Documentation of the book
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(book, documentation));
        }


        public override System.Object getContent()
        {

            return book;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.RESOURCES };
        }


        public override bool canAddElement(int type)
        {

            // It can always add new resources
            return type == Controller.RESOURCES;
        }


        public override bool canBeDeleted()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
        }


        public override bool canBeRenamed()
        {

            return true;
        }


        public override bool addElement(int type, string id)
        {

            bool elementAdded = false;

            if (type == Controller.RESOURCES)
            {
                elementAdded = Controller.Instance.AddTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, Controller.BOOK, this));
            }

            return elementAdded;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

            if (elementIndex > 0)
            {
                ResourcesUni e = resourcesList[elementIndex];
                ResourcesDataControl c = resourcesDataControlList[elementIndex];
                resourcesList.RemoveAt(elementIndex);
                resourcesDataControlList.RemoveAt(elementIndex);
                resourcesList.Insert(elementIndex - 1, e);
                resourcesDataControlList.Insert(elementIndex - 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;
            int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

            if (elementIndex < resourcesList.Count - 1)
            {
                ResourcesUni e = resourcesList[elementIndex];
                ResourcesDataControl c = resourcesDataControlList[elementIndex];
                resourcesList.RemoveAt(elementIndex);
                resourcesDataControlList.RemoveAt(elementIndex);
                resourcesList.Insert(elementIndex + 1, e);
                resourcesDataControlList.Insert(elementIndex + 1, c);
                controller.DataModified();
                elementMoved = true;
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {
            string oldBookId = book.getId();
            string references = controller.countIdentifierReferences(oldBookId).ToString();

            // Ask for confirmation
            if (name != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameBookTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldBookId, references })))
            {
                // Show a dialog asking for the new book id
                string newBookId = name;
                if (name == null)
                    controller.ShowInputDialog(TC.get("Operation.RenameBookTitle"), TC.get("Operation.RenameBookMessage"), oldBookId, (o,s) => performRenameElement(s));
                else
                {
                    controller.DataModified();
                    return performRenameElement(name);
                }
            }

            return null;
        }

        private string performRenameElement(string newBookId)
        {
            string oldBookId = book.getId();
            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newBookId))
                newBookId = controller.makeElementValid(newBookId);

            book.setId(newBookId);
            controller.replaceIdentifierReferences(oldBookId, newBookId);
            controller.IdentifierSummary.deleteId<Book>(oldBookId);
            controller.IdentifierSummary.addId<Book>(newBookId);

            return newBookId;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Do nothing
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the resources
            for (int i = 0; i < resourcesDataControlList.Count; i++)
            {
                string resourcesPath = currentPath + " >> " + TC.getElement(Controller.RESOURCES) + " #" + (i + 1);
                valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
            }

            // Spread the call to the paragraphs
            if (book.getType() == Book.TYPE_PARAGRAPHS)
                valid &= bookParagraphsListDataControl.isValid(currentPath, incidences);
            else if (book.getType() == Book.TYPE_PAGES)
                valid &= bookPagesListDataControl.isValid(currentPath, incidences);

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                count += resourcesDataControl.countAssetReferences(assetPath);

            // Spread the call to the paragraphs/pages
            if (book.getType() == Book.TYPE_PARAGRAPHS)
                count += bookParagraphsListDataControl.countAssetReferences(assetPath);
            else if (book.getType() == Book.TYPE_PAGES)
                count += bookPagesListDataControl.countAssetReferences(assetPath);
            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.getAssetReferences(assetPaths, assetTypes);

            // Spread the call to the paragraphs/pages
            if (book.getType() == Book.TYPE_PARAGRAPHS)
                bookParagraphsListDataControl.getAssetReferences(assetPaths, assetTypes);
            else if (book.getType() == Book.TYPE_PAGES)
                bookPagesListDataControl.getAssetReferences(assetPaths, assetTypes);
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.deleteAssetReferences(assetPath);

            // Spread the call to the paragraphs/pages
            if (book.getType() == Book.TYPE_PARAGRAPHS)
                bookParagraphsListDataControl.deleteAssetReferences(assetPath);
            else if (book.getType() == Book.TYPE_PAGES)
                bookPagesListDataControl.deleteAssetReferences(assetPath);
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

        public int getType()
        {

            return book.getType();
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            check(this.getDocumentation(), TC.get("Search.Documentation"));
            check(this.getId(), "ID");
            if (this.getBookParagraphsList() != null)
                this.getBookParagraphsList().recursiveSearch();
            if (this.getBookPagesList() != null)
                this.getBookPagesList().recursiveSearch();
            check(this.getPreviewImage(), TC.get("Search.PreviewImage"));
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path = getPathFromChild(dataControl, resourcesDataControlList.Cast<Searchable>().ToList());
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, bookParagraphsListDataControl);
            if (path != null)
                return path;
            return getPathFromSearchableChild(dataControl, bookPagesListDataControl);
        }

        public Vector2 getNextPagePosition()
        {
            return book.getNextPageVector2();
        }

        public Vector2 getPreviousPagePosition()
        {
            return book.getPreviousPageVector2();
        }

        public void setNextPagePosition(Vector2 p)
        {
            book.setNextPageVector2(p);
        }

        public void setPreviousPagePosition(Vector2 p)
        {
            book.setPreviousPageVector2(p);
        }
    }
}