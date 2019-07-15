using System;
using uAdventure.Core;
using UnityEditor;


namespace uAdventure.Editor
{
    public class AddObjectToInventoryEffectEditor : AbstractItemEffectEditor
    {
        private string[] items;

        private AddObjectToInventoryEffect effect;

        public AddObjectToInventoryEffectEditor()
        {
            items = Controller.Instance.SelectedChapterDataControl.getItemsList().getItemsIDs();
            this.effect = new AddObjectToInventoryEffect(items.Length > 0 ? items[0] : "");
        }

        public override void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name19"));
            effect.setTargetId(items[EditorGUILayout.Popup(Array.IndexOf(items, effect.getTargetId()), items)]);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("AddObjectToInventory.Description"), MessageType.Info);
        }

        public override IEffect Effect { get { return effect; } set { effect = value as AddObjectToInventoryEffect; } }
        public override string EffectName { get { return TC.get("HighlightItemEffect.Title"); } }
        public override IEffectEditor clone() { return new AddObjectToInventoryEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c is AddObjectToInventoryEffect;
        }
    }
}