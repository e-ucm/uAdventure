using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConversationNodeHolder{
    private int lastexecuted = 0;
    private ConversationNode node;
    public int child = -2;
    private EffectHolder additional_effects;
    private EffectHolder end_conversation_effects;


    public ConversationNodeHolder(ConversationNode node){
        this.node = node;

        if (node != null) {
            switch (node.getType ()) {
            case ConversationNodeViewEnum.DIALOGUE:
                DialogueConversationNode dialog = (DialogueConversationNode)node;
                this.additional_effects = new EffectHolder (((DialogueConversationNode)node).getEffects ());
                this.child = 0;
                break;
            case ConversationNodeViewEnum.OPTION:
                this.additional_effects = new EffectHolder (((OptionConversationNode)node).getEffects ());
                this.child = -2;
                break;
            }
        }
    }

    private int current_line = 0;
    public bool execute(){
        bool forcewait = false;
        ConversationLine l;

        if(node!=null)
            switch (node.getType ()) {
            case ConversationNodeViewEnum.DIALOGUE:
                while (current_line < node.getLineCount () && !forcewait) {
                    l = node.getLine (current_line);
                    if(ConditionChecker.check(l.getConditions())){
                        forcewait = true;
                        Game.Instance.talk (l.getText (), l.getName ());
                    }
                    current_line++;
                }
                if(!forcewait && additional_effects != null && additional_effects.effects.Count>0)
                   forcewait = additional_effects.execute ();

                break;
            case ConversationNodeViewEnum.OPTION:
                if (this.child == -2) {
                    Game.Instance.showOptions (this);
                    forcewait = true;
                }else {
                    if (additional_effects != null)
                        forcewait = additional_effects.execute ();
                    else
                        forcewait = false;
                }
                break;
            }

        return forcewait;
    }

    public void clicked(int option) {
        this.child = option;
        OptionConversationNode onode = ((OptionConversationNode)node);
        Tracker.T.alternative.Selected(onode.getXApiQuestion(), onode.getLine(option).getText().Replace(","," "), onode.getLine(option).getXApiCorrect(), AlternativeTracker.Alternative.Question);
        Tracker.T.RequestFlush();
    }

    public ConversationNodeHolder getChild(){
        return ( node!= null) ?new ConversationNodeHolder(node.getChild(this.child)) : null;
    }

    public ConversationNode getNode(){
        return node;
    }
}

public class GraphConversationHolder : Secuence {
    private List<ConversationNodeHolder> nodes;

    public GraphConversationHolder(Conversation conversation){
        this.nodes = new List<ConversationNodeHolder> ();

        foreach (ConversationNode node in conversation.getAllNodes()) {
            nodes.Add (new ConversationNodeHolder(node));
        }
    }

    private ConversationNodeHolder current;
    public bool execute(){
        bool forcewait = false;

        if(current==null)
            current = nodes [0];

        while(!forcewait){
            if(current.execute()){
                forcewait = true;
                break;
            }else{
                current = current.getChild();
                if(current == null)
                    break;
            }
        }

        if(!forcewait)
            this.current = null;

        return forcewait;
    }
}