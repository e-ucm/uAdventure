using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChangeDetailedDescriptionTool : Tool
    {


        private Detailed detailed;

        private string description;

        private string oldDescription;

        private Controller controller;

        public ChangeDetailedDescriptionTool(Detailed described, string description)
        {

            this.detailed = described;
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

            if (!description.Equals(detailed.getDetailedDescription()))
            {
                oldDescription = detailed.getDetailedDescription();
                detailed.setDetailedDescription(description);
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

            detailed.setDetailedDescription(description);
            controller.updatePanel();
            return true;
        }


        public override bool undoTool()
        {

            detailed.setDetailedDescription(oldDescription);
            controller.updatePanel();
            return true;
        }


        public override bool combine(Tool other)
        {

            if (other is ChangeDetailedDescriptionTool)
            {
                ChangeDetailedDescriptionTool cnt = (ChangeDetailedDescriptionTool)other;
                if (cnt.detailed == detailed && cnt.oldDescription == description)
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