using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeleteEffectTool : Tool
    {


        protected Effects effects;

        protected AbstractEffect effectDeleted;

        protected int index;

        protected Controller controller;

        protected List<ConditionsController> conditions;

        protected ConditionsController condition;

        public DeleteEffectTool(Effects effects, int index, List<ConditionsController> conditions)
        {
            this.effects = effects;
            this.index = index;
            this.conditions = conditions;
            controller = Controller.Instance;
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

            effectDeleted = effects.getEffects()[index];
            effects.getEffects().RemoveAt(index);
            condition = conditions[index];
            conditions.RemoveAt(index);
            updateVarFlagSummary((Effect)effectDeleted);
            return true;
        }

        public override bool redoTool()
        {

            effects.getEffects().RemoveAt(index);
            conditions.RemoveAt(index);
            updateVarFlagSummary((Effect)effectDeleted);
            Controller.Instance.updatePanel();
            return true;
        }

        public override bool undoTool()
        {

            effects.getEffects().Insert(index, effectDeleted);
            conditions.Insert(index, condition);
            undoUpdateVarFlagSummary((Effect)effectDeleted);
            Controller.Instance.updatePanel();
            return true;
        }

        /**
         * Updates the varFlag summary (the references to flags and variables if the
         * effect given as argument has any)
         * 
         * @param effect
         */
        protected void updateVarFlagSummary(Effect effect)
        {

            if (effect.getType() == EffectType.ACTIVATE)
            {
                ActivateEffect activateEffect = (ActivateEffect)effect;
                controller.VarFlagSummary.deleteReference(activateEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.DEACTIVATE)
            {
                DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                controller.VarFlagSummary.deleteReference(deactivateEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.SET_VALUE)
            {
                SetValueEffect setValueEffect = (SetValueEffect)effect;
                controller.VarFlagSummary.deleteReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.INCREMENT_VAR)
            {
                IncrementVarEffect setValueEffect = (IncrementVarEffect)effect;
                controller.VarFlagSummary.deleteReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.DECREMENT_VAR)
            {
                DecrementVarEffect setValueEffect = (DecrementVarEffect)effect;
                controller.VarFlagSummary.deleteReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.RANDOM_EFFECT)
            {
                RandomEffect randomEffect = (RandomEffect)effect;
                if (randomEffect.getPositiveEffect() != null)
                    updateVarFlagSummary((Effect)randomEffect.getPositiveEffect());
                if (randomEffect.getNegativeEffect() != null)
                    updateVarFlagSummary((Effect)randomEffect.getNegativeEffect());
            }
        }

        /**
         * Undoes the actions performed in updateVarFlagSummary
         * 
         * @param effect
         */
        protected void undoUpdateVarFlagSummary(Effect effect)
        {

            if (effect.getType() == EffectType.ACTIVATE)
            {
                ActivateEffect activateEffect = (ActivateEffect)effect;
                controller.VarFlagSummary.addReference(activateEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.DEACTIVATE)
            {
                DeactivateEffect deactivateEffect = (DeactivateEffect)effect;
                controller.VarFlagSummary.addReference(deactivateEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.SET_VALUE)
            {
                SetValueEffect setValueEffect = (SetValueEffect)effect;
                controller.VarFlagSummary.addReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.INCREMENT_VAR)
            {
                IncrementVarEffect setValueEffect = (IncrementVarEffect)effect;
                controller.VarFlagSummary.addReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.DECREMENT_VAR)
            {
                DecrementVarEffect setValueEffect = (DecrementVarEffect)effect;
                controller.VarFlagSummary.addReference(setValueEffect.getTargetId());
            }

            else if (effect.getType() == EffectType.RANDOM_EFFECT)
            {
                RandomEffect randomEffect = (RandomEffect)effect;
                if (randomEffect.getPositiveEffect() != null)
                    undoUpdateVarFlagSummary((Effect)randomEffect.getPositiveEffect());
                if (randomEffect.getNegativeEffect() != null)
                    undoUpdateVarFlagSummary((Effect)randomEffect.getNegativeEffect());
            }
        }
    }
}