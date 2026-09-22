using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class OptionsPositionTool : Tool
    {

        private bool bottomPosition;

        private OptionConversationNode optionNode;

        public OptionsPositionTool(OptionConversationNode optionNode, bool bottomPosition)
        {
            this.optionNode = optionNode;
            this.bottomPosition = bottomPosition;
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

            if (bottomPosition)
            {
                optionNode.setBottomPosition();
            }
            else
            {
                optionNode.setTopPosition();
            }

            Controller.Instance.updatePanel();
            return true;
        }


        public override bool redoTool()
        {

            return doTool();
        }


        public override bool undoTool()
        {
            if (bottomPosition)
            {
                optionNode.setTopPosition();
            }
            else
            {
                optionNode.setBottomPosition();
            }

            Controller.Instance.updatePanel();
            return true;
        }
    }
}