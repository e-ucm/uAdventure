using UnityEngine;
using UnityEditor;
using System.Collections;

public class LayoutWindow : BaseWindow
{
    protected GUILayoutOption[] m_Options;
    public LayoutWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle)
    {
        m_Options = aOptions;
    }
    public override void OnGUI()
    {
        m_Rect = GUILayout.Window(WindowID, m_Rect, Draw, m_Content, m_Style, m_Options);
    }
}
