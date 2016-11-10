using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class FlagConditionEditor : ConditionEditor {
    FlagCondition condition = new FlagCondition("");
    string[] types = { TC.get("Conditions.Flag.Active"), TC.get("Conditions.Flag.Inactive") };
    string name = TC.get("Flags.Flag");
    private string[] flags;

    public FlagConditionEditor()
    {
        flags = Controller.getInstance().getVarFlagSummary().getFlags();
        if (flags == null || flags.Length == 0)
        {
            Avaiable = false;
        }
        else
        {
            Avaiable = true;
            condition = new FlagCondition(flags[0]);
        }
    }

    public void draw(Condition c){
        condition = c as FlagCondition;

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (TC.get("Condition.FlagID"));

        if (Avaiable)
        {
            int index = Array.IndexOf(flags, c.getId());
            c.setId(flags[EditorGUILayout.Popup(index >= 0 ? index:0, flags)]);
            c.setState(EditorGUILayout.Popup(c.getState(), types));
        }
        else
        {
            EditorGUILayout.HelpBox(TC.get("Condition.Flag.Warning"), MessageType.Error);
        }

        EditorGUILayout.EndHorizontal ();
    }

    public bool manages(Condition c) {
        return c.GetType() == condition.GetType();
    }

    public string conditionName(){
        return name;
    }

    public Condition InstanceManagedCondition(){
        return new FlagCondition ("");
    }

    public bool Collapsed { get; set; }
    public Rect Window { get; set; }
    public bool Avaiable { get; set; }
}
