using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeElementReferenceTool : Tool
    {
        private readonly ElementReference elementReference;
        private int x, y;
        private readonly int oldX, oldY;
        private readonly bool changePosition, changeScale;
        private float scale;
        private readonly float oldScale;

        public ChangeElementReferenceTool(ElementReference elementReference, int x, int y)
        {

            this.elementReference = elementReference;
            this.x = x;
            this.y = y;
            this.oldX = elementReference.getX();
            this.oldY = elementReference.getY();
            changePosition = true;
            changeScale = false;
        }

        public ChangeElementReferenceTool(ElementReference elementReference, float scale)
        {

            this.elementReference = elementReference;
            this.scale = scale;
            this.oldScale = this.elementReference.Scale;
            changePosition = false;
            changeScale = true;
        }

        public ChangeElementReferenceTool(ElementReference elementReference, int x, int y, float scale)
        {

            this.elementReference = elementReference;
            this.x = x;
            this.y = y;
            this.scale = scale;
            this.oldX = elementReference.getX();
            this.oldY = elementReference.getY();
            this.oldScale = this.elementReference.Scale;
            changePosition = true;
            changeScale = true;
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
            var combined = false;
            var otherReferenceTool = other as ChangeElementReferenceTool;
            if (otherReferenceTool != null)
            {
                if (otherReferenceTool.elementReference != elementReference)
                {
                    return false;
                }
                if (otherReferenceTool.changePosition && changePosition)
                {
                    x = otherReferenceTool.x;
                    y = otherReferenceTool.y;
                    timeStamp = otherReferenceTool.timeStamp;
                    combined = true;
                }
                if (otherReferenceTool.changeScale && changeScale)
                {
                    scale = otherReferenceTool.scale;
                    timeStamp = otherReferenceTool.timeStamp;
                    combined = true;
                }
            }
            return combined;
        }


        public override bool doTool()
        {

            if (changeScale)
            {
                elementReference.Scale = scale;
            }
            if (changePosition)
            {
                elementReference.setPosition(x, y);
            }
            return true;
        }


        public override string getToolName()
        {

            // TODO Auto-generated method stub
            return null;
        }


        public override bool redoTool()
        {

            if (changeScale)
            {
                elementReference.Scale = scale;
            }
            if (changePosition)
            {
                elementReference.setPosition(x, y);
            }
            Controller.Instance.reloadPanel();
            return true;
        }


        public override bool undoTool()
        {

            if (changeScale)
            {
                elementReference.Scale = oldScale;
            }
            if (changePosition)
            {
                elementReference.setPosition(oldX, oldY);
            }
            Controller.Instance.reloadPanel();
            return true;
        }
    }
}