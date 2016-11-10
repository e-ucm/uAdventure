using UnityEngine;
using System.Collections;

public class OptionsPositionTool : Tool
{

    private bool bottomPosition;

    private OptionConversationNode optionNode;

    public OptionsPositionTool(OptionConversationNode optionNode, bool bottomPosition)
    {
        this.optionNode = optionNode;
        this.bottomPosition = bottomPosition;
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

        if (bottomPosition)
            optionNode.setBottomPosition();
        else
            optionNode.setTopPosition();

        Controller.getInstance().updatePanel();
        return true;
    }


    public override bool redoTool()
    {

        return doTool();
    }


    public override bool undoTool()
    {

        if (bottomPosition)
            optionNode.setTopPosition();
        else
            optionNode.setBottomPosition();
        Controller.getInstance().updatePanel();
        return true;
    }
}