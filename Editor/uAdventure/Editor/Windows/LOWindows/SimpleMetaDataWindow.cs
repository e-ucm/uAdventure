using UnityEngine;
using UnityEditor;

namespace uAdventure.Editor
{
    public class SimpleMetaDataWindow : DefaultButtonMenuEditorWindowExtension
    {
        public delegate void DrawDelegate(int id, SerializedProperty property);
        public DrawDelegate onDraw;
        public SerializedProperty property;

        public SimpleMetaDataWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
        }
        public override void Draw(int aID)
        {
            onDraw(aID, property);
        }

        protected override void OnButton()
        {
        }
    }
}