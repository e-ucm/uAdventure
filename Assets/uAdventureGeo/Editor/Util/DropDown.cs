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
    
    private Drop drop;
    private bool selected;
    private string nextValue;

    public DropDown(string label)
    {
        Label = label;
        drop = ScriptableObject.CreateInstance<Drop>();
        drop.OnOptionSelected = (index, option) =>
        {
            var os = drop.OnOptionSelected;
            drop = ScriptableObject.CreateInstance<Drop>();
            drop.OnOptionSelected = os;
            nextValue = option;
            selected = true;
        };
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
                drop.Elements = Elements;
                if (!drop.Showing)
                {
                    drop.ShowAt(addressRect);
                }
                drop.Repaint();
            }

            if (GUI.GetNameOfFocusedControl() != Label && drop.Showing)
            {
                drop.Showing = false;
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

    public List<string> Elements { get; set; }

    public bool Showing { get; set; }

    public void ShowAt(Rect rect)
    {
        this.position = new Rect(GUIUtility.GUIToScreenPoint(rect.position + new Vector2(0, rect.height)), new Vector2(rect.width, 200));
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