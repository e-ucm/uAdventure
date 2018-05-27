using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteBookPageTool : Tool
    {


        private List<BookPage> bookPagesList;

        private BookPage bookPage;

        private int index;

        public DeleteBookPageTool(List<BookPage> bookPagesList, BookPage page)
        {

            this.bookPagesList = bookPagesList;
            this.bookPage = page;
            this.index = bookPagesList.IndexOf(bookPage);
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

            return bookPagesList.Remove(bookPage);
        }


        public override bool redoTool()
        {

            bool temp = bookPagesList.Remove(bookPage);
            Controller.Instance.updatePanel();
            return temp;
        }


        public override bool undoTool()
        {

            bookPagesList.Insert(index, bookPage);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}