using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChapterToolManager {

    /**
    * Global tool manager. For undo/redo in main window (real changes in the
    * data structure)
    */
    private ToolManager globalToolManager;

    /**
     * Local tool managers. For undo/redo in dialogs (This will only reflect
     * temporal changes, not real changes in data)
     */
    private Stack<ToolManager> localToolManagers;

    public ChapterToolManager()
    {

        globalToolManager = new ToolManager(true);
        localToolManagers = new Stack<ToolManager>();
    }

    public void clear()
    {

        globalToolManager.clear();
        localToolManagers.Clear();
    }

    // METHODS TO MANAGE UNDO/REDO

    public bool addTool(Tool tool)
    {
        return addTool(true, tool);
    }

    public bool addTool(bool execute, Tool tool)
    {
        if (localToolManagers.Count == 0)
        {
            bool added = globalToolManager.addTool(execute, tool);
            /*if( added )
                System.out.println( "[ToolManager] Global Tool Manager: Tool \"" + tool.getToolName( ) + "\" ADDED" );
            else
                System.out.println( "[ToolManager] Global Tool Manager: Tool \"" + tool.getToolName( ) + "\" NOT ADDED" );*/
            return added;
        }
        else {
            bool added = localToolManagers.Peek().addTool(execute, tool);
            /*if( added )
                System.out.println( "[ToolManager] Local Tool Manager: Tool \"" + tool.getToolName( ) + "\" ADDED" );
            else
                System.out.println( "[ToolManager] Local Tool Manager: Tool \"" + tool.getToolName( ) + "\" NOT ADDED" );*/
            return added;
        }
    }

    public void undoTool()
    {
        if (localToolManagers.Count == 0)
        {
            globalToolManager.undoTool();
            //System.out.println( "[ToolManager] Global Tool Manager: Undo Performed" );
        }
        else {
            localToolManagers.Peek().undoTool();
            //System.out.println( "[ToolManager] Local Tool Manager: Undo Performed" );
        }
    }

    public void redoTool()
    {

        if (localToolManagers.Count == 0)
        {
            globalToolManager.redoTool();
            //System.out.println( "[ToolManager] Global Tool Manager: Redo Performed" );
        }
        else {
            localToolManagers.Peek().redoTool();
            //System.out.println( "[ToolManager] Local Tool Manager: Redo Performed" );
        }
    }

    public void pushLocalToolManager()
    {

        localToolManagers.Push(new ToolManager(false));
        //System.out.println( "[ToolManager] Local Tool Manager PUSHED: Total local tool managers = " + localToolManagers.size( ) );
    }

    public void popLocalToolManager()
    {

        if (localToolManagers.Count != 0)
        {
            localToolManagers.Pop();
            //System.out.println( "[ToolManager] Local Tool Manager POPED: Total local tool managers = " + localToolManagers.size( ) );
        }
        else {
            //System.out.println( "[ToolManager] Local Tool Manager Could NOT be POPED: Total local tool managers = " + localToolManagers.size( ) );
        }
    }

    // ONLY FOR DEBUGGING
    /**
     * @return the globalToolManager
     */
    public ToolManager getGlobalToolManager()
    {

        return globalToolManager;
    }
}
