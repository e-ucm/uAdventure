using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MoveNPCEffectEditor : IEffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);

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

        private MoveNPCEffect effect;

        public MoveNPCEffectEditor()
        {
            var npcs = Controller.Instance.IdentifierSummary.getIds<NPC>();
            if (npcs != null && npcs.Length > 0 && effect == null)
                effect = new MoveNPCEffect(npcs[0], 300, 300);
        }

        public void draw()
        {
            var npc = Controller.Instance.IdentifierSummary.getIds<NPC>();
            if(npc == null || npc.Length == 0)
            {
                EditorGUILayout.HelpBox(TC.get("Action.ErrorNoNPCs"), MessageType.Error);
                return;
            }

            effect.setTargetId(npc[EditorGUILayout.Popup(TC.get("Element.Name28"), Array.IndexOf(npc, effect.getTargetId()), npc)]);

            var value = EditorGUILayout.Vector2IntField("", new Vector2Int(effect.getX(), effect.getY()));
            effect.setDestiny(value.x, value.y);

            EditorGUILayout.HelpBox(TC.get("MoveNPCEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as MoveNPCEffect; } }
        public string EffectName { get { return TC.get("MoveNPCEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return effect != null;
            }
        }

        public IEffectEditor clone() { return new MoveNPCEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.getType() == EffectType.MOVE_NPC;
        }
    }
}