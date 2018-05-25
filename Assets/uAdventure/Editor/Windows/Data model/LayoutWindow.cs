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

        private Rect layoutRect;
        public override void OnGUI()
        {
            GUILayout.BeginArea(Rect, Content, "Window");
            Draw(GetHashCode());
            GUILayout.EndArea();
        }

        void AuxDraw(int id)
        {
            //GUILayout.BeginVertical(GUILayout.Width(Rect.width), GUILayout.Height(Rect.height));
            Draw(id);
            //GUILayout.EndVertical();
        }
    }
}