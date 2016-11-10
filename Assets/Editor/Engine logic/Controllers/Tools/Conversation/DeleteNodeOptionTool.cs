using UnityEngine;
using System.Collections;

public class DeleteNodeOptionTool : DeleteNodeLinkTool
{
    protected ConversationLine deletedLine;

    public DeleteNodeOptionTool(ConversationNode parent, int optionIndex) : base(parent)
    {
        this.confirmText = TC.get("Conversation.ConfirmationDeleteOption");
        this.confirmTitle = TC.get("Conversation.OperationDeleteOption");
        this.linkIndex = optionIndex;
    }

    public DeleteNodeOptionTool(ConversationNodeView parent, int optionIndex) : base(parent)
    {

        this.confirmText = TC.get("Conversation.ConfirmationDeleteOption");
        this.confirmTitle = TC.get("Conversation.OperationDeleteOption");
        this.linkIndex = optionIndex;
    }


    public override bool doTool()
    {

        bool done = base.doTool();
        if (done)
        {
            deletedLine = parent.getLine(linkIndex);
            parent.removeLine(linkIndex);
        }
        return done;
    }


    public override bool undoTool()
    {

        bool done = base.undoTool();
        if (done)
        {
            parent.addLine(linkIndex, deletedLine);
        }
        return done;
    }


    public override bool redoTool()
    {

        bool done = base.redoTool();
        if (done)
        {
            parent.removeLine(linkIndex);
        }
        return done;
    }
}