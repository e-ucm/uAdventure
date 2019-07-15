using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TriggerCutsceneEffectEditor : IEffectEditor
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

        private TriggerCutsceneEffect effect;

        public TriggerCutsceneEffectEditor()
        {
            var cutscenes = Controller.Instance.IdentifierSummary.getIds<Cutscene>();
            this.effect = new TriggerCutsceneEffect(cutscenes.Length> 0 ? cutscenes[0] : "");
        }

        public void draw()
        {
            var cutscenes = Controller.Instance.IdentifierSummary.getIds<Cutscene>();
            effect.setTargetId(cutscenes[EditorGUILayout.Popup(TC.get("Element.Name2"), Array.IndexOf(cutscenes, effect.getTargetId()), cutscenes)]);
            EditorGUILayout.HelpBox(TC.get("TriggerCutsceneEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as TriggerCutsceneEffect; } }
        public string EffectName { get { return TC.get("TriggerCutsceneEffect.Title"); } }

        public bool Usable
        {
            get
            {
                var cutscenes = Controller.Instance.IdentifierSummary.getIds<Cutscene>();
                return cutscenes.Length > 0;
            }
        }

        public IEffectEditor clone() { return new TriggerCutsceneEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}