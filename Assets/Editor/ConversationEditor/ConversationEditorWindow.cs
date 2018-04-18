using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    internal class ConversationEditor : GraphEditor<GraphConversation, ConversationNode>
    {
        private Dictionary<ConversationNode, ConversationNodeEditor> editors = new Dictionary<ConversationNode, ConversationNodeEditor>();

        protected override ConversationNode[] ChildsFor(GraphConversation Content, ConversationNode parent)
        {
            return Enumerable.Range(0, parent.getChildCount()).Select(parent.getChild).ToArray();
        }

        protected override void DrawNodeContent(GraphConversation content, ConversationNode node)
        {
            throw new NotImplementedException();
        }

        protected override Rect GetNodeRect(GraphConversation Content, ConversationNode node)
        {
            return editors.ContainsKey(node) ? editors[node].Window : new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), node.getEditorHeight()); 
        }

        protected override ConversationNode[] GetNodes(GraphConversation Content)
        {
            return Content.getAllNodes().ToArray();
        }

        protected override string GetTitle(GraphConversation Content, ConversationNode node)
        {
            return node.getType().ToString();
        }

        protected override bool IsResizable(GraphConversation content, ConversationNode node)
        {
            return editors.ContainsKey(node) ? !editors[node].Collapsed : false;
        }

        protected override void SetNodeChild(GraphConversation content, ConversationNode node, int slot, ConversationNode child)
        {
            throw new NotImplementedException();
        }

        protected override void SetNodeRect(GraphConversation Content, ConversationNode node, Rect rect)
        {
            node.setEditorX(Mathf.RoundToInt(rect.x));
            node.setEditorY(Mathf.RoundToInt(rect.y));

            if (editors.ContainsKey(node))
                editors[node].Window = rect;

            if (!editors.ContainsKey(node) || !editors[node].Collapsed)
            {
                node.setEditorWidth(Mathf.RoundToInt(rect.x));
                node.setEditorHeight(Mathf.RoundToInt(rect.x));
            }
        }
    }

    public class ConversationEditorWindow : EditorWindow
    {

        /*[MenuItem("eAdventure/Open conversation editor")]
        static public void Init()
        {
            editor = EditorWindow.GetWindow<ConversationEditorWindow>();
            editor.s = Color.black;

            //editor.conversation = new GraphConversation ("Prueba");

            //editor.InitWindows ();
        }

        [MenuItem("eAdventure/Close conversation editor")]
        static public void close(){
            if(editor!=null)
                editor.Close ();
        }*/

        /*******************
		 *  PROPERTIES
		 * *****************/

        private GraphConversation conversation;
        public GraphConversation Conversation
        {
            get { return conversation; }
            set { this.conversation = value; }
        }

        /*******************
		 *  ATTRIBUTES
		 * *****************/

        // Main vars
        private Dictionary<int, ConversationNode> nodes = new Dictionary<int, ConversationNode>();
        private GUIStyle closeStyle, collapseStyle, selectedStyle, buttonstyle;
        private Dictionary<ConversationNode, ConversationNodeEditor> editors =
            new Dictionary<ConversationNode, ConversationNodeEditor>();

        int nodespositioned = 0;

        // Graph control
        private int hovering = -1;
        private ConversationNode hoveringNode = null;
        private int focusing = -1;

        // Graph management
        private bool reinitWindows = false;
        private Rect baseRect = new Rect(10, 10, 25, 25);
        private int lookingChildSlot;
        private ConversationNode lookingChildNode;
        private Dictionary<ConversationNode, bool> loopCheck = new Dictionary<ConversationNode, bool>();

        // Graph scroll
        private Rect scrollRect = new Rect(0, 0, 1000, 1000);
        private Vector2 scroll;
        /*private Matrix4x4 graphMatrix = Matrix4x4.identity;
        private float scale;*/

        // Selection
        private bool toSelect = false;
        private List<ConversationNode> selection = new List<ConversationNode>();
        private Vector2 startPoint;

        NodePositioner positioner;

        /*******************************
		 * Initialization methods
		 ******************************/
        public void Init(GraphConversationDataControl conversation) { Init((GraphConversation)conversation.getConversation()); }
        public void Init(GraphConversation conversation)
        {
            this.conversation = conversation;

            ConversationNodeEditorFactory.Intance.ResetInstance();

            InitStyles();
            InitWindows();
        }


        void InitStyles()
        {
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

            if (collapseStyle == null)
            {
                collapseStyle = new GUIStyle(GUI.skin.button);
                collapseStyle.padding = new RectOffset(0, 0, 0, 0);
                collapseStyle.margin = new RectOffset(0, 5, 2, 0);
                collapseStyle.normal.textColor = Color.blue;
                collapseStyle.focused.textColor = Color.blue;
                collapseStyle.active.textColor = Color.blue;
                collapseStyle.hover.textColor = Color.blue;
            }

            if (buttonstyle == null)
            {
                buttonstyle = new GUIStyle();
                buttonstyle.padding = new RectOffset(5, 5, 5, 5);
                buttonstyle.wordWrap = true;
            }

            if (selectedStyle == null)
            {
                selectedStyle = Resources.Load<GUISkin>("resplandor").box;
            }
        }

        private void InitWindows()
        {
            ConversationNode current_node = this.conversation.getRootNode();

            loopCheck.Clear();

            this.positioner = new NodePositioner(conversation.getAllNodes().Count - 1, position);
            InitWindowsRecursive(current_node);
        }
        private void InitWindowsRecursive(ConversationNode node)
        {
            if (!loopCheck.ContainsKey(node))
            {
                loopCheck.Add(node, true);
                ConversationNodeEditor editor = ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(node);
                editor.setParent(this);
                editor.Node = node;
                editor.Collapsed = true;

                Rect previous = new Rect(0, 25, 0, 100);
                if (editors.ContainsKey(node))
                    previous = editors[node].Window;

                //Rect current = new Rect (previous.x + previous.width + 35, previous.y, 150, 0);

                Rect current;
                if (node.getEditorX() != -1)
                    current = new Rect(node.getEditorX(), node.getEditorY(), 100, 0);
                else
                    current = positioner.getRectFor(nodespositioned);

                nodespositioned++;

                editor.Window = current;

                editors.Add(node, editor);

                for (int i = 0; i < node.getChildCount(); i++)
                {
                    InitWindowsRecursive(node.getChild(i));
                }
            }
        }

        /******************
		 * Window behaviours
		 * ******************/

        void OnGUI()
        {
            if (conversation == null)
                this.Close();
            
            this.wantsMouseMove = true;

            if (reinitWindows)
            {
                InitWindows();
                reinitWindows = false;
            }

            ConversationNode nodoInicial = conversation.getRootNode();
            GUILayout.BeginVertical(GUILayout.Height(20));
            conversation.setId(EditorGUILayout.TextField(TC.get("Conversation.Title"), conversation.getId(),
                GUILayout.ExpandWidth(true)));
            GUILayout.EndVertical();

            // Print the toolbar
            var lastRect = DoToolbar();

            var rect = new Rect(0, lastRect.y + lastRect.height, position.width, position.height - lastRect.height - 23);

            // Do the sequence graph
            DoGraph(rect);
        }


        /**************************
		 * TOOLBAR
		 **************************/
        Rect DoToolbar()
        {

            GUILayout.BeginVertical(GUILayout.Height(17));
            GUILayout.BeginHorizontal("toolbar");

            using (new EditorGUI.DisabledScope())
            {
                if (GUILayout.Button("Variables and flags", "toolbarButton", GUILayout.Width(150)))
                {
                    var o = ChapterVarAndFlagsEditor.ShowAtPosition(GUILayoutUtility.GetLastRect().Move(new Vector2(5, 40)));
                    if (o) GUIUtility.ExitGUI();
                }
            }

            GUILayout.Space(5);
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            return GUILayoutUtility.GetLastRect();
        }


        /********************
		 * GRAPH
		 * ******************/


        /********************
		 * GRAPH
		 * ******************/
        void DoGraph(Rect rect)
        {
            // Scrolling area extension calculation based on the nodes
            float maxX = rect.width, maxY = rect.height;
            foreach (var node in conversation.getAllNodes())
            {
                var px = node.getEditorX() + node.getEditorWidth() + 500;
                var py = node.getEditorY() + node.getEditorHeight() + 200;
                maxX = Mathf.Max(maxX, px);
                maxY = Mathf.Max(maxY, py);
            }

            // Scroll area
            scrollRect = new Rect(0, 0, maxX, maxY);
            /*float scrolled = 0;
            if(rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ScrollWheel)
            {
                scrolled = -Event.current.delta.y * 0.03f;
                Event.current.Use();
            }
            */
            scroll = GUI.BeginScrollView(rect, scroll, scrollRect);

            /*var prevMatrix = GUI.matrix;
            GUI.matrix = graphMatrix;
            if (scrolled != 0f)
            {
                GUIUtility.ScaleAroundPivot(new Vector2(1 + scrolled, 1 + scrolled), Event.current.mousePosition);
            }*/

            // Clear mouse hover
            if (Event.current.type == EventType.MouseMove)
            {
                if (hovering != -1)
                    this.Repaint();

                hovering = -1;
                hoveringNode = null;
            }

            // Background
            drawBackground(scrollRect);

            BeginWindows();
            {
                nodes.Clear();
                CreateWindows(conversation);
                
                if (Event.current.type == EventType.Repaint)
                    foreach (var n in selection)
                        GUI.Box(new Rect(n.getEditorX(), n.getEditorY(), editors[n].Window.width, editors[n].Window.height), "", selectedStyle);

                //drawSlots(sequence);

                if (Event.current.type == EventType.Repaint)
                {
                    DrawLines(conversation);
                }
            }
            EndWindows();


            switch (Event.current.type)
            {
                case EventType.MouseMove:
                    if (lookingChildNode != null)
                    {
                        this.Repaint();
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    {
                        if (EditorGUIUtility.hotControl == 0)
                        {
                            scroll -= Event.current.delta;
                            Repaint();
                        }
                    }
                    break;
                case EventType.MouseDown:
                    {
                        if (Event.current.button == 0)
                        {
                            // Selecting
                            if (GUIUtility.hotControl == 0)
                            {
                                // Start selecting
                                GUIUtility.hotControl = this.GetHashCode();
                                startPoint = Event.current.mousePosition;
                                selection.Clear();
                                Event.current.Use();
                            }
                        }
                    }
                    break;
                case EventType.MouseUp:
                    {
                        if (Event.current.button == 0)
                        {
                            if (GUIUtility.hotControl == this.GetHashCode())
                            {
                                GUIUtility.hotControl = 0;

                                UpdateSelection();
                                Event.current.Use();
                            }

                        }
                    }
                    break;
                case EventType.Repaint:
                    // Draw selection rect 
                    if (GUIUtility.hotControl == GetHashCode())
                    {
                        UpdateSelection();
                        Handles.BeginGUI();
                        Handles.color = Color.white;
                        Handles.DrawSolidRectangleWithOutline(
                            Rect.MinMaxRect(startPoint.x, startPoint.y, Event.current.mousePosition.x, Event.current.mousePosition.y),
                            new Color(.3f, .3f, .3f, .3f),
                            Color.gray);
                        Handles.EndGUI();
                    }
                    break;
            }
            /*graphMatrix = GUI.matrix;
            GUI.matrix = prevMatrix;*/
            GUI.EndScrollView();
        }
        

        private int setNode(ConversationNode oldNode, ConversationNode newNode, ConversationNode rootnode = null)
        {
            if (rootnode == null)
                rootnode = conversation.getRootNode();

            if (oldNode == rootnode)
            {
                for (int i = 0; i < rootnode.getChildCount(); i++)
                {
                    if (newNode is DialogueConversationNode)
                    {
                        if (i > 1) break;
                    }
                    else if (newNode is OptionConversationNode)
                    {
                        newNode.addLine(new ConversationLine("Player", ""));
                    }

                    newNode.addChild(rootnode.getChild(i));
                }

                if (conversation.getRootNode() == rootnode)
                {
                    conversation = new GraphConversation(conversation.getId(), newNode);
                    return 1;
                }
                else
                {
                    return 2;
                }

            }
            else
            {
                for (int i = 0; i < rootnode.getChildCount(); i++)
                {
                    int result = setNode(oldNode, newNode, rootnode.getChild(i));

                    if (result == 2)
                    {
                        rootnode.removeChild(i);
                        rootnode.addChild(newNode);
                    }

                    if (result != 0)
                        return 1;
                }
            }
            return 0;
        }

        public void startSetChild(ConversationNode node, int child)
        {
            lookingChildNode = node;
            lookingChildSlot = child;
        }

        public void addChild(ConversationNode parent, ConversationNode child)
        {
            ConversationNodeEditor parentEditor = editors[parent];
            child.setEditorX(Mathf.RoundToInt(parentEditor.Window.x + parentEditor.Window.width + 35));
            child.setEditorY(Mathf.RoundToInt(parentEditor.Window.y));
            parent.addChild(child);

            ConversationNodeEditor editor = ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(child);
            editor.setParent(this);
            editor.Node = child;
            editor.Collapsed = true;

            editors.Add(child, editor);
        }



        // AUX GRAPH FUNCTIONS

        void drawBackground(Rect rect)
        {
            GUI.Box(rect, "", "preBackground");
            float increment = 15;

            float pos = 0;
            int i = 0;
            float max = Mathf.Max(rect.width, rect.height);

            Handles.BeginGUI();

            Handles.DrawSolidRectangleWithOutline(rect, new Color(.2f, .2f, .2f, 1f), new Color(.2f, .2f, .2f, 1f));

            while (pos < max)
            {

                Handles.color = new Color(.1f, .1f, .1f, 1f);

                // Horizontal
                Handles.DrawAAPolyLine(1f, new Vector3[] { new Vector2(0, pos), new Vector2(rect.width, pos) });
                // Vertical
                Handles.DrawAAPolyLine(1f, new Vector3[] { new Vector2(pos, 0), new Vector2(pos, rect.height) });

                i++;
                pos += increment;
            }

            i = 0;
            pos = 0;
            while (pos < max)
            {

                Handles.color = new Color(.05f, .05f, .05f, 1f);

                // Horizontal
                Handles.DrawAAPolyLine(1f, new Vector3[] { new Vector2(0, pos), new Vector2(rect.width, pos) });
                // Vertical
                Handles.DrawAAPolyLine(1f, new Vector3[] { new Vector2(pos, 0), new Vector2(pos, rect.height) });

                i += 10;
                pos += increment * 10;
            }

            Handles.EndGUI();
        }


        void curveFromTo(Rect wr, Rect wr2, Color color)
        {

            Vector2 start = new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
                startTangent = new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
                end = new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
                endTangent = new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2);

            Handles.BeginGUI();
            Handles.color = color;
            if (start.x > end.x)
            {
                var sep = 30f;
                var upDown = start.y > end.y ? 1 : -1;

                startTangent = start + new Vector2(1, 0) * 50;
                endTangent = end + new Vector2(-1, 0) * 50;

                Vector2 staCorner = new Vector2(wr.center.x, upDown < 0 ? wr.yMax : wr.yMin);
                Vector2 endCorner = new Vector2(wr2.center.x, upDown < 0 ? wr2.yMin : wr2.yMax);
                sep = Mathf.Clamp((staCorner - endCorner).magnitude, 0, sep);
                staCorner += new Vector2(0, upDown * -sep);
                endCorner += new Vector2(0, upDown * sep);

                var superacionHorizontal = Mathf.Clamp(endCorner.x - staCorner.x, 0, 100) / 100;

                Vector2 midCorner = (staCorner + endCorner) / 2f;
                midCorner = Vector2.Lerp(midCorner, (start + end) / 2f, superacionHorizontal);

                Vector2 staCornerT1 = staCorner + new Vector2(1, 0) * wr.width / 1.5f;
                Vector2 staCornerT2 = staCorner + new Vector2(-1, 0) * wr.width / 1.5f;
                Vector2 endCornerT1 = endCorner + new Vector2(1, 0) * wr2.width / 1.5f;
                Vector2 endCornerT2 = endCorner + new Vector2(-1, 0) * wr2.width / 1.5f;

                var aux = staCorner;
                var fus = Mathf.Clamp01(Mathf.Max(upDown * (staCorner.y - endCorner.y) + 2 * sep, 100 + 2 * sep - (staCorner.x - endCorner.x)) / 100);


                staCorner = Vector2.Lerp(staCorner, midCorner, fus);
                endCorner = Vector2.Lerp(endCorner, midCorner, fus);


                Vector2 midCornerT1 = new Vector2(staCornerT1.x, staCorner.y);
                Vector2 midCornerT2 = new Vector2(endCornerT2.x, endCorner.y);

                var ds = Mathf.Lerp(Math.Min(wr.width / 1.5f, Math.Abs(staCorner.x - endCorner.x) / 2f), 0, fus);
                var de = Mathf.Lerp(Math.Min(wr2.width / 1.5f, Math.Abs(staCorner.x - endCorner.x) / 2f), 0, fus);

                staCornerT2 = staCorner + new Vector2(-1, 0) * Mathf.Lerp(ds, (ds + de) / 2f, fus);
                endCornerT1 = endCorner + new Vector2(1, 0) * Mathf.Lerp(de, (ds + de) / 2f, fus);

                Handles.DrawBezier(start, staCorner, startTangent, midCornerT1, color /*Color.yellow*/, null, 3);
                if (fus < 1)
                    Handles.DrawBezier(staCorner, endCorner, staCornerT2, endCornerT1, color /*Color.blue*/, null, 3);
                Handles.DrawBezier(endCorner, end, midCornerT2, endTangent, color /*Color.red*/, null, 3);

            }
            else
            {

                Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);

            }
            Handles.EndGUI();
        }

        private Rect sumRect(Rect r1, Rect r2)
        {
            return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
        }

        void DrawLines(GraphConversation conversation)
        {
            loopCheck.Clear();
            //drawLines(new Rect(0, 0, 0, position.height), editors[conversation.getRootNode()]);

            // Draw the rest of the lines in red
            foreach (ConversationNode n in conversation.getAllNodes())
            {
                if (!loopCheck.ContainsKey(n))
                {
                    loopCheck.Add(n, true);

                    Rect from;
                    if (editors.ContainsKey(n))
                        from = editors[n].Window;
                    else
                        from = new Rect(n.getEditorX(), n.getEditorY(), n.getEditorWidth(), n.getEditorHeight());


                    int childcount = n.getChildCount();
                    if (lookingChildNode == n)
                    {
                        if (lookingChildSlot >= n.getChildCount())
                            childcount++;

                        if (n.getChildCount() == 0)
                        {
                            Rect fromRect = sumRect(from, new Rect(0, from.height / 2f, 0, from.height / 2f));
                            curveFromTo(fromRect, editors[nodes[hovering]].Window, Color.blue);
                        }
                    }

                    float h = from.height / (childcount * 1.0f);

                    for (int i = 0; i < n.getChildCount(); i++)
                        if (n.getChild(i) != null)
                        {
                            Rect to;
                            if (editors.ContainsKey(n.getChild(i)))
                                to = editors[n.getChild(i)].Window;
                            else
                            {
                                var nc = n.getChild(i);
                                to = new Rect(nc.getEditorX(), nc.getEditorY(), nc.getEditorWidth(), nc.getEditorHeight());
                            }

                            Rect fromRect = sumRect(from, new Rect(0, h * i, 0, h - from.height));
                            if (lookingChildNode == n && lookingChildSlot == i)
                            {
                                curveFromTo(fromRect, to, Color.red);

                                if (hovering != -1)
                                {
                                    curveFromTo(fromRect, editors[nodes[hovering]].Window, Color.blue);
                                }
                                else
                                    curveFromTo(fromRect,
                                        new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1),
                                        Color.blue);
                            }
                            else
                                curveFromTo(fromRect, to, l);
                        }
                }
            }
        }

        void CreateWindows(GraphConversation conversation)
        {
            conversation.getAllNodes().ForEach(n => CreateWindow(n));
        }

        void CreateWindow(ConversationNode node)
        {
            nodes.Add(node.GetHashCode(), node);
            var rect = new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), 0); //n.getEditorHeight());
            if (editors.ContainsKey(node))
            {
                if (editors[node].Collapsed)
                {
                    rect.width = editors[node].Window.width;
                    rect.height = editors[node].Window.height;
                }

                editors[node].Window = GUILayout.Window(node.GetHashCode(), rect, NodeWindow, node.getType().ToString(), GUILayout.MinWidth(150));
                if(editors[node].Window.width != rect.width && !editors[node].Collapsed && Event.current.type == EventType.Repaint)
                {
                    node.setEditorWidth(Mathf.RoundToInt(editors[node].Window.width));
                }

                if (editors[node].Window.position != rect.position)
                {
                    var oldRect = new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), node.getEditorHeight());

                    node.setEditorX(Mathf.RoundToInt(editors[node].Window.x));
                    node.setEditorY(Mathf.RoundToInt(editors[node].Window.y));

                    if (selection.Contains(node))
                    {
                        foreach (var n in selection)
                        {
                            if (n != node)
                            {
                                n.setEditorX(Mathf.RoundToInt(n.getEditorX() + (oldRect.x - rect.x)));
                                n.setEditorY(Mathf.RoundToInt(n.getEditorY() + (oldRect.y - rect.y)));
                                var repaintRect = new Rect(n.getEditorX(), n.getEditorY(), editors[n].Window.width, editors[n].Window.height);
                                editors[n].Window = GUILayout.Window(n.GetHashCode(), repaintRect, NodeWindow, n.getType().ToString(), GUILayout.MinWidth(150));
                                repaintRect.height = node.getEditorHeight();
                                editors[n].Window = repaintRect;
                            }
                        }
                    }
                }
            }
        }

        Color s = new Color(0.4f, 0.4f, 0.5f),
            l = new Color(0.3f, 0.7f, 0.4f),
            r = new Color(0.8f, 0.2f, 0.2f);

        /**********************
		 * Node windows
		 *********************/

        void NodeWindow(int id)
        {
            ConversationNode myNode = nodes[id];

            // Editor selection

            DoContentEditor(id);

            switch (Event.current.type)
            {
                case EventType.MouseMove:

                    if (new Rect(0, 0, myNode.getEditorWidth(), myNode.getEditorHeight()).Contains(Event.current.mousePosition))
                    {
                        if (hovering != id)
                        {
                            this.Repaint();
                        }

                        hovering = id;
                        hoveringNode = myNode;
                    }
                    break;
                case EventType.MouseDown:
                    if (hovering == id) focusing = hovering;
                    if (lookingChildNode != null)
                    {
                        // link creation between nodes
                        if (lookingChildNode.getChildCount() > lookingChildSlot)
                            lookingChildNode.removeChild(lookingChildSlot);
                        lookingChildNode.addChild(lookingChildSlot, myNode);
                        // finishing search
                        lookingChildNode = null;
                        Event.current.Use();
                    }
                    break;
            }

            
            DoNodeWindowEditorSelection(id);
            DoResizeEditorWindow(id);
            GUI.DragWindow();

        }

        void DoContentEditor(int id)
        {
            ConversationNode myNode = nodes[id];

            ConversationNodeEditor editor = null;
            editors.TryGetValue(myNode, out editor);

            if (editor != null && editor.Collapsed)
            {
                GUIContent bttext = new GUIContent(TC.get("GeneralText.Open"));
                Rect btrect = GUILayoutUtility.GetRect(bttext, buttonstyle);

                GUILayout.BeginHorizontal();
                if (GUI.Button(btrect, bttext))
                {
                    editor.Collapsed = false;
                }
                ;
                GUILayout.EndHorizontal();
            }
            else
            {
                string[] editorNames = ConversationNodeEditorFactory.Intance.CurrentConversationNodeEditors;

                GUILayout.BeginHorizontal();
                int preEditorSelected = ConversationNodeEditorFactory.Intance.ConversationNodeEditorIndex(myNode);
                int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);

                if (GUILayout.Button("-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                {
                    editor.Collapsed = true;
                    GUIContent bttext = new GUIContent(TC.get("GeneralText.Open"));
                    Rect btrect = GUILayoutUtility.GetRect(bttext, buttonstyle);
                    editor.Window = new Rect(editor.Window.x, editor.Window.y, btrect.width, btrect.height);
                }
                if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                {
                }
                //effects.getEffects().Remove(myEffect);

                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();

                if (editor == null || preEditorSelected != editorSelected)
                {
                    bool firstEditor = (editor == null);
                    var prevRect = (editor != null) ? editor.Window : Rect.zero;
                    prevRect.height = 0;
                    editor =
                        ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(editorNames[editorSelected]);
                    editor.setParent(this);
                    editor.Window = prevRect;
                    if (firstEditor) editor.Node = myNode;
                    else setNode(myNode, editor.Node);

                    editors.Remove(myNode);
                    editors.Add(editor.Node, editor);

                    if (selection.Contains(myNode))
                    {
                        selection.Remove(myNode);
                        selection.Add(editor.Node);
                    }
                }

                editor.draw();

                GUILayout.EndVertical();
            }
        }

        void DoResizeEditorWindow(int id)
        {

            var myNode = nodes[id];
            if (!editors.ContainsKey(myNode))
                return;

            if (editors[myNode].Collapsed)
                return;

            var resizeRect = new Rect(new Vector2(myNode.getEditorWidth() - 10, 0), new Vector2(10, myNode.getEditorHeight()));
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal, myNode.GetHashCode());
            if (EditorGUIUtility.hotControl == 0
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 0
                && resizeRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.hotControl = myNode.GetHashCode();
                Event.current.Use();
            }


            if (GUIUtility.hotControl == myNode.GetHashCode())
            {
                //Debug.Log("hotcontrol");
                myNode.setEditorWidth(Mathf.RoundToInt(Event.current.mousePosition.x + 5));
                this.Repaint();
                //Event.current.Use();
                if (Event.current.type == EventType.MouseUp)
                    EditorGUIUtility.hotControl = 0;
                //if(Event.current.type != EventType.layout)*/
            }

        }

        void DoNodeWindowEditorSelection(int id)
        {
            var myNode = nodes[id];

            switch (Event.current.type)
            {
                case EventType.MouseDown:

                    // Left button
                    if (Event.current.button == 0)
                    {
                        if (hovering == id)
                        {
                            toSelect = false;
                            focusing = hovering;
                            if (Event.current.control)
                            {
                                if (selection.Contains(myNode))
                                    selection.Remove(myNode);
                                else
                                    selection.Add(myNode);
                            }
                            else
                            {
                                toSelect = true;
                                if (!selection.Contains(myNode))
                                {
                                    selection.Clear();
                                    selection.Add(myNode);
                                }
                            }
                        }
                    }

                    break;

                case EventType.MouseDrag:
                    toSelect = false;
                    break;
                case EventType.MouseUp:
                    {
                        if (toSelect)
                        {
                            selection.Clear();
                            selection.Add(myNode);
                        }
                    }
                    break;
            }
        }

        /*************************
		 *  Selection
		 * **********************/

        void UpdateSelection()
        {

            var xmin = Math.Min(startPoint.x, Event.current.mousePosition.x);
            var ymin = Math.Min(startPoint.y, Event.current.mousePosition.y);
            var xmax = Math.Max(startPoint.x, Event.current.mousePosition.x);
            var ymax = Math.Max(startPoint.y, Event.current.mousePosition.y);
            selection = conversation.getAllNodes().FindAll(node =>
               RectContains(Rect.MinMaxRect(xmin, ymin, xmax, ymax), 
               (editors.ContainsKey(node) && editors[node].Collapsed )
                    ? new Rect(node.getEditorX(), node.getEditorY(), editors[node].Window.width, editors[node].Window.height) 
                    : new Rect(node.getEditorX(), node.getEditorY(), node.getEditorWidth(), node.getEditorHeight()))
            );
            Repaint();
        }

        bool RectContains(Rect r1, Rect r2)
        {
            var intersection = Rect.MinMaxRect(Mathf.Max(r1.xMin, r2.xMin), Mathf.Max(r1.yMin, r2.yMin), Mathf.Min(r1.xMax, r2.xMax), Mathf.Min(r1.yMax, r2.yMax));

            return Event.current.shift
                ? r1.xMin < r2.xMin && r1.xMax > r2.xMax && r1.yMin < r2.yMin && r1.yMax > r2.yMax // Completely inside
                    : intersection.width > 0 && intersection.height > 0;
        }
    }
}