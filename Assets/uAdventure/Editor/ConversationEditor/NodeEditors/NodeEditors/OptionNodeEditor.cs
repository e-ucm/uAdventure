using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class OptionNodeEditor : ConversationNodeEditor
    {
        private OptionConversationNode myNode;
        private Vector2 scroll = new Vector2(0, 0);

        private readonly Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex, linkTex, noLinkTex, answerTex, shuffleTex, questionTex;
        private readonly GUISkin noBackgroundSkin;
        private readonly GUIStyle closeStyle;
        private readonly GUIContent answerContent, questionContent, shuffleContent;

        private ConversationEditor parent;

        public Rect Window
        {
            get
            {
                return new Rect(myNode.getEditorX(), myNode.getEditorY(), myNode.getEditorWidth(), myNode.getEditorHeight());
            }
            set
            {
                myNode.setEditorX((int)value.x);
                myNode.setEditorY((int)value.y);
                myNode.setEditorWidth((int)value.width);
                myNode.setEditorHeight((int)value.height);
            }
        }

        public OptionNodeEditor()
        {
            myNode = new OptionConversationNode();

            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

            linkTex = Resources.Load<Texture2D>("EAdventureData/img/icons/linkNode");
            noLinkTex = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteNodeLink");

            effectTex = Resources.Load<Texture2D>("EAdventureData/img/icons/effects/32x32/has-macro");
            noEffectTex = Resources.Load<Texture2D>("EAdventureData/img/icons/effects/32x32/macro");

            answerTex = Resources.Load<Texture2D>("EAdventureData/img/icons/answer");
            questionTex = Resources.Load<Texture2D>("EAdventureData/img/icons/question");
            shuffleTex = Resources.Load<Texture2D>("EAdventureData/img/icons/shuffle");

            answerContent = new GUIContent(answerTex);
            questionContent = new GUIContent(questionTex);
            shuffleContent = new GUIContent(shuffleTex);

            noBackgroundSkin = Resources.Load<GUISkin>("EAdventureData/skin/EditorNoBackgroundSkin");
            noBackgroundSkin.button.margin = new RectOffset(1, 1, 1, 1);
            noBackgroundSkin.button.padding = new RectOffset(0, 0, 0, 0);
            
            closeStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 5, 2, 0)
            };
            closeStyle.normal.textColor = Color.red;
            closeStyle.focused.textColor = Color.red;
            closeStyle.active.textColor = Color.red;
            closeStyle.hover.textColor = Color.red;
        }

        public void setParent(ConversationEditor parent)
        {
            this.parent = parent;
        }

        public void draw()
        {

            GUIStyle style = new GUIStyle()
            {
                padding = new RectOffset(5, 5, 5, 5)
            };
            EditorGUILayout.BeginVertical();
            // Options configuration
            EditorGUILayout.BeginHorizontal();
            questionContent.tooltip = TC.get("Conversation.KeepShowing");
            myNode.setKeepShowing(GUILayout.Toggle(myNode.isKeepShowing(), questionContent, "Button"));
            shuffleContent.tooltip = TC.get("Conversation.OptionRandomly");
            myNode.setRandom(GUILayout.Toggle(myNode.isRandom(), shuffleContent, "Button"));
            answerContent.tooltip = TC.get("Conversation.ShowUserOption");
            myNode.setShowUserOption(GUILayout.Toggle(myNode.isShowUserOption(), answerContent, "Button"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("ConversationEditor.AtLeastOne"), MessageType.None);
			GUILayout.BeginHorizontal ();
			GUILayout.Label("Question ID: ");
			//Controller.getInstance ().getIdentifierSummary ().add
			myNode.setXApiQuestion(EditorGUILayout.TextField(myNode.getXApiQuestion()));
			if (myNode.getXApiQuestion () == "") {
				var lastRect = GUILayoutUtility.GetLastRect ();
				var guistyle = new GUIStyle (GUI.skin.label);
				guistyle.normal.textColor = Color.gray;
				GUI.Label (lastRect, " Required for analytics", guistyle);
			}
			GUILayout.EndHorizontal ();

            if (myNode.getLineCount() > 0)
            {
                bool isScrolling = false;

                if (myNode.getLineCount() > 10)
                {
                    scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MinWidth(360), GUILayout.Height(190));
                    isScrolling = true;
                }

                for (int i = 0; i < myNode.getLineCount(); i++)
                {
                    EditorGUILayout.BeginHorizontal();

					EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent((i+1) + ": ")).x;
					myNode.getLine(i).setText(EditorGUILayout.TextField((i+1) + ": ", myNode.getLine(i).getText(), GUILayout.ExpandWidth(true)));
					myNode.getLine (i).setXApiCorrect (EditorGUILayout.Toggle(myNode.getLine (i).getXApiCorrect (), GUILayout.Width(15)));
					GUILayout.Space (5);

                    if (DrawLineOptions(i, myNode.getLine(i).getConditions()))
                    {
                        myNode.removeLine(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                // Timer

                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 0;
                if (EditorGUILayout.Toggle("Timeout: ", myNode.Timeout >= 0))
                {
                    if (myNode.Timeout < 0)
                    {
                        parent.addChild(this.myNode, new DialogueConversationNode());
                    }

                    myNode.Timeout = Mathf.Clamp(EditorGUILayout.FloatField(myNode.Timeout), 0, float.MaxValue);
                    GUILayout.Space(5);
                    if (DrawLineOptions(myNode.getChildCount(), myNode.TimeoutConditions)) 
                    {
                        myNode.Timeout = -1f;
                    }
                }
                else
                {
                    myNode.Timeout = -1f;
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (isScrolling)
                {
                    EditorGUILayout.EndScrollView();
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUIContent bttext = new GUIContent(TC.get("ConversationEditor.AddOptionChild"));
            Rect btrect = GUILayoutUtility.GetRect(bttext, style);
            if (GUI.Button(btrect, bttext))
            {
                myNode.addLine(new ConversationLine("Player", ""));
                parent.addChild(this.myNode, new DialogueConversationNode());
            }

            var hasEffects = myNode.getEffects().getEffects().Count > 0;
            if (GUILayout.Button(hasEffects ? effectTex : noEffectTex, noBackgroundSkin.button, GUILayout.Width(24), GUILayout.Height(24)))
            {
                EffectEditorWindow window = (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(myNode.getEffects());
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private bool DrawLineOptions(int optionIndex, Conditions conditions)
        {
            var hasConditions = conditions.GetConditionsList().Count > 0;

            if (GUILayout.Button(hasConditions ? conditionsTex : noConditionsTex, noBackgroundSkin.button, GUILayout.Width(15), GUILayout.Height(15)))
            {
                ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                window.Init(conditions);
            }

            var hasLink = myNode.getChild(optionIndex) != null;
            if (GUILayout.Button(hasLink ? linkTex : noLinkTex, noBackgroundSkin.button, GUILayout.Width(15), GUILayout.Height(15)))
            {
                parent.StartSetChild(this.myNode, optionIndex);
            }

            if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
            {
                myNode.removeChild(optionIndex);
                return true;
            }
            return false;
        }

        public ConversationNode Node { get { return myNode; } set { myNode = value as OptionConversationNode; } }
        public string NodeName { get { return "Option"; } }
        public ConversationNodeEditor clone() { return new OptionNodeEditor(); }

        public bool manages(ConversationNode c)
        {
            return c.GetType() == myNode.GetType();
        }
    }
}