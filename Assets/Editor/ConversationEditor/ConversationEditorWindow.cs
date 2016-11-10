using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConversationEditorWindow : EditorWindow
{

    private ConversationEditorWindow editor;

    public Vector2 scrollPosition = Vector2.zero;

    GUIStyle buttonstyle;

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

    NodePositioner positioner;

    public void Init(GraphConversation conversation)
    {
        editor = EditorWindow.GetWindow<ConversationEditorWindow>();

        editor.conversation = conversation;

        ConversationNodeEditorFactory.Intance.ResetInstance();

        InitWindows();
    }

    public void Init(GraphConversationDataControl conversation)
    {
        editor = EditorWindow.GetWindow<ConversationEditorWindow>();

        editor.conversation = (GraphConversation) conversation.getConversation();

        ConversationNodeEditorFactory.Intance.ResetInstance();

        InitWindows();
    }

    private void InitWindows()
    {
        new Rect(0, 25, 0, 100);

        ConversationNode current_node = this.conversation.getRootNode();

        loopCheck.Clear();

        this.positioner = new NodePositioner(conversation.getAllNodes().Count - 1, editor.position);
        InitWindowsRecursive(current_node);
    }

    int nodespositioned = 0;

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

            if (!tmpRects.ContainsKey(node))
                tmpRects.Add(node, current);

            editor.Window = current;

            editors.Add(node, editor);

            for (int i = 0; i < node.getChildCount(); i++)
            {
                InitWindowsRecursive(node.getChild(i));
            }
        }
    }

    private GraphConversation conversation;

    public GraphConversation Conversation
    {
        get { return conversation; }
        set { this.conversation = value; }
    }

    private Rect baseRect = new Rect(10, 10, 25, 25);
    private Dictionary<ConversationNode, Rect> tmpRects = new Dictionary<ConversationNode, Rect>();

    private Dictionary<ConversationNode, ConversationNodeEditor> editors =
        new Dictionary<ConversationNode, ConversationNodeEditor>();

    private GUIStyle closeStyle, collapseStyle;

    private int hovering = -1;
    private int focusing = -1;

    private int lookingChildSlot;
    private ConversationNode lookingChildNode;

    bool reinitWindows = false;

    void nodeWindow(int id)
    {
        ConversationNode myNode = conversation.getAllNodes()[id];

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
                editor =
                    ConversationNodeEditorFactory.Intance.createConversationNodeEditorFor(editorNames[editorSelected]);
                editor.setParent(this);
                if (firstEditor) editor.Node = myNode;
                else setNode(myNode, editor.Node);

                if (editors.ContainsKey(myNode))
                {
                    editor.Window = editors[myNode].Window;
                }
                else
                {
                    editor.Window = tmpRects[myNode];
                }

                editors.Remove(myNode);
                editors.Add(editor.Node, editor);
            }

            editor.draw();

            GUILayout.EndVertical();
        }


        if (Event.current.type != EventType.layout)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect myRect = editors[myNode].Window;
            myRect.height = lastRect.y + lastRect.height;
            editors[myNode].Window = myRect;
            this.Repaint();
        }

        if (Event.current.type == EventType.mouseMove)
        {
            if (
                new Rect(0, 0, editors[myNode].Window.width, editors[myNode].Window.height).Contains(
                    Event.current.mousePosition))
            {
                hovering = id;
            }
        }

        if (Event.current.type == EventType.mouseDown)
        {
            if (hovering == id) focusing = hovering;
            if (lookingChildNode != null)
            {
                // link creation between nodes
                if (lookingChildNode.getChildCount() > 0)
                    lookingChildNode.removeChild(lookingChildSlot);
                lookingChildNode.addChild(lookingChildSlot, myNode);
                // finishing search
                lookingChildNode = null;
            }
        }

        GUI.DragWindow();
    }

    private int setNode(ConversationNode oldNode, ConversationNode newNode, ConversationNode rootnode = null)
    {
        if (rootnode == null)
            rootnode = conversation.getRootNode();

        if (oldNode == rootnode)
        {
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
        ConversationNodeEditor editor = editors[parent];
        tmpRects.Add(child, new Rect(editor.Window.x + editor.Window.width + 35, editor.Window.y, 0, 0));
        parent.addChild(child);
    }

    void curveFromTo(Rect wr, Rect wr2, Color color, Color shadow)
    {
        Vector2 start = new Vector2(wr.x + wr.width, wr.y + 3 + wr.height/2),
            startTangent = new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width))/2, wr.y + 3 + wr.height/2),
            end = new Vector2(wr2.x, wr2.y + 3 + wr2.height/2),
            endTangent = new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width))/2, wr2.y + 3 + wr2.height/2);

        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);
        Handles.EndGUI();
    }

    private Rect sumRect(Rect r1, Rect r2)
    {
        return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
    }

    private Dictionary<ConversationNode, bool> loopCheck = new Dictionary<ConversationNode, bool>();

    void drawLines(GraphConversation conversation)
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
                    from = tmpRects[n];


                int childcount = n.getChildCount();
                if (lookingChildNode == n)
                {
                    if (lookingChildSlot >= n.getChildCount())
                        childcount++;

                    if (n.getChildCount() == 0)
                    {
                        Rect fromRect = sumRect(from, new Rect(0, from.height/2f, 0, from.height/2f));
                        curveFromTo(fromRect, editors[conversation.getAllNodes()[hovering]].Window, Color.blue, s);
                    }
                }

                float h = from.height/(childcount*1.0f);

                for (int i = 0; i < n.getChildCount(); i++)
                    if (n.getChild(i) != null)
                    {
                        Rect to;
                        if (editors.ContainsKey(n.getChild(i)))
                            to = editors[n.getChild(i)].Window;
                        else
                            to = tmpRects[n.getChild(i)];

                        Rect fromRect = sumRect(from, new Rect(0, h*i, 0, h - from.height));
                        if (lookingChildNode == n && lookingChildSlot == i)
                        {
                            curveFromTo(fromRect, to, Color.red, s);

                            if (hovering != -1)
                                curveFromTo(fromRect, editors[conversation.getAllNodes()[hovering]].Window, Color.blue,
                                    s);
                            else
                                curveFromTo(fromRect,
                                    new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1),
                                    Color.blue, s);
                        }
                        else
                            curveFromTo(fromRect, to, l, s);
                    }
            }
        }
    }

    void createWindows(GraphConversation conversation)
    {
        float altura = 100;
        int i = 0;
        foreach (ConversationNode node in conversation.getAllNodes())
        {
            createWindow(node, i);
            i++;
        }
    }

    void createWindow(ConversationNode node, int windowId)
    {
        if (editors.ContainsKey(node))
            editors[node].Window = GUILayout.Window(windowId, editors[node].Window, nodeWindow,
                node.getType().ToString(), GUILayout.MinWidth(150));
        else
            tmpRects[node] = GUILayout.Window(windowId, tmpRects[node], nodeWindow, node.getType().ToString(),
                GUILayout.MinWidth(150));
    }

    Color s = new Color(0.4f, 0.4f, 0.5f),
        l = new Color(0.3f, 0.7f, 0.4f),
        r = new Color(0.8f, 0.2f, 0.2f);

    void OnGUI()
    {
        this.wantsMouseMove = true;

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

        // Clear mouse hover
        if (Event.current.type == EventType.mouseMove) hovering = -1;

        if (Event.current.type == EventType.mouseDown && Event.current.button == 1)
        {
            this.lookingChildNode = null;
            this.lookingChildSlot = -1;
        }

        BeginWindows();
        createWindows(conversation);

        if (Event.current.type == EventType.repaint)
            drawLines(conversation);

        EndWindows();
    }
}