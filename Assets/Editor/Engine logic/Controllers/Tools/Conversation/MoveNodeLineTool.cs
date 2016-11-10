using UnityEngine;
using System.Collections;

public class MoveNodeLineTool : Tool
{
    public const int UP = 1;

    public const int DOWN = 2;

    protected ConversationNode parent;

    protected int lineIndex;

    protected int mode;

    public MoveNodeLineTool(ConversationNodeView nodeView, int lineIndex, int mode):this((ConversationNode)nodeView, lineIndex, mode)
    { }

    public MoveNodeLineTool(ConversationNode parent, int lineIndex, int mode)
    {

        this.parent = parent;
        this.lineIndex = lineIndex;
        this.mode = mode;
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

        if (mode == UP)
        {
            return moveUp();
        }
        else if (mode == DOWN)
        {
            return moveDown();
        }
        else
            return false;

    }

    
    public override bool redoTool()
    {

        bool moved = false;
        if (mode == UP)
        {
            moved = moveUp();
        }
        else if (mode == DOWN)
        {
            moved = moveDown();
        }

        if (moved)
            Controller.getInstance().updatePanel();
        return moved;
    }

    
    public override bool undoTool()
    {

        bool moved = false;
        if (mode == UP)
        {
            moved = moveDown();
        }
        else if (mode == DOWN)
        {
            moved = moveUp();
        }

        if (moved)
            Controller.getInstance().updatePanel();
        return moved;

    }

    protected bool moveDown()
    {

        bool lineMoved = false;

        // Cannot move the line down if it is the last position
        if (lineIndex < parent.getLineCount() - 1)
        {
            // Remove the line and insert it in the lower position
            parent.addLine(lineIndex + 1, parent.removeLine(lineIndex));

            // If the node is a OptionNode, move the respective child along the line
            if (parent.getType() == ConversationNodeViewEnum.OPTION)
            {
                ConversationNode nodeMoved = parent.removeChild(lineIndex);
                parent.addChild(lineIndex + 1, nodeMoved);
            }

            lineMoved = true;
        }

        return lineMoved;

    }

    protected bool moveUp()
    {

        bool lineMoved = false;

        // Cannot move the line up if it is in the first position
        if (lineIndex > 0)
        {
            // Remove the line and insert it in the upper position
            parent.addLine(lineIndex - 1, parent.removeLine(lineIndex));

            // If the node is a OptionNode, move the respective child along the line
            if (parent.getType() == ConversationNodeViewEnum.OPTION)
            {
                ConversationNode nodeMoved = parent.removeChild(lineIndex);
                parent.addChild(lineIndex - 1, nodeMoved);
            }

            lineMoved = true;
        }

        return lineMoved;
    }
}
