using UnityEngine;
using UnityEditor;
using System;

public class ShowTextEffectEditor : EffectEditor
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

    private ShowTextEffect effect;

    private int x = 300, y = 300;

    public ShowTextEffectEditor()
    {
        this.effect = new ShowTextEffect("", x, y, ColorConverter.ColorToHex(Color.white), ColorConverter.ColorToHex(Color.black));
    }

    public void draw()
    {

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("ConversationEditor.Line"));
        effect.setText(EditorGUILayout.TextField(effect.getText()));
        EditorGUILayout.LabelField("X: ");
        x = EditorGUILayout.IntField(effect.getX());
        EditorGUILayout.LabelField("Y: ");
        y = EditorGUILayout.IntField(effect.getY());

        effect.setTextPosition(x,y);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("ShowTextEffect.Title"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as ShowTextEffect; } }
    public string EffectName { get { return TC.get("Effect.ShowText"); } }
    public EffectEditor clone() { return new ShowTextEffectEditor(); }

    public bool manages(AbstractEffect c)
    {

        return c.GetType() == effect.GetType();
    }
}
