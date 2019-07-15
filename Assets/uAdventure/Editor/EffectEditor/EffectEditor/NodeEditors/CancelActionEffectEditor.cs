using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    public class CancelActionEffectEditor : IEffectEditor
    {
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

        private CancelActionEffect effect;

        public CancelActionEffectEditor()
        {
            this.effect = new CancelActionEffect();
        }

        public void draw()
        {
            EditorGUILayout.HelpBox(TC.get("Effect.CancelActionInfo"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as CancelActionEffect; } }
        public string EffectName { get { return TC.get("Effect.CancelAction"); } }

        public bool Usable { get { return true; } }

        public IEffectEditor clone() { return new CancelActionEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }
    }
}