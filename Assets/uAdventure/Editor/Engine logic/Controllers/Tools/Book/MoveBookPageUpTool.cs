using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MoveBookPageUpTool : Tool
    {

        private List<BookPage> bookPagesList;

        private BookPage bookPage;

        private int oldElementIndex;

        private int newElementIndex;

        public MoveBookPageUpTool(List<BookPage> bookPagesList, BookPage page)
        {

            this.bookPagesList = bookPagesList;
            this.bookPage = page;
            this.oldElementIndex = bookPagesList.IndexOf(bookPage);
            this.newElementIndex = oldElementIndex - 1;
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

            if (other is MoveBookPageUpTool)
            {
                MoveBookPageUpTool mbput = (MoveBookPageUpTool)other;
                if (mbput.bookPage == bookPage)
                {
                    newElementIndex = mbput.newElementIndex;
                    timeStamp = mbput.timeStamp;
                    return true;
                }
            }
            return false;
        }


        public override bool doTool()
        {

            if (oldElementIndex > 0)
            {
                BookPage b = bookPagesList[oldElementIndex];
                bookPagesList.RemoveAt(oldElementIndex);
                bookPagesList.Insert(newElementIndex, b);
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {
            BookPage b = bookPagesList[oldElementIndex];
            bookPagesList.RemoveAt(oldElementIndex);
            bookPagesList.Insert(newElementIndex, b);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {
            BookPage b = bookPagesList[newElementIndex];
            bookPagesList.RemoveAt(newElementIndex);
            bookPagesList.Insert(oldElementIndex, b);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}