using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class PlayAnimationEffectEditor : IEffectEditor, DialogReceiverInterface
    {
        
        private string slidesPath = "assets/special/EmptyAnimation";
        private readonly AnimationField animationField;

        public bool Collapsed { get; set; }

        public Rect Window { get; set; }

        private PlayAnimationEffect effect;

        public PlayAnimationEffectEditor()
        {
            animationField = new AnimationField
            {
                FileType = FileType.PLAY_ANIMATION_EFFECT
            };
            this.effect = new PlayAnimationEffect(slidesPath, 300, 300);
            Window = new Rect(0, 0, 300, 0);
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

        public IEffectEditor clone()
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

        public void OnDialogCanceled(object workingObject = null)
        {
            // Nothing to do so far
        }

        public bool Usable { get { return true; } }
    }
}