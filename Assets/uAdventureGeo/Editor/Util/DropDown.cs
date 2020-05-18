using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Linq;

public class DropDown {

    private const float labelSize = 150;

    public string Value;
    public string Label;
    public List<string> Elements;
    
    private bool selected;
    private string nextValue;

    public DropDown(string label)
    {
        Label = label;
    }

    private void OnOptionSelected(int index, string option)
    {
        nextValue = option;
        selected = true;
    }


    public bool DoLayout()
    {
        return DoLayout(null);
    }


    public bool DoLayout(GUIStyle style)
    {
        // Creating Layout element
        var id = GUIUtility.GetControlID(Label.GetHashCode(), FocusType.Keyboard);
        if(Event.current.type == EventType.Used)
        {
            if(Drop.IsShowing(id))
            {
                Drop.Hide(id);
                Drop.Update(id, Elements);
            }
            if(GUI.GetNameOfFocusedControl() == id.ToString())
            {
                GUI.FocusControl(null);
            }
        }
        
        GUI.SetNextControlName(id.ToString());
        if (style == null)
        {
            Value = EditorGUILayout.TextField(Label, Value);
        }
        else
        {
            Value = EditorGUILayout.TextField(Label, Value, style);
        }

        // Calculating dropdown scroll rect
        var addressRect = GUILayoutUtility.GetLastRect();


        var r = selected;
        if (selected)
        {
            this.Elements.Clear();
            selected = false;
            this.Value = nextValue;
        }
        else
        {
            // If focused show
            if ((Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseMove) && GUI.GetNameOfFocusedControl() == id.ToString() &&
                Elements != null && Elements.Count > 0)
            {
                if (!Drop.IsShowing(id))
                {
                    Drop.ShowAt(id, addressRect, Elements, OnOptionSelected);
                }
                Drop.Update(id, Elements);
            }

            if (GUI.GetNameOfFocusedControl() != id.ToString() && Drop.IsShowing(id) || (Event.current.type == EventType.MouseDown && !addressRect.Contains(Event.current.mousePosition)))
            {
                Drop.Hide(id);
                Drop.Update(id, Elements);
                GUI.FocusControl(null);
            }
        }

        return r;
    }
    
}
public class Drop : EditorWindow
{
    private Vector2 scrollPos;
    private GUISkin dropdownskin;

    public delegate void OnOptionSelectedDelegate(int index, string option);

    public OnOptionSelectedDelegate OnOptionSelected;
    private static Dictionary<int, Drop> s_Drop = new Dictionary<int, Drop>();
    private static Dictionary<int, long> s_LastClosedTime = new Dictionary<int, long>();
    private static Dictionary<int, bool> s_showing = new Dictionary<int, bool>();
    private int id;

    public List<string> Elements { get; set; }


    public static bool ShowAt(int id, Rect rect, List<string> elements, OnOptionSelectedDelegate callback)
    {
        if (!s_LastClosedTime.ContainsKey(id))
        {
            s_LastClosedTime[id] = 0;
        }

        long num = DateTime.Now.Ticks / 10000L;
        if (num >= s_LastClosedTime[id] + 50L)
        {
            if (Event.current != null && Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
            {
                Event.current.Use();
            }
            if (!s_Drop.ContainsKey(id) || s_Drop[id] == null)
            {
                s_Drop[id] = ScriptableObject.CreateInstance<Drop>();
            }
            s_Drop[id].id = id;
            s_Drop[id].position = new Rect(GUIUtility.GUIToScreenPoint(rect.position + new Vector2(0, rect.height)), new Vector2(rect.width, 200));
            s_Drop[id].OnOptionSelected += callback;
            s_Drop[id].Elements = elements;
            s_Drop[id].ShowPopup();
            s_showing[id] = true;

            return true;
        }
        return false;
    }

    public static void Update(int id, List<string> elements)
    {
        if(s_Drop.ContainsKey(id) && s_Drop[id] != null)
        {
            s_Drop[id].Elements = elements;
            s_Drop[id].Repaint();
        }
    }

    public static bool IsShowing(int id)
    {
        if (s_showing.ContainsKey(id))
        {
            return s_showing[id];
        }
        return false;
    }

    public static void Hide(int id)
    {
        if (s_showing.ContainsKey(id))
        {
            s_showing[id] = false;
        }
    }

    protected void OnDisable()
    {
        s_LastClosedTime[id] = DateTime.Now.Ticks / 10000L;
        s_Drop[id] = null;
    }

    public void Awake()
    {
        dropdownskin = Resources.Load<GUISkin>("DropdownSkin");
    }

    public void OnGUI()
    {
        this.wantsMouseMove = true;
        if (!IsShowing(id))
        {
            this.Close();
            return;
        }

        // Show Scrollview
        using (new GUIUtil.SkinScope(dropdownskin))
        using (var scroll = new GUILayout.ScrollViewScope(scrollPos, false, false, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
        {
            scrollPos = scroll.scrollPosition; 
            for (int i = 0; i < Elements.Count; i++)
            {
                if (GUILayout.Button(Elements[i]))
                {
                    if (OnOptionSelected != null)
                    {
                        OnOptionSelected(i, Elements[i]);
                    }
                    GUI.FocusControl(null);
                    s_showing[id] = false;
                    this.Close();
                    break;
                }
            }
        }
        this.Repaint();
    }
}