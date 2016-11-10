using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This conversational node contains a variable number of dialogue lines,
 * between player characters and non-player characters. This node has a single
 * link to another node, of any kind
 */

public class DialogueConversationNode : ConversationNode, ICloneable
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
     * Indicates if the node is terminal or not
     */
    private bool terminal;

    /**
     * Effect to be triggered when the node has finished (if it's terminal)
     */
    private Effects effects;

    private bool effectConsumed = false;

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
        terminal = true;
        effects = new Effects();
        this.keepShowing = waitUserInteraction;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#getType()
     */

    public override ConversationNodeViewEnum getType()
    {

        return ConversationNodeViewEnum.DIALOGUE;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#isTerminal()
     */

    public override bool isTerminal()
    {

        return terminal;
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
        terminal = false;
        //TODO MODIFIED
        //effects.clear( );
    }

    public override void addChild(int index, ConversationNode child)
    {

        if (index != 0 || nextNode != null)
            throw new System.Exception();

        nextNode = child;
        terminal = false;
        //TODO MODIFIED
        //effects.clear( );
    }

    public override ConversationNode removeChild(int index)
    {

        if (index != 0 || nextNode == null)
            throw new System.Exception();

        ConversationNode deletedChild = nextNode;
        nextNode = null;
        terminal = true;
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
        // WORKAROUND
        ConversationLine tmp = dialogue[index];
        dialogue.RemoveAt(index);
        return tmp;
    }

    /*
     * (non-Javadoc)
     * 
     * @see es.eucm.eadventure.common.data.chapterdata.conversation.node.ConversationNodeView#hasEffects()
     */

    public override bool hasEffects()
    {

        return hasValidEffect() && !effects.isEmpty();
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

    /*
    @Override
    public Object clone() throws CloneNotSupportedException
    {

        DialogueConversationNode dcn = (DialogueConversationNode) super.clone( );
        if( dialogue != null ) {
            dcn.dialogue = new List<ConversationLine>();
            for (ConversationLine cl : dialogue)
                dcn.dialogue.add((ConversationLine)cl.clone());
        }
        dcn.effectConsumed = effectConsumed;
        dcn.effects = ( effects != null ? (Effects) effects.clone( ) : null );
        //dcn.nextNode = (nextNode != null ? (ConversationNode) nextNode.clone() : null);
        dcn.nextNode = null;
        dcn.terminal = terminal;
        dcn.keepShowing = keepShowing;
        return dcn;
    }*/

    public override object Clone()
    {
        DialogueConversationNode dcn = (DialogueConversationNode) base.Clone();
        if (dialogue != null)
        {
            dcn.dialogue = new List<ConversationLine>();
            foreach (ConversationLine cl in dialogue)
                dcn.dialogue.Add((ConversationLine) cl.Clone());
        }
        dcn.effectConsumed = effectConsumed;
        dcn.effects = (effects != null ? (Effects) effects.Clone() : null);
        //dcn.nextNode = (nextNode != null ? (ConversationNode) nextNode.clone() : null);
        dcn.nextNode = null;
        dcn.terminal = terminal;
        dcn.keepShowing = keepShowing;
        return dcn;
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

}