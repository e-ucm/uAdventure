using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class FlagConditionEditor : ConditionEditor
    {
        FlagCondition condition = new FlagCondition("");
        string[] types = { TC.get("Conditions.Flag.Active"), TC.get("Conditions.Flag.Inactive") };
        string name = TC.get("Flags.Flag");

        public FlagConditionEditor()
        {
            if (Avaiable)
                condition = new FlagCondition(Controller.Instance.VarFlagSummary.getFlags()[0]);
        }

        public void draw(Condition c)
        {
            condition = c as FlagCondition;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Condition.FlagID"));

            if (Avaiable)
            {
                var flags = Controller.Instance.VarFlagSummary.getFlags();
                int index = Array.IndexOf(flags, c.getId());
                c.setId(flags[EditorGUILayout.Popup(index >= 0 ? index : 0, flags)]);
                c.setState(EditorGUILayout.Popup(c.getState(), types));
            }
            else
            {
                EditorGUILayout.HelpBox(TC.get("Condition.Flag.Warning"), MessageType.Error);
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
            return new FlagCondition("");
        }

        public bool Collapsed { get; set; }
        public Rect Window { get; set; }
        public bool Avaiable { get { return Controller.Instance.VarFlagSummary.getFlags().Length > 0; } }
    }
}