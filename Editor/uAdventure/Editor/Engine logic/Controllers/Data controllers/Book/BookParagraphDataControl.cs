using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BookParagraphDataControl : DataControl
    {


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
            Controller.Instance.AddTool(new ChangeParagraphContentTool(bookParagraph, content));
        }

        public void setParagraphImagePath(string uri)
        {
            Controller.Instance.AddTool(new ChangeParagraphContentTool(bookParagraph, uri));
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
}