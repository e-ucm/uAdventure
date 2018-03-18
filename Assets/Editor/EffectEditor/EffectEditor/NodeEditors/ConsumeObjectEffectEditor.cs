using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ConsumeObjectEffectEditor : AbstractItemEffectEditor
    {
        private string[] items;

        private ConsumeObjectEffect effect;

        public ConsumeObjectEffectEditor()
        {
            items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            this.effect = new ConsumeObjectEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name19"));

            effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("ConsumeObject.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as ConsumeObjectEffect; } }
        public override string EffectName { get { return TC.get("Effect.ConsumeObject"); } }
        public override EffectEditor clone() { return new ConsumeObjectEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}