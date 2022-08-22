using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System.Linq;
using Xasu.HighLevel;
using Xasu;
using Xasu.Util;

namespace uAdventure.Runner
{
    public class ConversationNodeHolder
    {
        private GraphConversationHolder holder;
        private Conversation conversation;
        private int nodeIndex;
        private string initializedDialogNode;
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
        private StatementPromise trace;
        private float startTime;

        public bool TracePending { get { return isTracePending; } }


        public ConversationNodeHolder(GraphConversationHolder holder, Conversation conversation, ConversationNode node)
        {
            this.holder = holder;
            this.conversation = conversation;
            this.node = node;
            nodeIndex = conversation.getAllNodes().IndexOf(node);

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
                    if (!string.IsNullOrEmpty(initializedDialogNode))
                    {
                        if (GUIManager.Instance.InteractWithDialogue() == InteractuableResult.REQUIRES_MORE_INTERACTION)
                        {
                            forcewait = true;
                            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                            {
                                CompletableTracker.Instance.Progressed(initializedDialogNode, CompletableTracker.CompletableType.DialogFragment, 1f);
                            }
                        }
                        else if(XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                        {
                            CompletableTracker.Instance.Completed(initializedDialogNode, CompletableTracker.CompletableType.DialogFragment);
                            initializedDialogNode = null;
                        }
                    }

                    while (current_line < node.getLineCount() && !forcewait)
                    {
                        l = node.getLine(current_line);
                        if (ConditionChecker.check(l.getConditions()))
                        {
                            forcewait = true;
                            Game.Instance.Talk(l);
                            initializedDialogNode = conversation.getId() + "." + nodeIndex + "." + current_line;
                            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                            {
                                CompletableTracker.Instance.Initialized(initializedDialogNode, CompletableTracker.CompletableType.DialogFragment);
                            }
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

                        var order = Game.Instance.showOptions(this);
                        var orderedOptions = order.Select(o => node.getLine(o).getText()).ToArray();
                        var correct = order.Select(o => node.getLine(o).getXApiCorrect()).ToArray();
                        startTime = Time.realtimeSinceStartup;
                        forcewait = true;
                    }
                    else if (showOption)
                    {
                        l = node.getLine(child);
                        Game.Instance.Talk(l);
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

            if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized && !string.IsNullOrEmpty(onode.getXApiQuestion()))
            {
                holder.EndTracePending();
                isTracePending = true;
                xAPISuccess = onode.getLine(option).getXApiCorrect();
                xAPIQuestion = string.IsNullOrWhiteSpace(onode.getXApiQuestion()) 
                    ? conversation.getId() + "." + conversation.getAllNodes().IndexOf(node) 
                    : onode.getXApiQuestion();
                xAPIResponse = onode.getLine(option).getText().Replace(",", " ");
                trace = AlternativeTracker.Instance.Selected(xAPIQuestion, xAPIResponse, AlternativeTracker.AlternativeType.Question);
                Game.Instance.GameState.BeginChangeAmbit(trace);
                trace.Statement.result.duration = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup - startTime);
                trace.Statement.SetPartial();
                Game.Instance.OnActionCanceled += ActionCancelled;
            }
        }

        public void EndTrace()
        {
            if (trace == null)
            {
                return;
            }

            isTracePending = false;
            trace.WithSuccess(xAPISuccess);
            Game.Instance.GameState.EndChangeAmbitAsExtensions(trace);
            trace.Statement.Complete();
            Game.Instance.OnActionCanceled -= ActionCancelled;
        }

        public ConversationNodeHolder getChild()
        {
            return (node != null) ? new ConversationNodeHolder(holder, conversation, node.getChild(this.child)) : null;
        }

        public ConversationNode getNode()
        {
            return node;
        }

        private void ActionCancelled()
        {
            if (isTracePending)
                EndTrace();
        }
    }

    public class GraphConversationHolder : Secuence
    {
        private List<ConversationNodeHolder> nodes;
        private Conversation conversation;

        public GraphConversationHolder(Conversation conversation)
        {
            this.conversation = conversation;
            this.nodes = new List<ConversationNodeHolder>();

            foreach (ConversationNode node in conversation.getAllNodes())
            {
                nodes.Add(new ConversationNodeHolder(this, conversation, node));
            }
        }

        private ConversationNodeHolder current;
        private static ConversationNodeHolder tracePendingNode;
        public bool execute()
        {
            bool forcewait = false;

            if (current == null)
            {
                current = nodes[0];
                if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                {
                    CompletableTracker.Instance.Initialized(conversation.getId(), CompletableTracker.CompletableType.DialogNode);
                }
            }

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
                        EndTracePending();

                        // And we put this into pending
                        tracePendingNode = current;
                    }

                    current = child;
                    if (current == null)
                    {
                        // When the conversation is over if there's a
                        // pending node, we end it
                        EndTracePending();
                        if (XasuTracker.Instance.Status.State != TrackerState.Uninitialized)
                        {
                            CompletableTracker.Instance.Completed(conversation.getId(), CompletableTracker.CompletableType.DialogNode);
                        }
                        break;
                    }
                }
            }

            if (!forcewait)
                this.current = null;

            return forcewait;
        }

        public void EndTracePending()
        {
            if (tracePendingNode != null && tracePendingNode.TracePending)
                tracePendingNode.EndTrace();
        }
    }
}