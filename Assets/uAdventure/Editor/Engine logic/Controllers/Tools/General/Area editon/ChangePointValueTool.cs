using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangePointValueTool : Tool
    {

        private Vector2 point;

        private int x;

        private int y;

        private int originalX;

        private int originalY;

        public ChangePointValueTool(Vector2 point, int x, int y)
        {

            this.point = point;
            this.x = x;
            this.y = y;
            this.originalX = (int)point.x;
            this.originalY = (int)point.y;
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

            if (other is ChangePointValueTool)
            {
                ChangePointValueTool cpvt = (ChangePointValueTool)other;
                if (cpvt.point == point)
                {
                    cpvt.x = x;
                    cpvt.y = y;
                    cpvt.timeStamp = timeStamp;
                    return true;
                }
            }
            return false;
        }


        public override bool doTool()
        {

            point = new Vector2(x, y);
            return true;
        }


        public override bool redoTool()
        {
            point = new Vector2(x, y);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {
            point = new Vector2(originalX, originalY);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}