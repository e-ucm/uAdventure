using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * Implements Tree and Graph conversations
     */
    [GroupableType]
    public class Conversation : HasId, ICloneable
    {

        /**
         * Constant for tree conversations.
         */
        public const int TREE = 0;

        /**
         * Constant for graph conversations.
         */
        public const int GRAPH = 1;

        /* Attributes */

        /**
         * Type of the conversation.
         */
        private int conversationType;

        /**
         * Reference name of the conversation
         */
        private string conversationId;

        /**
         * Root of the conversation
         */
        private ConversationNode root;

        /* Methods */

        /**
         * Constructor
         * 
         * @param conversationType
         *            Type of the conversation
         * @param conversationId
         *            Identifier of the conversation
         * @param root
         *            Root node (start) of the conversation
         */

        //Activator purposes with ITypeGroupable
        private Conversation() { }

        protected Conversation(int conversationType, string conversationId, ConversationNode root)
        {

            this.conversationType = conversationType;
            this.conversationId = conversationId;
            this.root = root;
        }

        /**
         * Returns the type of the conversation.
         * 
         * @return Conversation's type
         */

        public int getType()
        {

            return conversationType;
        }

        /**
         * Returns the name of the conversation.
         * 
         * @return Conversation's name
         */

        public string getId()
        {

            return conversationId;
        }

        /**
         * Returns the initial node of the conversation, the one which starts the
         * conversation.
         * 
         * @return First node of the conversation
         */

        public ConversationNode getRootNode()
        {

            return root;
        }

        public void setRootNode(ConversationNode node)
        {
            root = node;
        }

        /**
         * Sets the a new identifier for the conversation.
         * 
         * @param id
         *            New identifier
         */

        public void setId(string id)
        {

            this.conversationId = id;
        }

        public virtual List<ConversationNode> getAllNodes()
        {

            List<ConversationNode> nodes = new List<ConversationNode>();
            getAllNodes(root, nodes);
            return nodes;
        }

        private void getAllNodes(ConversationNode firstNode, List<ConversationNode> nodes)
        {

            for (int i = -1; i < firstNode.getChildCount(); i++)
            {
                ConversationNode child = null;
                if (i == -1)
                    child = firstNode;
                else
                    child = firstNode.getChild(i);
                // Check the child is not in the list yet
                bool isInList = false;
                foreach (ConversationNode aNode in nodes)
                {
                    if (aNode == child)
                    {
                        isInList = true;
                        break;
                    }

                }
                if (!isInList)
                {
                    nodes.Add(child);
                    getAllNodes(child, nodes);
                }
            }
        }

        public virtual object Clone()
        {
            var c = (Conversation)this.MemberwiseClone();
            c.conversationId = conversationId;
            c.conversationType = conversationType;

            if (root == null)
            {
                c.root = null;
                return c;
            }

            c.root = (ConversationNode)root.Clone();

            var clonedNodes = new Dictionary<ConversationNode, ConversationNode> {{root, c.root}};
            var nodes = new Queue<ConversationNode>();
            nodes.Enqueue(root);

            while (nodes.Count > 0)
            {
                var original = nodes.Dequeue();
                var clone = clonedNodes[original];

                for (var i = 0; i < original.getChildCount(); i++)
                {
                    var child = original.getChild(i);
                    // If the child has not been cloned yet
                    if (child != null && !clonedNodes.ContainsKey(child))
                    {
                        // We must create the clone
                        var clonedChild = (ConversationNode)child.Clone();
                        clonedNodes.Add(child, clonedChild);
                        
                        // And we must clone its children in next iterations
                        nodes.Enqueue(child);
                    }

                    // And then we add the child to the current node
                    clone.addChild(child != null ? clonedNodes[child] : null);
                }
            }
            return c;
        }
    }
}