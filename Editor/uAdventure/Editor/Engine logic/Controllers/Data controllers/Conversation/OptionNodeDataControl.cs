using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Editor
{
    public class OptionNodeDataControl : ConversationNodeDataControl
    {
        private readonly OptionConversationNode optionConversationNode;
        private readonly ConditionsController timeoutConditions;

        public float Timeout
        {
            get
            {
                return optionConversationNode.Timeout;
            }
            set
            {
                value = value > 0 ? value : -1;
                if(value != Timeout)
                {
                    controller.AddTool(new ChangeTimeoutTool(this, value));
                }
            }
        }

        public OptionNodeDataControl(ConversationDataControl conversation, ConversationNode conversationNode) : base(conversation, conversationNode)
        {
            this.optionConversationNode = conversationNode as OptionConversationNode;
            this.timeoutConditions = new ConditionsController(optionConversationNode.TimeoutConditions);
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            base.replaceIdentifierReferences(oldId, newId);
            timeoutConditions.replaceIdentifierReferences(oldId, newId);
        }

        public override int countIdentifierReferences(string id)
        {
            return base.countIdentifierReferences(id) + timeoutConditions.countIdentifierReferences(id);
        }

        public override void deleteIdentifierReferences(string id)
        {
            base.deleteIdentifierReferences(id);
            timeoutConditions.deleteIdentifierReferences(id);
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

        public bool KeepShowing
        {
            get { return optionConversationNode.isKeepShowing(); }
            set
            {
                if(value != KeepShowing)
                {
                    controller.AddTool(new ChangeBooleanValueTool(optionConversationNode, value, "isKeepShowing", "setKeepShowing"));
                }
            }
        }

        public bool ShowUserOption
        {
            get { return optionConversationNode.isShowUserOption(); }
            set
            {
                if (value != ShowUserOption)
                {
                    controller.AddTool(new ChangeBooleanValueTool(optionConversationNode, value, "isShowUserOption", "setShowUserOption"));
                }
            }
        }

        public bool Random
        {
            get { return optionConversationNode.isRandom(); }
            set
            {
                if (value != Random)
                {
                    controller.AddTool(new ChangeBooleanValueTool(optionConversationNode, value, "isRandom", "setRandom"));
                }
            }
        }

        public ConditionsController TimeoutConditions {
            get
            {
                return timeoutConditions;
            }
        }

        protected override bool addChild(int index, ConversationNodeDataControl child, ref object data)
        {
            var line = data as ConversationLineDataControl;

            // If i receive a line its because I removed a line before, so I have to put the line making a new slot for the child
            if (line != null)
            {
                optionConversationNode.addLine(index, line.getContent() as ConversationLine);
                conversationLines.Insert(index, line);
                optionConversationNode.addChild(index, child.getContent() as ConversationNode);
            }
            // Otherwise push the current child to grandchild preserving the current line, except when adding in the end
            else
            {
                var timeout = data != null ? (float) data : 0f;
                if (timeout > 0)
                {
                    // Reinsert the timeout child
                    optionConversationNode.Timeout = timeout;
                    optionConversationNode.addChild(child.getContent() as ConversationNode);
                }
                else
                {
                    if (optionConversationNode.getChildCount() == index)
                    {
                        // We have to add brand new content
                        var newLine = new ConversationLine(ConversationLine.PLAYER, "");
                        optionConversationNode.addLine(newLine);
                        conversationLines.Add(new ConversationLineDataControl(newLine));
                        optionConversationNode.addChild(child.getContent() as ConversationNode);
                    }
                    else
                    {
                        // I have to push the current child to a grandchild position
                        var grandChild = optionConversationNode.getChild(index);
                        var newChild = child.getContent() as ConversationNode;
                        // Set the child
                        optionConversationNode.replaceChild(index, newChild);

                        // Set the grandchild (if the node contains other childs, these will be preserved)
                        if (newChild.getChildCount() == 0)
                        {
                            newChild.addChild(grandChild);
                        }
                        else
                        {
                            newChild.replaceChild(0, grandChild);
                        }
                    }
                }
            }
            return true;
        }

        protected override bool removeChild(int index, ConversationNodeDataControl child, ref object data)
        {
            // Add the child to the given 
            if (child.getChildCount() > 0 && canLinkNode(child.getChilds()[0]))
            {
                // We set the niece as child
                optionConversationNode.replaceChild(index, child.getChilds()[0].getContent() as ConversationNode);
            }
            else
            {

                if(optionConversationNode.Timeout > 0 && index == optionConversationNode.getChildCount() - 1)
                {
                    data = optionConversationNode.Timeout;

                    // We remove the timeout child
                    optionConversationNode.removeChild(index);
                    optionConversationNode.Timeout = -1;
                }
                else
                {
                    var line = data as ConversationLineDataControl;
                    if (line != null && line != conversationLines[index])
                    {
                        UnityEngine.Debug.Log("The line your're trying to remove is not in the same position as the child.");
                    }

                    data = conversationLines[index];
                    conversationLines.RemoveAt(index);
                    optionConversationNode.removeLine(index);

                    // We remove the option
                    optionConversationNode.removeChild(index);
                }
            }

            return true;
        }

        protected override bool canLinkNode(ConversationNodeDataControl child)
        {
            return child != this;
        }

        protected override void lineAdded(int v, object data)
        {
            var child = data as ConversationNode;
            if(child == null)
            {
                child = new DialogueConversationNode();
                var childPos = GetNewChildPosition();
                child.setEditorX(childPos.x);
                child.setEditorY(childPos.y);
            }
            optionConversationNode.addChild(v, child);
        }

        private Vector2Int GetNewChildPosition()
        {
            var myRect = this.getEditorRect();
            var x = myRect.x + myRect.width + 20;
            var y = getChildCount() == 0 ? myRect.y : getChilds().Max(c => c.getEditorRect().y + c.getEditorRect().height + 20);
            return new Vector2Int(x, y);
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



        /**
         * Sets if the option is correct
         * 
         * @param if the option is correct
         */

        public string getXApiQuestion()
        {
            return optionConversationNode.getXApiQuestion();
        }

        /**
         * Sets if the option is correct
         * 
         * @param if the option is correct
         */

        public void setXApiQuestion(string xApiQuestion)
        {
            controller.AddTool(new ChangeStringValueTool(optionConversationNode, xApiQuestion, "getXApiQuestion", "setXApiQuestion"));
        }

        private class ChangeTimeoutTool : Tool
        {
            private readonly OptionNodeDataControl optionNode;
            private float newValue;
            private readonly float oldValue;
            private readonly bool isOnlyValueChange;
            private readonly ConversationNodeDataControl child;

            public ChangeTimeoutTool(OptionNodeDataControl optionNode, float newValue)
            {
                this.optionNode = optionNode;
                this.oldValue = optionNode.Timeout;
                this.newValue = newValue;
                this.isOnlyValueChange = oldValue > 0 && newValue > 0;

                if(oldValue > 0)
                {
                    child = optionNode.getChilds().Last();
                }
                else
                {
                    var newChild = new DialogueConversationNode();
                    var childPos = optionNode.GetNewChildPosition();
                    newChild.setEditorX(childPos.x);
                    newChild.setEditorY(childPos.y);
                    child = optionNode.conversation.getNodeDataControl(newChild);
                }
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
                bool combined = false;

                var otherChange = other as ChangeTimeoutTool;
                if (otherChange != null && isOnlyValueChange && otherChange.isOnlyValueChange)
                {
                    this.newValue = otherChange.newValue;
                    combined = true;
                }

                return combined;
            }

            public override bool doTool()
            {
                bool done = false;
                if(newValue != oldValue)
                {
                    ChangeValue(oldValue, newValue);
                    done = true;
                }
                return done;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool undoTool()
            {
                bool done = false;
                if (newValue != oldValue)
                {
                    // Reverse the values
                    ChangeValue(newValue, oldValue);
                    done = true;
                }
                return done;
            }

            private void ChangeValue(float oldTimeout, float newTimeout)
            {
                var node = optionNode.getContent() as OptionConversationNode;
                if (oldTimeout <= 0 && newTimeout > 0)
                {
                    node.addChild(child.getContent() as ConversationNode);
                }
                else if (oldTimeout > 0 && newTimeout <= 0)
                {
                    node.removeChild(node.getLineCount());
                }

                node.Timeout = newTimeout;
            }
        }
    }
}
