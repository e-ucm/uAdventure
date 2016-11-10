using System;
using UnityEngine;
using UnityEditor;


public class WaitTimeEffectEditor : EffectEditor
{
    private bool collapsed = false;

    public bool Collapsed
    {
        get { return collapsed; }
        set { collapsed = value; }
    }

    private Rect window = new Rect(0, 0, 300, 0);
    private int time = 0;

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

    private WaitTimeEffect effect;

    public WaitTimeEffectEditor()
    {
        this.effect = new WaitTimeEffect(time);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Effect.WaitTime"));
        time = EditorGUILayout.IntField(effect.getTime());
        effect.setTime(time);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("WaitTimeEffect.Label"), MessageType.Info);
    }

    public AbstractEffect Effect
    {
        get { return effect; }
        set { effect = value as WaitTimeEffect; }
    }

    public string EffectName
    {
        get { return TC.get("Effect.WaitTime"); }
    }

    public EffectEditor clone()
    {
        return new WaitTimeEffectEditor();
    }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}