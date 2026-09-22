using UnityEngine;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
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

            this.conditionsOld = (Conditions)conditions;
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
                conditions.GetConditionsList().Clear();
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
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
                conditions = (Conditions)conditionsOld;
                Controller.Instance.updateVarFlagSummary();
                Controller.Instance.updatePanel();
            }
            else
                return false;
            return true;
        }
    }
}