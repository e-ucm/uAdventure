
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
    public class VarConditionEditor : ConditionEditor, DialogReceiverInterface
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

        private const string DEFAULT_VAR_ID = "IdVar";

        private readonly Dictionary<int, int> indexToState = new Dictionary<int, int>();
        private readonly Dictionary<int, int> stateToIndex = new Dictionary<int, int>();
        private readonly string[] valueStrings;
        private readonly string name = TC.get("Vars.Var");
        private static GUIStyle collapseStyle;

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

            if (collapseStyle == null)
            {
                collapseStyle = new GUIStyle(GUI.skin.button);
                collapseStyle.padding = new RectOffset(0, 0, 0, 0);
                collapseStyle.margin = new RectOffset(0, 5, 2, 0);
                collapseStyle.normal.textColor = Color.blue;
                collapseStyle.focused.textColor = Color.blue;
                collapseStyle.active.textColor = Color.blue;
                collapseStyle.hover.textColor = Color.blue;
            }
        }

        public void draw(Condition c)
        {
            condition = c as VarCondition;

            EditorGUILayout.BeginHorizontal();

            var target = c.getId();
            if (Available)
            {
                var vars = Controller.Instance.VarFlagSummary.getVars();
                var index = Mathf.Max(0, Array.IndexOf(vars, c.getId()));
                condition.setId(vars[EditorGUILayout.Popup(index >= 0 ? index : 0, vars)]);
            }
            else
            {
                using (new GUILayout.HorizontalScope(GUILayout.Height(15)))
                {
                    EditorGUILayout.HelpBox(TC.get("Condition.Var.Warning"), MessageType.Error);
                }
            }

            if (GUILayout.Button("New", collapseStyle, GUILayout.Width(35), GUILayout.Height(15)))
            {
                Controller.Instance.ShowInputDialog(TC.get("Vars.AddVar"), TC.get("Vars.AddVarMessage"), DEFAULT_VAR_ID, condition, this);
            }

            if (Available)
            {
                condition.setState(indexToState[EditorGUILayout.Popup(stateToIndex[condition.getState()], valueStrings)]);
                condition.setValue(int.Parse(EditorGUILayout.TextField(condition.getValue().ToString())));
            }

            var newTarget = c.getId();
            if (target != newTarget)
            {
                if (!string.IsNullOrEmpty(target))
                {
                    Controller.Instance.VarFlagSummary.deleteVarReference(target);
                }
                Controller.Instance.VarFlagSummary.addVarReference(newTarget);
                Controller.Instance.DataModified();
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

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            Controller.Instance.VarFlagSummary.addVar(message);
            if(condition == null)
            {
                condition = new VarCondition(message, 0, 0);
            }
            else
            {
                if (!string.IsNullOrEmpty(condition.getId()))
                {
                    Controller.Instance.VarFlagSummary.deleteVarReference(condition.getId());
                }
                condition.setId(message);
            }

            Controller.Instance.VarFlagSummary.addVarReference(message);
            Controller.Instance.DataModified();
        }

        public void OnDialogCanceled(object workingObject = null) { }

        public bool Collapsed { get; set; }
        public Rect Window { get; set; }
        public bool Available { get { return Controller.Instance.VarFlagSummary.getVars().Length > 0; } }


    }
}