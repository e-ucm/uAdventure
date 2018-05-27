using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ConversationEditor : CollapsibleGraphEditor<GraphConversation, ConversationNode>
    {
        private bool elegibleForClick = false;
        private Dictionary<ConversationNode, ConversationNodeEditor> editors = new Dictionary<ConversationNode, ConversationNodeEditor>();
        private GUIContent[] options = new GUIContent[] { new GUIContent("Cancel asignation"), new GUIContent("Create/Dialog Node"), new GUIContent("Create/Option Node") };

        protected override ConversationNode[] ChildsFor(GraphConversation Content, ConversationNode parent)
        {
            return Enumerable.Range(0, parent.getChildCount()).Select(parent.getChild).ToArray();
        }
        protected override void OnAfterDrawWindows()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    elegibleForClick = true;
                    break;
                case EventType.MouseUp:
                    if(elegibleForClick && lookingChildNode != null)
                    {
                        EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.one), options, -1, (param, ops, selected) =>
                        {
                            if(selected != 0)
                            {
                                var pos = (Vector2)param;
                                ConversationNode newNode = selected == 1 ? (ConversationNode) new DialogueConversationNode() : new OptionConversationNode();
                                lookingChildNode.replaceChild(lookingChildSlot, newNode);
                                newNode.setEditorX((int)pos.x);
                                newNode.setEditorY((int)pos.y);
                            }
                            lookingChildNode = null;
                            lookingChildSlot = -1;
                        }, Event.current.mousePosition);
                        elegibleForClick = false;
                    }
                    break;
                case EventType.MouseDrag:
                    elegibleForClick = false;
                    break;
            }

            base.OnAfterDrawWindows();
        }

        protected override void DeleteNode(GraphConversation content, ConversationNode node)
        {
            Stack<ConversationNode> parents = new Stack<ConversationNode>();
            Dictionary<ConversationNode, int> progress = new Dictionary<ConversationNode, int>();
            
            parents.Push(content.getRootNode());
            progress[content.getRootNode()] = 0;
            while (parents.Count > 0)
            {
                var current = parents.Peek();
                if(progress[current] == 0 && current == node)
                {
                    if(current == content.getRootNode())
                    {
                        if(current.getChildCount() == 0)
                        {
                            EditorUtility.DisplayDialog("Forbidden!", "Deleting the last node is forbidden!", "Ok...");
                            return;
                        }
                        else if (current.getChildCount() == 1 || EditorUtility.DisplayDialog("Forbidden!", "Deleting this node will keep only the first child (the rest nodes will be deleted!). Continue?", "Yes", "No"))
                        {
                            content.setRootNode(content.getRootNode().getChild(0));
                            return;
                        }
                    }

                    parents.Pop();
                    var parent = parents.Peek();
                    if (current.getChildCount() <= 1 || EditorUtility.DisplayDialog("Forbidden!", "Deleting this node will keep only the first child (the rest nodes will be deleted!). Continue?", "Yes", "No"))
                    {
                        if (current.getChildCount() > 0)
                            parent.replaceChild(progress[parent], current.getChild(0));
                        else
                        {
                            if (parent is OptionConversationNode)
                                parent.removeLine(progress[parent]);
                            parent.removeChild(progress[parent]);
                        }
                        if (Selection.Contains(current))
                            Selection.Remove(current);
                        return;
                    }
                }

                if(progress[current] < current.getChildCount())
                {
                    // If there are still childs to check
                    parents.Push(current.getChild(progress[current]));
                    progress[current.getChild(progress[current])] = 0;
                }
                else
                {
                    // Otherwise
                    parents.Pop();
                    if(parents.Peek() != null)
                        ++progress[parents.Peek()];
                }
            }
        }

        protected override void DrawOpenNodeContent(GraphConversation content, ConversationNode node)
        {
            ConversationNodeEditor editor = null;
            if(editors.TryGetValue(node, out editor))
            {
                editor.draw();
            }
        }

        protected override void DrawNodeControls(GraphConversation content, ConversationNode node)
        {
            ConversationNodeEditor editor = null;
            editors.TryGetValue(node, out editor);

            string[] editorNames = ConversationNodeEditorFactory.Intance.CurrentConversationNodeEditors;
            int preEditorSelected = ConversationNodeEditorFactory.Intance.ConversationNodeEditorIndex(node);
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);

            if (editor == null || preEditorSelected != editorSelected)
            {
                bool firstEditor = (editor == null);
                var prevRect = (editor != null) ? editor.Window : new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), node.getEditorHeight());
                editor = ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(editorNames[editorSelected]);
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

        private bool setNode(GraphConversation content, ConversationNode oldNode, ConversationNode newNode)
        {
            if (newNode is DialogueConversationNode && oldNode.getChildCount() > 1)
                if (!EditorUtility.DisplayDialog("Warning", "Switching this node to dialogue will only keep the first child (the rest of them will be deleted!). Continue?", "Yes", "No"))
                    return false;

            // Replace the childs of the current node
            for (int i = 0; i < oldNode.getChildCount(); i++)
            {
                if (newNode is DialogueConversationNode)
                {
                    if (i > 0) break;
                }
                else if (newNode is OptionConversationNode)
                {
                    newNode.addLine(new ConversationLine("Player", ""));
                }

                // Add the childs
                ConversationNode oldNodeChild = oldNode.getChild(i);
                // But if the child is looping to itself, just add the new node instead
                if (oldNodeChild == oldNode) oldNodeChild = newNode;
                newNode.addChild(oldNodeChild);
            }

            if (content.getRootNode() == oldNode)
            {
                content.setRootNode(newNode);
            }


            // Replace the node
            return setNode(new Dictionary<ConversationNode, bool>(), content, oldNode, newNode, content.getRootNode()) > 0;
        }


        private int setNode(Dictionary<ConversationNode, bool> opened, GraphConversation content, ConversationNode oldNode, ConversationNode newNode, ConversationNode currentNode)
        {
            int r = 0;

            if (!opened.ContainsKey(currentNode))
            {
                opened[currentNode] = true;
                for (int i = 0; i < currentNode.getChildCount(); i++)
                {
                    var child = currentNode.getChild(i);
                    r += setNode(opened, content, oldNode, newNode, child);
                    if (child == oldNode)
                        currentNode.replaceChild(i, newNode);
                }
            }

            return r;
        }

        public void addChild(ConversationNode parent, ConversationNode child)
        {
            var margin = 25f;

            ConversationNodeEditor parentEditor = editors[parent];
            child.setEditorX(Mathf.RoundToInt(parentEditor.Window.x + parentEditor.Window.width + 35));
            child.setEditorY(Mathf.RoundToInt(parentEditor.Window.y));
            parent.addChild(child);

            ConversationNodeEditor editor = ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(child);
            editor.setParent(this);
            editor.Node = child;

            float minX = parent.getEditorX() + parent.getEditorWidth() + margin*2;
            float minY = parent.getEditorY();
            for (int i = 0, end = parent.getChildCount(); i < end; ++i)
            {
                var childRect = GetOpenedNodeRect(Content, parent.getChild(i));
                minY = Mathf.Max(minY, childRect.y + childRect.height + margin);
            }

            var rect = editor.Window;
            rect.position = new Vector2(minX, minY);
            editor.Window = rect;

            editors.Add(child, editor);
        }
        

        protected override ConversationNode[] GetNodes(GraphConversation Content)
        {
            return Content.getAllNodes().ToArray();
        }

        protected override string GetTitle(GraphConversation Content, ConversationNode node)
        {
            return node.getType().ToString();
        }

        protected override void SetNodeChild(GraphConversation content, ConversationNode node, int slot, ConversationNode child)
        {
            if (node.getChildCount() > slot)
                node.removeChild(slot);
            node.addChild(slot, child);
        }

        protected override Rect GetOpenedNodeRect(GraphConversation content, ConversationNode node)
        {
            return editors.ContainsKey(node) ? editors[node].Window : new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), node.getEditorHeight());
        }

        protected override void SetNodePosition(GraphConversation content, ConversationNode node, Vector2 position)
        {
            node.setEditorX(Mathf.RoundToInt(position.x));
            node.setEditorY(Mathf.RoundToInt(position.y));
            if (editors.ContainsKey(node))
            {
                var rect = editors[node].Window;
                rect.position = position;
                editors[node].Window = rect;
            }
        }

        protected override void SetNodeSize(GraphConversation content, ConversationNode node, Vector2 size)
        {
            node.setEditorWidth(Mathf.RoundToInt(size.x));
            node.setEditorHeight(Mathf.RoundToInt(size.y));
            if (editors.ContainsKey(node))
            {
                var rect = editors[node].Window;
                rect.size = size;
                editors[node].Window = rect;
            }
        }
    }

    public class ConversationEditorWindow : EditorWindow
    {
        /*******************
		 *  PROPERTIES
		 * *****************/
        private ConversationEditor conversationEditor;
        private GraphConversation conversation; 
        public GraphConversation Conversation
        {
            get { return conversation; }
            set { this.conversation = value; }
        }
        

        /*******************************
		 * Initialization methods
		 ******************************/
        public void Init(GraphConversationDataControl conversation) { Init((GraphConversation)conversation.getConversation()); }
        public void Init(GraphConversation conversation)
        {
            this.conversation = conversation;

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

        void OnGUI()
        {
            if (conversation == null)
                this.Close();
            
            this.wantsMouseMove = true;

            ConversationNode nodoInicial = conversation.getRootNode();
            GUILayout.BeginVertical(GUILayout.Height(20));
            conversation.setId(EditorGUILayout.TextField(TC.get("Conversation.Title"), conversation.getId(),
                GUILayout.ExpandWidth(true)));
            GUILayout.EndVertical();

            conversationEditor.OnInspectorGUI();
        }
    }
}