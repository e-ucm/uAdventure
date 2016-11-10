using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class HighlightItemEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] items;
    private string[] higlightTypes = {TC.get("HighlightItemEffect.None"), TC.get("HighlightItemEffect.Blue"), TC.get("HighlightItemEffect.Red"), TC.get("HighlightItemEffect.Green"), TC.get("HighlightItemEffect.Border")};

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

    private HighlightItemEffect effect;

    public HighlightItemEffectEditor()
    {
        items = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs();
        this.effect = new HighlightItemEffect(items[0], 0, false);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name19"));
        effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);
        effect.setHighlightType(EditorGUILayout.Popup(Array.IndexOf(higlightTypes, effect.getHighlightType()), higlightTypes));
        effect.setHighlightAnimated(GUILayout.Toggle(effect.isHighlightAnimated(), TC.get("HighlightItemEffect.Animated")));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("HighlightItemEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as HighlightItemEffect; } }
    public string EffectName { get { return TC.get("HighlightItemEffect.Title"); } }
    public EffectEditor clone() { return new HighlightItemEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
