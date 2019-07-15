using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class HighlightItemEffectEditor : AbstractItemEffectEditor
    {

        private HighlightItemEffect effect;

        public HighlightItemEffectEditor()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            this.effect = new HighlightItemEffect(items.Length > 0 ? items[0] : "", 0, false);
        }

        public override void draw()
        {
            var higlightTypes = new string[]{ TC.get("HighlightItemEffect.None"), TC.get("HighlightItemEffect.Blue"), TC.get("HighlightItemEffect.Red"), TC.get("HighlightItemEffect.Green"), TC.get("HighlightItemEffect.Border") };

            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            effect.setTargetId(items[EditorGUILayout.Popup(TC.get("Element.Name19"), Array.IndexOf(items, effect.getTargetId()), items)]);
            effect.setHighlightType(EditorGUILayout.Popup(TC.get("HighlightItemEffect.ShortDescription"), effect.getHighlightType(), higlightTypes));
            effect.setHighlightAnimated(GUILayout.Toggle(effect.isHighlightAnimated(), TC.get("HighlightItemEffect.Animated")));

            EditorGUILayout.HelpBox(TC.get("HighlightItemEffect.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as HighlightItemEffect; } }
        public override string EffectName { get { return TC.get("HighlightItemEffect.Title"); } }
        public override IEffectEditor clone() { return new HighlightItemEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return EffectType.HIGHLIGHT_ITEM == c.getType();
        }
    }
}