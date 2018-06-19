using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class AddConditionTool : Tool
    {
        private Conditions conditions;

        private int index1;

        private int index2;

        private string conditionType;

        private string conditionId;

        private string conditionState;

        private string value;

        private List<Condition> blockAdded; //index1

        private Condition conditionAdded;

        private int indexAdded;

        public AddConditionTool(Conditions conditions, int index1, int index2, string conditionType, string conditionId, string conditionState, string value)
        {

            this.conditions = conditions;
            this.index1 = index1;
            this.index2 = index2;
            this.conditionType = conditionType;
            this.conditionId = conditionId;
            this.conditionState = conditionState;
            this.value = value;

            this.blockAdded = null;
            this.conditionAdded = null;
            this.indexAdded = -1;

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

            Condition newCondition = null;
            int type = ConditionsController.getTypeFromstring(conditionType);
            if (type == ConditionsController.FLAG_CONDITION)
            {
                newCondition = new FlagCondition(conditionId, ConditionsController.getStateFromstring(conditionState));
            }
            else if (type == ConditionsController.VAR_CONDITION)
            {
                newCondition = new VarCondition(conditionId, ConditionsController.getStateFromstring(conditionState), int.Parse(value));
            }
            else if (type == ConditionsController.GLOBAL_STATE_CONDITION)
            {
                newCondition = new GlobalStateCondition(conditionId, ConditionsController.getStateFromstring(conditionState));
            }

            if (newCondition != null)
            {
                if (index1 < conditions.Size())
                {

                    if (index2 == ConditionsController.INDEX_NOT_USED)
                    {
                        // Add new block
                        List<Condition> newBlock = new List<Condition>();
                        newBlock.Add(newCondition);
                        conditions.Add(index1, newBlock);
                        indexAdded = index1;
                        blockAdded = newBlock;
                    }
                    else
                    {
                        var block = conditions.Get(index1);
                        if (index2 < 0 || index2 > block.Count)
                            return false;

                        if (index2 == conditions.Size())
                        {
                            block.Add(newCondition);
                            indexAdded = block.IndexOf(newCondition);
                            conditionAdded = newCondition;
                        }
                        else
                        {
                            indexAdded = index2;
                            conditionAdded = newCondition;
                            block.Insert(index2, newCondition);
                        }
                    }

                }
                else
                {
                    // Add new block
                    List<Condition> newBlock = new List<Condition>();
                    newBlock.Add(newCondition);
                    conditions.Add(newBlock);
                    indexAdded = conditions.Size() - 1;
                    blockAdded = newBlock;
                }
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
                return true;
            }
            return false;
        }

        public override bool redoTool()
        {

            if (blockAdded != null)
            {
                conditions.Add(indexAdded, blockAdded);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
                return true;
            }
            else if (conditionAdded != null)
            {
                conditions.Get(index1).Insert(indexAdded, conditionAdded);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
                return true;
            }

            return false;
        }

        public override bool undoTool()
        {

            if (blockAdded != null)
            {
                conditions.Delete(indexAdded);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
                return true;
            }
            else if (conditionAdded != null)
            {
                conditions.Get(index1).RemoveAt(indexAdded);
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
                return true;
            }

            return false;
        }
    }
}