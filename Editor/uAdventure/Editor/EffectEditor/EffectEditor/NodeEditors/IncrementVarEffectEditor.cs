using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class IncrementVarEffectEditor : IEffectEditor
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

        private IncrementVarEffect effect;

        public IncrementVarEffectEditor()
        {
            var vars = Controller.Instance.VarFlagSummary.getVars();
            this.effect = new IncrementVarEffect(vars.Length > 0 ? vars[0] : "", 1);
        }

        public void draw()
        {
            var vars = Controller.Instance.VarFlagSummary.getVars();
            effect.setTargetId(vars[EditorGUILayout.Popup(TC.get("Vars.Var"), Array.IndexOf(vars, effect.getTargetId()), vars)]);
            effect.setIncrement(EditorGUILayout.IntField(TC.get("VarDialog.Value"), effect.getIncrement()));

            EditorGUILayout.HelpBox(TC.get("IncrementVarEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as IncrementVarEffect; } }
        public string EffectName { get { return TC.get("IncrementVarEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.VarFlagSummary.getVarCount() > 0;
            }
        }

        public IEffectEditor clone() { return new IncrementVarEffectEditor(); }

        public bool manages(IEffect c)
        {

            return c.GetType() == effect.GetType();
        }
    }
}