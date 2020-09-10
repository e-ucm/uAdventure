using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using AssetPackage;

namespace uAdventure.Runner
{
    public class ConversationNodeHolder
    {
        private ConversationNode node;
        public int child = -2;
        private EffectHolder additional_effects;
        private EffectHolder end_conversation_effects;
        private int current_line = 0;
        private bool showOption = false;
        private bool isTracePending = false;
        private string xAPIQuestion;
        private string xAPIResponse;
        private bool xAPISuccess;

        public bool TracePending { get { return isTracePending; } }


        public ConversationNodeHolder(ConversationNode node)
        {
            this.node = node;

            if (node != null)
            {
                if(node is DialogueConversationNode)
                {
                    var dialog = node as DialogueConversationNode;
                    this.additional_effects = new EffectHolder(dialog.getEffects());
                    this.child = 0;
                }
                else if (node is OptionConversationNode)
                {
                    var option = node as OptionConversationNode;
                    this.additional_effects = new EffectHolder(option.getEffects());
                    this.child = -2;
                    this.showOption = option.isShowUserOption();
                }
            }
        }

        public bool execute()
        {
            bool forcewait = false;
            ConversationLine l;

            if (node != null)
            {

                if (node is DialogueConversationNode)
                {
                    while (current_line < node.getLineCount() && !forcewait)
                    {
                        l = node.getLine(current_line);
                        if (ConditionChecker.check(l.getConditions()))
                        {
                            forcewait = true;
                            Game.Instance.Talk(l, l.getName());
                        }
                        current_line++;
                    }

                    if (!forcewait && additional_effects != null && additional_effects.effects.Count > 0)
                    {
                        forcewait = additional_effects.execute();
                    }
                }
                else if (node is OptionConversationNode)
                {
                    if (this.child == -2)
                    {
                        if (isTracePending)
                        {
                            EndTrace();
                        }

                        Game.Instance.showOptions(this);
                        forcewait = true;
                    }
                    else if (showOption)
                    {
                        l = node.getLine(child);
                        Game.Instance.Talk(l, l.getName());
                        forcewait = true;
                        showOption = false;
                    }
                    else
                    {
                        if (additional_effects != null)
                            forcewait = additional_effects.execute();
                        else
                            forcewait = false;
                    }
                }
            }

            return forcewait;
        }

        public void clicked(int option)
        {
            this.child = option;
            OptionConversationNode onode = ((OptionConversationNode)node);
            if(option == onode.getChildCount()) // Timeout
            {
                // TODO Analytics for timeout
                return;
            }

            if (TrackerAsset.Instance.Started && !string.IsNullOrEmpty(onode.getXApiQuestion()))
            {
                isTracePending = true;
                Game.Instance.GameState.BeginChangeAmbit();
                xAPISuccess = onode.getLine(option).getXApiCorrect();
                xAPIQuestion = onode.getXApiQuestion();
                xAPIResponse = onode.getLine(option).getText().Replace(",", " ");
            }
        }

        public void EndTrace()
        {
            if (!TrackerAsset.Instance.Started)
            {
                return;
            }

            Game.Instance.GameState.EndChangeAmbitAsExtensions();
            TrackerAsset.Instance.setSuccess(xAPISuccess);
            TrackerAsset.Instance.Alternative.Selected(xAPIQuestion, xAPIResponse, AlternativeTracker.Alternative.Question);
            TrackerAsset.Instance.Flush();
        }

        public ConversationNodeHolder getChild()
        {
            return (node != null) ? new ConversationNodeHolder(node.getChild(this.child)) : null;
        }

        public ConversationNode getNode()
        {
            return node;
        }
    }

    public class GraphConversationHolder : Secuence
    {
        private List<ConversationNodeHolder> nodes;

        public GraphConversationHolder(Conversation conversation)
        {
            this.nodes = new List<ConversationNodeHolder>();

            foreach (ConversationNode node in conversation.getAllNodes())
            {
                nodes.Add(new ConversationNodeHolder(node));
            }
        }

        private ConversationNodeHolder current;
        private ConversationNodeHolder tracePendingNode;
        public bool execute()
        {
            bool forcewait = false;

            if (current == null)
                current = nodes[0];

            while (!forcewait)
            {
                if (current.execute())
                {
                    forcewait = true;
                    break;
                }
                else
                {
                    ConversationNodeHolder child = current.getChild();
                    if (current.TracePending)
                    {
                        // If the current node has a pending trace
                        // We end the previous trace pending
                        if (tracePendingNode != null)
                            tracePendingNode.EndTrace();

                        // And we put this into pending
                        tracePendingNode = current;
                    }

                    current = child;
                    if (current == null)
                    {
                        // When the conversation is over if there's a
                        // pending node, we end it
                        if(tracePendingNode != null)
                            tracePendingNode.EndTrace();
                        break;
                    }
                }
            }

            if (!forcewait)
                this.current = null;

            return forcewait;
        }
    }
}