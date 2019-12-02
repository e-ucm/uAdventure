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
    private EditorWindow myWindow;

    public DropDown(string label)
    {
        Label = label;
        Drop.OnOptionSelected = (index, option) =>
        {
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

        if(drop != null && drop.editorWindow == EditorWindow.focusedWindow)
        {
            myWindow.Focus();
            GUI.FocusControl(Label);
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
                Drop.Elements = Elements;
                
                if (drop == null)
                {
                    myWindow = EditorWindow.focusedWindow;
                    drop = Drop.ShowAt(addressRect);
                }
                drop.Repaint();
            }

            if (GUI.GetNameOfFocusedControl() != Label && drop != null)
            {
                drop.editorWindow.Close(); 
            }
        }

        return r;
    }
    
}
public class Drop : PopupWindowContent
{
    private Vector2 scrollPos;
    private GUISkin dropdownskin;

    public delegate void OnOptionSelectedDelegate(int index, string option);

    public static OnOptionSelectedDelegate OnOptionSelected;

    public static List<string> Elements { get; set; }

    public static Drop ShowAt(Rect rect)
    {
        var drop = new Drop();
        PopupWindow.Show(rect, drop);
        return drop;
    }

    public override void OnOpen()
    {
        dropdownskin = Resources.Load<GUISkin>("DropdownSkin");
    }

    public override void OnGUI(Rect rect)
    {
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
                    break;
                }
            }
        }
    }

    public void Repaint()
    {
        this.editorWindow.Repaint();
    }
}