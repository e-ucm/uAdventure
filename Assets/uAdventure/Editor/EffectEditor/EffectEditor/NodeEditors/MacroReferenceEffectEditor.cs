using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MacroReferenceEffectEditor : IEffectEditor
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

        private MacroReferenceEffect effect;

        public MacroReferenceEffectEditor()
        {
            var macros = Controller.Instance.IdentifierSummary.getIds<Macro>();
            this.effect = new MacroReferenceEffect(macros.Length > 0 ? macros[0] : "");
        }

        public void draw()
        {
            var macros = Controller.Instance.IdentifierSummary.getIds<Macro>();
            effect.setTargetId(macros[EditorGUILayout.Popup(TC.get("Element.Name56"), Array.IndexOf(macros, effect.getTargetId()), macros)]);

            EditorGUILayout.HelpBox(TC.get("Effect.MacroReference"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as MacroReferenceEffect; } }
        public string EffectName { get { return TC.get("MacroReferenceEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.getAdvancedFeaturesController().getMacrosListDataControl().getMacrosIDs().Length > 0;
            }
        }

        public IEffectEditor clone() { return new MacroReferenceEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}