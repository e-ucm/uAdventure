using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class MacroReferenceEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] macros;

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

    private MacroReferenceEffect effect;

    public MacroReferenceEffectEditor()
    {
        macros = Controller.getInstance().getAdvancedFeaturesController().getMacrosListDataControl().getMacrosIDs();
        this.effect = new MacroReferenceEffect(macros[0]);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name56"));
        effect.setTargetId(macros[EditorGUILayout.Popup(Array.IndexOf(macros, effect.getTargetId()), macros)]);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("Effect.MacroReference"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as MacroReferenceEffect; } }
    public string EffectName { get { return TC.get("MacroReferenceEffect.Title"); } }
    public EffectEditor clone() { return new MacroReferenceEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
