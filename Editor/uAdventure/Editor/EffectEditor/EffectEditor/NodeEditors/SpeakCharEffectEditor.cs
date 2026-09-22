using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class SpeakCharEffectEditor : IEffectEditor
    {
        private FileChooser audioField;
        private SpeakCharEffect effect;
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
        

        public SpeakCharEffectEditor()
        {
            audioField = new FileChooser()
            {
                FileType = FileType.PLAY_SOUND_EFFECT
            };

            var npcs = Controller.Instance.IdentifierSummary.getIds<NPC>();
            if(npcs != null && npcs.Length > 0)
                this.effect = new SpeakCharEffect(npcs[0], "");
        }

        public void draw()
        {
            var npcs = Controller.Instance.IdentifierSummary.getIds<NPC>();

            // Character
            var selected = Mathf.Max(0,Array.IndexOf(npcs, effect.getTargetId()));
            effect.setTargetId(npcs[EditorGUILayout.Popup(TC.get("Element.Name28"), selected, npcs)]);

            // Line
            EditorGUILayout.LabelField(TC.get("ConversationEditor.Line"));
            effect.setLine(EditorGUILayout.TextArea(effect.getLine(), GUILayout.MinWidth(200), GUILayout.MinHeight(50)));

            // Sound
            audioField.Label = TC.get("Animation.Sound");
            audioField.Path = effect.getAudioPath();
            audioField.DoLayout();
            effect.setAudioPath(audioField.Path);

            EditorGUILayout.HelpBox(TC.get("SpeakCharacterEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as SpeakCharEffect; } }
        public string EffectName { get { return TC.get("SpeakCharacterEffect.Title"); } }
        public IEffectEditor clone() { return new SpeakCharEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.getType() == EffectType.SPEAK_CHAR;
        }

        public bool Usable
        {
            get
            {
                return Controller.Instance.IdentifierSummary.getIds<NPC>().Length > 0;
            }
        }
    }
}