using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ShowTextEffectEditor : EffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
        private GUIStyle previewTextStyle;
        public Rect Window
        {
            get
            {
                if (collapsed) return new Rect(window.x, window.y, 50, 30);
                else return window;
            }
            set
            {
                if (collapsed) window = new Rect(value.x, value.y, window.width, window.height);
                else window = value;
            }
        }

        private ShowTextEffect effect;

        private int x = 300, y = 300;

        public ShowTextEffectEditor()
        {
            this.effect = new ShowTextEffect("", x, y, Color.white, Color.black);
            previewTextStyle = new GUIStyle();
            previewTextStyle.fontSize = 24;
            previewTextStyle.alignment = TextAnchor.MiddleCenter;
            previewTextStyle.border = new RectOffset(12, 12, 12, 12);
            previewTextStyle.padding = new RectOffset(12, 12, 12, 12);
        }

        public void draw()
        {
            EditorGUI.BeginChangeCheck();
            // Line
            EditorGUILayout.LabelField(TC.get("ConversationEditor.Line"));
            effect.setText(EditorGUILayout.TextArea(effect.getText(), GUILayout.MinWidth(200), GUILayout.MinHeight(50)));
            // Position
            var position = EditorGUILayout.Vector2IntField("", new Vector2Int(effect.getX(), effect.getY()));
            effect.setTextPosition(position.x, position.y);
            // Style
            GUILayout.BeginHorizontal();
            effect.setRgbFrontColor(EditorGUILayout.ColorField(effect.getRgbFrontColor()));
            effect.setRgbBorderColor(EditorGUILayout.ColorField(effect.getRgbBorderColor()));
            GUILayout.EndHorizontal();

            // Preview
            CharactersWindowDialogConfiguration.DrawPreview(new GUIContent(effect.getText()), false, null, default(Color), default(Color), effect.getRgbFrontColor(), effect.getRgbBorderColor(), previewTextStyle);
            
            EditorGUILayout.HelpBox(TC.get("ShowTextEffect.Title"), MessageType.Info);
            if (EditorGUI.EndChangeCheck())
            {
                Event.current.Use();
            }
        }

        public IEffect Effect { get { return effect; } set { effect = value as ShowTextEffect; } }
        public string EffectName { get { return TC.get("Effect.ShowText"); } }
        public EffectEditor clone() { return new ShowTextEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }

        public bool Usable { get { return true; } }
    }
}