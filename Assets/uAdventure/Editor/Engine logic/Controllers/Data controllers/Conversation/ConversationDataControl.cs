using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;
using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class ConversationDataControl : DataControl
    {
        protected Conversation conversation;

        private readonly Dictionary<ConversationNode, ConversationNodeDataControl> dataControls;

        protected ConversationDataControl(Conversation conversation)
        {
            this.conversation = conversation;
            dataControls = new Dictionary<ConversationNode, ConversationNodeDataControl>();
            dataControls.Add(conversation.getRootNode(), ConversationNodeDataControlFactory.Instance
                .CreateDataControlFor(this, conversation.getRootNode()));
        }

        public ConversationNodeDataControl getNodeDataControl(ConversationNode node)
        {
            if (!dataControls.ContainsKey(node))
            {
                dataControls.Add(node, ConversationNodeDataControlFactory.Instance
                    .CreateDataControlFor(this, node));
            }

            return dataControls[node];
        }

        /**
        * Returns the type of the contained conversation.
        * 
        * @return Type of the contained conversation
        */
        public abstract int getType();

        /**
         * Returns the id of the contained conversation.
         * 
         * @return Id of the contained conversation
         */
        public virtual string getId()
        {
            return conversation.getId();
        }

        /**
         * Returns the id of the contained conversation.
         * 
         * @return Id of the contained conversation
         */
        public virtual void setId(string newId)
        {
            controller.AddTool(new ChangeIdTool(conversation, newId));
        }

        /**
         * Returns the root node of the conversation.
         * 
         * @return Root node
         */
        public abstract ConversationNodeDataControl getRootNode();

        /**
         * Sets the rootNode
         * 
         * @return Root node
         */
        public abstract void setRootNode();

        /**
         * Returns the number of lines that has the conversation.
         * 
         * @return Number of lines of the conversation
         */
        public abstract int getConversationLineCount();

        /**
         * Returns the types of nodes which can be added to the given node
         * 
         * @param nodeView
         *            Node which we want to know what kind of node can be added
         * @return Array of node types that can be added
         */
        public abstract int[] getAddableNodes(ConversationNodeDataControl nodeView);

        /**
         * Returns if it is possible to add a child to the given node
         * 
         * @param nodeView
         *            Node which we want to know if a child is addable
         * @param nodeType
         *            The type of node that we want to add
         * @return True if a child can be added (get NodeTypes with
         *         getAddeableNodes( ConversationalNode )), false otherwise
         */
        public abstract bool canAddChild(ConversationNodeDataControl nodeView, int nodeType);

        /**
         * Returns if it is possible to delete the given node
         * 
         * @param nodeView
         *            Node to be deleted
         * @return True if the node can be deleted, false otherwise
         */
        public abstract bool canDeleteNode(ConversationNodeDataControl nodeView);

        /**
         * Returns if it is possible to move the given node
         * 
         * @param nodeView
         *            Node to be moved
         * @return True if the node initially can be moved, false otherwise
         */
        public abstract bool canMoveNode(ConversationNodeDataControl nodeView);

        /**
         * Returns if it is possible to move the given node to a child position of
         * the given host node
         * 
         * @param nodeView
         *            Node to be moved
         * @param hostNodeView
         *            Node that will act as host
         * @return True if node can be moved as a child of host node, false
         *         otherwise
         */
        public abstract bool canMoveNodeTo(ConversationNodeDataControl nodeView, ConversationNodeDataControl hostNodeView);

        /**
         * Links the two given nodes, as father and child
         * 
         * @param fatherView
         *            Father node (first selected node)
         * @param childView
         *            Child node (second selected node)
         * @return True if the nodes had been successfully linked, false otherwise
         */
        public virtual bool addNode(ConversationNodeDataControl fatherView, int nodeType)
        {
            return controller.AddTool(new ConversationNodeDataControl.AddRemoveConversationNodeTool(nodeType, fatherView));
        }
        /**
         * Links the two given nodes, as father and child
         * 
         * @param fatherView
         *            Father node (first selected node)
         * @param childView
         *            Child node (second selected node)
         * @return True if the nodes had been successfully linked, false otherwise
         */
        public virtual bool addNode(ConversationNodeDataControl fatherView, int nodeType, int position)
        {
            return controller.AddTool(new ConversationNodeDataControl.AddRemoveConversationNodeTool(nodeType, fatherView, position));
        }

        /**
         * Links the two given nodes, as father and child
         * 
         * @param fatherView
         *            Father node (first selected node)
         * @param childView
         *            Child node (second selected node)
         * @return True if the nodes had been successfully linked, false otherwise
         */
        public abstract bool linkNode(ConversationNodeDataControl fatherView, int nodeType, int position);

        /**
         * Links the two given nodes, as father and child
         * 
         * @param fatherView
         *            Father node (first selected node)
         * @param childView
         *            Child node (second selected node)
         * @return True if the nodes had been successfully linked, false otherwise
         */
        public abstract bool linkNode(ConversationNodeDataControl fatherView, ConversationNodeDataControl childView, int position);

        /**
         * Deletes the given node in the conversation
         * 
         * @param nodeView
         *            Node to be deleted
         * @return True if the node was successfully deleted, false otherwise
         */
        public virtual bool deleteNode(ConversationNodeDataControl nodeView)
        {
            return controller.AddTool(new DeleteNodeTool(this, nodeView));
        }

        private class DeleteNodeTool : Tool
        {
            private readonly List<ConversationNodeDataControl.AddRemoveConversationNodeTool> subTools;
            private readonly ConversationNodeDataControl toRemove;
            private readonly ConversationDataControl content;
            private bool isRoot = false;

            public DeleteNodeTool(ConversationDataControl content, ConversationNodeDataControl node)
            {
                this.content = content;
                this.toRemove = node;
                this.subTools = CreateTools();
            }

            private List<ConversationNodeDataControl.AddRemoveConversationNodeTool> CreateTools()
            {

                if (toRemove.getChildCount() > 1 && !Controller.Instance.ShowStrictConfirmDialog("Forbidden!", "Deleting this node will keep only the first child (the rest nodes will be deleted!). Continue?"))
                {
                    return null;
                }

                if (toRemove == content.getRootNode())
                {
                    if (toRemove.getChildCount() == 0)
                    {
                        Controller.Instance.ShowErrorDialog("Forbidden!", "Deleting the last node is forbidden!");
                        return null;
                    }

                    isRoot = true;
                }


                var tools = from node in content.getAllNodes()
                            from child in node.getChilds().Select((n, i) => new { n, i })
                            orderby child.i descending
                            where child.n == toRemove
                            select new ConversationNodeDataControl.AddRemoveConversationNodeTool(node, child.i);

                return tools.ToList();
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
                if(subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(conversation.getRootNode().getChild(0));
                }

                return subTools.All(t => t.doTool());
            }

            public override bool redoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(conversation.getRootNode().getChild(0));
                }

                return subTools.All(t => t.redoTool());
            }

            public override bool undoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(toRemove.getContent() as ConversationNode);
                }

                subTools.Reverse();
                var result = subTools.All(t => t.undoTool());
                subTools.Reverse();

                return result;
            }
        }


        /**
         * Moves the given node to a child position of the given host node
         * 
         * @param nodeView
         *            Node to be moved
         * @param hostNodeView
         *            Node that will act as host
         * @return True if the node was succesfully moved, false otherwise
         */
        public abstract bool moveNode(ConversationNodeDataControl nodeView, ConversationNodeDataControl hostNodeView);

        /**
         * Default getter for the data contained
         * 
         * @return The conversation
         */
        public abstract Conversation getConversation();

        /**
         * Default setter
         * 
         * @param conversation
         * @return
         */
        public abstract void setConversation(Conversation conversation);

        public abstract void updateAllConditions();
        

        /**
         * Deletes the link with the child node. This method should only be used
         * with dialogue nodes (to delete the link with their only child). For
         * option nodes, the method <i>deleteNodeOption</i> should be used instead.
         * 
         * @param nodeView
         *            Dialogue node to delete the link
         * @return True if the link was deleted, false otherwise
         */
        public bool deleteNodeLink(ConversationNodeDataControl node)
        {

            return controller.AddTool(new ConversationNodeDataControl.DeleteNodeLinkTool(node));
        }

        /**
         * Deletes the link with the child node. This method should only be used
         * with dialogue nodes (to delete the link with their only child). For
         * option nodes, the method <i>deleteNodeOption</i> should be used instead.
         * 
         * @param nodeView
         *            Dialogue node to delete the link
         * @return True if the link was deleted, false otherwise
         */
        public bool deleteNodeLink(ConversationNodeDataControl node, int index)
        {

            return controller.AddTool(new ConversationNodeDataControl.DeleteNodeLinkTool(node,index));
        }

        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override bool canAddElement(int type)
        {

            return false;
        }


        public override bool canBeDeleted()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
        }


        public override bool canBeRenamed()
        {

            return true;
        }


        public override bool addElement(int type, string id)
        {

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return getAllNodes().All(n => n.isValid(currentPath, incidences));
        }

        public abstract List<ConversationNodeDataControl> getAllNodes();



        private static int countNodeReferences(ConversationNodeDataControl root, ConversationNodeDataControl lookup)
        {
            return countNodeReferences(root, lookup, new List<ConversationNodeDataControl> { root });
        }

        private static int countNodeReferences(ConversationNodeDataControl actual, ConversationNodeDataControl lookup, List<ConversationNodeDataControl> visited)
        {
            var i = 0;
            if (!visited.Contains(actual))
            {
                visited.Add(actual);
                foreach (var child in actual.getChilds())
                {
                    if (child == lookup || child.getContent() == lookup.getContent())
                    {
                        i++;
                    }

                    i += countNodeReferences(child, lookup, visited);
                }

            }
            return i;
        }

        public virtual bool replaceNode(ConversationNodeDataControl oldNode, ConversationNodeDataControl newNode)
        {
            if (newNode != null && oldNode != null && oldNode != newNode)
            {
                return controller.AddTool(new ReplaceNodeTool(this, oldNode, newNode));
            }
            return false;
        }

        public virtual bool insertNode(ConversationNodeDataControl parent, int nodeType, int index)
        {
            return controller.AddTool(new InsertNodeTool(this, parent, nodeType, index));
        }

        private class InsertNodeTool : Tool
        {
            protected const int DIALOG_NODE = Controller.CONVERSATION_DIALOGUE_LINE;
            protected const int OPTION_NODE = Controller.CONVERSATION_OPTION_LINE;

            private readonly List<Tool> subTools;
            private readonly ConversationNodeDataControl parent, newNode;
            private readonly ConversationDataControl content;
            private readonly int index;
            private readonly bool isRootNode;
            private readonly ConversationNodeDataControl oldRoot;

            public InsertNodeTool(ConversationDataControl content, ConversationNodeDataControl parent, int nodeType, int index)
            {
                this.content = content;
                this.isRootNode = parent == null;
                this.parent = isRootNode ? content.getRootNode() : parent;
                ConversationNode node = null;
                switch (nodeType)
                {
                    case DIALOG_NODE: node = new DialogueConversationNode(); break;
                    case OPTION_NODE: node = new OptionConversationNode(); break;
                }


                this.index = index;
                var parentRect = isRootNode ? new RectInt(0, 25, 0, 0) : parent.getEditorRect();
                var childRect = isRootNode ? content.getRootNode().getEditorRect() : parent.getChilds()[index].getEditorRect();

                var center = (parentRect.center + childRect.center) / 2f;

                node.setEditorX((int)(center.x - node.getEditorWidth() / 2f));
                node.setEditorY((int)(center.y - node.getEditorHeight() / 2f));

                this.newNode = ConversationNodeDataControlFactory.Instance.CreateDataControlFor(content, node);
                this.subTools = CreateTools();
            }

            private List<Tool> CreateTools()
            {
                var tools = new List<Tool>();
                
                if (!isRootNode)
                {
                    tools.Add(new ConversationNodeDataControl.LinkConversationNodeTool(content, parent, newNode, index));
                }

                if (isRootNode || parent.getChildCount() > 0)
                {
                    tools.Add(new ConversationNodeDataControl.AddRemoveConversationNodeTool(newNode, isRootNode ? parent : parent.getChilds()[index], 0));
                }

                return tools;
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
                if (subTools == null)
                {
                    return false;
                }

                if (isRootNode)
                {
                    var conversation = content.getContent() as Conversation;
                    conversation.setRootNode(newNode.getContent() as ConversationNode);
                }

                return subTools.All(t => t.doTool());
            }

            public override bool redoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                if (isRootNode)
                {
                    var conversation = content.getContent() as Conversation;
                    conversation.setRootNode(newNode.getContent() as ConversationNode);
                }

                return subTools.All(t => t.redoTool());
            }

            public override bool undoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                subTools.Reverse();
                var result = subTools.All(t => t.undoTool());
                subTools.Reverse();


                if (isRootNode)
                {
                    var conversation = content.getContent() as Conversation;
                    conversation.setRootNode(parent.getContent() as ConversationNode);
                }

                return result;
            }
        }


        private class ReplaceNodeTool : Tool
        {
            private readonly List<Tool> subTools;
            private readonly ConversationNodeDataControl oldNode, newNode;
            private readonly ConversationDataControl content;
            private bool isRoot = false;

            public ReplaceNodeTool(ConversationDataControl content, ConversationNodeDataControl oldNode, ConversationNodeDataControl newNode)
            {
                this.content = content;
                this.oldNode = oldNode;
                this.newNode = newNode;
                this.subTools = CreateTools();
            }

            private List<Tool> CreateTools()
            {

                if (oldNode.getChildCount() > 1 && !Controller.Instance.ShowStrictConfirmDialog("Forbidden!", "Replacing this node will keep only the first child (the rest nodes will be deleted!). Continue?"))
                {
                    return null;
                }

                var tools = from node in content.getAllNodes()
                            from child in node.getChilds().Select((n, i) => new { n, i })
                            where child.n == oldNode
                            select (Tool) new ConversationNodeDataControl.LinkConversationNodeTool(content, node, newNode, child.i);

                var toolsList = tools.ToList();

                if (oldNode.getChildCount() > 0)
                {
                    toolsList.Add(new ConversationNodeDataControl.AddRemoveConversationNodeTool(newNode, oldNode.getChilds()[0], 0));
                }

                if (oldNode == content.getRootNode())
                {
                    isRoot = true;
                }

                return toolsList;
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
                if (subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(newNode.getContent() as ConversationNode);
                }

                return subTools.All(t => t.doTool());
            }

            public override bool redoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(conversation.getRootNode().getChild(0));
                }

                return subTools.All(t => t.redoTool());
            }

            public override bool undoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                if (isRoot)
                {
                    var conversation = content.getConversation();
                    conversation.setRootNode(oldNode.getContent() as ConversationNode);
                }

                subTools.Reverse();
                var result = subTools.All(t => t.undoTool());
                subTools.Reverse();

                return result;
            }
        }

        public virtual bool moveNodes(ConversationNodeDataControl grabbed, List<ConversationNodeDataControl> selection, Vector2Int delta)
        {
            return controller.AddTool(new MoveNodesTool(this, grabbed, selection, delta));
        }

        private class MoveNodesTool : Tool
        {
            private readonly ConversationNodeDataControl grabbed;
            private readonly Dictionary<ConversationNodeDataControl, Tool> subTools;
            private readonly ConversationDataControl content;

            public MoveNodesTool(ConversationDataControl content, ConversationNodeDataControl grabbed, List<ConversationNodeDataControl> selection, Vector2Int alpha)
            {
                this.content = content;
                this.grabbed = grabbed;
                this.subTools = CreateTools(selection, alpha);
            }

            private Dictionary<ConversationNodeDataControl, Tool> CreateTools(List<ConversationNodeDataControl> selection, Vector2Int alpha)
            {
                var tools = new Dictionary<ConversationNodeDataControl, Tool>();

                selection.ForEach(n =>
                {
                    var rect = n.getEditorRect();
                    rect.position += alpha;
                    tools.Add(n, new ConversationNodeDataControl.ChangeNodeRectTool(n, rect));
                });

                return tools;
            }

            public override bool canRedo()
            {
                return subTools.All(kv => kv.Value.canRedo());
            }

            public override bool canUndo()
            {
                return subTools.All(kv => kv.Value.canUndo());
            }

            public override bool combine(Tool other)
            {
                var combined = false;
                var otherMove = other as MoveNodesTool;
                if(otherMove != null && grabbed == otherMove.grabbed && ContainSameKeys(subTools, otherMove.subTools))
                {
                    foreach(var kvSubTool in otherMove.subTools)
                    {
                        if (!subTools[kvSubTool.Key].combine(kvSubTool.Value))
                        {
                            return false;
                        }
                    }
                    combined = true;
                }

                return combined;
            }

            bool ContainSameKeys(Dictionary<ConversationNodeDataControl, Tool> a, Dictionary<ConversationNodeDataControl, Tool> b)
            {
                return a.Keys.Count == b.Keys.Count && a.Keys.All(k => b.ContainsKey(k));
            }

            public override bool doTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                return subTools.All(kv => kv.Value.doTool());
            }

            public override bool redoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                return subTools.All(kv => kv.Value.redoTool());
            }

            public override bool undoTool()
            {
                if (subTools == null)
                {
                    return false;
                }

                subTools.Reverse();
                var result = subTools.All(kv => kv.Value.undoTool());
                subTools.Reverse();

                return result;
            }
        }
    }

}