using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BookPagesListDataControl : Searchable
    {


        /**
         * List of book paragraphs.
         */
        private List<BookPage> bookPagesList;

        private int selectedPage;

        private bool defaultScrollable;

        private int defaultMargin;

        private int defaultType;

        private BookDataControl dControl;

        /**
         * Constructor.
         * 
         * @param bookParagraphsList
         *            List of book paragraphs
         */
        public BookPagesListDataControl(List<BookPage> bookPagesList, BookDataControl dControl)
        {

            this.bookPagesList = bookPagesList;
            this.dControl = dControl;
            selectedPage = -1;
            defaultType = BookPage.TYPE_RESOURCE;
            defaultMargin = 0;
            defaultScrollable = false;
        }

        /**
         * Return the list of book paragraph controllers.
         * 
         * @return Book paragraph controllers
         */
        public List<BookPage> getBookPages()
        {

            return bookPagesList;
        }

        public BookPage addPage()
        {

            BookPage newBookPage = new BookPage("", defaultType, defaultMargin, defaultScrollable);

            Controller.Instance.AddTool(new AddBookPageTool(bookPagesList, newBookPage, selectedPage));

            selectedPage = bookPagesList.IndexOf(newBookPage);

            return newBookPage;
        }

        public bool deletePage(BookPage page)
        {

            Controller.Instance.AddTool(new DeleteBookPageTool(bookPagesList, page));
            return true;
        }

        public bool movePageUp(BookPage page)
        {

            Controller.Instance.AddTool(new MoveBookPageUpTool(bookPagesList, page));
            return true;
        }

        public bool movePageDown(BookPage page)
        {

            Controller.Instance.AddTool(new MoveBookPageDownTool(bookPagesList, page));
            return true;
        }

        public int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Spread the call to the pages
            foreach (BookPage bookPage in bookPagesList)
                if (bookPage.getUri().Equals(assetPath) && (bookPage.getType() == BookPage.TYPE_RESOURCE || bookPage.getType() == BookPage.TYPE_IMAGE))
                    count++;
            return count;
        }

        public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Spread the call to the pages
            foreach (BookPage bookPage in bookPagesList)
            {

                string uri = bookPage.getUri();
                if (uri != null && !uri.Equals("") && (bookPage.getType() == BookPage.TYPE_RESOURCE || bookPage.getType() == BookPage.TYPE_IMAGE))
                {
                    // Search assetPaths
                    bool add = true;
                    foreach (string asset in assetPaths)
                    {
                        if (asset.Equals(uri))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        int last = assetPaths.Count;
                        assetPaths.Insert(last, uri);
                        if (bookPage.getType() == BookPage.TYPE_RESOURCE)
                            assetTypes.Insert(last, AssetsConstants.CATEGORY_STYLED_TEXT);
                        else if (bookPage.getType() == BookPage.TYPE_IMAGE)
                            assetTypes.Insert(last, AssetsConstants.CATEGORY_IMAGE);
                    }
                }
            }
        }

        public void deleteAssetReferences(string assetPath)
        {

            List<BookPage> toRemove = new List<BookPage>();

            //Spread the call to the paragraphs
            foreach (BookPage bookPage in bookPagesList)
                if (bookPage.getUri().Equals(assetPath) && (bookPage.getType() == BookPage.TYPE_RESOURCE || bookPage.getType() == BookPage.TYPE_IMAGE))
                    toRemove.Add(bookPage);

            foreach (BookPage bookPage in toRemove)
                bookPagesList.Remove(bookPage);

        }

        public BookPage changeCurrentPage(int page)
        {

            BookPage currentPage = null;
            if (page >= 0 && page < bookPagesList.Count)
            {
                currentPage = bookPagesList[page];
                this.selectedPage = page;
            }
            return currentPage;
        }

        public bool editStyledTextAssetPath()
        {

            if (selectedPage < 0 || selectedPage >= bookPagesList.Count)
                return false;
            ////TODO: implement
            //string selectedAsset = null;
            //AssetChooser chooser = AssetsController.getAssetChooser(AssetsConstants.CATEGORY_STYLED_TEXT, AssetsController.FILTER_NONE);
            //int option = chooser.showAssetChooser(Controller.getInstance().peekWindow());
            ////In case the asset was selected from the zip file
            //if (option == AssetChooser.ASSET_FROM_ZIP)
            //{
            //    selectedAsset = chooser.getSelectedAsset();
            //}

            ////In case the asset was not in the zip file: first add it
            //else if (option == AssetChooser.ASSET_FROM_OUTSIDE)
            //{
            //    bool added = AssetsController.AddSingleAsset(AssetsConstants.CATEGORY_STYLED_TEXT, chooser.getSelectedFile().getAbsolutePath());

            //    //Check if there are referenced files. Those files must be in a folder where the asset is contained, and that folder must be called
            //    //assetname_files
            //    string filePath = chooser.getSelectedFile().getAbsolutePath();
            //    string filesFolderPath = filePath.substring(0, filePath.lastIndexOf(".")) + "_files";
            //    File filesFolder = new File(filesFolderPath);
            //    if (filesFolder.exists() && filesFolder.isDirectory())
            //    {
            //        added &= AssetsController.AddSingleAsset(AssetsConstants.CATEGORY_STYLED_TEXT, filesFolderPath);
            //    }

            //    if (added)
            //    {
            //        selectedAsset = chooser.getSelectedFile().getName();
            //    }
            //}

            //// If a file was selected
            //if (selectedAsset != null)
            //{
            //    // Take the index of the selected asset
            //    string[] assetFilenames = AssetsController.getAssetFilenames(AssetsConstants.CATEGORY_STYLED_TEXT);
            //    string[] assetPaths = AssetsController.getAssetsList(AssetsConstants.CATEGORY_STYLED_TEXT);
            //    int assetIndex = -1;
            //    for (int i = 0; i < assetFilenames.length; i++)
            //        if (assetFilenames[i].Equals(selectedAsset))
            //            assetIndex = i;

            //    Controller.getInstance().addTool(new ChangeBookPageUriTool(bookPagesList[selectedPage], assetPaths[assetIndex]));
            //    return true;
            //}
            //else
            //    return false;
            return true;
        }

        public bool editImageAssetPath()
        {

            if (selectedPage < 0 || selectedPage >= bookPagesList.Count)
                return false;
            ////TODO: implement
            //string selectedAsset = null;
            //AssetChooser chooser = AssetsController.getAssetChooser(AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE);
            //int option = chooser.showAssetChooser(Controller.getInstance().peekWindow());
            ////In case the asset was selected from the zip file
            //if (option == AssetChooser.ASSET_FROM_ZIP)
            //{
            //    selectedAsset = chooser.getSelectedAsset();
            //}

            ////In case the asset was not in the zip file: first add it
            //else if (option == AssetChooser.ASSET_FROM_OUTSIDE)
            //{
            //    bool added = AssetsController.AddSingleAsset(AssetsConstants.CATEGORY_IMAGE, chooser.getSelectedFile().getAbsolutePath());

            //    //Check if there are referenced files. Those files must be in a folder where the asset is contained, and that folder must be called
            //    //assetname_files
            //    string filePath = chooser.getSelectedFile().getAbsolutePath();
            //    string filesFolderPath = filePath.substring(0, filePath.lastIndexOf(".")) + "_files";
            //    File filesFolder = new File(filesFolderPath);
            //    if (filesFolder.exists() && filesFolder.isDirectory())
            //    {
            //        added &= AssetsController.AddSingleAsset(AssetsConstants.CATEGORY_IMAGE, filesFolderPath);
            //    }

            //    if (added)
            //    {
            //        selectedAsset = chooser.getSelectedFile().getName();
            //    }
            //}

            //// If a file was selected
            //if (selectedAsset != null)
            //{
            //    // Take the index of the selected asset
            //    string[] assetFilenames = AssetsController.getAssetFilenames(AssetsConstants.CATEGORY_IMAGE);
            //    string[] assetPaths = AssetsController.getAssetsList(AssetsConstants.CATEGORY_IMAGE);
            //    int assetIndex = -1;
            //    for (int i = 0; i < assetFilenames.length; i++)
            //        if (assetFilenames[i].Equals(selectedAsset))
            //            assetIndex = i;

            //    Controller.getInstance().addTool(new ChangeBookPageUriTool(bookPagesList[selectedPage], assetPaths[assetIndex]));
            //    return true;
            //}
            //else
            //    return false;
            return true;
        }

        public bool editURL(string newURL)
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count && bookPagesList[selectedPage].getType() == BookPage.TYPE_URL)
            {
                Controller.Instance.AddTool(new ChangeBookPageUriTool(bookPagesList[selectedPage], newURL));
                return true;
            }
            return false;
        }

        public bool setType(int newType)
        {

            bool typeSet = false;
            if (selectedPage >= 0 && selectedPage < bookPagesList.Count && newType != bookPagesList[selectedPage].getType())
            {
                Controller.Instance.AddTool(new ChangeBookPageTypeTool(bookPagesList[selectedPage], newType));
                typeSet = true;
            }
            return typeSet;
        }

        public void setMargins(int newMargin, int newMarginTop, int newMarginBottom, int newMarginEnd)
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count)
            {
                Controller.Instance.AddTool(new ChangeBookPageMarginsTool(bookPagesList[selectedPage], newMargin, newMarginTop, newMarginBottom, newMarginEnd));
            }
        }

        public void setScrollable(bool scrollable)
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count)
            {
                Controller.Instance.AddTool(new ChangeBookPageScrollableTool(bookPagesList[selectedPage], scrollable));
            }
        }

        public bool isValidPage(BookPage page)
        {

            bool isValid = false;
            ////TODO: implement
            //try
            //{
            //    if (page.getType() == BookPage.TYPE_RESOURCE)
            //    {
            //        BookPagePreviewPanel panel = new BookPagePreviewPanel(dControl, true);
            //        panel.setCurrentBookPage(page);
            //        isValid = !page.getUri().Equals("") && panel.isValid();
            //    }
            //    else if (page.getType() == BookPage.TYPE_URL)
            //    {
            //        //Check the URL exists and is accessible
            //        URL url = new URL(page.getUri());
            //        url.openStream().close();
            //        isValid = true;
            //    }
            //    else if (page.getType() == BookPage.TYPE_IMAGE)
            //    {
            //        if (page.getUri().length() > 0)
            //            isValid = true;
            //    }
            //}
            //catch (Exception e)
            //{
            //    isValid = false;
            //}
            return isValid;
        }

        public bool isValidPage(int page)
        {

            return isValidPage(bookPagesList[page]);
        }

        public bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the paragraphs
            for (int i = 0; i < bookPagesList.Count; i++)
            {
                string bookParagraphPath = currentPath + " >> " + TC.get("Element.BookPage") + " #" + (i + 1);
                bool isPageValid = true;
                if (bookPagesList[i].getType() == BookPage.TYPE_RESOURCE && bookPagesList[i].getUri().Length == 0)
                {
                    isPageValid = false;
                    if (incidences != null)
                        incidences.Add(bookParagraphPath + " >> " + TC.get("Operation.AdventureConsistencyErrorBookPage"));
                }
                valid &= isPageValid;

            }

            return valid;

        }

        public BookPage getSelectedPage()
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count)
                return bookPagesList[selectedPage];
            else
                return null;
        }

        public int getIndexSelectedPage()
        {
            return selectedPage;
        }

        public BookPage getLastPage()
        {

            return bookPagesList[bookPagesList.Count - 1];
        }


        public override void recursiveSearch()
        {

            for (int i = 0; i < bookPagesList.Count; i++)
            {
                if (bookPagesList[i].getType() == BookPage.TYPE_RESOURCE)
                {

                    ////TODO: implement
                    //JEditorPane editor = new JEditorPane();

                    //try
                    //{
                    //    editor.setPage(AssetsController.getResourceAsURLFromZip(bookPagesList.get(i).getUri()));

                    //    check(editor.getDocument().getText(0, editor.getDocument().getLength()), TC.get("Search.HTMLBookPage"));
                    //    check(bookPagesList.get(i).getUri(), TC.get("Search.HTMLBookPage"));
                    //}
                    //catch (MalformedURLException e1)
                    //{
                    //    //writeFileNotFound(folder + helpPath);
                    //}
                    //catch (IOException e1)
                    //{
                    //    //writeFileNotFound(folder + helpPath);
                    //}
                    //catch (BadLocationException e)
                    //{
                    //    // TODO Auto-generated catch block
                    //    e.printStackTrace();
                    //}

                    // case image and URL, search at 
                }
                else
                {
                    check(bookPagesList[i].getUri(), TC.get("Search.HTMLBookPage"));
                }
            }

        }


        protected override List<Searchable> getPath(Searchable dataControl)
        {

            if (dataControl == this)
            {
                List<Searchable> path = new List<Searchable>();
                path.Add(this);
                return path;
            }
            return null;
        }
    }
}