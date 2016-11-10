using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class GenerateObjectEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] items;
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

    private GenerateObjectEffect effect;

    public GenerateObjectEffectEditor()
    {
        items = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs();
        this.effect = new GenerateObjectEffect(items[0]);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name19"));

        effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("GenerateObject.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as GenerateObjectEffect; } }
    public string EffectName { get { return TC.get("Effect.GenerateObject"); } }
    public EffectEditor clone() { return new GenerateObjectEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
