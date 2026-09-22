using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System.Text.RegularExpressions;
using System.Linq;
using UniRx;
using System;

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

        private readonly DataControlList linesList;
        private DialogNodeDataControl myNode;
        private IDisposable disposable;

        private ConversationEditor parent;

        private readonly Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex;
        private readonly GUISkin noBackgroundSkin;
        private readonly GUIStyle closeStyle, buttonstyle;

        public Rect Window
        {
            get
            {
                return myNode.getEditorRect().ToRect();
            }
            set
            {
                value.width = Mathf.Max(value.width, 200);
                myNode.setEditorRect(value.ToRectInt());
            }
        }
        public DialogNodeEditor()
        {

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

            linesList = new DataControlList
            {
                Columns = new List<ColumnList.Column>
                {
                    new ColumnList.Column
                    {
                        Text = "Speaker",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(60) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Emote",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(30) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Line",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MinWidth(250), GUILayout.ExpandWidth(true)  }
                    },
                    new ColumnList.Column
                    {
                        Text = "Res.",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(25) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Cond.",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(30) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    var line = this.linesList.list[index] as ConversationLineDataControl;
                    BubbleType bubbleType = GetBubbleType(line);
                    var text = line.getText();

                    // Mark the line as selected as soon as any click is performed in the line
                    if(Event.current.isMouse && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        linesList.index = index;
                    }

                    // Extract the bubble type
                    if (bubbleType.Identifier != "-")
                    {
                        text = text.Remove(0, bubbleType.Identifier.Length + 2);
                    }

                    switch (column)
                    {
                        case 0: // Speaker

                            var npc = new List<string> { TC.get("ConversationLine.PlayerName") };
                            npc.AddRange(Controller.Instance.IdentifierSummary.getIds<NPC>());

                            var speaker = line.getName().Equals("Player") ? 0 : npc.IndexOf(line.getName());
                            EditorGUI.BeginChangeCheck();
                            var newSpeaker = EditorGUI.Popup(rect, speaker, npc.ToArray());
                            if (EditorGUI.EndChangeCheck())
                            {
                                var newSpeakerName = newSpeaker == 0 ? "Player" : npc[newSpeaker];
                                line.setName(newSpeakerName);
                            }
                            break;

                        case 1: // Bubble type
                            // Control
                            if (GUI.Button(rect, bubbleType.Identifier))
                            {
                                bubbleType = BubbleTypes.FirstOrDefault(b => bubbleType.Next == b.Identifier);
                                // Reinsert the bubble type
                                if (bubbleType.Identifier != "-")
                                {
                                    text = "#" + bubbleType.Identifier + " " + text;
                                }
                                line.setText(text);
                            }

                            break;
                        case 2: // Text
                            EditorGUI.BeginChangeCheck();
                            var newText = EditorGUI.TextField(rect, text);
                            if(EditorGUI.EndChangeCheck())
                            {
                                // Reinsert the bubble type
                                if (bubbleType.Identifier != "-")
                                {
                                    newText = "#" + bubbleType.Identifier + " " + newText;
                                }
                                line.setText(newText);
                            }
                            break;
                        case 3: // Conditions
                            var hasResources = line.isValidAudio() || line.isValidImage();
                            ResourcesPopup.DoResourcesButton(rect, line, false, hasResources, noBackgroundSkin.button);
                            break;
                        case 4: // Conditions
                            var hasConditions = line.getConditions().getBlocksCount() > 0;
                            if (GUI.Button(rect, hasConditions ? conditionsTex : noConditionsTex, noBackgroundSkin.button))
                            {
                                ConditionEditorWindow.ShowAtPosition(line.getConditions(), new Rect(rect.x + rect.width, rect.y, 0, 0));
                            }
                            break;
                    }
                }
            };
        }
        public void setParent(ConversationEditor parent)
        {
            this.parent = parent;
            if (Node == null || !parent.Content.getAllNodes().Contains(Node))
            {
                Node = parent.Content.getNodeDataControl(new DialogueConversationNode()) as DialogNodeDataControl;
            }
        }

        public void draw()
        {
            using (new GUILayout.VerticalScope())
            {
                EditorGUILayout.HelpBox(TC.get("ConversationEditor.AtLeastOne"), MessageType.None);

                var min = linesList.headerHeight + linesList.footerHeight + linesList.elementHeight + 5;
                linesList.DoList(Mathf.Max(min, Mathf.Min(200, linesList.elementHeight * (linesList.count - 1) + min)));

                EditorGUILayout.HelpBox(TC.get("ConversationEditor.NodeOption"), MessageType.None);

                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledGroupScope(myNode.getChildCount() > 0 || myNode.getAddableNodes().Length == 0))
                    {
                        if (GUILayout.Button(TC.get("ConversationEditor.CreateChild")))
                        {
                            var options = new List<GUIContent>();
                            foreach (var addable in Node.getAddableNodes())
                            {
                                options.Add(new GUIContent("Create " + TC.get("Element.Name" + addable)));
                            }

                            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.one), options.ToArray(), -1, (param, ops, selected) =>
                            {
                                var option = Node.getAddableNodes()[selected];
                                parent.Content.addNode(myNode, option);
                            }, Event.current.mousePosition);
                        }
                    }

                    if (GUILayout.Button(TC.get("ConversationEditor.SetChild")))
                    {
                        parent.StartSetChild(this.myNode, 0);
                    }

                    var hasEffects = myNode.getEffects().getEffects().Count > 0;
                    if (GUILayout.Button(hasEffects ? effectTex : noEffectTex, noBackgroundSkin.button, GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        EffectEditorWindow window = (EffectEditorWindow)ScriptableObject.CreateInstance(typeof(EffectEditorWindow));
                        window.Init(myNode.getEffects());
                    }
                }
            }
        }

        public ConversationNodeDataControl Node { get { return myNode; }
            set {
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                myNode = value as DialogNodeDataControl;
                disposable = myNode.Subscribe(_ => UpdateList());
                UpdateList();
            }
        }

        private void UpdateList()
        {
            linesList.SetData(myNode, (node) => (node as ConversationNodeDataControl).getLines().Cast<DataControl>().ToList());
        }

        public string NodeName { get { return "Dialog"; } }
        public ConversationNodeEditor clone() { return new DialogNodeEditor(); }

        public bool manages(ConversationNodeDataControl c)
        {
            return c is DialogNodeDataControl;
        }

        private static BubbleType GetBubbleType(ConversationLineDataControl line)
        {
            var matched = ExString.Default(Regex.Match(line.getText(), @"^#([^\s]+)").Groups[1].Value, "-");
            var bubbleType = BubbleTypes.FirstOrDefault(b => matched == b.Identifier);
            if (string.IsNullOrEmpty(bubbleType.Identifier))
            {
                bubbleType = new BubbleType(matched);
            }

            return bubbleType;
        }
    }
}