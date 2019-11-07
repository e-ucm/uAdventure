using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using uAdventure.Core;
using System.Text.RegularExpressions;
using System.Linq;
using UniRx;
using System;
using Microsoft.Msagl.Core.Layout.ProximityOverlapRemoval.ConjugateGradient;

namespace uAdventure.Editor
{
    public class OptionNodeEditor : ConversationNodeEditor
    {
        private OptionNodeDataControl myNode;

        private readonly Texture2D conditionsTex, noConditionsTex, effectTex, noEffectTex, linkTex, noLinkTex, answerTex, shuffleTex, questionTex;
        private readonly GUISkin noBackgroundSkin;
        private readonly GUIStyle closeStyle;
        private readonly GUIContent answerContent, questionContent, shuffleContent;

        private ConversationEditor parent;
        private readonly DataControlList linesList;
        private IDisposable disposable;

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

        public OptionNodeEditor()
        {
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

            linesList = new DataControlList
            {
                Columns = new List<ColumnList.Column>
                {
                    new ColumnList.Column
                    {
                        Text = "",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(20) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Text",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.ExpandWidth(true), GUILayout.MinWidth(100) }
                    },
                    new ColumnList.Column
                    {
                        Text = "¿✓?",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(25) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Res.",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(25) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Cond.",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(25) }
                    },
                    new ColumnList.Column
                    {
                        Text = "Child",
                        SizeOptions = new GUILayoutOption[]{ GUILayout.MaxWidth(25) }
                    }
                },
                drawCell = (rect, index, column, isActive, isFocused) =>
                {
                    // Mark the line as selected as soon as any click is performed in the line
                    if (Event.current.isMouse && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        linesList.index = index;
                    }

                    var line = this.linesList.list[index] as ConversationLineDataControl;
                    switch (column)
                    {
                        case 0: GUI.Label(rect, index.ToString()); break;
                        case 1: // Text
                            EditorGUI.BeginChangeCheck();
                            var newText = EditorGUI.TextField(rect, line.getText());
                            if (EditorGUI.EndChangeCheck())
                            {
                                line.setText(newText);
                            }
                            break;
                        case 2: // Correct
                            DoCorrectEditor(rect, line);
                            break;
                        case 3: // Resources
                            DoResourcesEditor(rect, line);
                            break;
                        case 4: // Conditions
                            DoConditionsEditor(rect, line.getConditions());
                            break;
                        case 5: // Child
                            DoChildEditor(rect, index);
                            break;
                    }
                }
            };
        }

        private void DoChildEditor(Rect rect, int index)
        {
            if (myNode.getChilds().Count <= index)
            {
                return;
            }

            var hasLink = myNode.getChilds()[index] != null;
            if (GUI.Button(rect, hasLink ? linkTex : noLinkTex, noBackgroundSkin.button))
            {
                parent.StartSetChild(this.myNode, index);
            }
        }

        private void DoResourcesEditor(Rect rect, ConversationLineDataControl data)
        {
            var hasResources = data.isValidAudio() || data.isValidImage();
            ResourcesPopup.DoResourcesButton(rect, data, false, hasResources, noBackgroundSkin.button);
        }

        private void DoConditionsEditor(Rect rect, ConditionsController conditions)
        {
            var hasConditions = conditions.getBlocksCount() > 0;
            if (GUI.Button(rect, hasConditions ? conditionsTex : noConditionsTex, noBackgroundSkin.button))
            {
                ConditionEditorWindow.ShowAtPosition(conditions, new Rect(rect.x + rect.width, rect.y, 0, 0));
            }
        }

        private static void DoCorrectEditor(Rect rect, ConversationLineDataControl line)
        {
            EditorGUI.BeginChangeCheck();
            var correct = EditorGUI.Toggle(rect, line.getXApiCorrect());
            if (EditorGUI.EndChangeCheck())
            {
                line.setXApiCorrect(correct);
            }
        }

        public void setParent(ConversationEditor parent)
        {
            this.parent = parent;
            if (Node == null || !parent.Content.getAllNodes().Contains(Node))
            {
                Node = parent.Content.getNodeDataControl(new OptionConversationNode()) as OptionNodeDataControl;
            }
        }

        public void draw()
        {
            using (new GUILayout.VerticalScope())
            {
                // Options configuration
                using (new GUILayout.HorizontalScope())
                {
                    // KeepShowing
                    questionContent.tooltip = TC.get("Conversation.KeepShowing");
                    EditorGUI.BeginChangeCheck();
                    var keepShowing = GUILayout.Toggle(myNode.KeepShowing, questionContent, "Button");
                    if (EditorGUI.EndChangeCheck())
                    {
                        myNode.KeepShowing = keepShowing;
                    }

                    // KeepShowing
                    shuffleContent.tooltip = TC.get("Conversation.OptionRandomly");
                    EditorGUI.BeginChangeCheck();
                    var random = GUILayout.Toggle(myNode.Random, shuffleContent, "Button");
                    if (EditorGUI.EndChangeCheck())
                    {
                        myNode.Random = random;
                    }

                    // Show User Option
                    answerContent.tooltip = TC.get("Conversation.ShowUserOption");
                    EditorGUI.BeginChangeCheck();
                    var showUserOption = GUILayout.Toggle(myNode.ShowUserOption, answerContent, "Button");
                    if (EditorGUI.EndChangeCheck())
                    {
                        myNode.ShowUserOption = showUserOption;
                    }
                }

                EditorGUILayout.HelpBox(TC.get("ConversationEditor.AtLeastOne"), MessageType.None);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Question ID: ");
                    EditorGUI.BeginChangeCheck();
                    var newXApiQuestion = EditorGUILayout.TextField(myNode.getXApiQuestion());
                    if (EditorGUI.EndChangeCheck())
                    {
                        myNode.setXApiQuestion(newXApiQuestion);
                    }

                    if (myNode.getXApiQuestion() == "")
                    {
                        var lastRect = GUILayoutUtility.GetLastRect();
                        var guistyle = new GUIStyle(GUI.skin.label);
                        guistyle.normal.textColor = Color.gray;
                        GUI.Label(lastRect, " Required for analytics", guistyle);
                    }
                }

                var min = linesList.headerHeight + linesList.footerHeight + linesList.elementHeight + 5;
                linesList.DoList(Mathf.Max(min, Mathf.Min(150, linesList.elementHeight * (linesList.count-1) + min)));

                // Timer
                using (new GUILayout.HorizontalScope())
                {
                    if (EditorGUILayout.Toggle("Timeout: ", myNode.Timeout >= 0))
                    {
                        myNode.Timeout = Mathf.Clamp(EditorGUILayout.FloatField(myNode.Timeout), 0.1f, float.MaxValue);
                        GUILayout.Space(5);
                        DoConditionsEditor(GUILayoutUtility.GetRect(15,15), myNode.TimeoutConditions);
                        DoChildEditor(GUILayoutUtility.GetRect(15, 15), myNode.getChildCount() - 1);
                    }
                    else
                    {
                        myNode.Timeout = -1f;
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
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
            set
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                myNode = value as OptionNodeDataControl;
                disposable = myNode.Subscribe(_ => UpdateList());
                UpdateList();
            }
        }

        private void UpdateList()
        {
            linesList.SetData(myNode, (node) => (node as ConversationNodeDataControl).getLines().Cast<DataControl>().ToList());
        }

        public string NodeName { get { return "Option"; } }
        public ConversationNodeEditor clone() { return new OptionNodeEditor(); }

        public bool manages(ConversationNodeDataControl c)
        {
            return c is OptionNodeDataControl;
        }
    }
}