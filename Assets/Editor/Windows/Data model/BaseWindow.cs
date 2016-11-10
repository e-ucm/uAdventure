using UnityEngine;
using UnityEditor;
using System.Collections;

public class BaseWindow
{
    protected static int m_WindowIDCounter = 5555;
    private int m_WindowID = m_WindowIDCounter++; // simple automatic id distribution
    protected GUIContent m_Content;
    protected GUIStyle m_Style;
    protected Rect m_Rect;

    public int WindowID { get { return m_WindowID; } }
    public BaseWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle)
    {
        m_Content = aContent;
        m_Style = aStyle;
        m_Rect = aStartPos;
    }
    public virtual void OnGUI()
    {
        m_Rect = GUI.Window(WindowID, m_Rect, Draw, m_Content, m_Style);
    }
    public virtual void Draw(int aID)
    { }

}
