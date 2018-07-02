using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PlayAnimationEffectEditor : EffectEditor, DialogReceiverInterface
    {

        private Texture2D clearImg = null;
        private string slidesPath = "assets/special/EmptyAnimation";
        private AnimationField animationField;

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

        private PlayAnimationEffect effect;

        public PlayAnimationEffectEditor()
        {
            animationField = new AnimationField()
            {
                FileType = FileType.PLAY_ANIMATION_EFFECT
            };
            this.effect = new PlayAnimationEffect(slidesPath, 300, 300);
        }

        public void draw()
        {
            // Position
            var pos = EditorGUILayout.Vector2IntField("", new Vector2Int(effect.getX(), effect.getY()));
            effect.setDestiny(pos.x, pos.y);
            // Animation
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;
            animationField.Label = TC.get("Effect.PlayAnimation");
            animationField.Path = effect.getPath();
            animationField.DoLayout();
            effect.setPath(animationField.Path);
            EditorGUIUtility.labelWidth = prevLabelWidth;

            EditorGUILayout.HelpBox(TC.get("PlayAnimationEffect.Description"), MessageType.Info);
        }

        public IEffect Effect
        {
            get { return effect; }
            set { effect = value as PlayAnimationEffect; }
        }

        public string EffectName
        {
            get { return TC.get("PlayAnimationEffect.Title"); }
        }

        public EffectEditor clone()
        {
            return new PlayAnimationEffectEditor();
        }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }

        void OnSlidesceneChanged(string val)
        {
            slidesPath = val;
            effect.setPath(val);
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            switch ((FileType)workingObject)
            {
                case FileType.PLAY_ANIMATION_EFFECT:
                    OnSlidesceneChanged(message);
                    break;
            }
        }

        public void OnDialogCanceled(object workingObject = null) { }

        void EditCutscene()
        {
            CutsceneSlidesEditor slidesEditor =
                (CutsceneSlidesEditor)ScriptableObject.CreateInstance(typeof(CutsceneSlidesEditor));
            slidesEditor.Init(this, effect.getPath());
        }


        public bool Usable { get { return true; } }
    }
}