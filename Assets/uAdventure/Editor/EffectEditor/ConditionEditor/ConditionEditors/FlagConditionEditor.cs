using UnityEngine;
using UnityEditor;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class FlagConditionEditor : ConditionEditor
    {
        private FlagCondition condition = new FlagCondition("");
        private readonly string[] types = { TC.get("Conditions.Flag.Active"), TC.get("Conditions.Flag.Inactive") };
        private readonly string name = TC.get("Flags.Flag");

        public FlagConditionEditor()
        {
            if (Available)
            {
                condition = new FlagCondition(Controller.Instance.VarFlagSummary.getFlags()[0]);
            }
        }

        public void draw(Condition c)
        {
            condition = c as FlagCondition;
            using (new EditorGUILayout.HorizontalScope())
            {
                if (Available)
                {
                    var flags = Controller.Instance.VarFlagSummary.getFlags();
                    var index = Mathf.Max(0, Array.IndexOf(flags, c.getId()));
                    c.setId(flags[EditorGUILayout.Popup(index >= 0 ? index : 0, flags)]);
                    c.setState(EditorGUILayout.Popup(c.getState(), types));
                }
                else
                {
                    EditorGUILayout.HelpBox(TC.get("Condition.Flag.Warning"), MessageType.Error);
                }
            }
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
        public bool Available { get { return Controller.Instance.VarFlagSummary.getFlags().Length > 0; } }
    }
}