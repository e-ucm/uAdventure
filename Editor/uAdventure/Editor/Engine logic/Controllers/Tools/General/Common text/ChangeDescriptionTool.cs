using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeDescriptionTool : Tool
    {

        private Described described;

        private string description;

        private string oldDescription;

        private Controller controller;

        public ChangeDescriptionTool(Described described, string description)
        {

            this.described = described;
            this.description = description;
            this.controller = Controller.Instance;
        }

        public override bool canRedo()
        {

            return true;
        }

        public override bool canUndo()
        {
            return true;
        }

        public override bool doTool()
        {

            if (!description.Equals(described.getDescription()))
            {
                oldDescription = described.getDescription();
                described.setDescription(description);
                return true;
            }
            return false;
        }

        public override string getToolName()
        {

            return "Change description";
        }

        public override bool redoTool()
        {

            described.setDescription(description);
            controller.updatePanel();
            return true;
        }

        public override bool undoTool()
        {

            described.setDescription(oldDescription);
            controller.updatePanel();
            return true;
        }

        public override bool combine(Tool other)
        {

            if (other is ChangeDescriptionTool)
            {
                ChangeDescriptionTool cnt = (ChangeDescriptionTool)other;
                if (cnt.described == described && cnt.oldDescription == description)
                {
                    description = cnt.description;
                    timeStamp = cnt.timeStamp;
                    return true;
                }
            }
            return false;
        }
    }
}