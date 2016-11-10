using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Implements Tree and Graph conversations
 */

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
        Conversation c = (Conversation) this.MemberwiseClone();
        c.conversationId = (conversationId != null ? conversationId : null);
        c.conversationType = conversationType;

        Dictionary<ConversationNode, ConversationNode> clonedNodes =
            new Dictionary<ConversationNode, ConversationNode>();

        c.root = (root != null ? (ConversationNode) root.Clone() : null);

        clonedNodes.Add(root, c.root);
        List<ConversationNode> nodes = new List<ConversationNode>();
        List<ConversationNode> visited = new List<ConversationNode>();
        nodes.Add(root);

        while (nodes.Count > 0)
        {
            ConversationNode temp = nodes[0];
            ConversationNode cloned = clonedNodes[temp];
            nodes.RemoveAt(0);
            visited.Add(temp);

            for (int i = 0; i < temp.getChildCount(); i++)
            {
                ConversationNode tempCloned = clonedNodes[temp.getChild(i)];
                if (tempCloned == null)
                {
                    tempCloned = (ConversationNode) temp.getChild(i).Clone();
                    clonedNodes.Add(temp.getChild(i), tempCloned);
                }
                cloned.addChild(tempCloned);

                if (!visited.Contains(temp.getChild(i)) && !nodes.Contains(temp.getChild(i)))
                    nodes.Add(temp.getChild(i));
            }
        }
        return c;
    }

    /*
@Override
public Object clone() throws CloneNotSupportedException
{

   Conversation c = (Conversation) super.clone( );
   c.conversationId = ( conversationId != null ? new string(conversationId ) : null );
   c.conversationType = conversationType;

   HashMap<ConversationNode, ConversationNode> clonedNodes = new HashMap<ConversationNode, ConversationNode>();

c.root = ( root != null ? (ConversationNode) root.clone( ) : null );

   clonedNodes.put( root, c.root );
   List<ConversationNode> nodes = new List<ConversationNode>();
List<ConversationNode> visited = new List<ConversationNode>();
nodes.add( root );

   while( !nodes.isEmpty( ) ) {
       ConversationNode temp = nodes.get(0);
ConversationNode cloned = clonedNodes.get(temp);
nodes.remove( 0 );
       visited.add( temp );

       for( int i = 0; i<temp.getChildCount( ); i++ ) {
           ConversationNode tempCloned = clonedNodes.get(temp.getChild(i));
           if( tempCloned == null ) {
               tempCloned = (ConversationNode) temp.getChild( i ).clone();
clonedNodes.put( temp.getChild( i ), tempCloned );
           }
cloned.addChild( tempCloned );

           if( !visited.contains( temp.getChild( i ) ) && !nodes.contains( temp.getChild( i ) ) )
               nodes.add( temp.getChild( i ) );
       }
   }
   return c;
}*/
}