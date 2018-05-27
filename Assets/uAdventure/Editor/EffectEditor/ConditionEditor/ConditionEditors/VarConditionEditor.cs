using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class VarConditionEditor : ConditionEditor
    {
        VarCondition condition = new VarCondition("", 4, 0);
        string[] types = { " > ", " >= ", " == ", " <= ", " != " };
        string name = TC.get("Vars.Var");

        public VarConditionEditor()
        {
            if (Avaiable)
                condition = new VarCondition(Controller.Instance.VarFlagSummary.getVars()[0], 4, 0);
        }

        public void draw(Condition c)
        {
            condition = c as VarCondition;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Condition.VarID"));

            if (Avaiable)
            {
                var vars = Controller.Instance.VarFlagSummary.getVars();
                int index = Array.IndexOf(vars, c.getId());
                condition.setId(vars[EditorGUILayout.Popup(index >= 0 ? index : 0, vars)]);
                condition.setState(EditorGUILayout.Popup(c.getState() - 2, types) + 2);
                condition.setValue(int.Parse(EditorGUILayout.TextField(condition.getValue().ToString())));
            }
            else
            {
                EditorGUILayout.HelpBox(TC.get("Condition.Var.Warning"), MessageType.Error);
            }

            EditorGUILayout.EndHorizontal();
        }

        public bool manages(Condition c)
        {
            return c.GetType() == condition.GetType();
        }

        public string conditionName()
        {
            return name;
        }

        public Condition InstanceManagedCondition()
        {
            return new VarCondition("", 4, 0);
        }

        public bool Collapsed { get; set; }
        public Rect Window { get; set; }
        public bool Avaiable { get { return Controller.Instance.VarFlagSummary.getVars().Length > 0; } }
    }
}