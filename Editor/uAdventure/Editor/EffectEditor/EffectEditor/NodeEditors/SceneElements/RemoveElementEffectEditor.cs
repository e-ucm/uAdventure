using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class RemoveElementEffectEditor : IEffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
        private string[] items;
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

        private RemoveElementEffect effect;

        public RemoveElementEffectEditor()
        {
            items = Controller.Instance.IdentifierSummary.getIds<Item>();
            this.effect = new RemoveElementEffect(items.Length > 0 ? items[0] : "");
        }

        public void draw()
        {
            items = Controller.Instance.IdentifierSummary.getIds<Item>();
            effect.setTargetId(items[EditorGUILayout.Popup(TC.get("Condition.FlagID"), Array.IndexOf(items, effect.getTargetId()), items)]);

            EditorGUILayout.HelpBox(TC.get("RemoveElementEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as RemoveElementEffect; } }
        public string EffectName { get { return TC.get("RemoveElementEffect.Title"); } }

        public bool Usable
        {
            get
            {
                var items = Controller.Instance.IdentifierSummary.getIds<Item>();
                return items.Length > 0;
            }
        }

        public IEffectEditor clone() { return new RemoveElementEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}