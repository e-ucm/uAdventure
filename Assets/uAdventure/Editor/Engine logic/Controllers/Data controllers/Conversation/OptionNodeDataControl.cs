using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class OptionNodeDataControl : ConversationNodeDataControl
    {
        private readonly OptionConversationNode optionConversationNode;

        public OptionNodeDataControl(ConversationDataControl conversation, ConversationNode conversationNode) : base(conversation, conversationNode)
        {
            this.optionConversationNode = conversationNode as OptionConversationNode;
        }

        /**
         * Select the position of the options node (top or bottom)
         * 
         * @param selectedNode
         * @param bottomPosition
         *          if true, the option will appear at the bottom, if false, the options will appear at the top
         */
        public void setOptionPositions(bool bottomPosition)
        {
            controller.AddTool(new OptionsPositionTool(optionConversationNode, bottomPosition));
        }

        protected override bool addChild(ConversationNodeDataControl child, ref object data)
        {
            var objectData = data as object[];
            if (objectData != null && objectData.Length == 2)
            {
                // We have to re add the content
                var index = (int)(data as object[])[0];
                var line = (data as object[])[1] as ConversationLineDataControl;

                optionConversationNode.addLine(index, line.getContent() as ConversationLine);
                conversationLines.Insert(index, line);
            }
            else
            {
                // We have to add brand new content
                optionConversationNode.addChild(child.getContent() as ConversationNode);
                var newLine = new ConversationLine(ConversationLine.PLAYER, "");
                optionConversationNode.addLine(newLine);
                conversationLines.Add(new ConversationLineDataControl(newLine));
            }
            return true;
        }

        protected override bool removeChild(ConversationNodeDataControl child, ref object data)
        {
            // Add the child to the given node
            var index = getChilds().IndexOf(child);
            optionConversationNode.removeChild(index);

            var line = data as ConversationLineDataControl;
            if (line == null)
            {
                data = new object[] { index, optionConversationNode.removeLine(index) };
            }

            return true;
        }

        protected override bool canLinkNode(ConversationNodeDataControl child)
        {
            return true;
        }

        protected override void lineAdded(int v, object data)
        {
            var child = data as ConversationNode;
            if(child != null)
            {
                optionConversationNode.addChild(v, child);
            }
        }

        protected override void lineMovedDown(int index)
        {
            var child = optionConversationNode.getChild(index);
            optionConversationNode.removeChild(index);
            optionConversationNode.addChild(index + 1, child);
        }

        protected override void lineMovedUp(int index)
        {
            var child = optionConversationNode.getChild(index);
            optionConversationNode.removeChild(index);
            optionConversationNode.addChild(index - 1, child);
        }

        protected override object lineRemoved(int lineIndex)
        {            
            return optionConversationNode.removeChild(lineIndex);
        }
    }
}
