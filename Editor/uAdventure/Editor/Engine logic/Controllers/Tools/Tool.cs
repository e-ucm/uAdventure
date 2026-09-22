using System;
using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public abstract class Tool
    {

        /**
         * Stores the time when the tool was created
         */
        protected long timeStamp = Environment.TickCount;

        protected bool doesClone = false;

        public bool GetDoesClone()
        {

            return doesClone;
        }

        public void SetDoesClone(bool doesClone)
        {
            this.doesClone = doesClone;
        }

        /**
         * Get the time when the tool was created
         * 
         * @return The time when the tool was created
         */
        public long getTimeStamp()
        {

            return timeStamp;
        }

        /**
         * Returns the tool name
         * 
         * @return the tool name
         */
        public virtual String getToolName()
        {

            return Language.GetText(this.GetType().Name);
        }

        /**
         * Do the actual work. Returns true if it could be done, false in other
         * case.
         * 
         * @return True if the tool was applied correctly
         */
        public abstract bool doTool();

        /**
         * Returns true if the tool can be undone
         * 
         * @return True if the tool can be undone
         */
        public abstract bool canUndo();

        /**
         * Undo the work done by the tool. Returns true if it could be undone, false
         * in other case.
         * 
         * @return True if the tool was undone correctly
         */
        public abstract bool undoTool();

        /**
         * Returns true if the tool can be redone
         * 
         * @return True if the tool can be redone
         */
        public abstract bool canRedo();

        /**
         * Re-do the work done by the tool before it was undone.
         * 
         * @return True if the tool was re-done correctly
         */
        public abstract bool redoTool();

        /**
         * Combines this tool with other similar tool (if possible). Useful for
         * combining simple changes as characters typed in the same field
         * 
         * @param other
         * @return
         */
        public abstract bool combine(Tool other);
    }
}