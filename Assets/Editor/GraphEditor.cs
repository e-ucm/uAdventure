using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using MoreLinq;
using System.Linq;

namespace uAdventure.Editor
{
    public interface IParentEditor
    {
        void Repaint();
    }

    public interface INodeEditor
    {
        IParentEditor Parent { get; set; }
        object Node { get; set; }
        bool Collapsed { get; set; }
        Rect Window { get; set; }

        void Draw();
    }

    public abstract class GraphEditor<T, N> : UnityEditor.Editor, IParentEditor
    {
        protected T Content { get; set; }
        /*******************
		 *  PROPERTIES
		 * *****************/
        protected abstract N[] GetNodes(T Content);
        protected abstract N[] ChildsFor(T Content, N parent);
        protected abstract void SetNodeRect(T Content, N node, Rect rect);
        protected abstract Rect GetNodeRect(T Content, N node);
        protected abstract string GetTitle(T Content, N node);
        protected abstract void DrawNodeContent(T content, N node);
        protected abstract bool IsResizable(T content, N node);
        protected abstract void SetNodeChild(T content, N node, int slot, N child);

        /*******************
		 *  ATTRIBUTES
		 * *****************/

        // Main vars
        private GUIStyle closeStyle, collapseStyle, selectedStyle, buttonstyle;
        private Dictionary<int, N> nodes = new Dictionary<int, N>();

        int nodespositioned = 0;

        // Graph control
        private int hovering = -1;
        private N hoveringNode = default(N);
        private int focusing = -1;

        // Graph management
        private bool reinitWindows = false;
        private Rect baseRect = new Rect(10, 10, 25, 25);
        private int lookingChildSlot;
        private N lookingChildNode;
        private Dictionary<N, bool> loopCheck = new Dictionary<N, bool>();

        // Graph scroll
        private Rect scrollRect = new Rect(0, 0, 1000, 1000);
        private Vector2 scroll;

        // Selection
        private bool toSelect = false;
        private List<N> selection = new List<N>();
        private Vector2 startPoint;

        NodePositioner positioner;

        /*******************************
		 * Initialization methods
		 ******************************/
        public virtual void Init(T content)
        {
            Content = content;

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
            N[] nodes = GetNodes(Content);
            N current_node = nodes[0];

            loopCheck.Clear();

            this.positioner = new NodePositioner(nodes.Length - 1, new Rect(0,0, 1280, 720));
            InitWindowsRecursive(current_node);
        }
        private void InitWindowsRecursive(N node)
        {
            if (!loopCheck.ContainsKey(node))
            {
                loopCheck.Add(node, true);

                //Rect current = new Rect (previous.x + previous.width + 35, previous.y, 150, 0);

                Rect current = GetNodeRect(Content, node);
                if (current.x == -1)
                    current = positioner.getRectFor(nodespositioned);

                nodespositioned++;

                var childs = ChildsFor(Content, node);
                for (int i = 0, end = childs.Length; i < end; i++)
                {
                    InitWindowsRecursive(childs[i]);
                }
            }
        }

        /******************
		 * Window behaviours
		 * ******************/

        public override void OnInspectorGUI()
        {
            if (Content == null)
            {
                EditorGUILayout.HelpBox("No content!", MessageType.Error);
                return;
            }

            if (reinitWindows)
            {
                InitWindows();
                reinitWindows = false;
            }

            // Print the toolbar
            var lastRect = DoToolbar();

            var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Do the sequence graph
            DoGraph(rect);
        }


        /**************************
		 * TOOLBAR
		 **************************/
        protected virtual Rect DoToolbar()
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
            foreach (var node in GetNodes(Content))
            {
                var nodeRect = GetNodeRect(Content, node);

                var px = nodeRect.x + nodeRect.width + 500;
                var py = nodeRect.y + nodeRect.height + 200;
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
                hoveringNode = default(N);
            }

            // Background
            DrawBackground(scrollRect);

