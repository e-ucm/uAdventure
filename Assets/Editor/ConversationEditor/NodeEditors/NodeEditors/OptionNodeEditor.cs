using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class OptionNodeEditor : ConversationNodeEditor {

    private OptionConversationNode myNode;
    private Vector2 scroll = new Vector2(0,0);
    private List<string> npc;

    private bool collapsed = false;
    public bool Collapsed { get { return collapsed; } set { collapsed = value; } }

    private Rect window = new Rect(0, 0, 100, 0), collapsedWindow = new Rect(0,0,150,43);
    public Rect Window
    {
        get {
            if (collapsed) return new Rect(window.x, window.y, collapsedWindow.width, collapsedWindow.height);
            else           return window; 
        }
        set {
            myNode.setEditorX ((int) value.x);
            myNode.setEditorY ((int) value.y);
            if (collapsed) window = new Rect(value.x, value.y, collapsedWindow.width, collapsedWindow.height);
            else           window = value; 
        }
    }

    Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex, linkTex, noLinkTex, tmpTex;
    GUISkin noBackgroundSkin, defaultSkin;
    GUIStyle closeStyle;

    public OptionNodeEditor(){
        myNode = new OptionConversationNode ();

        conditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof (Texture2D));
        noConditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof (Texture2D));

        linkTex = (Texture2D) Resources.Load("EAdventureData/img/icons/linkNode", typeof (Texture2D));
        noLinkTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteNodeLink", typeof (Texture2D));

        effectTex = (Texture2D) Resources.Load("EAdventureData/img/icons/effects/32x32/has-macro", typeof (Texture2D));
        noEffectTex = (Texture2D) Resources.Load("EAdventureData/img/icons/effects/32x32/macro", typeof (Texture2D));

        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));
        noBackgroundSkin.button.margin = new RectOffset (1, 1, 1, 1);
        noBackgroundSkin.button.padding = new RectOffset (0, 0, 0, 0);
    }

    ConversationEditorWindow parent;
    public void setParent (ConversationEditorWindow parent){
        this.parent = parent;
    }

    public void draw(){
        if (closeStyle == null)
        {
            closeStyle = new GUIStyle(GUI.skin.button);
            closeStyle.padding = new RectOffset(0, 0, 0, 0);
            closeStyle.margin = new RectOffset(0, 5, 2, 0);
            closeStyle.normal.textColor = Color.red;
            closeStyle.focused.textColor = Color.red;
            closeStyle.active.textColor = Color.red;
            closeStyle.hover.textColor = Color.red;
        }

        GUIStyle style = new GUIStyle();
        style.padding = new RectOffset(5,5,5,5);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.HelpBox(TC.get("ConversationEditor.AtLeastOne"), MessageType.None);
        bool infoShown = false;
        if(myNode.getLineCount() > 0){
            bool isScrolling = false;

            if(myNode.getLineCount() > 10){
                scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinWidth(360), GUILayout.Height(190));
                isScrolling = true;
            }

            for(int i = 0; i < myNode.getLineCount(); i++){
                EditorGUILayout.BeginHorizontal();

                bool showInfo = false;

                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(TC.get("ConversationEditor.Option") + " " + i + ": ")).x;
                myNode.getLine(i).setText(EditorGUILayout.TextField(TC.get("ConversationEditor.Option") + " " + i + ": ", myNode.getLine(i).getText(),GUILayout.Width(200)));

                tmpTex = (myNode.getLine(i).getConditions().getConditionsList().Count > 0
                    ? conditionsTex
                    : noConditionsTex);

                if (GUILayout.Button(tmpTex,noBackgroundSkin.button,GUILayout.Width(15),GUILayout.Height(15))){
                    ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(myNode.getLine(i).getConditions());
                }

                if (GUILayout.Button(linkTex,noBackgroundSkin.button,GUILayout.Width(15),GUILayout.Height(15))){
                    parent.startSetChild (this.myNode, i);
                }


                if(GUILayout.Button ("X", closeStyle, GUILayout.Width (15), GUILayout.Height (15))){
                    myNode.removeLine(i);
                    myNode.removeChild(i);
                };
                EditorGUILayout.EndHorizontal();

            }
            if(isScrolling)
                EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.BeginHorizontal ();
        GUIContent bttext = new GUIContent(TC.get("ConversationEditor.AddOptionChild"));
        Rect btrect = GUILayoutUtility.GetRect(bttext, style);      
        if(GUI.Button(btrect,bttext)){
            myNode.addLine(new ConversationLine("Player",""));
            parent.addChild(this.myNode,new DialogueConversationNode());
        };

        tmpTex = (myNode.getEffects().getEffects().Count > 0
            ? effectTex
            : noEffectTex);
        if (GUILayout.Button (tmpTex, noBackgroundSkin.button,GUILayout.Width(24),GUILayout.Height(24))){
            EffectEditorWindow window = (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
            window.Init(myNode.getEffects());
        }
        EditorGUILayout.EndHorizontal ();
        EditorGUILayout.EndVertical ();
    }

    public ConversationNode Node { get { return myNode; } set { myNode = value as OptionConversationNode; } }
    public string NodeName { get { return "Option"; } }
    public ConversationNodeEditor clone() { return new OptionNodeEditor(); }

    public bool manages(ConversationNode c)
    {
        return c.GetType() == myNode.GetType();
    }
}