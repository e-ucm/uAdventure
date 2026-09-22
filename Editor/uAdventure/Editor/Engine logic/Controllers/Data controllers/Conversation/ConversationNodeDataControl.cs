using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;
using System.Collections.ObjectModel;
using System;

namespace uAdventure.Editor
{
    internal class ConversationNodeDataControlFactory
    {
        private static ConversationNodeDataControlFactory instance;

        public static ConversationNodeDataControlFactory Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ConversationNodeDataControlFactory();
                }
                return instance;
            }
        }

        protected ConversationNodeDataControlFactory()
        {

        }

        public ConversationNodeDataControl CreateDataControlFor(ConversationDataControl conversation, ConversationNode node)
        {
            if(node is DialogueConversationNode)
            {
                return new DialogNodeDataControl(conversation, node);
            }
            else if (node is OptionConversationNode)
            {
                return new OptionNodeDataControl(conversation, node);
            }
            return null;
        }
    }


    public abstract class ConversationNodeDataControl : DataControl
    {

        public const int CONVERSATION_LINE = 649541026;
        private const string ERROR_REMOVING_LINE = "The conversation line couldn't be removed: the line to be " +
                            "removed is either not a ConversationLineDataControl or is not part of the line.";

        private const string CONVERSATIONLINE_DELETETITLE = "ConversationLine.DeleteTitle";

        private const string CONVERSATIONLINE_DELETEMESSAGE = "ConversationLine.DeleteMessage";
        private static readonly int[] addableElements = { CONVERSATION_LINE };

        protected readonly List<ConversationLineDataControl> conversationLines;

        protected readonly ConversationDataControl conversation;
        protected readonly ConversationNode conversationNode;
        protected readonly EffectsController effectsController;
        
        private readonly int[] allNodes = new int[] { Controller.CONVERSATION_DIALOGUE_LINE, Controller.CONVERSATION_OPTION_LINE };

        protected ConversationNodeDataControl(ConversationDataControl conversation, ConversationNode conversationNode)
        {
            this.conversation = conversation;
            this.conversationNode = conversationNode;
            this.effectsController = new EffectsController(conversationNode.getEffects());

            conversationLines = new List<ConversationLineDataControl>();
            for (int i = 0; i < conversationNode.getLineCount(); i++)
            {
                conversationLines.Add(new ConversationLineDataControl(conversationNode.getLine(i)));
            }
        }


        protected override List<Searchable> getPath(Searchable dataControl)
        {
            List<Searchable> path = getPathFromChild(dataControl, conversationLines.Cast<Searchable>().ToList());
            var conversationNodeDataControl = dataControl as ConversationNodeDataControl; 
            if (path == null && conversationNodeDataControl != null && conversationNodeDataControl.conversationNode == conversationNode)
            {
                path = new List<Searchable> { this };
            }
            return path;
        }

        public override void recursiveSearch()
        {
            conversationLines.ForEach(l => l.recursiveSearch());
        }

        public override object getContent()
        {
            return conversationNode;
        }

        public override int[] getAddableElements()
        {
            return addableElements;
        }

        public override bool canAddElement(int type)
        {
            return type == CONVERSATION_LINE;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
        {
            return true;
        }

        public override bool canBeMoved()
        {
            return false;
        }

        public override bool canBeRenamed()
        {
            return false;
        }

        public override bool addElement(int type, string id)
        {
            bool added = false;
            if (type == CONVERSATION_LINE)
            {
                var npcIds = Controller.Instance.IdentifierSummary.getIds<NPC>();
                var targetId = !string.IsNullOrEmpty(id) && npcIds.Contains(id) ? id : Player.IDENTIFIER;
                added = controller.AddTool(new AddRemoveLineTool(this, targetId, ""));
            }
            return added;
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            bool deleted = false;
            var lineDataControl = dataControl as ConversationLineDataControl;
            if (lineDataControl != null && conversationLines.Contains(lineDataControl))
            {
                var deleteTitle = CONVERSATIONLINE_DELETETITLE.Traslate();
                var deleteMessage = CONVERSATIONLINE_DELETEMESSAGE.Traslate();
                if (!askConfirmation || controller.ShowStrictConfirmDialog(deleteTitle, deleteMessage))
                {
                    deleted = controller.AddTool(new AddRemoveLineTool(this, lineDataControl));
                }
            }
            else
            {
                UnityEngine.Debug.LogError(ERROR_REMOVING_LINE);
            }
            return deleted;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            var moved = false;

            var lineDataControl = dataControl as ConversationLineDataControl;
            if (lineDataControl != null)
            {
                moved = controller.AddTool(new MoveLineTool(false, this, lineDataControl));
            }

            return moved;
        }

        public override bool moveElementUp(DataControl dataControl)
            {
            var moved = false;

            var lineDataControl = dataControl as ConversationLineDataControl;
            if (lineDataControl != null)
            {
                moved = controller.AddTool(new MoveLineTool(true, this, lineDataControl));
            }

            return moved;
        }

        protected abstract bool addChild(int index, ConversationNodeDataControl child, ref object data);
        protected abstract bool removeChild(int index, ConversationNodeDataControl child, ref object data);
        protected abstract void lineMovedDown(int index);
        protected abstract void lineMovedUp(int index);
        protected abstract object lineRemoved(int lineIndex);
        protected abstract void lineAdded(int v, object data);
        protected abstract bool canLinkNode(ConversationNodeDataControl child);
        public virtual int[] getAddableNodes() { return allNodes; }

        public override string renameElement(string newName)
        {
            return null;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            conversationLines.ForEach(l => l.updateVarFlagSummary(varFlagSummary));

            EffectsController.updateVarFlagSummary(varFlagSummary, effectsController.getEffectsDirectly());
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {
            return conversationNode.hasValidEffect() &&
                conversationLines.All(l => l.isValid(currentPath, incidences));
        }

        public override int countAssetReferences(string assetPath)
        {
            return conversationLines.Sum(l => l.countAssetReferences(assetPath)) + EffectsController.countAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            conversationLines.ForEach(l => l.getAssetReferences(assetPaths, assetTypes));
            EffectsController.getAssetReferences(assetPaths, assetTypes, effectsController.getEffectsDirectly());
        }

        public override int countIdentifierReferences(string id)
        {
            return conversationLines.Sum(l => l.countIdentifierReferences(id)) + EffectsController.countIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        public override void deleteAssetReferences(string assetPath)
        {
            conversationLines.ForEach(l => l.deleteAssetReferences(assetPath));
            EffectsController.deleteAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            conversationLines.ForEach(l => l.replaceIdentifierReferences(oldId, newId));
            EffectsController.replaceIdentifierReferences(oldId, newId, effectsController.getEffectsDirectly());
        }

        public override void deleteIdentifierReferences(string id)
        {
            conversationLines.ForEach(l => l.deleteIdentifierReferences(id));
            EffectsController.deleteIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        /**
         * Returns the lines
         * 
         * @return The conversation lines
         */
        public ReadOnlyCollection<ConversationNodeDataControl> getChilds()
        {
            var childNodes = new List<ConversationNodeDataControl>();
            for(int i = 0; i < this.conversationNode.getChildCount(); i++)
            {
                var child = this.conversationNode.getChild(i);
                childNodes.Add(conversation.getNodeDataControl(child));
            }

            return childNodes.AsReadOnly();
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return getPathFromChild(dataControl, conversationLines.ConvertAll(l => (object)l));
        }

        public EffectsController getEffects()
        {
            return effectsController;
        }

        public bool getEditorCollapsed()
        {
            return conversationNode.getEditorCollapsed();
        }

        public void setEditorCollapsed(bool editorCollapsed)
        {
            if (editorCollapsed != getEditorCollapsed())
            {
                controller.AddTool(new ChangeBooleanValueTool(conversationNode, editorCollapsed, "getEditorCollapsed", "setEditorCollapsed"));
            }
        }

        public RectInt getEditorRect()
        {
            return new RectInt(conversationNode.getEditorX(), conversationNode.getEditorY(),
                conversationNode.getEditorWidth(), conversationNode.getEditorHeight());
        }

        public void setEditorRect(RectInt rect)
        {
            if(!rect.IsSameRect(getEditorRect()))
            {
                if(Event.current.type != EventType.Used)
                {
                    conversationNode.setEditorX(rect.x);
                    conversationNode.setEditorY(rect.y);
                    conversationNode.setEditorWidth(rect.width);
                    conversationNode.setEditorHeight(rect.height);
                }
                else
                {
                    controller.AddTool(new ChangeNodeRectTool(this, rect));
                }
            }
        }

        /**
         * Returns if the node is terminal (has no children).
         * 
         * @return True if the node is terminal, false otherwise
         */
        public bool isTerminal()
        {
            return conversationNode.isTerminal();
        }

        /**
         * Returns the children's number of the node.
         * 
         * @return The number of children
         */
        public int getChildCount()
        {
            return conversationNode.getChildCount();
        }

        /**
         * Returns the lines' number of the node.
         * 
         * @return The number of lines
         */
        public int getLineCount()
        {
            return conversationNode.getLineCount();
        }

        /**
         * Returns the lines
         * 
         * @return The conversation lines
         */
        public ReadOnlyCollection<ConversationLineDataControl> getLines()
        {
            return conversationLines.AsReadOnly();
        }

        /**
         * Returns the line in the specified position.
         * 
         * @param index
         *            Index for extraction
         * @return The conversation line selected
         */
        public ConversationLineDataControl getLine(int index)
        {
            return conversationLines[index];
        }

        /**
         * Returns if the node has a valid effect set.
         * 
         * @return True if the node has a set of effects, false otherwise
         */
        public bool hasEffects()
        {
            return conversationNode.hasEffects();
        }


        public class ChangeNodeRectTool : Tool
        {
            private readonly RectInt oldPos;
            private RectInt newPos;
            private readonly ConversationNodeDataControl nodeDataControl;

            public ChangeNodeRectTool(ConversationNodeDataControl nodeDataControl, RectInt newPos)
            {
                this.nodeDataControl = nodeDataControl;
                this.oldPos = nodeDataControl.getEditorRect();
                this.newPos = newPos;
            }

            public override bool canRedo() { return true; }

            public override bool canUndo() { return true; }

            public override bool combine(Tool other)
            {
                bool combined = false;
                var otherChange = other as ChangeNodeRectTool;
                if(otherChange != null && otherChange.nodeDataControl.conversationNode == nodeDataControl.conversationNode)
                {
                    this.newPos = otherChange.newPos;
                    combined = true;
                }
                return combined;
            }

            public override bool doTool()
            {
                SetNodeRect(nodeDataControl.conversationNode, newPos);
                return true;
            }

            public override bool redoTool() { return doTool(); }

            public override bool undoTool()
            {
                SetNodeRect(nodeDataControl.conversationNode, oldPos);
                return true;
            }

            private void SetNodeRect(ConversationNode node, RectInt rect)
            {
                node.setEditorX(rect.x);
                node.setEditorY(rect.y);
                node.setEditorWidth(rect.width);
                node.setEditorHeight(rect.height);
            }
        }
        
        private class AddRemoveLineTool : Tool
        {
            private readonly bool isAdd;
            private readonly ConversationNodeDataControl node;
            private readonly ConversationLineDataControl line;
            private object data;
            private int lineIndex = -1;

            public AddRemoveLineTool(ConversationNodeDataControl node, string name, string text)
            {
                this.isAdd = true;
                this.node = node;
                var newLine = new ConversationLine(name, text);
                newLine.addResources(new ResourcesUni());
                this.line = new ConversationLineDataControl(newLine);
            }
            public AddRemoveLineTool(ConversationNodeDataControl node, ConversationLineDataControl toRemove)
            {
                this.isAdd = false;
                this.node = node;
                this.line = toRemove;
            }

            public override bool canRedo() { return true; }

            public override bool canUndo() { return true; }

            public override bool combine(Tool other) { return false; }

            public override bool doTool() { return isAdd ? add() : remove(); }

            public override bool redoTool() { return doTool(); }

            public override bool undoTool() { return isAdd ? remove() : add(); }

            private bool add()
            {
                bool added = false;
                if (!node.conversationLines.Contains(line))
                {
                    if(lineIndex >= 0 && lineIndex < node.conversationLines.Count)
                    {
                        node.conversationNode.addLine(lineIndex, line.getContent() as ConversationLine);
                        node.conversationLines.Insert(lineIndex, line);
                        node.lineAdded(lineIndex, data);
                    }
                    else
                    {
                        node.conversationNode.addLine(line.getContent() as ConversationLine);
                        node.conversationLines.Add(line);
                        node.lineAdded(node.conversationLines.Count - 1, data);
                    }

                    node.Changed();
                    added = true;
                }
                return added;
            }

            private bool remove()
            {
                bool removed = false;
                lineIndex = node.conversationLines.IndexOf(line);
                if (lineIndex != -1)
                {
                    node.conversationNode.removeLine(lineIndex);
                    node.conversationLines.RemoveAt(lineIndex);
                    data = node.lineRemoved(lineIndex);
                    node.Changed();
                    removed = true;
                }
                return removed;
            }
        }

        private class MoveLineTool : Tool
        {
            private int times = 1;
            private readonly bool isUp;
            private readonly ConversationNodeDataControl node;
            private readonly ConversationLineDataControl line;

            public MoveLineTool(bool isUp, ConversationNodeDataControl node, ConversationLineDataControl line)
            {
                this.isUp = isUp;
                this.node = node;
                this.line = line;
            }

            public override bool canRedo() { return true; }

            public override bool canUndo() { return true; }

            public override bool combine(Tool other)
            {
                var combined = false;
                var otherMoveLine = other as MoveLineTool;
                if(otherMoveLine != null && otherMoveLine.isUp == isUp && otherMoveLine.node == node && otherMoveLine.line == line)
                {
                    times++;
                    combined = true;
                }
                return combined;
            }

            public override bool doTool() { return moveElement(isUp); }

            public override bool redoTool() { return doTool(); }

            public override bool undoTool() { return moveElement(!isUp); }

            private bool moveElement(bool up)
            {
                bool moved = false;
                int movement = up ? -1 : 1;

                var lineIndex = node.conversationLines.IndexOf(line);

                for(int i = 0; i< times; i++)
                {
                    if (lineIndex != -1 && lineIndex >= 0 - movement && lineIndex < node.conversationLines.Count - movement)
                    {
                        node.conversationNode.removeLine(lineIndex);
                        node.conversationNode.addLine(lineIndex + movement, line.getContent() as ConversationLine);
                        node.conversationLines.RemoveAt(lineIndex);
                        node.conversationLines.Insert(lineIndex + movement, line);
                        moved = true;

                        if (up)
                        {
                            node.lineMovedUp(lineIndex);
                        }
                        else
                        {
                            node.lineMovedDown(lineIndex);
                        }
                        lineIndex += movement;
                        node.Changed();
                    }
                }

                return moved;
            }
        }

        public class AddRemoveConversationNodeTool : Tool
        {
            protected ConversationNodeDataControl parent;
            protected ConversationNodeDataControl childDataControl;
            protected ConversationLineDataControl line;

            protected int index = -1;
            protected const int DIALOG_NODE = Controller.CONVERSATION_DIALOGUE_LINE;
            protected const int OPTION_NODE = Controller.CONVERSATION_OPTION_LINE;

            private object data;

            private bool add = false;
            public AddRemoveConversationNodeTool(ConversationNodeDataControl parent, ConversationNodeDataControl child, int index)
            {
                this.parent = parent;
                this.childDataControl = child;
                this.index = index;
                this.add = true;
            }

            public AddRemoveConversationNodeTool(int nodeType, ConversationNodeDataControl parent, int index)
            {
                this.parent = parent;

                ConversationNode child = null;

                if (nodeType == DIALOG_NODE)
                {
                    child = new DialogueConversationNode();
                }
                else if (nodeType == OPTION_NODE)
                {
                    child = new OptionConversationNode();
                }

                if (child != null)
                {
                    var parentRect = parent.getEditorRect();
                    child.setEditorX(parentRect.x + parentRect.width + 35);
                    child.setEditorY(parentRect.y);

                    this.childDataControl = parent.conversation.getNodeDataControl(child);
                    this.index = index; // Insert at last
                    this.add = true;
                }
            }

            public AddRemoveConversationNodeTool(int nodeType, ConversationNodeDataControl parent) : this(nodeType, parent, parent.getChildCount())
            {
            }

            public AddRemoveConversationNodeTool(ConversationNodeDataControl parent, ConversationNodeDataControl childDataControl)
            {
                this.parent = parent;
                this.childDataControl = childDataControl;
                this.index = parent.getChilds().IndexOf(childDataControl); // Remove at index
            }

            public AddRemoveConversationNodeTool(ConversationNodeDataControl parent, int index)
            {
                this.parent = parent;
                this.childDataControl = parent.getChilds()[index];
                this.index = index; // Remove at index
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool canUndo()
            {
                return index != -1 && childDataControl != null;
            }

            public override bool combine(Tool other)
            {
                return false;
            }


            public override bool doTool()
            {
                var doTool = index != -1 && childDataControl != null;
                if (doTool)
                {
                    if(add)
                    {
                        doTool = parent.addChild(index, childDataControl, ref data);
                    }
                    else
                    {
                        doTool = parent.removeChild(index, childDataControl, ref data);
                    }
                    if (doTool)
                    {
                        parent.Changed();
                    }
                }

                return doTool;
            }


            public override bool redoTool()
            {
                return doTool();
            }

            public override bool undoTool()
            {
                add = !add;
                var result = doTool();
                add = !add;
                return result;
            }
        }
        
        public class LinkConversationNodeTool : Tool
        {
            private readonly ConversationNodeDataControl father, child;
            private readonly Controller controller;

            private ConversationNode previousChild;

            private readonly int index = -1;

            protected const int DIALOG_NODE = Controller.CONVERSATION_DIALOGUE_LINE;
            protected const int OPTION_NODE = Controller.CONVERSATION_OPTION_LINE;
            private bool nodeLinked, nodeAdded;
            private object data;

            public LinkConversationNodeTool(ConversationDataControl conversation, ConversationNodeDataControl father, int nodeType, int index)
            {
                this.father = father;
                ConversationNode childNode = null;
                if (nodeType == DIALOG_NODE)
                {
                    childNode = new DialogueConversationNode();
                }
                else if (nodeType == OPTION_NODE)
                {
                    childNode = new OptionConversationNode();
                }
                else
                {
                    throw new Exception("Not valid node type!");
                }
                var parentRect = this.father.getEditorRect();
                childNode.setEditorX(parentRect.x + parentRect.width + 35);
                childNode.setEditorY(parentRect.y);

                child = this.father.conversation.getNodeDataControl(childNode);

                this.father = father;
                this.controller = Controller.Instance;
                this.index = index;
            }

            public LinkConversationNodeTool(ConversationDataControl conversation, ConversationNodeDataControl father, ConversationNodeDataControl child, int index)
            {
                this.father = father;
                this.child = child;
                this.controller = Controller.Instance;
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
                nodeLinked = false;
                nodeAdded = false;

                // If it is not possible to link the node to the given one, show a message
                if (!father.canLinkNode(child))
                {
                    controller.ShowErrorDialog(TC.get("Conversation.OperationLinkNode"), TC.get("Conversation.ErrorLinkNode"));
                }
                // If it can be linked
                else if (index >= 0 && index < father.getChilds().Count && father.getChilds()[index] != child)
                {
                    nodeLinked = true;
                    var node = father.getContent() as ConversationNode;
                    previousChild = node.replaceChild(index, child.getContent() as ConversationNode);
                    father.Changed();
                }
                else if(index == father.getChilds().Count)
                {
                    nodeAdded = true;
                    father.addChild(index, child, ref data);
                    father.Changed();
                }

                return nodeLinked || nodeAdded;
            }

            public override bool redoTool()
            {
                return (nodeLinked || nodeAdded) && doTool();
            }


            public override bool undoTool()
            {
                if (nodeLinked)
                {
                    var node = father.getContent() as ConversationNode;
                    node.replaceChild(index, previousChild);
                    father.Changed();
                }
                else if(nodeAdded)
                {
                    father.removeChild(index, child, ref data);
                    father.Changed();
                }
                return true;
            }
        }


        public class DeleteNodeLinkTool : Tool
        {
            protected readonly ConversationNodeDataControl parent;
            protected readonly ConversationNodeDataControl child;
            protected readonly int linkIndex;
            protected ConversationNode linkDeleted;
            protected readonly string confirmTitle;
            protected readonly string confirmText;
            protected object data;

            public DeleteNodeLinkTool(ConversationNodeDataControl parent) : this(parent, 0)
            {
            }


            public DeleteNodeLinkTool(ConversationNodeDataControl parent, int index)
            {
                this.parent = parent;
                this.linkIndex = index;
                this.confirmTitle = TC.get("Conversation.OperationDeleteLink");
                this.confirmText = TC.get("Conversation.ConfirmationDeleteLink");
                this.child = parent.getChilds()[linkIndex];
            }


            public override bool canRedo()
            {
                return true;
            }


            public override bool canUndo()
            {
                return linkDeleted != null;
            }


            public override bool combine(Tool other)
            {

                return false;
            }


            public override bool doTool()
            {
                // Ask for confirmation
                if (Controller.Instance.ShowStrictConfirmDialog(confirmTitle, confirmText) && 
                    parent.removeChild(linkIndex, child, ref data))
                { 
                    linkDeleted = child.getContent() as ConversationNode;
                    parent.Changed();
                    return true;
                }
                return false;
            }


            public override bool redoTool()
            {
                var removed = parent.removeChild(linkIndex, child, ref data);
                if (removed)
                {
                    parent.Changed();
                }
                return removed;
            }


            public override bool undoTool()
            {
                var added = parent.addChild(linkIndex, child, ref data);
                if (added)
                {
                    parent.Changed();
                }
                return added;
            }
        }
    }
}