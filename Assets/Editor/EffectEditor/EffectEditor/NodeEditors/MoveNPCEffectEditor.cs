using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;

public class MoveNPCEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] npc;
    private string xString = "", yString = "";

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

    private MoveNPCEffect effect;

    public MoveNPCEffectEditor()
    {
        npc = Controller.getInstance().getSelectedChapterDataControl().getNPCsList().getNPCsIDs();
        this.effect = new MoveNPCEffect(npc[0], 300, 300);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name28"));
        effect.setTargetId(npc[EditorGUILayout.Popup(Array.IndexOf(npc, effect.getTargetId()), npc)]);
        EditorGUILayout.EndHorizontal();

        xString = effect.getX().ToString();
        yString = effect.getY().ToString();

        xString = EditorGUILayout.TextField(xString);
        xString = Regex.Replace(xString, "[^0-9]", "");

        yString = EditorGUILayout.TextField(yString);
        yString = Regex.Replace(xString, "[^0-9]", "");
        effect.setDestiny(int.Parse(xString), int.Parse(yString));

        EditorGUILayout.HelpBox(TC.get("MoveNPCEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as MoveNPCEffect; } }
    public string EffectName { get { return TC.get("MoveNPCEffect.Title"); } }
    public EffectEditor clone() { return new MoveNPCEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
