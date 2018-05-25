using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteCursorTool : Tool
    {

        private AdventureData adventureData;

        private CustomCursor cursorDeleted;

        private int index;

        public DeleteCursorTool(AdventureData adventureData, int index)
        {

            this.adventureData = adventureData;
            this.index = index;
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

            cursorDeleted = adventureData.getCursors()[index];
            adventureData.getCursors().RemoveAt(index);
            return true;
        }

        public override bool redoTool()
        {

            cursorDeleted = adventureData.getCursors()[index];
            adventureData.getCursors().RemoveAt(index);
            Controller.Instance.updatePanel();
            return true;
        }

        public override bool undoTool()
        {
            adventureData.getCursors().Insert(index, cursorDeleted);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}