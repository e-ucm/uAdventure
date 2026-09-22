using UnityEngine;
using System.Collections;
using uAdventure.Core;
using Side = uAdventure.Core.Trajectory.Side;

namespace uAdventure.Editor
{
    public class SetSideLengthTool : Tool
    {

        private float oldLength;

        private Side side;

        private float value;

        public SetSideLengthTool(Side side, float value)
        {
            this.side = side;
            this.value = value;
            this.oldLength = side.getLength();
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
            if (other is SetSideLengthTool)
            {
                SetSideLengthTool crvt = (SetSideLengthTool)other;
                if (crvt.side != side)
                    return false;
                value = crvt.value;
                timeStamp = crvt.timeStamp;
                return true;
            }
            return false;
        }


        public override bool doTool()
        {
            side.setLenght(value);
            return true;
        }


        public override bool redoTool()
        {
            side.setLenght(value);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {
            side.setLenght(oldLength);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}