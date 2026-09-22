using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    public class GraphConversation : Conversation, ICloneable
    {
        protected GraphConversation() : base(Conversation.GRAPH, "GraphConversation" +
                                                                 UnityEngine.Random.Range(100000, 999999),
            new DialogueConversationNode())
        {

        }

        /**
          * Graph conversation constructor.
          * 
          * @param conversationName
          *            Name of the conversation
          */
        public GraphConversation(string conversationName) : base(Conversation.GRAPH, conversationName, new DialogueConversationNode())
        {
            getRootNode().setEditorX(50);
            getRootNode().setEditorY(50);
        }

        /**
         * Graph conversation constructor.
         * 
         * @param conversationName
         *            Name of the conversation
         * @param root
         *            Root of the conversation
         */
        public GraphConversation(string conversationName, ConversationNode root) : base(Conversation.GRAPH, conversationName, root)
        {
        }

        public GraphConversation(TreeConversation conversation) : base(Conversation.GRAPH, conversation.getId(), conversation.getRootNode())
        {
        }

        /**
         * Returns a list with all the nodes in the conversation.
         * 
         * @return List with the nodes of the conversation
         */
        public override List<ConversationNode> getAllNodes()
        {

            List<ConversationNode> nodes = new List<ConversationNode>();

            nodes.Add(getRootNode());
            int i = 0;
            while (i < nodes.Count)
            {
                ConversationNode temp = nodes[i];
                i++;
                for (int j = 0; j < temp.getChildCount(); j++)
                {
                    ConversationNode temp2 = temp.getChild(j);
                    if (!nodes.Contains(temp2))
                        nodes.Add(temp2);
                }
            }

            return nodes;
        }

        public override object Clone()
        {
            GraphConversation tc = (GraphConversation)base.Clone();
            return tc;
        }
    }
}