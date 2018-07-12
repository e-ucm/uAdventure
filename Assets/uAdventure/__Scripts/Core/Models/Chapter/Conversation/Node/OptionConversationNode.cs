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

    public class OptionConversationNode : ConversationNode, ICloneable
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

        private bool effectConsumed = false;

        /**
         * Effect to be triggered when the node has finished
         */
        private Effects effects;

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

        /**
         * The position to be painted the option nodes
         */
        private int x, y;

        private const int DEFAULT_OPTION_NODE_POSITION_X = 10;

        private const int DEFAULT_OPTION_NODE__UPPER_POSITION_Y = 10;

        private const int DEFAULT_OPTION_NODE__BOTTOM_POSITION_Y = 480;

        private float timeout = -1f;

        private Conditions timeoutConditions;

        private ConversationNode timerChild;

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

        public OptionConversationNode(bool random, bool keepShowing, bool showUserOption, bool preHearing, int x, int y)
        {

            options = new List<ConversationLine>();
            optionNodes = new List<ConversationNode>();
            this.random = random;
            this.keepShowing = keepShowing;
            this.showUserOption = showUserOption;
            this.preListening = preHearing;
            this.timeout = -1f;
            this.timeoutConditions = new Conditions();
            // the position of the option has been set, use it
            if (x > 0 && y > 0)
            {
                this.x = x;
                this.y = y;
            } // if the positions hasn't been set, select the bottom position
            else
            {
                this.x = DEFAULT_OPTION_NODE_POSITION_X;
                this.y = DEFAULT_OPTION_NODE__BOTTOM_POSITION_Y;
            }
            effects = new Effects();
        }

        /**
         * Constructor
         */

        public OptionConversationNode()
            : this(false, false, false, false, DEFAULT_OPTION_NODE_POSITION_X, DEFAULT_OPTION_NODE__BOTTOM_POSITION_Y)
        {
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
         * @see es.eucm.eadventure.engine.engine.data.conversation.node.Node#getType()
         */

        public override ConversationNodeViewEnum getType()
        {
            return ConversationNodeViewEnum.OPTION;
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

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#hasEffects()
         */

        public override bool hasEffects()
        {

            return hasValidEffect() && !effects.IsEmpty();
        }

        public override void setEffects(Effects effects)
        {
            this.effects = effects;
        }

        public override Effects getEffects()
        {

            return effects;
        }

        public override void consumeEffect()
        {

            effectConsumed = true;
        }

        public override bool isEffectConsumed()
        {

            return effectConsumed;
        }

        public override void resetEffect()
        {

            effectConsumed = false;
        }

        public override bool hasValidEffect()
        {

            return effects != null;
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

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            OptionConversationNode ocn = (OptionConversationNode) super.clone( );
            ocn.effectConsumed = effectConsumed;
            ocn.effects = ( effects != null ? (Effects) effects.clone( ) : null );
            /*		if (optionNodes != null) {
                        ocn.optionNodes = new List<ConversationNode>();
                        for (ConversationNode cn : optionNodes)
                            ocn.optionNodes.add((ConversationNode) cn.clone());
                    }*/ /*
            ocn.optionNodes = new List<ConversationNode>( );
            if( options != null ) {
                ocn.options = new List<ConversationLine>( );
                for( ConversationLine cl : options )
                    ocn.options.add( (ConversationLine) cl.clone( ) );
            }
    ocn.random = random;
            ocn.keepShowing = keepShowing;
            ocn.showUserOption = showUserOption;
            return ocn;
        }*/

        public override object Clone()
        {
            OptionConversationNode ocn = (OptionConversationNode)base.Clone();
            ocn.effectConsumed = effectConsumed;
            ocn.effects = (effects != null ? (Effects)effects.Clone() : null);
            /*		if (optionNodes != null) {
                        ocn.optionNodes = new ArrayList<ConversationNode>();
                        for (ConversationNode cn : optionNodes)
                            ocn.optionNodes.add((ConversationNode) cn.clone());
                    }*/
            ocn.optionNodes = new List<ConversationNode>();
            if (options != null)
            {
                ocn.options = new List<ConversationLine>();
                foreach (ConversationLine cl in options)
                    ocn.options.Add((ConversationLine)cl.Clone());
            }
            ocn.random = random;
            ocn.keepShowing = keepShowing;
            ocn.showUserOption = showUserOption;
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

        public bool isTopPosition()
        {

            return y == DEFAULT_OPTION_NODE__UPPER_POSITION_Y;
        }

        public bool isBottomPosition()
        {

            return y == DEFAULT_OPTION_NODE__BOTTOM_POSITION_Y;
        }

        public void setTopPosition()
        {

            y = DEFAULT_OPTION_NODE__UPPER_POSITION_Y;
        }

        public void setBottomPosition()
        {

            y = DEFAULT_OPTION_NODE__BOTTOM_POSITION_Y;
        }

        public int getX()
        {

            return x;
        }

        public int getY()
        {

            return y;
        }
        public override ConversationNode replaceChild(int index, ConversationNode node)
        {
            if (index > getChildCount())
                throw new Exception("Replacing out of bounds!");

            optionNodes[index] = node;
            return optionNodes[index] = node;
        }
    }
}