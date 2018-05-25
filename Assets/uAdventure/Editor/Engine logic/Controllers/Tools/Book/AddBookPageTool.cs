using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddBookPageTool : Tool
    {
        private List<BookPage> bookPagesList;

        private BookPage newBookPage;

        private int selectedPage;

        public AddBookPageTool(List<BookPage> bookPagesList, BookPage newBookPage, int selectedPage)
        {

            this.bookPagesList = bookPagesList;
            this.newBookPage = newBookPage;
            this.selectedPage = selectedPage;
        }


        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return true;
        }


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count)
            {
                bookPagesList.Insert(selectedPage + 1, newBookPage);
            }
            else
            {
                bookPagesList.Add(newBookPage);
            }
            return true;
        }


        public override bool redoTool()
        {

            if (selectedPage >= 0 && selectedPage < bookPagesList.Count)
            {
                bookPagesList.Insert(selectedPage + 1, newBookPage);
            }
            else
            {
                bookPagesList.Add(newBookPage);
            }
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bookPagesList.Remove(newBookPage);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}