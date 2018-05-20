using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddEffectTool : Tool
    {

        protected Effects effects;

        protected IEffect effectToAdd;

        protected List<ConditionsController> conditions;

        protected ConditionsController condition;

        public AddEffectTool(Effects effects, IEffect effectToAdd, List<ConditionsController> conditions)
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

            effects.Add(effectToAdd);
            if (conditions != null && effectToAdd is AbstractEffect)
            {
                condition = new ConditionsController((effectToAdd as AbstractEffect).getConditions(), Controller.EFFECT, EffectsController.getEffectInfo(effectToAdd));
                conditions.Add(condition);
            }

            return true;
        }

        public override bool redoTool()
        {

            bool done = doTool();
            if (done)
            {
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
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
            Controller.Instance.updateVarFlagSummary();
            Controller.Instance.updatePanel();
            return true;
        }

    }
}