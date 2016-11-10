using UnityEngine;
using System.Collections;

/**
 * 
 * Clear conditions tool
 * 
 */

public class ClearConditionsTool : Tool
{


    Conditions conditions;
    Conditions conditionsOld;

    public ClearConditionsTool(Conditions conditions)
    {

        this.conditionsOld = (Conditions) conditions;
        this.conditions = conditions;

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
        if (conditions != null)
        {
            conditions.getConditionsList().Clear();
            Controller.getInstance().updateVarFlagSummary();
            Controller.getInstance().updatePanel();
            return true;
        }
        else
            return false;
    }

    public override bool redoTool()
    {

        return doTool();
    }

    public override bool undoTool()
    {

        if (conditionsOld != null)
        {
            conditions = (Conditions) conditionsOld;
            Controller.getInstance().updateVarFlagSummary();
            Controller.getInstance().updatePanel();
        }
        else
            return false;
        return true;
    }
}
