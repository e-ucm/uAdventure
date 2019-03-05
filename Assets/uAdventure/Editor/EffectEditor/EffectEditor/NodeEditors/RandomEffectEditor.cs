using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class RandomEffectEditor : EffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
        public Rect Window
        {
            get
            {
                if (collapsed)
                {
                    return new Rect(window.x, window.y, 50, 30);
                }
                else
                {
                    return window;
                }
            }
            set
            {
                if (collapsed)
                {
                    window = new Rect(value.x, value.y, window.width, window.height);
                }
                else
                {
                    window = value;
                }
            }
        }

        private RandomEffect effect;

        public RandomEffectEditor()
        {
            this.effect = new RandomEffect();
        }

        public void draw()
        {
            EditorGUILayout.HelpBox(TC.get("RandomEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as RandomEffect; } }
        public string EffectName { get { return TC.get("RandomEffect.Title"); } }

        public bool Usable { get { return true; } }

        public EffectEditor clone() { return new RandomEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}