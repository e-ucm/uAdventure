using UnityEngine;
using System.Collections;
using Side = Trajectory.Side;

public class SetSideLengthTool : Tool {

    private float oldLength;

    private Side side;

    private float value;

    public SetSideLengthTool(Side side, int value)
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
        if (other is SetSideLengthTool ) {
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
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {
        side.setLenght(oldLength);
        Controller.getInstance().updatePanel();
        return true;
    }
}
