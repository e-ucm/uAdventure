using System;
using UnityEngine;
using System.Collections;

namespace uAdventure.Core
{
    /**
     * Abstract class that comprises all the possible nodes for a conversation.
     * Initially, two classes implement this interface: DialogueNode and OptionNode
     */
    public abstract class ConversationNode 
    {
        private bool editorCollapsed;
        private int xEditor;
        private int yEditor;
        private int wEditor;
        private int hEditor;
        /**
         * Effect to be triggered when the node has finished
         */
        private Effects effects;

        private bool effectConsumed = false;


        protected ConversationNode()
        {
            this.editorCollapsed = false;
            this.xEditor = -1;
            this.yEditor = -1;
            this.wEditor = 150;
            this.hEditor = 150;
            this.effects = new Effects();
        }

        /**
         * Returns the child in the specified position
         * 
         * @param index
         *            Index for extraction
         * @return The child conversation node selected
         */
        public abstract ConversationNode getChild(int index);

        /**
         * Adds a new child to the node, in the last position
         * 
         * @param child
         *            Node for insertion
         */
        public abstract void addChild(ConversationNode child);

        /**
         * Adds a new child to the node, in the specified position
         * 
         * @param index
         *            Index for insertion
         * @param child
         *            Node for insertion
         */
        public abstract void addChild(int index, ConversationNode child);

        /**
         * Removes the child in the specified position
         * 
         * @param index
         *            Index for removal
         * @return Reference to the removed child
         */
        public abstract ConversationNode removeChild(int index);

        /**
         * Removes the child in the specified position
         * 
         * @param index
         *            Index for removal
         * @return Reference to the removed child
         */
        public abstract ConversationNode replaceChild(int index, ConversationNode node);

        /**
         * Returns the line in the specified position.
         * 
         * @param index
         *            Index for extraction
         * @return The conversation line selected
         */
        public abstract ConversationLine getLine(int index);

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#isPlayerLine(int)
         */
        public bool isPlayerLine(int index)
        {

            return getLine(index).isPlayerLine();
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getLineName(int)
         */
        public string getLineName(int index)
        {

            return getLine(index).getName();
        }

        /*
         * (non-Javadoc)
         * 
         * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getLineText(int)
         */
        public string getLineText(int index)
        {

            return getLine(index).getText();
        }

        /**
         * Adds a new line to the node, in the last position
         * 
         * @param line
         *            Line for insertion
         */
        public abstract void addLine(ConversationLine line);

        /**
         * Adds a new line to the node, in the specified position
         * 
         * @param index
         *            Index for insertion
         * @param line
         *            Line for insertion
         */
        public abstract void addLine(int index, ConversationLine line);

        /**
         * Removes the line in the specified position
         * 
         * @param index
         *            Index for removal
         * @return Reference to the removed line
         */
        public abstract ConversationLine removeLine(int index);


        public bool getEditorCollapsed()
        {
            return editorCollapsed;
        }

        public void setEditorCollapsed(bool editorCollapsed)
        {
            this.editorCollapsed = editorCollapsed;
        }

        public int getEditorX()
        {

            return xEditor;
        }


        public void setEditorX(int xEditor)
        {

            this.xEditor = xEditor;
        }


        public int getEditorY()
        {

            return yEditor;
        }


        public void setEditorY(int yEditor)
        {

            this.yEditor = yEditor;
        }



        public int getEditorWidth()
        {

            return wEditor;
        }


        public void setEditorWidth(int wEditor)
        {

            this.wEditor = wEditor;
        }


        public int getEditorHeight()
        {

            return hEditor;
        }


        public void setEditorHeight(int hEditor)
        {

            this.hEditor = hEditor;
        }

        public virtual bool hasEffects()
        {

            return hasValidEffect() && !effects.IsEmpty();
        }

        /**
         * Sets the effects triggered when the conversation is finished (only
         * terminal nodes accept effects)
         * 
         * @param effects
         *            New effects
         */
        public virtual void setEffects(Effects effects)
        {
            this.effects = effects;
        }

        /**
         * Returns the effect triggered when the conversation is finished
         * 
         * @return The effect held by the node if it is terminal, null otherwise
         */
        public virtual Effects getEffects()
        {
            return effects;
        }

        public virtual void consumeEffect()
        {

            effectConsumed = true;
        }

        public virtual bool isEffectConsumed()
        {

            return effectConsumed;
        }

        public virtual void resetEffect()
        {

            effectConsumed = false;
        }

        public virtual bool hasValidEffect()
        {

            return effects != null;
        }

        public abstract bool isTerminal();
        public abstract int getChildCount();
        public abstract int getLineCount();
        public abstract Conditions getLineConditions(int index);
        public abstract ConversationLine getConversationLine(int index);

        /// <summary>
        /// This method clones the node but leaves the children empty.
        /// This clone must be used only by Conversation, as the children are managed by it.
        /// </summary>
        /// <returns>A cloned node </returns>
        internal virtual object Clone()
        {
            var cn = (ConversationNode)this.MemberwiseClone();
            cn.effectConsumed = effectConsumed;
            cn.effects = effects != null ? (Effects)effects.Clone() : null;
            return cn;
        }
    }
}