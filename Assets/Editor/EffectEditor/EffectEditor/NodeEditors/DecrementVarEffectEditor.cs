using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class DecrementVarEffectEditor : EffectEditor
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

    private DecrementVarEffect effect;

    public DecrementVarEffectEditor()
    {
        List<string> tmp = new List<string> ();
        tmp.Add ("");
        tmp.AddRange(Controller.getInstance().getVarFlagSummary().getVars());
        vars = tmp.ToArray ();

        this.effect = new DecrementVarEffect (vars [0], 1);
    }

    public void draw()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Vars.Var"));

        effect.setTargetId(vars[EditorGUILayout.Popup(Array.IndexOf(vars, effect.getTargetId()), vars)]);
        effect.setDecrement(EditorGUILayout.IntField(effect.getDecrement()));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("DecrementVarEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as DecrementVarEffect; } }
    public string EffectName { get { return TC.get("DecrementVarEffect.Title"); } }
    public EffectEditor clone() { return new DecrementVarEffectEditor(); }

    public bool manages(AbstractEffect c)
    {

        return c.GetType() == effect.GetType();
    }
}
