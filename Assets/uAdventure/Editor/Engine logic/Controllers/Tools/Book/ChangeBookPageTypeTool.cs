using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeBookPageTypeTool : Tool
    {

        private BookPage bookPage;

        private int newType;

        private int oldType;

        private string oldUri;

        public ChangeBookPageTypeTool(BookPage bookPage, int newType)
        {

            this.bookPage = bookPage;
            this.newType = newType;
            this.oldType = bookPage.getType();
            this.oldUri = bookPage.getUri();
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

            if (newType != oldType)
            {
                bookPage.setType(newType);
                if (newType == BookPage.TYPE_RESOURCE)
                    bookPage.setUri("");
                else if (newType == BookPage.TYPE_IMAGE)
                    bookPage.setUri("");
                else
                    bookPage.setUri("http://www.");
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            bookPage.setType(newType);
            if (newType == BookPage.TYPE_RESOURCE)
                bookPage.setUri("");
            else if (newType == BookPage.TYPE_IMAGE)
                bookPage.setUri("");
            else
                bookPage.setUri("http://www.");
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bookPage.setType(oldType);
            bookPage.setUri(oldUri);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}