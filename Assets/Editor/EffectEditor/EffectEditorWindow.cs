using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class EffectEditorWindow : EditorWindow
{
    private static EffectEditorWindow editor;

    public Vector2 scrollPosition = Vector2.zero;

    public void Init(Effects e)
    {
        editor = EditorWindow.GetWindow<EffectEditorWindow>();
        editor.s = Color.black;

        editor.Effects = e;

        EffectEditorFactory.Intance.ResetInstance();

        InitWindows();
    }

    public void Init(EffectsController e)
    {
        editor = EditorWindow.GetWindow<EffectEditorWindow>();
        editor.s = Color.black;

        editor.Effects = e.getEffectsDirectly();

        EffectEditorFactory.Intance.ResetInstance();

        InitWindows();
    }
    
    private void InitWindows()
    {
        Rect previous = new Rect(0, 25, 0, 100);

        for (int i = 0; i < effects.getEffects().Count; i++)
        {
            AbstractEffect myEffect = this.effects.getEffects()[i];

            EffectEditor editor = EffectEditorFactory.Intance.createEffectEditorFor(myEffect);
            editor.Effect = myEffect;

            if (i > 0)
            {
                previous = editors[effects.getEffects()[i - 1]].Window;
            }

            Rect current = new Rect(previous.x + previous.width + 35, previous.y, 150, 0);
            if (!tmpRects.ContainsKey(effects.getEffects()[i]))
                tmpRects.Add(effects.getEffects()[i], current);

            editor.Window = current;

            editor.Effect.setConditions(myEffect.getConditions());
            editors.Add(editor.Effect, editor);
        }

    }

    private Effects effects;

    public Effects Effects
    {
        get { return effects; }
        set { this.effects = value; }
    }

    private Rect baseRect = new Rect(10, 10, 25, 25);
    private Dictionary<AbstractEffect, Rect> tmpRects = new Dictionary<AbstractEffect, Rect>();
    private Dictionary<AbstractEffect, EffectEditor> editors = new Dictionary<AbstractEffect, EffectEditor>();

    private GUIStyle closeStyle, collapseStyle;

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width*height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }


    void nodeWindow(int id)
    {
        AbstractEffect myEffect = this.effects.getEffects()[id];

        EffectEditor editor = null;
        editors.TryGetValue(myEffect, out editor);

        if (editor != null && editor.Collapsed)
        {
            if (GUILayout.Button(TC.get("GeneralText.Open")))
                editor.Collapsed = false;
        }
        else
        {

            string[] editorNames = EffectEditorFactory.Intance.CurrentEffectEditors;

            GUILayout.BeginHorizontal();
            int preEditorSelected = EffectEditorFactory.Intance.EffectEditorIndex(myEffect);
            int editorSelected = EditorGUILayout.Popup(preEditorSelected, editorNames);

            if (GUILayout.Button("-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                editor.Collapsed = true;
            if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                effects.getEffects().Remove(myEffect);

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(conditionStyle);
            GUILayout.Label("CONDITIONS");
            if (GUILayout.Button("Add Block"))
            {
                myEffect.getConditions().add(new FlagCondition(""));
            }

            if (editor == null || preEditorSelected != editorSelected)
            {
                editor = EffectEditorFactory.Intance.createEffectEditorFor(editorNames[editorSelected]);

                if (editors.ContainsKey(myEffect))
                {
                    editor.Window = editors[myEffect].Window;
                }
                else
                {
                    editor.Window = tmpRects[myEffect];

                }

                editor.Effect.setConditions(myEffect.getConditions());
                editors.Remove(myEffect);
                editors.Add(editor.Effect, editor);
            }

            //##################################################################################
            //############################### CONDITION HANDLING ###############################
            //##################################################################################

            foreach (List<Condition> cl in myEffect.getConditions().getConditionsList())
            {
                if (cl.Count > 1)
                    GUILayout.BeginVertical(eitherConditionStyle);
                for (int i = 0; i < cl.Count; i++)
                {

                    GUILayout.BeginHorizontal();
                    int preConEdiSel = ConditionEditorFactory.Intance.ConditionEditorIndex(cl[i]);
                    int conEdiSel = EditorGUILayout.Popup(preConEdiSel,
                        ConditionEditorFactory.Intance.CurrentConditionEditors);

                    if (preConEdiSel != conEdiSel)
                        cl[i] = ConditionEditorFactory.Intance.Editors[conEdiSel].InstanceManagedCondition();

                    ConditionEditorFactory.Intance.getConditionEditorFor(cl[i]).draw(cl[i]);

                    if (GUILayout.Button("+", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        cl.Add(new FlagCondition(""));
                    }

                    if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                    {
                        cl.Remove(cl[i]);

                        if (cl.Count == 0)
                            myEffect.getConditions().getConditionsList().Remove(cl);
                    }

                    GUILayout.EndHorizontal();
                }
                if (cl.Count > 1)
                    GUILayout.EndVertical();
            }

            //##################################################################################


            GUILayout.EndVertical();

            editor.draw();

            this.effects.getEffects()[id] = editor.Effect;
        }


        if (Event.current.type != EventType.layout)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect myRect = editors[myEffect].Window;
            myRect.height = lastRect.y + lastRect.height;
            editors[myEffect].Window = myRect;
            this.Repaint();
        }

        GUI.DragWindow();
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

    private int windowId;

    void createWindows()
    {
        float altura = 100;
        Rect previous = new Rect(0, 25, 0, 100);
        for (int i = 0; i < effects.getEffects().Count; i++)
        {
            if (i > 0)
            {
                previous = editors[effects.getEffects()[i - 1]].Window;
            }

            Rect current;
            if (!editors.ContainsKey(effects.getEffects()[i]))
            {
                current = new Rect(previous.x + previous.width + 35, previous.y, 0, 0);
                if (!tmpRects.ContainsKey(effects.getEffects()[i]))
                    tmpRects.Add(effects.getEffects()[i], current);
            }
            else
                current = editors[effects.getEffects()[i]].Window;

            curveFromTo(previous, current, new Color(0.3f, 0.7f, 0.4f), s);
            createWindow(effects.getEffects()[i]);
            windowId++;
        }
    }

    void createWindow(AbstractEffect effect)
    {
        if (editors.ContainsKey(effect))
            editors[effect].Window = GUILayout.Window(windowId, editors[effect].Window, nodeWindow,
                effect.getType().ToString(), GUILayout.MinWidth(150));
        else
            tmpRects[effect] = GUILayout.Window(windowId, tmpRects[effect], nodeWindow, effect.getType().ToString(),
                GUILayout.MinWidth(150));
    }

    Color s = new Color(0.4f, 0.4f, 0.5f),
        l = new Color(0.3f, 0.7f, 0.4f),
        r = new Color(0.8f, 0.2f, 0.2f);

    GUIStyle conditionStyle, eitherConditionStyle;

    void OnGUI()
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

        if (conditionStyle == null)
        {
            conditionStyle = new GUIStyle(GUI.skin.box);
            conditionStyle.normal.background = MakeTex(1, 1, new Color(0.627f, 0.627f, 0.627f));
        }

        if (eitherConditionStyle == null)
        {
            eitherConditionStyle = new GUIStyle(GUI.skin.box);
            eitherConditionStyle.normal.background = MakeTex(1, 1, new Color(0.568f, 0.568f, 0.568f));
            eitherConditionStyle.padding.left = 15;
        }

        windowId = 0;
        BeginWindows();

        GUILayout.BeginVertical(GUILayout.Height(20));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("New Effect"))
        {
            effects.add(new ActivateEffect(""));
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginScrollView(scrollPosition);
        createWindows();
        GUILayout.EndScrollView();
        EndWindows();
    }
}