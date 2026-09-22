using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    public class TreeConversation : Conversation, ICloneable
    {

        /**
         * Tree conversation constructor.
         * 
         * @param conversationName
         *            Name of the conversation
         */
        public TreeConversation(string conversationName) : base(Conversation.TREE, conversationName, new DialogueConversationNode())
        {
        }

        /**
         * Tree conversation constructor.
         * 
         * @param conversationName
         *            Name of the conversation
         * @param root
         *            Root of the conversation
         */
        public TreeConversation(string conversationName, ConversationNode root) : base(Conversation.TREE, conversationName, root)
        {
        }

        /**
         * Checks if there is a "go-back" tag in the given node. This is, if the
         * node is a DialogueNode, and is linked to the OptionNode from which came
         * from
         * 
         * @param node
         *            Node (must be a DialogueNode) to check
         * @return True if the node has a "go-back" tag, false otherwise
         */
        public static bool thereIsGoBackTag(ConversationNode node)
        {

            bool goBackTag = false;

            // Perform the check only if the node is a DialogueNode and it has a child
            if (node is DialogueConversationNode && node.getChildCount() > 0)
            {
                ConversationNode possibleFather = node.getChild(0);

                // For each child of the possible father node, check if it match with the possible child
                for (int i = 0; i < possibleFather.getChildCount(); i++)
                    if (possibleFather.getChild(i) == node)
                        goBackTag = true;
            }

            return goBackTag;
        }
        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            TreeConversation tc = (TreeConversation) super.clone( );
            return tc;
        }*/

        public override object Clone()
        {
            TreeConversation tc = (TreeConversation)base.Clone();
            return tc;
        }
    }
}