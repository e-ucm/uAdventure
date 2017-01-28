using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class LayoutWindow : BaseWindow
    {
        public GUILayoutOption[] Options { get { return m_options; } set { m_options = value; } }
        protected GUILayoutOption[] m_options;
        public LayoutWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
        }

        public override void OnGUI()
        {
            //Options
            Rect = GUILayout.Window(WindowID, Rect, AuxDraw, Content);
        }

        void AuxDraw(int id)
        {
            //GUILayout.BeginVertical(GUILayout.Width(Rect.width), GUILayout.Height(Rect.height));
            Draw(id);
            //GUILayout.EndVertical();
        }
    }
}