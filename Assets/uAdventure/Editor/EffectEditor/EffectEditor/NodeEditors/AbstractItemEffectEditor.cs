using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class AbstractItemEffectEditor : IEffectEditor
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

        public virtual bool Usable
        {
            get
            {
                return (Controller.Instance.IdentifierSummary.getIds<Item>().Length > 0);
            }
        }

        public abstract IEffect Effect { get; set; }
        public abstract string EffectName { get; }
        public abstract void draw();
        public abstract IEffectEditor clone();
        public abstract bool manages(IEffect c);
    }
}
