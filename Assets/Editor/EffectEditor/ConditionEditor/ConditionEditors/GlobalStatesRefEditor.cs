using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class GlobalStatesRefEditor : ConditionEditor
{
    GlobalStateCondition condition = new GlobalStateCondition("");
    string[] types = { TC.get("Conditions.ConditionGroup.Satisfied"), TC.get("Conditions.ConditionGroup.NotSatisfied") };
    string name = TC.get("Element.Name54");
    private string[] states;

    public GlobalStatesRefEditor()
    {
        states = Controller.getInstance().getSelectedChapterDataControl().getGlobalStatesListDataControl().getGlobalStatesIds();
        if (states == null || states.Length == 0)
        {
            Avaiable = false;
        }
        else
        {
            Avaiable = true;
            condition = new GlobalStateCondition(states[0]);
        }
    }

    public void draw(Condition c)
    {
        condition = c as GlobalStateCondition;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Condition.GlobalStateID"));
        if (Avaiable)
        {
            int index = Array.IndexOf(states, c.getId());
            c.setId(states[EditorGUILayout.Popup(index >= 0 ? index : 0, states)]);
            c.setState(EditorGUILayout.Popup(c.getState() - GlobalStateCondition.GS_SATISFIED, types) +
                       GlobalStateCondition.GS_SATISFIED);
        }
        else
        {
            EditorGUILayout.HelpBox(TC.get("Condition.GlobalState.Warning"), MessageType.Error);
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
        return new GlobalStateCondition("");
    }

    public bool Collapsed { get; set; }
    public Rect Window { get; set; }
    public bool Avaiable { get; set; }
}
