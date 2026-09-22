using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class WaitTimeEffectEditor : IEffectEditor
    {
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

        private WaitTimeEffect effect;

        public WaitTimeEffectEditor()
        {
            this.effect = new WaitTimeEffect(1);
        }

        public void draw()
        {
            effect.setTime(Mathf.Max(0, EditorGUILayout.IntField(TC.get("Effect.WaitTime"), effect.getTime())));
            EditorGUILayout.HelpBox(TC.get("WaitTimeEffect.Label"), MessageType.Info);
        }

        public IEffect Effect
        {
            get { return effect; }
            set { effect = value as WaitTimeEffect; }
        }

        public string EffectName
        {
            get { return TC.get("Effect.WaitTime"); }
        }

        public IEffectEditor clone()
        {
            return new WaitTimeEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
        public bool Usable { get { return true; } }
    }
}