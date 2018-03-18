using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class GenerateObjectEffectEditor : AbstractItemEffectEditor
    {
        private string[] items;

        private GenerateObjectEffect effect;

        public GenerateObjectEffectEditor()
        {
            items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            this.effect = new GenerateObjectEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name19"));

            effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("GenerateObject.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as GenerateObjectEffect; } }
        public override string EffectName { get { return TC.get("Effect.GenerateObject"); } }
        public override EffectEditor clone() { return new GenerateObjectEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}