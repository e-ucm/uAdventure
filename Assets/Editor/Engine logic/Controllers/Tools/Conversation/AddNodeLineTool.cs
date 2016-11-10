using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddNodeLineTool : Tool {


    protected ConversationNode parent;

    protected int lineIndex;

    protected ConversationLine lineAdded;

    protected string name;

    protected List<ConditionsController> node;

    public AddNodeLineTool(ConversationNodeView nodeView, int lineIndex, string name, List<ConditionsController> node):this((ConversationNode)nodeView, lineIndex, name, node)
    {}

    public AddNodeLineTool(ConversationNode parent, int lineIndex, string name, List<ConditionsController> node)
    {

        this.parent = parent;
        this.lineIndex = lineIndex;
        this.name = name;
        this.node = node;
    }

    
    public override bool canRedo()
    {

        return true;
    }

    
    public override bool canUndo()
    {

        return lineAdded != null;
    }

    
    public override bool combine(Tool other)
    {

        return false;
    }

    
    public override bool doTool()
    {

        lineAdded = new ConversationLine(name, TC.get("ConversationLine.DefaultText"));
        parent.addLine(lineIndex, lineAdded);
        node.Insert(lineIndex, new ConditionsController(lineAdded.getConditions(), Controller.CONVERSATION_OPTION_LINE, lineIndex.ToString()));
        return true;
    }

    
    public override bool redoTool()
    {

        parent.addLine(lineIndex, lineAdded);
        node.Insert(lineIndex, new ConditionsController(lineAdded.getConditions(), Controller.CONVERSATION_OPTION_LINE, lineIndex.ToString()));
        Controller.getInstance().updatePanel();
        return true;
    }

    
    public override bool undoTool()
    {

        parent.removeLine(lineIndex);
        node.RemoveAt(lineIndex);
        Controller.getInstance().updatePanel();
        return true;
    }
}
