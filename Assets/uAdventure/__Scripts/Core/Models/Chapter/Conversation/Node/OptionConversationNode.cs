using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This conversational node contains a set of lines, which represent the
     * possible options that the player can choose in a certain point of the
     * conversation. For it's correct use, there must be the same number of lines
     * and children, for each line represents an option, linked with the path the
     * conversation will follow if the option is chosen. Only DialogueNode can be
     * linked with this kind of node
     */

    public class OptionConversationNode : ConversationNode
    {
        /* Attributes */

        /**
         * xApi question
         */
        private string xapiQuestion;

        /**
         * Conversational line's vector
         */
        private List<ConversationLine> options;

        /**
         * Links to the path to follow for each option
         */
        private List<ConversationNode> optionNodes;

        /**
         * Show the options randomly
         */
        private bool random;

        /**
         * Keep the last conversation line showing
         */
        private bool keepShowing;

        /**
         * Show the option selected by user
         */
        private bool showUserOption;

        /**
         * Pre-listening the conversation line
         */
        private bool preListening;

        private float timeout = -1f;

        private Conditions timeoutConditions;

        private ConversationNode timerChild;

        private bool horizontal = false;

        private int maxElemsPerRow = -1;

        private TextAnchor alignment = TextAnchor.UpperCenter;


        /* Methods */

        public bool isRandom()
        {
            return random;
        }

        public void setRandom(bool newValue)
        {

            this.random = newValue;
        }

        /**
         * Constructor
         */
        public OptionConversationNode()
            : this(false, false, false, false)
        {
        }

        /**
         * Constructor
         */

        public OptionConversationNode(bool random, bool keepShowing, bool showUserOption, bool preHearing) : base()
        {
            options = new List<ConversationLine>();
            optionNodes = new List<ConversationNode>();
            this.random = random;
            this.keepShowing = keepShowing;
            this.showUserOption = showUserOption;
            this.preListening = preHearing;
            this.timeout = -1f;
            this.timeoutConditions = new Conditions();
        }


        /*
         * the xApi question for the option node
         * 
         * @return the xApi question for the option node
         */

        public string getXApiQuestion()
        {
            return xapiQuestion;
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.engine.engine.data.conversation.node.Node#isTerminal()
         */

        public override bool isTerminal()
        {

            return false;
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getChildCount()
         */

        public override int getChildCount()
        {

            return optionNodes.Count + (timeout >= 0 ? 1: 0);
        }

        public override ConversationNode getChild(int index)
        {
            return index == optionNodes.Count ? timerChild : optionNodes[index];
        }

        public override void addChild(ConversationNode child)
        {
            if (getLineCount() > optionNodes.Count)
                optionNodes.Add(child);
            else
            {
                timerChild = child;
            }
        }

        public override void addChild(int index, ConversationNode child)
        {
            if(getLineCount() > optionNodes.Count)
            {
                optionNodes.Insert(index, child);
            }
            else
            {
                timerChild = child;
            }
        }

        public override ConversationNode removeChild(int index)
        {
            if(optionNodes.Count == index)
            {
                var tmp = timerChild;
                timerChild = null;
                return tmp;
            }
            else
            {
                ConversationNode item = optionNodes[index];
                optionNodes.RemoveAt(index);
                return item;
            }
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getLineCount()
         */

        public override int getLineCount()
        {
            return options.Count;
        }

        public override ConversationLine getLine(int index)
        {
            return options[index];
        }

        public override void addLine(ConversationLine line)
        {

            options.Add(line);
        }

        public override void addLine(int index, ConversationLine line)
        {

            options.Insert(index, line);
        }

        public override ConversationLine removeLine(int index)
        {
            ConversationLine item = options[index];
            options.RemoveAt(index);
            return item;
        }


        /**
         * Change randomly the position of the options.
         */

        public void doRandom()
        {

            // If option of randomly are activated
            if (random && getLineCount() > 0)
            {
                int cont = getLineCount();
                System.Random rnd = new System.Random();
                int pos;
                List<ConversationLine> op = new List<ConversationLine>();
                List<ConversationNode> opNode = new List<ConversationNode>();
                // Iterate the array and change randomly the position
                while (cont > 1)
                {
                    pos = rnd.Next(cont);
                    op.Add(options[pos]);
                    opNode.Add(optionNodes[pos]);
                    options.RemoveAt(pos);
                    optionNodes.RemoveAt(pos);
                    cont--;

                }
                // It must be out of loop 
                op.Add(options[0]);
                opNode.Add(optionNodes[0]);

                options = op;
                optionNodes = opNode;
            }
        }

        public float Timeout { get { return timeout; } set { timeout = value; } }
        public Conditions TimeoutConditions { get { return timeoutConditions; } set { timeoutConditions = value; } }

        public bool Horizontal
        {
            get
            {
                return horizontal;
            }

            set
            {
                horizontal = value;
            }
        }

        public int MaxElemsPerRow
        {
            get
            {
                return maxElemsPerRow;
            }

            set
            {
                maxElemsPerRow = value;
            }
        }

        public TextAnchor Alignment
        {
            get
            {
                return alignment;
            }

            set
            {
                alignment = value;
            }
        }

        internal override object Clone()
        {
            var ocn = (OptionConversationNode) base.Clone();

            ocn.optionNodes = new List<ConversationNode>();
            ocn.options = options != null ? options.ConvertAll(o => o.Clone() as ConversationLine) : null;

            ocn.random = random;
            ocn.keepShowing = keepShowing;
            ocn.showUserOption = showUserOption;
            ocn.timeout = timeout;
            ocn.timeoutConditions = timeoutConditions.Clone() as Conditions;
            ocn.timerChild = null;

            return ocn;
        }

        /**
         * In that case, return the conditions of the option equals to the given
         * index.
         */

        public override Conditions getLineConditions(int index)
        {

            return options[index].getConditions();
        }

        public override ConversationLine getConversationLine(int index)
        {

            return options[index];
        }

        public bool isKeepShowing()
        {

            return keepShowing;
        }

        /*
         * the xApi question for the option node
         */

        public void setXApiQuestion(string xapiQuestion)
        {
            this.xapiQuestion = xapiQuestion;
        }

        public void setKeepShowing(bool keepShowing)
        {

            this.keepShowing = keepShowing;
        }

        public bool isShowUserOption()
        {

            return showUserOption;
        }

        public void setShowUserOption(bool showUserOption)
        {

            this.showUserOption = showUserOption;
        }

        public bool isPreListening()
        {

            return preListening;
        }

        public void setPreListening(bool preHearing)
        {

            this.preListening = preHearing;
        }

        public void setBottomPosition()
        {
            throw new NotImplementedException();
        }

        public void setTopPosition()
        {
            throw new NotImplementedException();
        }

        public override ConversationNode replaceChild(int index, ConversationNode node)
        {
            if (index > getChildCount())
            {
                throw new Exception("Replacing out of bounds!");
            }

            if (optionNodes.Count == index)
            {
                var previousChild = timerChild;
                timerChild = node;
                return previousChild;
            }
            else
            {
                var previousChild = optionNodes[index];
                optionNodes[index] = node;
                return previousChild;
            }

        }
    }
}