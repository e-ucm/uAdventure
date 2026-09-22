using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ActivateEffectEditor : IEffectEditor
    {
        public bool Collapsed { get; set; }
        private Rect window = new Rect(0, 0, 300, 0);
        public Rect Window
        {
            get
            {
                if (Collapsed) return new Rect(window.x, window.y, 50, 30);
                else return window;
            }
            set
            {
                if (Collapsed) window = new Rect(value.x, value.y, window.width, window.height);
                else window = value;
            }
        }

        private ActivateEffect effect;

        public ActivateEffectEditor()
        {
            var flags = Controller.Instance.VarFlagSummary.getFlags();
            this.effect = new ActivateEffect(flags.Length > 0 ? flags[0] : "");
        }

        public void draw()
        {
            var flags = Controller.Instance.VarFlagSummary.getFlags();
            effect.setTargetId(flags[EditorGUILayout.Popup(TC.get("Condition.FlagID"), Array.IndexOf(flags, effect.getTargetId()), flags)]);
            EditorGUILayout.HelpBox(TC.get("ActivateEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as ActivateEffect; } }
        public string EffectName { get { return TC.get("ActivateEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.VarFlagSummary.getFlagCount() > 0;
            }
        }

        public IEffectEditor clone() { return new ActivateEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }
    }
}