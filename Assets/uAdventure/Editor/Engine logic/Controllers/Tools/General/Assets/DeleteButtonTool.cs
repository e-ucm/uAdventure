using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteButtonTool : Tool
    {

        private AdventureData adventureData;

        private CustomButton cursorDeleted;

        private string action;

        private string type;

        private int index;

        public DeleteButtonTool(AdventureData adventureData, string action, string type)
        {

            this.adventureData = adventureData;
            this.action = action;
            this.type = type;
        }

        public override bool canRedo()
        {

            return true;
        }


        public override bool canUndo()
        {

            return cursorDeleted != null;
        }


        public override bool combine(Tool other)
        {

            return false;
        }


        public override bool doTool()
        {

            bool deleted = false;
            CustomButton button = new CustomButton(action, type, null);
            for (int i = 0; i < adventureData.getButtons().Count; i++)
            {
                CustomButton cb = adventureData.getButtons()[i];
                if (cb.Equals(button))
                {
                    cursorDeleted = adventureData.getButtons()[i];
                    adventureData.getButtons().RemoveAt(i);
                    index = i;
                    deleted = true;
                    break;
                }
            }
            return deleted;
        }


        public override bool redoTool()
        {

            adventureData.getButtons().RemoveAt(index);
            Controller.Instance.updatePanel();
            return true;
        }


        public override bool undoTool()
        {
            adventureData.getButtons().Insert(index, cursorDeleted);
            Controller.Instance.updatePanel();
            return true;
        }
    }
}