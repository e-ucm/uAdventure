using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Text.RegularExpressions;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class MovePlayerEffectEditor : IEffectEditor
    {
        private bool collapsed = false;

        public bool Collapsed
        {
            get { return collapsed; }
            set { collapsed = value; }
        }

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

        private MovePlayerEffect effect;

        public MovePlayerEffectEditor()
        {
            this.effect = new MovePlayerEffect(300, 300);
        }

        public void draw()
        {
            var value = EditorGUILayout.Vector2IntField("", new Vector2Int(effect.getX(), effect.getY()));
            effect.setDestiny(value.x, value.y);

            EditorGUILayout.HelpBox(TC.get("MovePlayerEffect.Description"), MessageType.Info);
        }

        public IEffect Effect
        {
            get { return effect; }
            set { effect = value as MovePlayerEffect; }
        }

        public string EffectName
        {
            get { return TC.get("MovePlayerEffect.Title"); }
        }

        public IEffectEditor clone()
        {
            return new MovePlayerEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }


        public bool Usable { get { return true; } }
    }
}