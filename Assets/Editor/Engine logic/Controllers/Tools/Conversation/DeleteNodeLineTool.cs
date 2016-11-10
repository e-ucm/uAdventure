using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteNodeLineTool : Tool {
    protected ConversationNode parent;

    protected int lineIndex;

    protected ConversationLine lineDeleted;

    protected List<ConditionsController> node;

    protected ConditionsController conditionDeleted;

    public DeleteNodeLineTool(ConversationNodeView nodeView, int lineIndex, List<ConditionsController> node):this((ConversationNode)nodeView, lineIndex, node)
    {}

    public DeleteNodeLineTool(ConversationNode parent, int lineIndex, List<ConditionsController> node)
    {

        this.parent = parent;
        this.lineIndex = lineIndex;
        this.node = node;
    }

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return lineDeleted != null;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        lineDeleted = parent.getLine(lineIndex);
        parent.removeLine(lineIndex);
        conditionDeleted = node[lineIndex];
        node.RemoveAt(lineIndex);
        return true;
    }

    
    public override bool redoTool()
    {

        parent.removeLine(lineIndex);
        node.RemoveAt(lineIndex);
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {

        parent.addLine(lineIndex, lineDeleted);
        node.Insert(lineIndex, conditionDeleted);
        Controller.getInstance().updatePanel();
        return true;
    }
}
