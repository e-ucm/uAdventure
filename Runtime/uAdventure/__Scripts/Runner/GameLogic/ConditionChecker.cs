using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Runner
{
    public static class ConditionChecker
    {
        /*private static ConditionChecker instance;
        public static ConditionChecker Instance {
            get { 
                if (this.instance == null)
                    this.instance = new ConditionChecker ();
                return instance;
            }
        }*/

        public static bool check(Conditions conditions)
        {
            foreach (Condition c in conditions.GetSimpleConditions())
            {
                if (!check(c))
                    return false;
            }

            for (int i = 0; i < conditions.GetEitherConditionsBlockCount(); i++)
            {
                bool block = false;
                foreach (Condition c in conditions.GetEitherConditions(i))
                {
                    block |= check(c);

                    if (block)
                        break;
                }

                if (!block)
                    return false;
            }

            return true;
        }

        public static int check(GlobalState globalstate)
        {
            if (check(globalstate as Conditions))
                return GlobalStateCondition.GS_SATISFIED;
            else
                return GlobalStateCondition.GS_NOT_SATISFIED;
        }

        public static bool check(Condition condition)
        {
            bool ret = true;
            switch (condition.getType())
            {
                case Condition.FLAG_CONDITION:
                    ret = Game.Instance.GameState.CheckFlag(condition.getId()) == condition.getState();
                    break;
                case Condition.GLOBAL_STATE_CONDITION:
                    ret = Game.Instance.GameState.CheckGlobalState(condition.getId()) == condition.getState();
                    break;
                case Condition.NO_STATE: break;
                case Condition.VAR_CONDITION:
                    VarCondition c = (VarCondition)condition;
                    int val = Game.Instance.GameState.GetVariable(condition.getId());

                    switch (c.getState())
                    {
                        case VarCondition.VAR_EQUALS:
                            ret = val == c.getValue();
                            break;
                        case VarCondition.VAR_GREATER_THAN:
                            ret = val > c.getValue();
                            break;
                        case VarCondition.VAR_GREATER_EQUALS_THAN:
                            ret = val >= c.getValue();
                            break;
                        case VarCondition.VAR_LESS_THAN:
                            ret = val < c.getValue();
                            break;
                        case VarCondition.VAR_LESS_EQUALS_THAN:
                            ret = val <= c.getValue();
                            break;
                        case VarCondition.VAR_NOT_EQUALS:
                            ret = val != c.getValue();
                            break;
                    }
                    break;
            }

            return ret;
        }
    }
}