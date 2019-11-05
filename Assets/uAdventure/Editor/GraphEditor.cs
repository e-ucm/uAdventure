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
    

    public abstract class GraphEditor<T, N> : UnityEditor.Editor, IParentEditor
    {
        public delegate void VoidCall();
        public BaseWindow.VoidMethodDelegate BeginWindows;
        public BaseWindow.VoidMethodDelegate EndWindows;
        public new BaseWindow.VoidMethodDelegate Repaint;

        public T Content { get; protected set; }
        /*******************
		 *  PROPERTIES
		 * *****************/
        protected abstract N[] GetNodes(T Content);
        protected abstract N[] ChildsFor(T Content, N parent);
        protected abstract void SetNodeRect(T Content, N node, Rect rect);
        protected abstract void MoveNodes(T Content, IEnumerable<N> nodes, Vector2 delta);
        protected abstract Rect GetNodeRect(T Content, N node);
        protected abstract string GetTitle(T Content, N node);
        protected abstract void DrawNodeContent(T content, N node);
        protected abstract void OnDrawLine(T content, N originNode, N destinationNode, Rect originRect, Rect destinationRect, bool isHovered, bool isRemoving);
        protected abstract bool IsResizable(T content, N node);
        protected abstract void SetNodeChild(T content, N node, int slot, N child);
        public List<N> Selection { get { return selection; } }

        /*******************
		 *  ATTRIBUTES
		 * *****************/

        // Main vars
        private readonly Dictionary<int, N> nodes = new Dictionary<int, N>();
        private GUIStyle selectedStyle;
        
        protected readonly Color blue  = new Color(0.3f, 0.3f, 0.9f);
        protected readonly Color red   = new Color(0.9f, 0.1f, 0.1f);
        protected readonly Color green = new Color(0.3f, 0.7f, 0.4f);

        int nodespositioned = 0;

        // Graph control
        private int hovering = -1;

        // Graph management
        protected int lookingChildSlot = -1;
        protected N lookingChildNode = default(N);
        private readonly Dictionary<N, bool> loopCheck = new Dictionary<N, bool>();

        // Graph scroll
        private Rect scrollRect = new Rect(0, 0, 1000, 1000);
        private static readonly Rect spaceRect = new Rect(0, 0, float.MaxValue, float.MaxValue);
        private Vector2 scroll;

        // Selection
        private bool toSelect = false;
        private List<N> selection = new List<N>();
        private Vector2 startPoint;

        protected void Awake()
        {
            Repaint = base.Repaint;
        }

        /*******************************
		 * Initialization methods
		 ******************************/
        public virtual void Init(T content)
        {

            if (selectedStyle == null)
                selectedStyle = Resources.Load<GUISkin>("resplandor").box;

            Content = content;

            ConversationNodeEditorFactory.Intance.ResetInstance();
            
        }

        private void InitWindowsRecursive(N node)
        {
            if (!loopCheck.ContainsKey(node))
            {
                loopCheck.Add(node, true);
                
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

            // Print the toolbar
            DoToolbar();

            var rect = GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // Do the sequence graph
            DoGraph(rect);
        }


        /**************************
		 * TOOLBAR
		 **************************/
        public virtual Rect DoToolbar()
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
        public void DoGraph(Rect rect)
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
            scroll = GUI.BeginScrollView(rect, scroll, scrollRect);

            // Clear mouse hover
            if (Event.current.type == EventType.MouseMove)
            {
                if (hovering != -1)
                    this.Repaint();

                hovering = -1;
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
                        GUI.Box(nodeRect, "", selectedStyle);
                    }

                DrawLines();
            }
            EndWindows();

            OnAfterDrawWindows();
            GUI.EndScrollView();
        }

        protected virtual void OnAfterDrawWindows()
        {

            switch (Event.current.type)
            {
                case EventType.MouseMove:

                    if (GUIUtility.hotControl == GetHashCode())
                    {
                        // Drawing selection => repaint afterwards
                        this.Repaint();
                        Event.current.Use();
                    }

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
                        // Selecting
                        if (Event.current.button == 0 && GUIUtility.hotControl == 0)
                        {
                            // Start selecting
                            GUIUtility.hotControl = this.GetHashCode();
                            startPoint = Event.current.mousePosition;
                            selection.Clear();
                            Event.current.Use();
                        }
                    }
                    break;
                case EventType.MouseUp:
                    {
                        if (Event.current.button == 0 && GUIUtility.hotControl == this.GetHashCode())
                        {
                            GUIUtility.hotControl = 0;

                            UpdateSelection();
                            Event.current.Use();
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

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Vector2 start = new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
                startTangent = new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
                end = new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
                endTangent = new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2);

            Handles.BeginGUI();
            Handles.color = color;
            List<Vector3> points = new List<Vector3>();
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
                Vector2 staCornerT2;
                Vector2 endCornerT1;
                Vector2 endCornerT2 = endCorner + new Vector2(-1, 0) * wr2.width / 1.5f;
                
                var fus = Mathf.Clamp01(Mathf.Max(upDown * (staCorner.y - endCorner.y) + 2 * sep, 100 + 2 * sep - (staCorner.x - endCorner.x)) / 100);
                
                staCorner = Vector2.Lerp(staCorner, midCorner, fus);
                endCorner = Vector2.Lerp(endCorner, midCorner, fus);


                Vector2 midCornerT1 = new Vector2(staCornerT1.x, staCorner.y);
                Vector2 midCornerT2 = new Vector2(endCornerT2.x, endCorner.y);

                var ds = Mathf.Lerp(Math.Min(wr.width / 1.5f, Math.Abs(staCorner.x - endCorner.x) / 2f), 0, fus);
                var de = Mathf.Lerp(Math.Min(wr2.width / 1.5f, Math.Abs(staCorner.x - endCorner.x) / 2f), 0, fus);

                staCornerT2 = staCorner + new Vector2(-1, 0) * Mathf.Lerp(ds, (ds + de) / 2f, fus);
                endCornerT1 = endCorner + new Vector2(1, 0) * Mathf.Lerp(de, (ds + de) / 2f, fus);


                points.AddRange(Handles.MakeBezierPoints(start, staCorner, startTangent, midCornerT1, 4));
                Handles.DrawBezier(start, staCorner, startTangent, midCornerT1, color, null, 3);
                if (fus < 1)
                {   
                    Handles.DrawBezier(staCorner, endCorner, staCornerT2, endCornerT1, color, null, 3);
                }
                else
                    points.RemoveAt(points.Count - 1);
                points.AddRange(Handles.MakeBezierPoints(endCorner, end, midCornerT2, endTangent, 3));
                Handles.DrawBezier(endCorner, end, midCornerT2, endTangent, color, null, 3);

            }
            else
            {
                points.AddRange(Handles.MakeBezierPoints(start, end, startTangent, endTangent, 4));
                Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);

            }
            for (int i = 2, pl = points.Count - 1; i < pl; ++i)
            {
                DrawTriangle(points[i], points[i + 1] - points[i - 1], 10f);
            }
            Handles.EndGUI();
        }

        private void DrawTriangle(Vector2 center, Vector2 direction, float size)
        {
            direction = direction.normalized;
            var perpendicularDirection = new Vector2(direction.y, -direction.x);
            Handles.DrawAAConvexPolygon(new Vector3[]
            {
                center + direction * size,
                center + perpendicularDirection * size*0.5f,
                center + perpendicularDirection * size*-0.5f
            });
        }

        private Rect SumRect(Rect r1, Rect r2)
        {
            return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
        }

        void DrawLines()
        {
            loopCheck.Clear();

            var contentNodes = GetNodes(Content);
            if(contentNodes.Length > 0)
            {
                Rect from = new Rect(0, 0, 0, 50f);
                Rect to = GetNodeRect(Content, contentNodes[0]);
                CurveFromTo(from, to, new Color(0.8f, 0.5f, 0.1f));
                OnDrawLine(Content, default(N), contentNodes[0], from, to, false, false);
            }

            // Draw the rest of the lines in red
            foreach (N n in contentNodes)
            {
                if (!loopCheck.ContainsKey(n))
                {
                    loopCheck.Add(n, true);

                    Rect from = GetNodeRect(Content, n);

                    var childs = ChildsFor(Content, n);

                    var isLookingThis = n.Equals(lookingChildNode);
                    int childcount = isLookingThis ? Mathf.Max(childs.Length, lookingChildSlot + 1) : childs.Length;

                    float h = from.height / (childcount * 1.0f);

                    for (int i = 0; i < childcount; i++)
                    {
                        Rect fromRect = SumRect(from, new Rect(0, h * i, 0, h - from.height));
                        var isCurrentChild = isLookingThis && lookingChildSlot == i;
                        if (isCurrentChild)
                        {
                            if (hovering != -1)
                            {
                                var toRect = GetNodeRect(Content, this.nodes[hovering]);
                                CurveFromTo(fromRect, toRect, blue);
                                OnDrawLine(Content, n, nodes[hovering], fromRect, toRect, true, false);
                            }
                            else
                            {
                                var toRect = new Rect(Event.current.mousePosition, new Vector2(150, 0));
                                CurveFromTo(fromRect, toRect, blue);
                                OnDrawLine(Content, n, default(N), fromRect, toRect, true, false);
                            }
                        }
                        if (i < childs.Length && childs[i] != null)
                        {
                            Rect to = GetNodeRect(Content, childs[i]);
                            if (isCurrentChild)
                            {
                                CurveFromTo(fromRect, to, red);
                                OnDrawLine(Content, n, childs[i], fromRect, to, false, true);
                            }
                            else
                            {
                                CurveFromTo(fromRect, to, green);
                                OnDrawLine(Content, n, childs[i], fromRect, to, false, false);
                            }
                        }
                    }
                        
                }
            }
        }

        void CreateWindows()
        {
            nodes.Clear();
            GetNodes(Content).ForEach(n => CreateWindow(n));
        }

        private Dictionary<N, Rect> rects = new Dictionary<N, Rect>();

        protected virtual void CreateWindow(N node)
        {
            nodes[node.GetHashCode()] = node;
            if (!rects.ContainsKey(node))
            {
                rects.Add(node, GetNodeRect(Content, node));
            }

            var rect = rects[node];

            if (Event.current.type == EventType.Layout)
            {
                rect = GetNodeRect(Content, node);
                rect.height = 0; // Reset the height for layout
                rects[node] = rect;
            }

            var newRect = GUILayout.Window(node.GetHashCode(), rect, NodeWindow, GetTitle(Content, node), GUILayout.MinWidth(150));
            newRect = newRect.TrapInside(spaceRect);
            rects[node] = newRect;
            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            // In case any node has been just moved
            if (newRect.position != rect.position && selection.Contains(node))
            {
                var delta = newRect.position - rect.position;
                
                foreach (var n in selection.Where(o => !o.Equals(node)))
                {
                    var trappedDelta = rects[n].Move(delta).TrapInside(spaceRect).position - rects[n].position;
                    delta = delta.sqrMagnitude > trappedDelta.sqrMagnitude ? trappedDelta : delta;
                }

                MoveNodes(Content, selection, delta);

                foreach (var n in selection.Where(o => !o.Equals(node)))
                {
                    rects[n] = GetNodeRect(Content, n);
                    GUILayout.Window(n.GetHashCode(), rects[n], NodeWindow, GetTitle(Content, node), GUILayout.MinWidth(150));
                }
            }
            else
            {
                SetNodeRect(Content, node, rects[node]);
            }
        }

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
                    }
                    break;
                case EventType.MouseDown:
                    if (lookingChildSlot != -1)
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
            var resizeRect = new Rect(new Vector2(rect.width - 10, 0), new Vector2(10, rect.height));
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
                    if (Event.current.button == 0 && hovering == id)
                    {
                        toSelect = false;
                        if (Event.current.control)
                        {
                            if (selection.Contains(myNode))
                            {
                                selection.Remove(myNode);
                            }
                            else
                            {
                                selection.Add(myNode);
                            }
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

    public abstract class CollapsibleGraphEditor<T, N> : GraphEditor<T, N>
    {
        private readonly Vector2 CollapsedSize = new Vector2(200, 50);
        private readonly GUIContent openButton = new GUIContent();
        private GUIStyle closeStyle, collapseStyle, buttonstyle;
        private Dictionary<N, bool> collapsedState;
        private N open;

        public override void Init(T content)
        {
            collapsedState = new Dictionary<N, bool>();
            InitStyles();
            base.Init(content);
        }

        void InitStyles()
        {
            if (closeStyle == null)
            {
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

            if (collapseStyle == null)
            {
                collapseStyle = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 5, 2, 0)
                };
                collapseStyle.normal.textColor = Color.blue;
                collapseStyle.focused.textColor = Color.blue;
                collapseStyle.active.textColor = Color.blue;
                collapseStyle.hover.textColor = Color.blue;
            }

            if (buttonstyle == null)
            {
                buttonstyle = new GUIStyle()
                {
                    padding = new RectOffset(5, 5, 5, 5),
                    wordWrap = true
                };
            }
        }

        protected override void CreateWindow(N node)
        {
            base.CreateWindow(node);
            if (open != null && open.Equals(node) && Event.current.type != EventType.Layout)
            {
                SetCollapsed(Content, node, false);
                open = default(N);
            }
        }

        protected override void DrawNodeContent(T content, N node)
        {

            if (IsCollapsed(content, node))
            {
                GUIContent bttext = OpenButtonText(content, node);
                Rect btrect = GUILayoutUtility.GetRect(bttext, buttonstyle);

                GUILayout.BeginHorizontal();
                if (GUI.Button(btrect, bttext))
                {
                    open = node;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                DrawNodeControls(content, node);
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                DrawOpenNodeContent(content, node);
                GUILayout.EndVertical();
            }
        }

        protected virtual void DrawNodeControls(T content, N node)
        {
            if (GUILayout.Button("-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
            {
                SetCollapsed(Content, node, true);
            }
            if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
            {
                DeleteNode(Content, node);
            }
        }
        protected virtual GUIContent OpenButtonText(T content, N node)
        {
            openButton.text = TC.get("GeneralText.Open");
            return openButton;
        }

        protected virtual void SetCollapsed(T Content, N node, bool collapsed)
        {
            collapsedState[node] = collapsed;
        }

        protected virtual bool IsCollapsed(T Content, N node)
        {
            return !collapsedState.ContainsKey(node) || collapsedState[node];
        }

        protected override void SetNodeRect(T Content, N node, Rect rect)
        {
            SetNodePosition(Content, node, rect.position);
            if (!IsCollapsed(Content, node))
            {
                SetNodeSize(Content, node, rect.size);
            }
        }

        protected override Rect GetNodeRect(T Content, N node)
        {
            var rect = GetOpenedNodeRect(Content, node);
            if(IsCollapsed(Content, node))
            {
                rect.size = CollapsedSize;
            }
            return rect;
        }

        protected override bool IsResizable(T content, N node)
        {
            return !IsCollapsed(content, node);
        }

        protected abstract Rect GetOpenedNodeRect(T content, N node);
        protected abstract void SetNodePosition(T content, N node, Vector2 position);
        protected abstract void SetNodeSize(T content, N node, Vector2 size);
        protected abstract void DrawOpenNodeContent(T content, N node);
        protected abstract void DeleteNode(T content, N node);
    }
}