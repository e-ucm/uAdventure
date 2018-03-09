using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;

namespace uAdventure.Runner
{
    public class ConversationNodeHolder
    {
        private ConversationNode node;
        public int child = -2;
        private EffectHolder additional_effects;
        private EffectHolder end_conversation_effects;


        public ConversationNodeHolder(ConversationNode node)
        {
            this.node = node;

            if (node != null)
            {
                switch (node.getType())
                {
                    case ConversationNodeViewEnum.DIALOGUE:
                        var dialog = node as DialogueConversationNode;
                        this.additional_effects = new EffectHolder(dialog.getEffects());
                        this.child = 0;
                        break;
                    case ConversationNodeViewEnum.OPTION:
                        var option = node as OptionConversationNode;
                        this.additional_effects = new EffectHolder(option.getEffects());
                        this.child = -2;
                        this.showOption = option.isShowUserOption();
                        break;
                }
            }
        }

        private int current_line = 0;
        private bool showOption = false;
        public bool execute()
        {
            bool forcewait = false;
            ConversationLine l;

            if (node != null)
                switch (node.getType())
                {
                    case ConversationNodeViewEnum.DIALOGUE:
                        while (current_line < node.getLineCount() && !forcewait)
                        {
                            l = node.getLine(current_line);
                            if (ConditionChecker.check(l.getConditions()))
                            {
                                forcewait = true;
                                Game.Instance.talk(l.getText(), l.getName());
                            }
                            current_line++;
                        }
                        if (!forcewait && additional_effects != null && additional_effects.effects.Count > 0)
                            forcewait = additional_effects.execute();

                        break;
                    case ConversationNodeViewEnum.OPTION:
                        if (this.child == -2)
                        {
                            Game.Instance.showOptions(this);
                            forcewait = true;
                        }
                        else if (showOption)
                        {
                            l = node.getLine(child);
                            Game.Instance.talk(l.getText(), l.getName());
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
                        break;
                }

            return forcewait;
        }

        public void clicked(int option)
        {
            this.child = option;
            OptionConversationNode onode = ((OptionConversationNode)node);
            if (!string.IsNullOrEmpty(onode.getXApiQuestion()))
            {
                Tracker.T.alternative.Selected(onode.getXApiQuestion(), onode.getLine(option).getText().Replace(",", " "), onode.getLine(option).getXApiCorrect(), AlternativeTracker.Alternative.Question);
                Tracker.T.RequestFlush();
            }
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
                    current = current.getChild();
                    if (current == null)
                        break;
                }
            }

            if (!forcewait)
                this.current = null;

            return forcewait;
        }
    }
}