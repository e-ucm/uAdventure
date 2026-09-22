using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TriggerBookEffectEditor : IEffectEditor
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

        private TriggerBookEffect effect;

        public TriggerBookEffectEditor()
        {
            var books = Controller.Instance.IdentifierSummary.getIds<Book>();
            this.effect = new TriggerBookEffect(books.Length > 0 ? books[0] : "");
        }

        public void draw()
        {
            var books = Controller.Instance.IdentifierSummary.getIds<Book>();
            effect.setTargetId(books[EditorGUILayout.Popup(TC.get("Element.Name12"), Array.IndexOf(books, effect.getTargetId()), books)]);
            EditorGUILayout.HelpBox(TC.get("TriggerBookEffect.Description"), MessageType.Info);
        }

        public IEffect Effect { get { return effect; } set { effect = value as TriggerBookEffect; } }
        public string EffectName { get { return TC.get("TriggerBookEffect.Title"); } }

        public bool Usable
        {
            get
            {
                return Controller.Instance.SelectedChapterDataControl.getBooksList().getBooksIDs().Length > 0;
            }
        }

        public IEffectEditor clone() { return new TriggerBookEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}