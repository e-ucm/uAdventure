using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PlaySoundEffectEditor : IEffectEditor
    {
        private FileChooser musicField;

        private bool collapsed = false;

        public bool Collapsed
        {
            get { return collapsed; }
            set { collapsed = value; }
        }

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

        private PlaySoundEffect effect;

        public PlaySoundEffectEditor()
        {
            musicField = new FileChooser()
            {
                FileType = FileType.PLAY_SOUND_EFFECT                
            };
            this.effect = new PlaySoundEffect(false, "");
        }

        public void draw()
        {
            // Music field
            musicField.Label = TC.get("Effect.PlaySound");
            musicField.Path = effect.getPath();
            musicField.DoLayout();
            effect.setPath(musicField.Path);
            // Play at background
            effect.setBackground(GUILayout.Toggle(effect.isBackground(), TC.get("PlaySoundEffect.BackgroundCheckBox")));

            EditorGUILayout.HelpBox(TC.get("PlaySoundEffect.Description"), MessageType.Info);
        }

        public IEffect Effect
        {
            get { return effect; }
            set { effect = value as PlaySoundEffect; }
        }

        public string EffectName
        {
            get { return TC.get("PlaySoundEffect.Title"); }
        }

        public IEffectEditor clone()
        {
            return new PlaySoundEffectEditor();
        }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }


        public bool Usable { get { return true; } }
    }
}