using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DialogNodeDataControl : ConversationNodeDataControl
    {
        private readonly DialogueConversationNode dialogConversationNode;

        public DialogNodeDataControl(ConversationDataControl conversation, ConversationNode conversationNode) : base(conversation, conversationNode)
        {
            this.dialogConversationNode = conversationNode as DialogueConversationNode;
        }

        protected override bool canLinkNode(ConversationNodeDataControl child)
        {
            return true;
        }

        protected override void lineAdded(int v, object data) {}

        protected override void lineMovedDown(int index) {}

        protected override void lineMovedUp(int index) {}

        protected override object lineRemoved(int lineIndex) { return null; }
        
        protected override bool addChild(ConversationNodeDataControl child, ref object data)
        {
            bool added = false;
            if (getChildCount() < 1 && canLinkNode(child))
            {
                added = true;
                dialogConversationNode.addChild(child.getContent() as ConversationNode);
            }
            return added;
        }

        protected override bool removeChild(ConversationNodeDataControl child, ref object data)
        {
            bool removed = false;
            var childIndex = getChilds().IndexOf(child);

            if (childIndex != -1)
            {
                data = conversationNode.removeChild(childIndex);
            }

            return removed;
        }
    }
}
