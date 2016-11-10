using UnityEngine;
using System.Collections;

public class DeleteNodeLinkTool : Tool {

    protected ConversationNode parent;

    protected int linkIndex;

    protected ConversationNode linkDeleted;

    protected string confirmTitle;

    protected string confirmText;

    public DeleteNodeLinkTool(ConversationNodeView nodeView):this((ConversationNode)nodeView)
    { 
    }

    public DeleteNodeLinkTool(ConversationNode parent)
    {

        this.parent = parent;
        this.linkIndex = 0;
        this.confirmTitle = TC.get("Conversation.OperationDeleteLink");
        this.confirmText = TC.get("Conversation.ConfirmationDeleteLink");
    }

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return linkDeleted != null;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        // Ask for confirmation
        if (Controller.getInstance().showStrictConfirmDialog(confirmTitle, confirmText))
        {
            linkDeleted = parent.removeChild(linkIndex);
            return true;
        }
        return false;
    }

    
    public override bool redoTool()
    {

        parent.removeChild(linkIndex);
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {

        parent.addChild(linkIndex, linkDeleted);
        Controller.getInstance().updatePanel();
        return true;
    }
}
