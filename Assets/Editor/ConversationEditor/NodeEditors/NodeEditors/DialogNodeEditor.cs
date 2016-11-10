using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DialogNodeEditor : ConversationNodeEditor {

	private DialogueConversationNode myNode;
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

    Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex, tmpTex;
    GUISkin noBackgroundSkin, defaultSkin;
    GUIStyle closeStyle, buttonstyle;
    public DialogNodeEditor(){
        myNode = new DialogueConversationNode ();
        npc = new List<string> ();
        npc.Add ("Player");
        if(Controller.getInstance ().getSelectedChapterDataControl ()!=null)
            npc.AddRange (Controller.getInstance ().getSelectedChapterDataControl ().getNPCsList ().getNPCsIDs ());

        conditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof (Texture2D));
        noConditionsTex = (Texture2D) Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof (Texture2D));

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

        if (buttonstyle == null) {
            buttonstyle = new GUIStyle ();
            buttonstyle.padding = new RectOffset (5, 5, 5, 5);
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
				//myNode.getLine(i).IsEntityFragment = EditorGUILayout.Toggle("Is entity: ", frg.IsEntityFragment);

				bool showInfo = false;
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(TC.get("ConversationEditor.Speaker"))).x;
                myNode.getLine(i).setName(npc[EditorGUILayout.Popup(TC.get("ConversationEditor.Speaker"),npc.IndexOf(myNode.getLine (i).getName ()), npc.ToArray())]);

                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(TC.get("ConversationEditor.Line"))).x;
                myNode.getLine(i).setText(EditorGUILayout.TextField(TC.get("ConversationEditor.Line"), myNode.getLine(i).getText(),GUILayout.Width(200)));

                tmpTex = (myNode.getLine(i).getConditions().getConditionsList().Count > 0
                    ? conditionsTex
                    : noConditionsTex);
                
                if (GUILayout.Button(tmpTex,noBackgroundSkin.button,GUILayout.Width(15),GUILayout.Height(15))){
                    ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(myNode.getLine(i).getConditions());
                }
					
                if(GUILayout.Button ("X", closeStyle, GUILayout.Width (15), GUILayout.Height (15))){
                    myNode.removeLine(i);
				};
				EditorGUILayout.EndHorizontal();
				
			}
			if(isScrolling)
				EditorGUILayout.EndScrollView();
		}

		
		GUIContent bttext = new GUIContent(TC.get("ConversationLine.DefaultText"));
		Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
		if(GUI.Button(btrect,bttext)){
            myNode.addLine(new ConversationLine(TC.get("ConversationLine.PlayerName"),""));
		};


        EditorGUILayout.HelpBox(TC.get("ConversationEditor.NodeOption"), MessageType.None);

        EditorGUILayout.BeginHorizontal ();
        GUI.enabled = (myNode.getChildCount () == 0) || myNode.getType() == ConversationNodeViewEnum.OPTION;
        bttext = new GUIContent (TC.get("ConversationEditor.CreateChild"));
        btrect = GUILayoutUtility.GetRect (bttext, buttonstyle);      
        if (GUI.Button (btrect, bttext)) {
            parent.addChild (myNode, new DialogueConversationNode ());
        }
        GUI.enabled = true;

        bttext = new GUIContent (TC.get("ConversationEditor.SetChild"));
        btrect = GUILayoutUtility.GetRect (bttext, buttonstyle);      
        if (GUI.Button (btrect, bttext)){
            parent.startSetChild (this.myNode, 0);
        }
            
        tmpTex = (myNode.getEffects().getEffects().Count > 0
            ? effectTex
            : noEffectTex);
        if (GUILayout.Button (tmpTex, noBackgroundSkin.button,GUILayout.Width(24),GUILayout.Height(24))){
            EffectEditorWindow window = (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
            window.Init(myNode.getEffects());
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical ();
	}
	
    public ConversationNode Node { get { return myNode; } set { myNode = value as DialogueConversationNode; } }
    public string NodeName { get { return "Dialog"; } }
    public ConversationNodeEditor clone() { return new DialogNodeEditor(); }

    public bool manages(ConversationNode c)
    {
        return c.GetType() == myNode.GetType();
    }
}