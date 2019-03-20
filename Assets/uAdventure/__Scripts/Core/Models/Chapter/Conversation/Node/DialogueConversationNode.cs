using System;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This conversational node contains a variable number of dialogue lines,
     * between player characters and non-player characters. This node has a single
     * link to another node, of any kind
     */

    public class DialogueConversationNode : ConversationNode
    {
        /* Attributes */

        /**
         * Conversational line's vector
         */
        private List<ConversationLine> dialogue;

        /**
         * Link to the next node
         */
        private ConversationNode nextNode;

        /**
         * Store if stop the line until the user decides.
         */
        private bool keepShowing;

        /* Methods */

        /**
         * Constructor
         */

        public DialogueConversationNode() : this(false)
        {
        }

        public DialogueConversationNode(bool waitUserInteraction)
        {

            dialogue = new List<ConversationLine>();
            nextNode = null;
            this.keepShowing = waitUserInteraction;
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#isTerminal()
         */

        public override bool isTerminal()
        {

            return nextNode == null;
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getChildCount()
         */

        public override int getChildCount()
        {

            int childCount = 0;

            if (nextNode != null)
                childCount++;

            return childCount;
        }

        public override ConversationNode getChild(int index)
        {

            if (index != 0)
                throw new System.Exception();

            return nextNode;
        }

        public override void addChild(ConversationNode child)
        {

            if (nextNode != null)
                throw new System.Exception();

            nextNode = child;
        }

        public override void addChild(int index, ConversationNode child)
        {
            if (index != 0 || nextNode != null)
                throw new System.Exception();

            nextNode = child;
        }

        public override ConversationNode removeChild(int index)
        {

            if (index != 0 || nextNode == null)
                throw new System.Exception();

            ConversationNode deletedChild = nextNode;
            nextNode = null;
            return deletedChild;
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getLineCount()
         */

        public override int getLineCount()
        {

            return dialogue.Count;
        }

        public override ConversationLine getLine(int index)
        {

            return dialogue[index];
        }

        public override void addLine(ConversationLine line)
        {

            dialogue.Add(line);
        }

        public override void addLine(int index, ConversationLine line)
        {

            dialogue.Insert(index, line);
        }

        public override ConversationLine removeLine(int index)
        {
            ConversationLine tmp = dialogue[index];
            dialogue.RemoveAt(index);
            return tmp;
        }

        internal override object Clone()
        {
            var clone = (DialogueConversationNode) base.Clone();

            clone.dialogue = (dialogue != null) ? dialogue.ConvertAll(l => l.Clone() as ConversationLine) : null;
            clone.nextNode = null;
            clone.keepShowing = keepShowing;

            return clone;
        }

        public override Conditions getLineConditions(int index)
        {

            return dialogue[index].getConditions();
        }

        public override ConversationLine getConversationLine(int index)
        {

            return dialogue[index];
        }


        public bool isKeepShowing()
        {

            return keepShowing;
        }


        public void setKeepShowing(bool keepShowing)
        {

            this.keepShowing = keepShowing;
        }

        public override ConversationNode replaceChild(int index, ConversationNode node)
        {
            if (index > 0)
            {
                throw new Exception("You can only replace the first child");
            }
            var previousChild = nextNode;
            nextNode = node;
            return previousChild;
        }
    }
}