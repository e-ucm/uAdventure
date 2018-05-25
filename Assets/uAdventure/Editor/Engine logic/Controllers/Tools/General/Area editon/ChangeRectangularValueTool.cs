using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeRectangularValueTool : Tool
    {

        private Rectangle rectangle;

        private bool rectangular;

        public ChangeRectangularValueTool(Rectangle rectangle, bool rectangular)
        {

            this.rectangle = rectangle;
            this.rectangular = rectangular;
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

            if (rectangle.isRectangular() != rectangular)
            {
                rectangle.setRectangular(rectangular);
                return true;
            }
            return false;
        }


        public override bool redoTool()
        {

            rectangle.setRectangular(rectangular);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            rectangle.setRectangular(!rectangular);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}