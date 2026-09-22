using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GenerateObjectEffectEditor : AbstractItemEffectEditor
    {
        private GenerateObjectEffect effect;

        public GenerateObjectEffectEditor()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            this.effect = new GenerateObjectEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            effect.setTargetId(items[EditorGUILayout.Popup(TC.get("Element.Name19"), Array.IndexOf(items, effect.getTargetId()), items)]);
            EditorGUILayout.HelpBox(TC.get("GenerateObject.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as GenerateObjectEffect; } }
        public override string EffectName { get { return TC.get("Effect.GenerateObject"); } }
        public override IEffectEditor clone() { return new GenerateObjectEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}