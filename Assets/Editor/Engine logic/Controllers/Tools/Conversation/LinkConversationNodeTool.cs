using UnityEngine;
using System.Collections;

public class LinkConversationNodeTool : Tool
{
    private ConversationNodeView fatherView;

    private ConversationNodeView childView;

    private Controller controller;

    private ConversationDataControl dataControl;

    public LinkConversationNodeTool(ConversationDataControl _dataControl, ConversationNodeView _fatherView,
        ConversationNodeView _childView)
    {

        this.fatherView = _fatherView;
        this.childView = _childView;
        this.dataControl = _dataControl;
        this.controller = Controller.getInstance();
    }


    public override bool canRedo()
    {

        return true;
    }


    public override bool canUndo()
    {

        return true;
    }


    public override bool combine(Tool other)
    {

        return false;
    }


    public override bool doTool()
    {

        bool nodeLinked = false;

        // If it is not possible to link the node to the given one, show a message
        if (!dataControl.canLinkNodeTo(fatherView, childView))
            controller.showErrorDialog(TC.get("Conversation.OperationLinkNode"), TC.get("Conversation.ErrorLinkNode"));

        // If it can be linked
        else
        {
            bool linkNode = true;

            // If the node has an effect, ask for confirmation (for the effect will be deleted)
            //if( fatherView.hasEffects( ) )
            //linkNode = controller.showStrictConfirmDialog( TextConstants.getText( "Conversation.OperationLinkNode" ), TextConstants.getText( "Conversation.ErrorLinkNode" ) );

            // If the node must be linked
            if (linkNode)
            {
                // Take the complete nodes
                ConversationNode father = (ConversationNode) fatherView;
                ConversationNode child = (ConversationNode) childView;

                // Add the new child
                father.addChild(child);

                // If the father is an option node, add a new line
                if (father.getType() == ConversationNodeViewEnum.OPTION)
                    father.addLine(new ConversationLine(ConversationLine.PLAYER, TC.get("ConversationLine.DefaultText")));

                // The node was successfully linked
                nodeLinked = true;
                dataControl.updateAllConditions();
            }
        }

        return nodeLinked;
    }


    public override bool redoTool()
    {

        // Take the complete nodes
        ConversationNode father = (ConversationNode) fatherView;
        ConversationNode child = (ConversationNode) childView;
        // Add the new child
        father.addChild(child);
        // If the father is an option node, add a new line
        if (father.getType() == ConversationNodeViewEnum.OPTION)
            father.addLine(new ConversationLine(ConversationLine.PLAYER, TC.get("ConversationLine.DefaultText")));
        dataControl.updateAllConditions();
        controller.updatePanel();
        return true;
    }


    public override bool undoTool()
    {

        // Take the complete nodes
        ConversationNode father = (ConversationNode) fatherView;
        ConversationNode child = (ConversationNode) childView;

        // Add the new child
        int index = -1;
        for (int i = 0; i < father.getChildCount(); i++)
        {
            if (father.getChild(i) == child)
            {
                index = i;
                father.removeChild(i);
                break;
            }
        }

        // If the father is an option node, add a new line
        if (father.getType() == ConversationNodeViewEnum.OPTION && index != -1)
            father.removeLine(index);
        dataControl.updateAllConditions();
        controller.updatePanel();
        return true;
    }
}