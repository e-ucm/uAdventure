using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TriggerConversationEffectEditor : IEffectEditor
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

        private TriggerConversationEffect effect;

        public TriggerConversationEffectEditor()
        {
            var conversations = Controller.Instance.IdentifierSummary.getIds<Conversation>();
            this.effect = new TriggerConversationEffect(conversations.Length > 0 ? conversations[0] : "");
        }

        public void draw()
        {
            var conversations = Controller.Instance.IdentifierSummary.getIds<Conversation>();
            effect.setTargetId(conversations[EditorGUILayout.Popup(TC.get("Conversation.Title"), Array.IndexOf(conversations, effect.getTargetId()), conversations)]);

            EditorGUILayout.HelpBox(TC.get("TriggerConversationEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as TriggerConversationEffect; } }
        public string EffectName { get { return TC.get("TriggerConversationEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.IdentifierSummary.getIds<Conversation>().Length > 0;
            }
        }

        public IEffectEditor clone() { return new TriggerConversationEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}