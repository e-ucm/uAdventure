using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class HighlightItemEffectEditor : AbstractItemEffectEditor
    {
        private string[] items;
        private string[] higlightTypes = { TC.get("HighlightItemEffect.None"), TC.get("HighlightItemEffect.Blue"), TC.get("HighlightItemEffect.Red"), TC.get("HighlightItemEffect.Green"), TC.get("HighlightItemEffect.Border") };
        

        private HighlightItemEffect effect;

        public HighlightItemEffectEditor()
        {
            items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            this.effect = new HighlightItemEffect(items.Length > 0 ? items[0] : "", 0, false);
        }

        public override void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name19"));
            effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);
            effect.setHighlightType(EditorGUILayout.Popup(Array.IndexOf(higlightTypes, effect.getHighlightType()), higlightTypes));
            effect.setHighlightAnimated(GUILayout.Toggle(effect.isHighlightAnimated(), TC.get("HighlightItemEffect.Animated")));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("HighlightItemEffect.Description"), MessageType.Info);
        }

        public override AbstractEffect Effect { get { return effect; } set { effect = value as HighlightItemEffect; } }
        public override string EffectName { get { return TC.get("HighlightItemEffect.Title"); } }
        public override EffectEditor clone() { return new HighlightItemEffectEditor(); }

        public override bool manages(AbstractEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}