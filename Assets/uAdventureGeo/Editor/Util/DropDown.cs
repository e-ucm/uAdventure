using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;

public class DropDown {

    private const float labelSize = 150;

    public string Value;
    public string Label;
    public List<string> Elements;
    
    private bool selected;
    private string nextValue;
    private bool showing = false;

    public DropDown(string label)
    {
        Label = label;
    }


    public bool DoLayout()
    {
        return DoLayout(null);
    }


    public bool DoLayout(GUIStyle style)
    {
        // Creating Layout element
        GUI.SetNextControlName(Label);
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
            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() == Label &&
                Elements != null && Elements.Count > 0)
            {
                if (!Drop.Showing)
                {
                    Drop.ShowAtPosition(addressRect, (index, option) =>
                    {
                        nextValue = option;
                        selected = true;
                    });
                }
                Drop.Elements = Elements;
                Drop.Refresh();
            }

            if (GUI.GetNameOfFocusedControl() != Label && Drop.Showing)
            {
                Drop.Showing = false;
            }
        }

        return r;
    }
    
}
public class Drop : EditorWindow
{
    private Vector2 scrollPos;
    private GUISkin dropdownskin;

    public static Drop s_Drop;
    private static long s_LastClosedTime;

    public delegate void OnOptionSelectedDelegate(int index, string option);

    public OnOptionSelectedDelegate OnOptionSelected;

    public static List<string> Elements { get; set; }
    public static bool Showing { get; set; }
    public static void Refresh()
    {
        if (s_Drop)
        {
            s_Drop.Repaint();
        }
    }

    internal static bool ShowAtPosition(Rect buttonRect, OnOptionSelectedDelegate OnOptionSelected)
    {
        long num = System.DateTime.Now.Ticks / 10000L;
        if (num >= Drop.s_LastClosedTime + 50L)
        {
            if (Event.current != null && Event.current.type != EventType.Repaint)
            {
                Event.current.Use();
            }
            if (Drop.s_Drop == null)
            {
                Drop.s_Drop = ScriptableObject.CreateInstance<Drop>();
            }
            Drop.s_Drop.Init(buttonRect, OnOptionSelected);
            return true;
        }
        return false;
    }


    public void Init(Rect rect, OnOptionSelectedDelegate OnOptionSelected)
    {
        this.position = new Rect(GUIUtility.GUIToScreenPoint(rect.position + new Vector2(0, rect.height)), new Vector2(rect.width, 200));
        this.OnOptionSelected = OnOptionSelected;
        this.ShowPopup();
        Showing = true;
    }

    public void Awake()
    {
        dropdownskin = Resources.Load<GUISkin>("DropdownSkin");
    }

    public void OnGUI()
    {
        if (!Showing)
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
                    Showing = false;
                    this.Close();
                    break;
                }
            }
        }
    }
}