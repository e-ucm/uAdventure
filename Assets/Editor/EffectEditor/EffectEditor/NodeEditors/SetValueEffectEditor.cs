using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SetValueEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    public Rect Window
    {
        get
        {
            if (collapsed) return new Rect(window.x, window.y, 50, 30);
            else return window;
        }
        set
        {
            if (collapsed) window = new Rect(value.x, value.y, window.width, window.height);
            else window = value;
        }
    }
    private string[] vars;

    private SetValueEffect effect;

    public SetValueEffectEditor()
    {
        List<string> tmp = new List<string> ();
        tmp.Add ("");
        tmp.AddRange(Controller.getInstance().getVarFlagSummary().getVars());
        vars = tmp.ToArray ();

        this.effect = new SetValueEffect(vars[0], 1);
    }

    public void draw()
    {

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(TC.get("Vars.Var"));
        effect.setTargetId(vars[EditorGUILayout.Popup(Array.IndexOf(vars, effect.getTargetId()), vars)]);
        effect.setValue(EditorGUILayout.IntField(effect.getValue()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("SetValueEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as SetValueEffect; } }
    public string EffectName { get { return TC.get("SetValueEffect.Title"); } }
    public EffectEditor clone() { return new SetValueEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
