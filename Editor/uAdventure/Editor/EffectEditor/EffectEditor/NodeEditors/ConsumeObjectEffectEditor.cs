using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ConsumeObjectEffectEditor : AbstractItemEffectEditor
    {
        private ConsumeObjectEffect effect;

        public ConsumeObjectEffectEditor()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            this.effect = new ConsumeObjectEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            effect.setTargetId(items[EditorGUILayout.Popup(TC.get("Element.Name19"), Array.IndexOf(items, effect.getTargetId()), items)]);
            EditorGUILayout.HelpBox(TC.get("ConsumeObject.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as ConsumeObjectEffect; } }
        public override string EffectName { get { return TC.get("Effect.ConsumeObject"); } }
        public override IEffectEditor clone() { return new ConsumeObjectEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}