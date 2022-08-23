using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ConversationEditor : CollapsibleGraphEditor<ConversationDataControl, ConversationNodeDataControl>
    {
        private readonly Dictionary<ConversationNodeDataControl, ConversationNodeEditor> editors = new Dictionary<ConversationNodeDataControl, ConversationNodeEditor>();

        protected override ConversationNodeDataControl[] ChildsFor(ConversationDataControl Content, ConversationNodeDataControl parent)
        {
            return parent.getChilds().ToArray();
        }

        protected override void DeleteNode(ConversationDataControl content, ConversationNodeDataControl node)
        {
            if (content.deleteNode(node) && Selection.Contains(node))
            {
                Selection.Remove(node);
            }
        }

        protected override void DrawOpenNodeContent(ConversationDataControl content, ConversationNodeDataControl node)
        {
            ConversationNodeEditor editor = null;
            if(editors.TryGetValue(node, out editor))
            {
                editor.draw();
            }
        }

        protected override void DrawNodeControls(ConversationDataControl content, ConversationNodeDataControl node)
        {
            ConversationNodeEditor editor = null;
            editors.TryGetValue(node, out editor);

            string[] editorNames = ConversationNodeEditorFactory.Intance.CurrentConversationNodeEditors;
            int preEditorSelected = ConversationNodeEditorFactory.Intance.ConversationNodeEditorIndex(node);
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);

            if (editor == null || preEditorSelected != editorSelected)
            {
                bool firstEditor = (editor == null);
                var intRect = node.getEditorRect();
                var prevRect = (editor != null) ? editor.Window : new Rect(intRect.x, intRect.y, intRect.width, intRect.height);
                editor = ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(content, editorNames[editorSelected]);
                editor.setParent(this);
                editor.Window = prevRect;
                if (firstEditor) editor.Node = node;
                else setNode(content, node, editor.Node);

                editors.Remove(node);
                editors[editor.Node] = editor;

                if (Selection.Contains(node))
                {
                    Selection.Remove(node);
                    Selection.Add(editor.Node);
                }
            }

            base.DrawNodeControls(content, node);
        }

        private bool setNode(ConversationDataControl content, ConversationNodeDataControl oldNode, ConversationNodeDataControl newNode)
        {
            return Content.replaceNode(oldNode, newNode);
        }      

        protected override ConversationNodeDataControl[] GetNodes(ConversationDataControl Content)
        {
            return Content.getAllNodes().ToArray();
        }

        protected override string GetTitle(ConversationDataControl Content, ConversationNodeDataControl node)
        {
            return node.ToString();
        }

        protected override void SetNodeChild(ConversationDataControl content, ConversationNodeDataControl node, int slot, ConversationNodeDataControl child)
        {
            content.linkNode(node, child, slot);
        }

        protected override void SetCollapsed(ConversationDataControl Content, ConversationNodeDataControl node, bool collapsed)
        {
            node.setEditorCollapsed(collapsed);
        }

        protected override bool IsCollapsed(ConversationDataControl Content, ConversationNodeDataControl node)
        {
            return node.getEditorCollapsed();
        }

        protected override Rect GetOpenedNodeRect(ConversationDataControl content, ConversationNodeDataControl node)
        {
            var intRect = node.getEditorRect();
            var rect = new Rect(intRect.x, intRect.y, intRect.width, intRect.height);
            return editors.ContainsKey(node) ? editors[node].Window : rect;
        }

        protected override void SetNodePosition(ConversationDataControl content, ConversationNodeDataControl node, Vector2 position)
        {
            var nodeRect = node.getEditorRect();
            nodeRect.position = new Vector2Int((int)position.x, (int)position.y);
            node.setEditorRect(nodeRect);

            if (editors.ContainsKey(node))
            {
                var rect = editors[node].Window;
                rect.position = position;
                editors[node].Window = rect;
            }
        }

        protected override void SetNodeSize(ConversationDataControl content, ConversationNodeDataControl node, Vector2 size)
        {
            var nodeRect = node.getEditorRect();
            nodeRect.size = new Vector2Int((int)size.x, (int)size.y);
            node.setEditorRect(nodeRect);

            if (editors.ContainsKey(node))
            {
                var rect = editors[node].Window;
                rect.size = size;
                editors[node].Window = rect;
            }
        }

        protected override void MoveNodes(ConversationDataControl Content, IEnumerable<ConversationNodeDataControl> nodes, Vector2 delta)
        {
            Content.moveNodes(nodes.First(), nodes.ToList(), new Vector2Int(Mathf.RoundToInt(delta.x), Mathf.RoundToInt(delta.y)));
        }

        protected override void OnDrawLine(ConversationDataControl content, ConversationNodeDataControl originNode, ConversationNodeDataControl destinationNode, Rect originRect, Rect destinationRect, bool isHovered, bool isRemoving)
        {
            var button = new Rect(0, 0, 20, 20);

            if(destinationNode == null)
            {
                button.center = Event.current.mousePosition;
                if (GUI.Button(button, "+"))
                {
                    ShowOptionsForNode(originNode, true, lookingChildNode.getChildCount() > lookingChildSlot, selected =>
                    {
                        if(selected > 0)
                        {
                            Content.linkNode(lookingChildNode, selected, lookingChildSlot);
                        }
                        if(selected == REMOVE_OPTION)
                        {
                            Content.deleteNodeLink(lookingChildNode, lookingChildSlot);
                        }
                        lookingChildNode = null;
                        lookingChildSlot = -1;
                    });
                }
            }


            if(lookingChildNode != null)
            {
                return;
            }

            button.center = (originRect.position + new Vector2(originRect.width, originRect.height / 2f) + destinationRect.position + new Vector2(0, destinationRect.height/2f)) / 2f;
            if (GUI.Button(button, "+"))
            {
                ShowOptionsForNode(originNode, true, false, selected =>
                {
                    if (selected != CANCEL_OPTION)
                    {
                        int index = originNode == null ? 0 : originNode.getChilds().IndexOf(destinationNode);
                        Content.insertNode(originNode, selected, index);
                    }
                });
            }
        }

        private const int CANCEL_OPTION = -1;
        private const int REMOVE_OPTION = -2;

        private static void ShowOptionsForNode(ConversationNodeDataControl node, bool showCancel, bool showRemove, Action<int> onOptionSelected)
        {
            var optionsDict = new Dictionary<int, string> { };
            var addableNodes = node != null ? node.getAddableNodes() : new int[] { Controller.CONVERSATION_DIALOGUE_LINE, Controller.CONVERSATION_OPTION_LINE };
            foreach (var addable in addableNodes)
            {
                optionsDict.Add(addable, "Create " + TC.get("Element.Name" + addable));
            }

            if (showRemove)
            {
                optionsDict.Add(REMOVE_OPTION, "Remove link");
            }

            if (showCancel)
            {
                optionsDict.Add(CANCEL_OPTION, "Cancel asignation");
            }

            if(optionsDict.Count == 1)
            {
                onOptionSelected(optionsDict.Keys.First());
            }
            else
            {
                EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.one), 
                    optionsDict.Values.Select(v => new GUIContent(v)).ToArray(), -1, (param, ops, selected) =>
                {
                    var d = param as int[];
                    onOptionSelected(d[selected]);
                }, optionsDict.Keys.ToArray());
            }
        }
    }

    public class ConversationEditorWindow : EditorWindow
    {
        /*******************
		 *  PROPERTIES
		 * *****************/
        private ConversationEditor conversationEditor;
        public ConversationDataControl Conversation { get; set; }
        

        /*******************************
		 * Initialization methods
		 ******************************/
        public void Init(ConversationDataControl conversation)
        {
            Conversation = conversation;

            ConversationNodeEditorFactory.Intance.ResetInstance();
            conversationEditor = CreateInstance<ConversationEditor>();
            conversationEditor.BeginWindows = BeginWindows; 
            conversationEditor.EndWindows = EndWindows;
            conversationEditor.Repaint = Repaint;
            conversationEditor.Init(conversation);
        }

        

        /******************
		 * Window behaviours
		 * ******************/

        protected void OnGUI()
        {
            if (Conversation == null)
            {
                this.Close();
            }
            
            this.wantsMouseMove = true;
            
            GUILayout.BeginVertical(GUILayout.Height(20));
            Conversation.setId(EditorGUILayout.TextField(TC.get("Conversation.Title"), Conversation.getId(),
                GUILayout.ExpandWidth(true)));
            GUILayout.EndVertical();

            conversationEditor.OnInspectorGUI();
        }
    }
}