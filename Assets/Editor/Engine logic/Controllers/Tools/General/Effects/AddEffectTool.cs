using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddEffectTool : Tool {

    protected Effects effects;

    protected AbstractEffect effectToAdd;

    protected List<ConditionsController> conditions;

    protected ConditionsController condition;

    public AddEffectTool(Effects effects, AbstractEffect effectToAdd, List<ConditionsController> conditions)
    {

        this.effects = effects;
        this.effectToAdd = effectToAdd;
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

        effects.add(effectToAdd);
        if (conditions != null)
        {
            condition = new ConditionsController(effectToAdd.getConditions(), Controller.EFFECT, EffectsController.getEffectInfo(effectToAdd));
            conditions.Add(condition);
        }

        return true;
    }

    public override bool redoTool()
    {

        bool done = doTool();
        if (done)
        {
            Controller.getInstance().updateVarFlagSummary();
            Controller.getInstance().updatePanel();
        }
        return done;
    }

    public override bool undoTool()
    {

        effects.getEffects().Remove(effectToAdd);
        if (conditions != null)
        {
            conditions.Remove(condition);
        }
        Controller.getInstance().updateVarFlagSummary();
        Controller.getInstance().updatePanel();
        return true;
    }

}
