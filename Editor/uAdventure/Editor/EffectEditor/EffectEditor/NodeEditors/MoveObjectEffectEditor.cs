using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MoveObjectEffectEditor : AbstractItemEffectEditor
    {
        private MoveObjectEffect effect;

        public MoveObjectEffectEditor()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            if (items != null && items.Length > 0 && effect == null)
                this.effect = new MoveObjectEffect(items[0], 300, 300, 1.0f, false, 1, 1);
        }

        public override void draw()
        {
            var items = Controller.Instance.IdentifierSummary.getIds<Item>();
            if (items == null || items.Length == 0)
            {
                EditorGUILayout.HelpBox(TC.get("Action.ErrorNoItems"), MessageType.Error);
                return;
            }

            effect.setTargetId(items[EditorGUILayout.Popup(TC.get("Element.Name19"), Array.IndexOf(items, effect.getTargetId()), items)]);

            var value = EditorGUILayout.Vector2IntField("", new Vector2Int(effect.getX(), effect.getY()));
            effect.setX(value.x);
            effect.setY(value.y);
            effect.setScale(EditorGUILayout.FloatField(TC.get("SceneLocation.Scale"), effect.getScale()));
            effect.setAnimated(EditorGUILayout.BeginToggleGroup(TC.get("MoveObjectEffect.Animated"), effect.isAnimated()));
            var prevLabelSize = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 95;
            effect.setTranslateSpeed(EditorGUILayout.IntField(TC.get("MoveObjectEffect.TranslateSpeed"), effect.getTranslateSpeed()));
            effect.setScaleSpeed(EditorGUILayout.IntField(TC.get("MoveObjectEffect.ScaleSpeed"), effect.getScaleSpeed()));
            EditorGUIUtility.labelWidth = prevLabelSize;
            EditorGUILayout.EndToggleGroup();

            EditorGUILayout.HelpBox(TC.get("MoveObjectEffect.Title"), MessageType.Info);
        }

        public override bool Usable
        {
            get
            {
                return effect != null;
            }
        }

        public override IEffect Effect { get { return effect; } set { effect = value as MoveObjectEffect; } }
        public override string EffectName { get { return TC.get("Effect.MoveObject"); } }
        public override IEffectEditor clone() { return new MoveObjectEffectEditor(); }

        public override bool manages(IEffect c)
        {
            return c.getType() == EffectType.MOVE_OBJECT;
        }
    }
}