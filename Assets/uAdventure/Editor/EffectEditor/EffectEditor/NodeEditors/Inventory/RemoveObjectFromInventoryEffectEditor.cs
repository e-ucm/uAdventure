using System;
using uAdventure.Core;
using UnityEditor;


namespace uAdventure.Editor
{
    public class RemoveObjectFromInventoryEffectEditor : AbstractItemEffectEditor
    {
        private string[] items;

        private RemoveObjectFromInventoryEffect effect;

        public RemoveObjectFromInventoryEffectEditor()
        {
            items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            this.effect = new RemoveObjectFromInventoryEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name19"));
            effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("RemoveObjectFromInventory.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as RemoveObjectFromInventoryEffect; } }
        public override string EffectName { get { return TC.get("HighlightItemEffect.Title"); } }
        public override IEffectEditor clone() { return new RemoveObjectFromInventoryEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c is RemoveObjectFromInventoryEffect;
        }
    }
}