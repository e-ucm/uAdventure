using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GlobalStatesRefEditor : ConditionEditor
    {
        private GlobalStateCondition condition = new GlobalStateCondition("");
        private readonly string[] types = { TC.get("Conditions.ConditionGroup.Satisfied"), TC.get("Conditions.ConditionGroup.NotSatisfied") };
        private readonly string name = TC.get("Element.Name54");

        public GlobalStatesRefEditor()
        {
             condition = new GlobalStateCondition(Available ? Controller.Instance.SelectedChapterDataControl
                 .getGlobalStatesListDataControl().getGlobalStatesIds()[0] : "");
        }

        public void draw(Condition c)
        {
            condition = c as GlobalStateCondition;
            
            using (new EditorGUILayout.HorizontalScope())
            {
                if (Available)
                {
                    var states = Controller.Instance.SelectedChapterDataControl.getGlobalStatesListDataControl().getGlobalStatesIds();
                    var index = Mathf.Max(0, Array.IndexOf(states, c.getId()));
                    c.setId(states[EditorGUILayout.Popup(index >= 0 ? index : 0, states)]);
                    c.setState(EditorGUILayout.Popup(c.getState() - GlobalStateCondition.GS_SATISFIED, types) +
                               GlobalStateCondition.GS_SATISFIED);
                }
                else
                {
                    EditorGUILayout.HelpBox(TC.get("Condition.GlobalState.Warning"), MessageType.Error);
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
            return new GlobalStateCondition("");
        }

        public bool Collapsed { get; set; }
        public Rect Window { get; set; }
        public bool Available
        {
            get
            {
                return Controller.Instance.SelectedChapterDataControl.getGlobalStatesListDataControl()
                    .getGlobalStatesIds().Length > 0;
            }
        }
    }
}