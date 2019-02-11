using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public abstract class ConversationDataControl : DataControl
    {
        protected Conversation conversation;

        private readonly Dictionary<ConversationNode, ConversationNodeDataControl> dataControls;

        public ConversationDataControl(Conversation conversation)
        {
            this.conversation = conversation;
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
        public abstract string getId();

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
        public abstract bool linkNode(ConversationNodeDataControl fatherView, ConversationNodeDataControl childView);

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
        public abstract bool deleteNode(ConversationNodeDataControl nodeView);

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
        public bool deleteNodeLink(ConversationNodeDataControl nodeView)
        {

            return controller.AddTool(new DeleteNodeLinkTool(nodeView));
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
    }
}