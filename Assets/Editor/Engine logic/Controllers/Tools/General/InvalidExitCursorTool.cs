using UnityEngine;
using System.Collections;

public class InvalidExitCursorTool : Tool {
    protected string oldCursorPath;

    protected ExitLook exitLook;

    public InvalidExitCursorTool(ExitLook exitLook)
    {

        this.exitLook = exitLook;
        this.oldCursorPath = exitLook.getCursorPath();
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

        if (exitLook.getCursorPath() != null)
        {
            exitLook.setCursorPath(null);
            return true;
        }
        else {
            return false;
        }
    }

    
    public override bool redoTool()
    {

        exitLook.setCursorPath(null);
        Controller.getInstance().reloadPanel();
        return true;
    }

    
    public override bool undoTool()
    {

        exitLook.setCursorPath(oldCursorPath);
        Controller.getInstance().reloadPanel();
        return true;
    }
}
