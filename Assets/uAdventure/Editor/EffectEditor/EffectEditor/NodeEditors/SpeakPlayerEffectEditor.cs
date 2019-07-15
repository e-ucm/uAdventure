using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SpeakPlayerEffectEditor : IEffectEditor
    {
        private FileChooser audioField;
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
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

        private SpeakPlayerEffect effect;

        public SpeakPlayerEffectEditor()
        {
            audioField = new FileChooser()
            {
                FileType = FileType.PLAY_SOUND_EFFECT
            };
            this.effect = new SpeakPlayerEffect("");
        }

        public void draw()
        {
            // Line
            EditorGUILayout.LabelField(TC.get("ConversationEditor.Line"));
            effect.setLine(EditorGUILayout.TextArea(effect.getLine(), GUILayout.MinWidth(200), GUILayout.MinHeight(50)));

            // Sound
            audioField.Label = TC.get("Animation.Sound");
            audioField.Path = effect.getAudioPath();
            audioField.DoLayout();
            effect.setAudioPath(audioField.Path);

            EditorGUILayout.HelpBox(TC.get("SpeakPlayerEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as SpeakPlayerEffect; } }
        public string EffectName { get { return TC.get("SpeakPlayerEffect.Title"); } }
        public IEffectEditor clone() { return new SpeakPlayerEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }
        public bool Usable { get { return true; } }
    }
}