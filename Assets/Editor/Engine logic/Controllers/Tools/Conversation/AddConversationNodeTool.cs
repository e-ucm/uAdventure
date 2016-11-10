using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Edition tool for adding a child to a node in a conversation
 */

public class AddConversationNodeTool : Tool
{


    protected ConversationNode newChild;

    protected ConversationNode parent;

    protected int index;

    protected int nodeType;

    protected Dictionary<ConversationNodeView, List<ConditionsController>> allConditions;

    public AddConversationNodeTool(ConversationNode parent, int nodeType,
        Dictionary<ConversationNodeView, List<ConditionsController>> allConditions)
    {

        this.parent = parent;
        this.nodeType = nodeType;
        this.allConditions = allConditions;
    }

    public AddConversationNodeTool(ConversationNodeView nodeView, int nodeType,
        Dictionary<ConversationNodeView, List<ConditionsController>> allConditions)
    {

        this.parent = (ConversationNode) nodeView;
        this.nodeType = nodeType;
        this.allConditions = allConditions;
    }


    public override bool canRedo()
    {

        return true;
    }


    public override bool canUndo()
    {

        return index != -1 && newChild != null;
    }


    public override bool combine(Tool other)
    {

        return false;
    }


    public override bool doTool()
    {

        // By default, add the child
        bool addChild = true;

        // If it's sure to add the child
        if (addChild)
        {

            // Create the requested node (only accept dialogue and option node)
            if (nodeType == (int) ConversationNodeViewEnum.DIALOGUE)
                newChild = new DialogueConversationNode();
            if (nodeType == (int) ConversationNodeViewEnum.OPTION)
                newChild = new OptionConversationNode();

            // If a child has been created
            if (newChild != null)
            {

                // Add the child to the given node
                parent.addChild(newChild);

                // Add to Conditions controller
                allConditions.Add(newChild, new List<ConditionsController>());

                // If the node was an option node, add a new line
                if (parent.getType() == ConversationNodeViewEnum.OPTION)
                {
                    parent.addLine(new ConversationLine(ConversationLine.PLAYER, TC.get("ConversationLine.NewOption")));
                    allConditions[parent].Add(new ConditionsController(new Conditions(),
                        Controller.CONVERSATION_OPTION_LINE, "0"));
                }
                // Save the index of the newChild
                index = -1;
                for (int i = 0; i < parent.getChildCount(); i++)
                {
                    if (parent.getChild(i) == newChild)
                    {
                        index = i;
                        break;
                    }
                }

            }
        }

        return newChild != null;
    }


    public override bool redoTool()
    {

        parent.addChild(index, newChild);
        // Add to Conditions controller
        allConditions.Add(newChild, new List<ConditionsController>());
        // If the node was an option node, add a new line
        if (parent.getType() == ConversationNodeViewEnum.OPTION)
        {
            parent.addLine(index, new ConversationLine(ConversationLine.PLAYER, TC.get("ConversationLine.NewOption")));
            allConditions[parent].Add(new ConditionsController(new Conditions(), Controller.CONVERSATION_OPTION_LINE,
                "0"));
        }
        Controller.getInstance().updatePanel();
        return true;
    }


    public override bool undoTool()
    {

        parent.removeChild(index);
        allConditions.Remove(newChild);
        if (parent.getType() == ConversationNodeViewEnum.OPTION)
        {
            parent.removeLine(index);
            allConditions[parent].RemoveAt(index);

        }
        Controller.getInstance().updatePanel();
        return true;
    }
}