using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookParagraphDataControl : DataControl {


    /**
     * Contained book paragraph.
     */
    private BookParagraph bookParagraph;

    /**
     * Type of the book paragraph.
     */
    private int bookParagraphType;

    /**
     * Constructor.
     * 
     * @param bookParagraph
     *            Contained book paragraph
     */
    public BookParagraphDataControl(BookParagraph bookParagraph)
    {

        this.bookParagraph = bookParagraph;

        // Store the type of the paragraph
        switch (bookParagraph.getType())
        {
            case BookParagraph.TITLE:
                bookParagraphType = Controller.BOOK_TITLE_PARAGRAPH;
                break;
            case BookParagraph.TEXT:
                bookParagraphType = Controller.BOOK_TEXT_PARAGRAPH;
                break;
            case BookParagraph.IMAGE:
                bookParagraphType = Controller.BOOK_IMAGE_PARAGRAPH;
                break;
            case BookParagraph.BULLET:
                bookParagraphType = Controller.BOOK_BULLET_PARAGRAPH;
                break;
        }
    }

    /**
     * Returns the type of the contained paragraph.
     * 
     * @return Type of contained book paragraph
     */
    public int getType()
    {

        return bookParagraphType;
    }

    /**
     * Returns the content of the paragraph.
     * 
     * @return Paragraph's content
     */
    public string getParagraphContent()
    {

        return bookParagraph.getContent();
    }

    /**
     * Sets the new content for the paragraph. This method must be used only
     * with text and bullet paragraphs.
     * 
     * @param content
     *            New content for the paragtaph
     */
    public void setParagraphTextContent(string content)
    {

        Controller.getInstance().addTool(new ChangeParagraphContentTool(bookParagraph, content));
    }

    /**
     * Shows a dialog to choose a new path for the given asset.
     * 
     * @param index
     *            Index of the asset
     */
    public void editImagePath()
    {

        // Get the list of assets from the ZIP file
        string selectedAsset = null;
        //TODO: implement
        //AssetChooser chooser = AssetsController.getAssetChooser(AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE);
        //int option = chooser.showAssetChooser(controller.peekWindow());
        ////In case the asset was selected from the zip file
        //if (option == AssetChooser.ASSET_FROM_ZIP)
        //{
        //    selectedAsset = chooser.getSelectedAsset();
        //}

        ////In case the asset was not in the zip file: first add it
        //else if (option == AssetChooser.ASSET_FROM_OUTSIDE)
        //{
        //    bool added = AssetsController.addSingleAsset(AssetsConstants.CATEGORY_IMAGE, chooser.getSelectedFile().getAbsolutePath());
        //    if (added)
        //    {
        //        selectedAsset = chooser.getSelectedFile().getName();
        //    }
        //}

        // If a file was selected
        if (selectedAsset != null)
        {
            // Take the index of the selected asset
            string[] assetFilenames = AssetsController.getAssetFilenames(AssetsConstants.CATEGORY_IMAGE);
            string[] assetPaths = AssetsController.getAssetsList(AssetsConstants.CATEGORY_IMAGE);
            int assetIndex = -1;
            for (int i = 0; i < assetFilenames.Length; i++)
                if (assetFilenames[i].Equals(selectedAsset))
                    assetIndex = i;

            Controller.getInstance().addTool(new ChangeParagraphContentTool(bookParagraph, assetPaths[assetIndex]));

        }
    }

    /**
     * Sets the new path for the paragraph. This method must be used only with
     * image paragraphs.
     */
    public void setImageParagraphContent()
    {

        // Get the list of assets from the ZIP file
        string[] assetFilenames = AssetsController.getAssetFilenames(AssetsConstants.CATEGORY_IMAGE);
        string[] assetPaths = AssetsController.getAssetsList(AssetsConstants.CATEGORY_IMAGE);

        // If the list of assets is empty, show an error message
        if (assetFilenames.Length == 0)
            controller.showErrorDialog(TC.get("Resources.EditAsset"), TC.get("Resources.ErrorNoAssets"));

        // If not empty, select one of them
        else {
            // Let the user choose between the assets
            string selectedAsset = controller.showInputDialog(TC.get("Resources.EditAsset"), TC.get("Resources.EditAssetMessage"), assetFilenames);

            // If a file was selected
            if (selectedAsset != null)
            {
                // Take the index of the selected asset
                int assetIndex = -1;
                for (int i = 0; i < assetFilenames.Length; i++)
                    if (assetFilenames[i].Equals(selectedAsset))
                        assetIndex = i;

                Controller.getInstance().addTool(new ChangeParagraphContentTool(bookParagraph, assetPaths[assetIndex]));
            }
        }
    }

    /**
     * Deletes the content of the image paragraph. This method must be used only
     * with image paragraphs.
     */
    public void deleteImageParagraphContent()
    {

        Controller.getInstance().addTool(new ChangeParagraphContentTool(bookParagraph, ""));
    }

    
    public override System.Object getContent()
    {

        return bookParagraph;
    }

    
    public override int[] getAddableElements()
    {

        return new int[] { };
    }

    
    public override bool canAddElement(int type)
    {

        return false;
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

        return false;
    }

    
    public override bool addElement(int type, string id)
    {

        return false;
    }

    
    public override bool deleteElement(DataControl dataControl, bool askConfirmation)
    {

        return false;
    }

    
    public override bool moveElementUp(DataControl dataControl)
    {

        return false;
    }

    
    public override bool moveElementDown(DataControl dataControl)
    {

        return false;
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

        // If it is an image paragraph and it's not linked to an asset, is invalid
        if (bookParagraph.getType() == BookParagraph.IMAGE && bookParagraph.getContent().Length == 0)
        {
            valid = false;

            // Store the incidence
            if (incidences != null)
                incidences.Add(currentPath + " >> " + TC.get("Operation.AdventureConsistencyErrorBookParagraph"));
        }

        return valid;
    }

    
    public override int countAssetReferences(string assetPath)
    {

        // Return 1 if it is an image paragraph and the asset matches
        return bookParagraph.getType() == BookParagraph.IMAGE && bookParagraph.getContent().Equals(assetPath) ? 1 : 0;
    }

    
    public override void deleteAssetReferences(string assetPath)
    {

        // If it is an image paragraph and contains the asset, delete it
        if (bookParagraph.getType() == BookParagraph.IMAGE && bookParagraph.getContent().Equals(assetPath))
        {
            bookParagraph.setContent("");
        }
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

    
    public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
    {

        // Only if book paragraph is image
        if (bookParagraph.getType() == BookParagraph.IMAGE)
        {
            string imagePath = bookParagraph.getContent();
            // Search in assetPaths
            bool add = true;
            foreach (string asset in assetPaths)
            {
                if (asset.Equals(imagePath))
                {
                    add = false;
                    break;
                }
            }
            if (add)
            {
                int last = assetPaths.Count;
                assetPaths.Insert(last, imagePath);
                assetTypes.Insert(last, AssetsConstants.CATEGORY_IMAGE);
            }
        }
    }

    
    public override bool canBeDuplicated()
    {

        return true;
    }

    
    public override void recursiveSearch()
    {

        check(this.getParagraphContent(), TC.get("Search.ParagraphContent"));
    }

    
    public override List<Searchable> getPathToDataControl(Searchable dataControl)
    {

        return null;
    }
}
