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
            return child != this;
        }

        protected override void lineAdded(int v, object data) {}

        protected override void lineMovedDown(int index) {}

        protected override void lineMovedUp(int index) {}

        protected override object lineRemoved(int lineIndex) { return null; }
        
        protected override bool addChild(int index, ConversationNodeDataControl child, ref object data)
        {
            bool added = false;
            if(index == 0 && canLinkNode(child))
            {
                added = true;
                if (getChildCount() < 1)
                {
                    dialogConversationNode.addChild(child.getContent() as ConversationNode);
                }
                else
                {
                    var grandchild = conversationNode.getChild(0);
                    var newChild = child.getContent() as ConversationNode;
                    dialogConversationNode.replaceChild(index, newChild);
                    if(newChild.getChildCount() > 0)
                    {
                        newChild.replaceChild(0, grandchild);
                    }
                    else
                    {
                        newChild.addChild(grandchild);
                        UnityEngine.Debug.Log("A child was added without checking consistence");
                    }
                }
            }
            return added;
        }

        protected override bool removeChild(int index, ConversationNodeDataControl child, ref object data)
        {
            bool removed = false;

            if (index == 0 && conversationNode.getChildCount() > 0)
            {
                if(conversationNode.getChild(index).getChildCount() > 0 && canLinkNode(child.getChilds()[0]))
                {
                    // We set the niece as child
                    data = conversationNode.replaceChild(index, conversationNode.getChild(index).getChild(0));
                }
                else
                {
                    data = conversationNode.removeChild(index);
                }
                removed = true;
            }

            return removed;
        }
    }
}
