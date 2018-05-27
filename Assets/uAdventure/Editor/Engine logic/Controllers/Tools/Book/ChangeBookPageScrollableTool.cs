using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeBookPageScrollableTool : Tool
    {

        private BookPage bookPage;

        private bool newScrollable;

        private bool oldScrollable;

        public ChangeBookPageScrollableTool(BookPage bookPage, bool scrollable)
        {

            this.bookPage = bookPage;
            this.newScrollable = scrollable;
            this.oldScrollable = bookPage.getScrollable();
        }


        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return false;
        }


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            if (oldScrollable == newScrollable)
                return false;
            bookPage.setScrollable(newScrollable);
            return true;
        }


        public override bool redoTool()
        {

            bookPage.setScrollable(newScrollable);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bookPage.setScrollable(oldScrollable);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}