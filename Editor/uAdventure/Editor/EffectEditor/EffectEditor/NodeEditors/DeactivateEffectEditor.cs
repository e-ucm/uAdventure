using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class DeactivateEffectEditor : IEffectEditor
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

        private DeactivateEffect effect;

        public DeactivateEffectEditor()
        {
            var flags = Controller.Instance.VarFlagSummary.getFlags();
            this.effect = new DeactivateEffect(flags.Length > 0 ? flags[0] : "");
        }

        public void draw()
        {
            var flags = Controller.Instance.VarFlagSummary.getFlags();
            effect.setTargetId(flags[EditorGUILayout.Popup(TC.get("Condition.FlagID"), Array.IndexOf(flags, effect.getTargetId()), flags)]);
            EditorGUILayout.HelpBox(TC.get("DeactivateEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as DeactivateEffect; } }
        public string EffectName { get { return TC.get("DeactivateEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.VarFlagSummary.getFlagCount() > 0;
            }
        }

        public IEffectEditor clone() { return new DeactivateEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }
    }
}