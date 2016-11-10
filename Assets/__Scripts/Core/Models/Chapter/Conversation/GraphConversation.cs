using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GraphConversation : Conversation, ICloneable
{
    /**
      * Graph conversation constructor.
      * 
      * @param conversationName
      *            Name of the conversation
      */
    public GraphConversation(string conversationName) : base(Conversation.GRAPH, conversationName, new DialogueConversationNode())
    {
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

    public GraphConversation(TreeConversation conversation) :base(Conversation.GRAPH, conversation.getId(), conversation.getRootNode())
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
    /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

        GraphConversation gc = (GraphConversation) super.clone( );
        return gc;
    }*/

    public override object Clone()
    {
        GraphConversation tc = (GraphConversation)base.Clone();
        return tc;
    }
}
