using System;
using UnityEngine;
using UnityEditor;

public class TriggerBookEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
    private string[] books;
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

    private TriggerBookEffect effect;

    public TriggerBookEffectEditor()
    {
        books = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooksIDs();
        this.effect = new TriggerBookEffect(books[0]);
    }

    public void draw()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(TC.get("Element.Name12"));

        effect.setTargetId(books[EditorGUILayout.Popup(Array.IndexOf(books, effect.getTargetId()), books)]);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(TC.get("TriggerBookEffect.Description"), MessageType.Info);
    }

    public AbstractEffect Effect { get { return effect; } set { effect = value as TriggerBookEffect; } }
    public string EffectName { get { return TC.get("TriggerBookEffect.Title"); } }
    public EffectEditor clone() { return new TriggerBookEffectEditor(); }

    public bool manages(AbstractEffect c)
    {
        return c.GetType() == effect.GetType();
    }
}
