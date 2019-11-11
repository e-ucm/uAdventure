
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace uAdventure.Editor
{
    public class VarConditionEditor : ConditionEditor
    {
        private VarCondition condition = new VarCondition("", 4, 0);

        private static readonly Dictionary<int, string> stateToString = new Dictionary<int, string>
        {
            { VarCondition.VAR_GREATER_THAN, " > " }, 
            { VarCondition.VAR_GREATER_EQUALS_THAN, " ≥ " }, 
            { VarCondition.VAR_EQUALS, " = " }, 
            { VarCondition.VAR_LESS_THAN, " < " }, 
            { VarCondition.VAR_LESS_EQUALS_THAN, " ≤ " },
            { VarCondition.VAR_NOT_EQUALS, " ≠ " }
        };

        private readonly Dictionary<int, int> indexToState = new Dictionary<int, int>();
        private readonly Dictionary<int, int> stateToIndex = new Dictionary<int, int>();
        private readonly string[] valueStrings;
        private readonly string name = TC.get("Vars.Var");

        public VarConditionEditor()
        {
            var stateToStringEnumerator = stateToString.GetEnumerator();
            for(int i = 0; i < stateToString.Count; i++)
            {
                stateToStringEnumerator.MoveNext();
                indexToState.Add(i, stateToStringEnumerator.Current.Key);
                stateToIndex.Add(stateToStringEnumerator.Current.Key, i);
            }
            valueStrings = stateToString.Values.ToArray();

            if (Available)
            {
                condition = new VarCondition(Controller.Instance.VarFlagSummary.getVars()[0], 4, 0);
            }
        }

        public void draw(Condition c)
        {
            condition = c as VarCondition;

            EditorGUILayout.BeginHorizontal();

            if (Available)
            {
                var vars = Controller.Instance.VarFlagSummary.getVars();
                var index = Mathf.Max(0, Array.IndexOf(vars, c.getId()));
                condition.setId(vars[EditorGUILayout.Popup(index >= 0 ? index : 0, vars)]);
                condition.setState(indexToState[EditorGUILayout.Popup(stateToIndex[condition.getState()], valueStrings)]);
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
        public bool Available { get { return Controller.Instance.VarFlagSummary.getVars().Length > 0; } }


    }
}