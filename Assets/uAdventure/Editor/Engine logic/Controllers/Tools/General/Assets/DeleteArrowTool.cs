using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteArrowTool : Tool
    {

        private AdventureData adventureData;

        private CustomArrow arrowDeleted;

        private int index;

        public DeleteArrowTool(AdventureData adventureData, int index)
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

            arrowDeleted = adventureData.getArrows()[index];
            adventureData.getArrows().RemoveAt(index);
            return true;
        }

        public override bool redoTool()
        {

            arrowDeleted = adventureData.getArrows()[index];
            adventureData.getArrows().RemoveAt(index);
            Controller.Instance.updatePanel();
            return true;
        }

        public override bool undoTool()
        {

            adventureData.getArrows().Insert(index, arrowDeleted);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}