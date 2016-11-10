using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class DeactivateEffectEditor : EffectEditor {
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] flags;
    public Rect Window
    {
        get {
            if (collapsed) return new Rect(window.x, window.y, 50, 30);
            else           return window; 
        }
        set {
            if (collapsed) window = new Rect(value.x, value.y, window.width, window.height);
            else           window = value; 
        }
    }

    private DeactivateEffect effect;

    public DeactivateEffectEditor()
    {
        flags = Controller.getInstance().getVarFlagSummary().getFlags();
        this.effect = new DeactivateEffect (flags[0]);
    }

    public void draw(){

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (TC.get("Condition.FlagID"));

        effect.setTargetId(flags[EditorGUILayout.Popup(Array.IndexOf(flags, effect.getTargetId()), flags)]);

        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.HelpBox(TC.get("DeactivateEffect.Description"),MessageType.Info);
    }

    public AbstractEffect Effect { get{ return effect; } set { effect = value as DeactivateEffect; } }
    public string EffectName{ get { return TC.get("DeactivateEffect.Title"); } }
    public EffectEditor clone(){ return new DeactivateEffectEditor(); }

    public bool manages(AbstractEffect c) { 

        return c.GetType() == effect.GetType();
    }
}
