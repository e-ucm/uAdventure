using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChapterToolManager
    {

        /**
        * Global tool manager. For undo/redo in main window (real changes in the
        * data structure)
        */
        private readonly ToolManager globalToolManager;

        /**
         * Local tool managers. For undo/redo in dialogs (This will only reflect
         * temporal changes, not real changes in data)
         */
        private readonly Stack<ToolManager> localToolManagers;

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
                return globalToolManager.addTool(execute, tool);
            }
            else
            {
                return localToolManagers.Peek().addTool(execute, tool);
            }
        }

        public void undoTool()
        {
            if (localToolManagers.Count == 0)
            {
                globalToolManager.undoTool();
                Debug.Log("[ToolManager] Global Tool Manager: Undo Performed" );
            }
            else
            {
                localToolManagers.Peek().undoTool();
                Debug.Log("[ToolManager] Local Tool Manager: Undo Performed" );
            }
        }

        public void redoTool()
        {

            if (localToolManagers.Count == 0)
            {
                globalToolManager.redoTool();
                Debug.Log("[ToolManager] Global Tool Manager: Redo Performed" );
            }
            else
            {
                localToolManagers.Peek().redoTool();
                Debug.Log("[ToolManager] Local Tool Manager: Redo Performed" );
            }
        }

        public void pushLocalToolManager()
        {

            localToolManagers.Push(new ToolManager(false));
            Debug.Log("[ToolManager] Local Tool Manager PUSHED: Total local tool managers = " + localToolManagers.Count);
        }

        public void popLocalToolManager()
        {

            if (localToolManagers.Count != 0)
            {
                localToolManagers.Pop();
                Debug.Log("[ToolManager] Local Tool Manager POPED: Total local tool managers = " + localToolManagers.Count);
            }
            else
            {
                Debug.Log("[ToolManager] Local Tool Manager Could NOT be POPED: Total local tool managers = " + localToolManagers.Count);
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
}