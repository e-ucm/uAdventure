using UnityEngine;
using UnityEditor;
using System.Collections;

public class SpeakPlayerEffectEditor : EffectEditor
{
    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
    private Rect window = new Rect(0, 0, 300, 0);
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

    private SpeakPlayerEffect effect;

    public SpeakPlayerEffectEditor(){
        this.effect = new SpeakPlayerEffect ("");
    }

    public void draw(){

        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.LabelField (TC.get("ConversationEditor.Line"));

        effect.setLine (EditorGUILayout.TextField (effect.getLine ()));

        EditorGUILayout.EndHorizontal ();

        EditorGUILayout.HelpBox(TC.get("SpeakPlayerEffect.Description"),MessageType.Info);
    }

    public AbstractEffect Effect { get{ return effect; } set { effect = value as SpeakPlayerEffect; } }
    public string EffectName{ get { return TC.get("SpeakPlayerEffect.Title"); } }
    public EffectEditor clone(){ return new SpeakPlayerEffectEditor(); }

    public bool manages(AbstractEffect c) { 

        return c.GetType() == effect.GetType();
    }
}
