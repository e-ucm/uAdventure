using UnityEngine;
namespace uAdventure.Editor
{
    public class BaseWindow
    {
        protected static int m_WindowIDCounter = 5555;
        private int m_WindowID = m_WindowIDCounter++; // simple automatic id distribution
        protected Rect m_Rect;

        public GUIStyle Style;
        public GUIContent Content;

        // Request repaint
        public delegate void RequestRepaint();
        public RequestRepaint OnRequestRepaint;
        protected void Repaint(){ OnRequestRepaint(); }

        public int WindowID { get { return m_WindowID; } }
        
        public BaseWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle)
        {
            Content = aContent;
            Style = aStyle;
            m_Rect = aStartPos;
        }

        public virtual void OnGUI()
        {
            m_Rect = GUI.Window(WindowID, m_Rect, Draw, Content, Style);
        }
        public virtual void Draw(int aID)
        { }

    }
}