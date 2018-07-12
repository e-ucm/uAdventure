using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System.Text.RegularExpressions;
using System.Linq;

namespace uAdventure.Editor
{
    public class DialogNodeEditor : ConversationNodeEditor
    {
        private struct BubbleType
        {
            public BubbleType(string identifier)
            {
                Identifier = identifier;
                Help = "Unknown";
                Next = "-";
            }

            public string Identifier;
            public string Help;
            public string Next;
        }

        private static BubbleType[] BubbleTypes = new BubbleType[]
        {
            new BubbleType(){
                Identifier = "-",
                Help = "Normal",
                Next = "!"
            },
            new BubbleType(){
                Identifier = "!",
                Help = "Yell",
                Next = "O"
            },
            new BubbleType(){
                Identifier = "O",
                Help = "Think",
                Next = "-"
            }
        };

        private DialogueConversationNode myNode;
        private Vector2 scroll;
        private readonly List<string> npc;

        private readonly Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex;
        private readonly GUISkin noBackgroundSkin;
        private readonly GUIStyle closeStyle, buttonstyle;

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

        public DialogNodeEditor()
        {
            myNode = new DialogueConversationNode();
            npc = new List<string> { "Player" };
            if (Controller.Instance.SelectedChapterDataControl!= null)
            {
                npc.AddRange(Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCsIDs());
            }

            conditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/conditions-24x24");
            noConditionsTex = Resources.Load<Texture2D>("EAdventureData/img/icons/no-conditions-24x24");

            effectTex = Resources.Load<Texture2D>("EAdventureData/img/icons/effects/32x32/has-macro");
            noEffectTex = Resources.Load<Texture2D>("EAdventureData/img/icons/effects/32x32/macro");

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

            buttonstyle = new GUIStyle()
            {
                padding = new RectOffset(5, 5, 5, 5)
            };
        }

        ConversationEditor parent;
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

            EditorGUILayout.HelpBox(TC.get("ConversationEditor.AtLeastOne"), MessageType.None);
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
                    var line = myNode.getLine(i);
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(TC.get("ConversationEditor.Speaker"))).x;
                    line.setName(npc[EditorGUILayout.Popup(TC.get("ConversationEditor.Speaker"), npc.IndexOf(line.getName()), npc.ToArray())]);
                    
                    // Bubble type extraction
                    var matched = ExString.Default(Regex.Match(line.getText(), @"^#([^\s]+)").Groups[1].Value, "-");
                    var bubbleType = BubbleTypes.FirstOrDefault(b => matched == b.Identifier);
                    if (string.IsNullOrEmpty(bubbleType.Identifier))
                    {
                        bubbleType = new BubbleType(matched);
                    }

                    if (bubbleType.Identifier != "-")
                        line.setText(line.getText().Remove(0, matched.Length + 2));
                    
                    // Bubble type control
                    if (GUILayout.Button(bubbleType.Identifier, GUILayout.Width(19), GUILayout.Height(14)))
                    {
                        bubbleType = BubbleTypes.FirstOrDefault(b => bubbleType.Next == b.Identifier);
                    }

                    EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(new GUIContent(TC.get("ConversationEditor.Line"))).x;
                    line.setText(EditorGUILayout.TextField(TC.get("ConversationEditor.Line"), line.getText(), GUILayout.Width(200)));

                    // Bubble type reinsert
                    if (bubbleType.Identifier != "-")
                    {
                        line.setText("#" + bubbleType.Identifier + " " + line.getText());
                    }

                    var hasConditions = line.getConditions().GetConditionsList().Count > 0;

                    if (GUILayout.Button(hasConditions ? conditionsTex : noConditionsTex, noBackgroundSkin.button, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        ConditionEditorWindow window = (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                        window.Init(line.getConditions());
                    }

                    if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        myNode.removeLine(i);
                    }
                    EditorGUILayout.EndHorizontal();

                }
                if (isScrolling)
                {
                    EditorGUILayout.EndScrollView();
                }
            }


            GUIContent bttext = new GUIContent(TC.get("ConversationLine.DefaultText"));
            Rect btrect = GUILayoutUtility.GetRect(bttext, style);
            if (GUI.Button(btrect, bttext))
            {
                myNode.addLine(new ConversationLine(TC.get("ConversationLine.PlayerName"), ""));
            }

            EditorGUILayout.HelpBox(TC.get("ConversationEditor.NodeOption"), MessageType.None);

            EditorGUILayout.BeginHorizontal();
            GUI.enabled = (myNode.getChildCount() == 0) || myNode.getType() == ConversationNodeViewEnum.OPTION;
            bttext = new GUIContent(TC.get("ConversationEditor.CreateChild"));
            btrect = GUILayoutUtility.GetRect(bttext, buttonstyle);
            if (GUI.Button(btrect, bttext))
            {
                parent.addChild(myNode, new DialogueConversationNode());
            }
            GUI.enabled = true;

            bttext = new GUIContent(TC.get("ConversationEditor.SetChild"));
            btrect = GUILayoutUtility.GetRect(bttext, buttonstyle);
            if (GUI.Button(btrect, bttext))
            {
                parent.StartSetChild(this.myNode, 0);
            }

            var hasEffects = myNode.getEffects().getEffects().Count > 0;
            if (GUILayout.Button(hasEffects ? effectTex : noEffectTex, noBackgroundSkin.button, GUILayout.Width(24), GUILayout.Height(24)))
            {
                EffectEditorWindow window = (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                window.Init(myNode.getEffects());
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        public ConversationNode Node { get { return myNode; } set { myNode = value as DialogueConversationNode; } }
        public string NodeName { get { return "Dialog"; } }
        public ConversationNodeEditor clone() { return new DialogNodeEditor(); }

        public bool manages(ConversationNode c)
        {
            return c.GetType() == myNode.GetType();
        }
    }
}