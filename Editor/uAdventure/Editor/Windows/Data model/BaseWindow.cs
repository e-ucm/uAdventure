using UnityEngine;
namespace uAdventure.Editor
{
    public abstract class BaseWindow
    {
        protected static int m_WindowIDCounter = 5555;
        private int m_WindowID = m_WindowIDCounter++; // simple automatic id distribution

        public delegate void VoidMethodDelegate();

        // Public properties
        public Rect Rect { get { return m_Rect; } set { m_Rect = value; } }
        public GUIStyle Style { get; set; }
        public GUIContent Content { get; set; }
        
        public VoidMethodDelegate BeginWindows;
        public VoidMethodDelegate EndWindows;

        protected Rect m_Rect;


        // Request repaint
        public VoidMethodDelegate OnRequestRepaint;
        protected void Repaint()
        {
            if(OnRequestRepaint != null)
            {
                OnRequestRepaint();
            }
        }

        public int WindowID { get { return m_WindowID; } }
        
        public BaseWindow(Rect rect, GUIContent content, GUIStyle style)
        {
            Content = content;
            Style = style;
            m_Rect = rect;
        }

        public virtual void OnGUI()
        {
            m_Rect = GUI.Window(WindowID, m_Rect, Draw, Content, Style);
        }

        public abstract void Draw(int aID);

    }
}