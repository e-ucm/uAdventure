using UnityEngine;
using System.Collections;

public class ChangeElementReferenceTool : Tool
{

    private ElementReference elementReference;

    private int x, y;

    private int oldX, oldY;

    private bool changePosition;

    private bool changeScale;

    private float scale, oldScale;

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

    public ChangeElementReferenceTool(ElementReference elementReference2, float scale2)
    {

        this.elementReference = elementReference2;
        this.scale = scale2;
        this.oldScale = elementReference.getScale();
        changePosition = false;
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

        if (other is ChangeElementReferenceTool ) {
            ChangeElementReferenceTool crvt = (ChangeElementReferenceTool)other;
            if (crvt.elementReference != elementReference)
                return false;
            if (crvt.changePosition && changePosition)
            {
                x = crvt.x;
                y = crvt.y;
                timeStamp = crvt.timeStamp;
                return true;
            }
            if (crvt.changeScale && changeScale)
            {
                scale = crvt.scale;
                timeStamp = crvt.timeStamp;
                return true;
            }
        }
        return false;
    }

    
    public override bool doTool()
    {

        if (changeScale)
        {
            elementReference.setScale(scale);
        }
        else if (changePosition)
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
            elementReference.setScale(scale);
        }
        else if (changePosition)
        {
            elementReference.setPosition(x, y);
        }
        Controller.getInstance().reloadPanel();
        return true;
    }

    
    public override bool undoTool()
    {

        if (changeScale)
        {
            elementReference.setScale(oldScale);
        }
        else if (changePosition)
        {
            elementReference.setPosition(oldX, oldY);
        }
        Controller.getInstance().reloadPanel();
        return true;
    }
}
