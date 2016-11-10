using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;


public class MoveObjectEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] items;
    private string xString = "", yString = "", translateSpeed ="", scaleSpeed = "";
    private float floatScale ;

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

    private MoveObjectEffect effect;

    public MoveObjectEffectEditor()
    {
        items = Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItemsIDs();
        this.effect = new MoveObjectEffect(items[0], 300, 300, 1.0f, false, 1, 1);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name19"));
        effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);
        EditorGUILayout.EndHorizontal();

        xString = effect.getX().ToString();
        yString = effect.getY().ToString();
        translateSpeed = effect.getTranslateSpeed().ToString();
        scaleSpeed = effect.getScaleSpeed().ToString();
        floatScale = effect.getScale();

        xString = EditorGUILayout.TextField(xString);
        xString = Regex.Replace(xString, "[^0-9]", "");

        yString = EditorGUILayout.TextField(yString);
        yString = Regex.Replace(xString, "[^0-9]", "");

        translateSpeed = EditorGUILayout.TextField(translateSpeed);
        translateSpeed = Regex.Replace(translateSpeed, "[^0-9]", "");

        scaleSpeed = EditorGUILayout.TextField(scaleSpeed);
        scaleSpeed = Regex.Replace(scaleSpeed, "[^0-9]", "");

        floatScale = EditorGUILayout.FloatField(floatScale);

        effect.setX(int.Parse(xString));
        effect.setY(int.Parse(yString));
        effect.setTranslateSpeed(int.Parse(translateSpeed));
        effect.setScaleSpeed(int.Parse(scaleSpeed));
        effect.setScale(floatScale);

        EditorGUILayout.HelpBox(TC.get("MoveObjectEffect.Title"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as MoveObjectEffect; } }
    public string EffectName { get { return TC.get("Effect.MoveObject"); } }
    public EffectEditor clone() { return new MoveObjectEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
