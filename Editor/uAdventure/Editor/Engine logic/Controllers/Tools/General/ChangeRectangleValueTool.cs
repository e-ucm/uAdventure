using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeRectangleValueTool : Tool
    {

        private Rectangle rectangle;

        private int x, y, width, height;

        private int oldX, oldY, oldWidth, oldHeight;

        public ChangeRectangleValueTool(Rectangle rectangle, int x, int y, int width, int height)
        {

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.rectangle = rectangle;

            this.oldX = rectangle.getX();
            this.oldY = rectangle.getY();
            this.oldWidth = rectangle.getWidth();
            this.oldHeight = rectangle.getHeight();
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

            if (other is ChangeRectangleValueTool)
            {
                ChangeRectangleValueTool crvt = (ChangeRectangleValueTool)other;
                if (crvt.rectangle != rectangle)
                    return false;
                if (crvt.isChangePos() && isChangePos())
                {
                    x = crvt.x;
                    y = crvt.y;
                    timeStamp = crvt.timeStamp;
                    return true;
                }
                if (crvt.isChangeSize() && isChangeSize())
                {
                    width = crvt.width;
                    height = crvt.height;
                    timeStamp = crvt.timeStamp;
                    return true;
                }
            }
            return false;
        }


        public override bool doTool()
        {

            rectangle.setValues(x, y, width, height);
            return true;
        }


        public override bool redoTool()
        {

            rectangle.setValues(x, y, width, height);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            rectangle.setValues(oldX, oldY, oldWidth, oldHeight);
            Controller.Instance.updatePanel();
            return true;
        }

        private bool isChangeSize()
        {

            if (x == oldX && y == oldY)
                return true;
            return false;
        }

        private bool isChangePos()
        {

            if (width == oldWidth && height == oldHeight)
                return true;
            return false;
        }
    }
}