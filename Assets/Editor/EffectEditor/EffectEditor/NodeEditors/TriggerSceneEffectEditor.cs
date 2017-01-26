using System;
using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TriggerSceneEffectEditor : EffectEditor
    {
        private bool collapsed = false;
        public bool Collapsed { get { return collapsed; } set { collapsed = value; } }
        private Rect window = new Rect(0, 0, 300, 0);
        private string[] scenes;
        private int x = 300, y = 300;

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

        protected TriggerSceneEffect effect;

        public TriggerSceneEffectEditor()
        {
            scenes = Controller.getInstance().getSelectedChapterDataControl()
                .getObjects()
                .FindAll(o => o is IChapterTarget)
                .ConvertAll(t => (t as IChapterTarget).getId())
                .ToArray();

            this.effect = new TriggerSceneEffect(scenes[0], x, y);
        }

        public virtual void draw()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(TC.get("Element.Name2"));

            effect.setTargetId(scenes[EditorGUILayout.Popup(Array.IndexOf(scenes, effect.getTargetId()), scenes)]);
            EditorGUILayout.LabelField("X: ");
            x = EditorGUILayout.IntField(effect.getX());
            EditorGUILayout.LabelField("Y: ");
            y = EditorGUILayout.IntField(effect.getY());

            effect.setPosition(x, y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(TC.get("TriggerSceneEffect.Description"), MessageType.Info);
        }

        public AbstractEffect Effect { get { return effect; } set { effect = value as TriggerSceneEffect; } }
        public virtual string EffectName { get { return TC.get("TriggerSceneEffect.Title"); } }
        public virtual EffectEditor clone() { return new TriggerSceneEffectEditor(); }

        public bool manages(AbstractEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}