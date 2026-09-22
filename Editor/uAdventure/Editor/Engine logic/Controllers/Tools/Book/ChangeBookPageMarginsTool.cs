using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeBookPageMarginsTool : Tool
    {

        private BookPage bookPage;

        private int newMargin;

        private int newMarginTop;

        private int newMarginBottom;

        private int newMarginEnd;

        private int oldMargin;

        private int oldMarginTop;

        private int oldMarginBottom;

        private int oldMarginEnd;

        public ChangeBookPageMarginsTool(BookPage bookPage, int newMargin, int newMarginTop, int newMarginBottom, int newMarginEnd)
        {

            this.bookPage = bookPage;
            this.newMargin = newMargin;
            this.newMarginTop = newMarginTop;
            this.newMarginBottom = newMarginBottom;
            this.newMarginEnd = newMarginEnd;
            this.oldMargin = bookPage.getMargin();
            this.oldMarginTop = bookPage.getMarginTop();
            this.oldMarginBottom = bookPage.getMarginBottom();
            this.oldMarginEnd = bookPage.getMarginEnd();
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

            if (other is ChangeBookPageMarginsTool)
            {
                ChangeBookPageMarginsTool cbpmt = (ChangeBookPageMarginsTool)other;
                if (cbpmt.bookPage == bookPage)
                {
                    if (oldMargin != newMargin && cbpmt.oldMargin != cbpmt.newMargin)
                    {
                        newMargin = cbpmt.newMargin;
                        timeStamp = cbpmt.timeStamp;
                        return true;
                    }
                    if (oldMarginBottom != newMarginBottom && cbpmt.oldMarginBottom != cbpmt.newMarginBottom)
                    {
                        oldMarginBottom = cbpmt.oldMarginBottom;
                        timeStamp = cbpmt.timeStamp;
                        return true;
                    }
                    if (oldMarginTop != newMarginTop && cbpmt.oldMarginTop != cbpmt.newMarginTop)
                    {
                        newMarginTop = cbpmt.newMarginTop;
                        timeStamp = cbpmt.timeStamp;
                        return true;
                    }
                    if (oldMarginEnd != newMarginEnd && cbpmt.oldMarginEnd != cbpmt.newMarginEnd)
                    {
                        newMarginEnd = cbpmt.newMarginEnd;
                        timeStamp = cbpmt.timeStamp;
                        return true;
                    }
                }
            }
            return false;
        }


        public override bool doTool()
        {

            if (oldMargin != newMargin)
            {
                bookPage.setMargin(newMargin);
                return true;
            }
            if (oldMarginTop != newMarginTop)
            {
                bookPage.setMarginTop(newMarginTop);
                return true;
            }
            if (oldMarginBottom != newMarginBottom)
            {
                bookPage.setMarginBottom(newMarginBottom);
                return true;
            }
            if (oldMarginEnd != newMarginEnd)
            {
                bookPage.setMarginEnd(newMarginEnd);
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            bookPage.setMargin(newMargin);
            bookPage.setMarginBottom(newMarginBottom);
            bookPage.setMarginEnd(newMarginEnd);
            bookPage.setMarginTop(newMarginTop);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            bookPage.setMargin(oldMargin);
            bookPage.setMarginBottom(oldMarginBottom);
            bookPage.setMarginEnd(oldMarginEnd);
            bookPage.setMarginTop(oldMarginTop);
            Controller.Instance.updatePanel();
            return true;
        }

    }
}