            BeginWindows();
            {
                CreateWindows();

                if (Event.current.type == EventType.Repaint)
                    foreach (var n in selection)
                    {
                        var nodeRect = GetNodeRect(Content, n);
                        GUI.Box(GetNodeRect(Content, n), "", selectedStyle);
                    }

                //drawSlots(sequence);

                if (Event.current.type == EventType.Repaint)
                {
                    DrawLines();
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

        private void EndWindows()
        {
            throw new NotImplementedException();
        }

        private void BeginWindows()
        {
            throw new NotImplementedException();
        }

        public void StartSetChild(N node, int child)
        {
            lookingChildNode = node;
            lookingChildSlot = child;
        }

        // AUX GRAPH FUNCTIONS

        void DrawBackground(Rect rect)
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


        void CurveFromTo(Rect wr, Rect wr2, Color color)
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

        private Rect SumRect(Rect r1, Rect r2)
        {
            return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
        }

        void DrawLines()
        {
            loopCheck.Clear();
            //drawLines(new Rect(0, 0, 0, position.height), editors[conversation.getRootNode()]);

            // Draw the rest of the lines in red
            foreach (N n in GetNodes(Content))
            {
                if (!loopCheck.ContainsKey(n))
                {
                    loopCheck.Add(n, true);

                    Rect from = GetNodeRect(Content, n);

                    var childs = ChildsFor(Content, n);
                    int childcount = childs.Length;
                    if (n.Equals(lookingChildNode))
                    {
                        if (lookingChildSlot >= childcount)
                            childcount++;

                        if (childcount == 0)
                        {
                            Rect fromRect = SumRect(from, new Rect(0, from.height / 2f, 0, from.height / 2f));
                            CurveFromTo(fromRect, GetNodeRect(Content, nodes[hovering]), Color.blue);
                        }
                    }

                    float h = from.height / (childcount * 1.0f);

                    for (int i = 0; i < childcount; i++)
                        if (childs[i] != null)
                        {
                            Rect to = GetNodeRect(Content, childs[i]);
                            Rect fromRect = SumRect(from, new Rect(0, h * i, 0, h - from.height));

                            if (n.Equals(lookingChildNode) && lookingChildSlot == i)
                            {
                                CurveFromTo(fromRect, to, Color.red);

                                if (hovering != -1) CurveFromTo(fromRect, GetNodeRect(Content, nodes[hovering]), Color.blue);
                                else CurveFromTo(fromRect, new Rect(Event.current.mousePosition, Vector2.one), Color.blue);
                            }
                            else
                                CurveFromTo(fromRect, to, l);
                        }
                }
            }
        }

        void CreateWindows()
        {
            nodes.Clear();
            GetNodes(Content).ForEach(n => CreateWindow(n));
        }

        void CreateWindow(N node)
        {
            nodes[node.GetHashCode()] = node;

            var rect = GetNodeRect(Content, node);
            if (Event.current.type == EventType.Layout)
                rect.height = 0; // Reset the height for layouting

            var newRect = GUILayout.Window(node.GetHashCode(), rect, NodeWindow, GetTitle(Content, node), GUILayout.MinWidth(150));
            SetNodeRect(Content, node, newRect);

            if (newRect.position != rect.position)
            {
                var delta = newRect.position - rect.position;
                rect.position += delta;
                if (selection.Contains(node))
                {
                    foreach (var n in selection.Where(o => !o.Equals(node)))
                    {
                        var selectedNodeRect = GetNodeRect(Content, n);
                        selectedNodeRect.position += delta;
                        SetNodeRect(Content, n, selectedNodeRect);

                        var repaintRect = GetNodeRect(Content, n);
                        GUILayout.Window(n.GetHashCode(), repaintRect, NodeWindow, GetTitle(Content, node), GUILayout.MinWidth(150));
                        repaintRect.height = selectedNodeRect.height;
                        SetNodeRect(Content, n, repaintRect);
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
            N myNode = nodes[id];

            // Editor selection

            DoContentEditor(id);

            switch (Event.current.type)
            {
                case EventType.MouseMove:
                    var rect = GetNodeRect(Content, myNode);
                    if (new Rect(Vector2.zero, rect.size).Contains(Event.current.mousePosition))
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
                    if (lookingChildNode != null || !lookingChildNode.Equals(default(N)))
                    {
                        SetNodeChild(Content, lookingChildNode, lookingChildSlot, myNode);
                        // finishing search
                        lookingChildNode = default(N);
                        lookingChildSlot = -1;
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
            DrawNodeContent(Content, nodes[id]);
        }

        void DoResizeEditorWindow(int id)
        {

            var myNode = nodes[id];

            if (!IsResizable(Content, myNode))
                return;

            var rect = GetNodeRect(Content, myNode);
            var resizeRect = new Rect(new Vector2(rect.width - 10, 0), new Vector2(10, rect.width));
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
                rect.width = Mathf.RoundToInt(Event.current.mousePosition.x + 5);
                SetNodeRect(Content, myNode, rect);
                this.Repaint();
                if (Event.current.type == EventType.MouseUp)
                    EditorGUIUtility.hotControl = 0;
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
            selection = GetNodes(Content).Where(node =>
               RectContains(Rect.MinMaxRect(xmin, ymin, xmax, ymax),
               GetNodeRect(Content, node))
            ).ToList();
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