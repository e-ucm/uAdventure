using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    /**
     * Controller that manages a double list of tools for undo/redo
     */
    public class ToolManager
    {

        private List<Tool> undoList;

        private List<Tool> redoList;

        private bool notifyController;

        /**
         * Void and private constructor.
         */
        public ToolManager(bool notifyController)
        {

            undoList = new List<Tool>();
            redoList = new List<Tool>();
            this.notifyController = notifyController;
        }

        public bool addTool(Tool tool)
        {

            return addTool(true, tool);
        }

        public bool addTool(bool execute, Tool tool)
        {

            bool done = execute ? tool.doTool() : true;
            if (done)
            {
                if (undoList.Count == 0)
                    undoList.Add(tool);
                else
                {
                    Tool last = undoList[undoList.Count - 1];
                    if (last.getTimeStamp() < tool.getTimeStamp() - 1500 || !last.combine(tool))
                        undoList.Add(tool);
                }
                redoList.Clear();
                if (notifyController)
                    Controller.Instance.DataModified();

                if (!tool.canUndo())
                {
                    undoList.Clear();
                }
            }
            return done;
        }

        public bool undoTool()
        {

            if (undoList.Count > 0)
            {
                Tool temp = undoList[undoList.Count - 1];
                undoList.RemoveAt(undoList.Count - 1);
                bool undone = temp.undoTool();
                if (undone)
                {
                    if (temp.canRedo())
                        redoList.Add(temp);
                    else
                        redoList.Clear();
                    return true;
                }
            }
            return false;
        }

        public bool redoTool()
        {

            if (redoList.Count > 0)
            {
                Tool temp = redoList[redoList.Count - 1];
                redoList.RemoveAt(redoList.Count - 1);
                bool done = temp.redoTool();
                if (done)
                {
                    undoList.Add(temp);
                    if (!temp.canUndo())
                    {
                        undoList.Clear();
                    }
                }
                return done;
            }
            return false;
        }

        public void clear()
        {

            undoList.Clear();
            redoList.Clear();
        }

        // DEbugging
        /**
         * @return the undoList
         */
        public List<Tool> getUndoList()
        {

            return undoList;
        }

        /**
         * @return the redoList
         */
        public List<Tool> getRedoList()
        {

            return redoList;
        }
    }
}