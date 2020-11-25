using UnityEngine;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class RandomEffectEditor : IEffectEditor
    {
        private readonly Texture2D addTexture, removeTexture;

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

        private RandomEffect effect;

        public RandomEffectEditor()
        {
            this.effect = new RandomEffect();

            removeTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteContent");
            addTexture = Resources.Load<Texture2D>("EAdventureData/img/icons/addNode");
        }

        public void draw()
        {
            EditorGUILayout.HelpBox(TC.get("RandomEffect.Description"), MessageType.Info);
            EditorGUI.BeginChangeCheck();
            var newProbability = EditorGUILayout.IntSlider("RandomEffect.Probability.Description".Traslate(),
                        effect.getProbability(), 0, 100);

            if (EditorGUI.EndChangeCheck())
            {
                effect.setProbability(newProbability);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("RandomEffect.Positive.Title".Traslate());
                if (GUILayout.Button(effect.getPositiveEffect() == null ? addTexture : removeTexture, GUILayout.Width(20)))
                {
                    if (effect.getPositiveEffect() == null)
                    {
                        EffectEditorWindow.CreateEffect(effect.setPositiveEffect);
                    }
                    else
                    {
                        effect.setPositiveEffect(null);
                        effect.setNegativeEffect(null);
                    }
                }
            }

            using (new EditorGUI.DisabledScope(effect.getPositiveEffect() == null))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("RandomEffect.Negative.Title".Traslate());
                    if (GUILayout.Button(effect.getNegativeEffect() == null ? addTexture : removeTexture, GUILayout.Width(20)))
                    {
                        if (effect.getNegativeEffect() == null)
                        {
                            EffectEditorWindow.CreateEffect(effect.setNegativeEffect);
                        }
                        else
                        {
                            effect.setNegativeEffect(null);
                        }
                    }
                }
            }
        }

        public IEffect Effect { get { return effect; } set { effect = value as RandomEffect; } }
        public string EffectName { get { return TC.get("RandomEffect.Title"); } }

        public bool Usable { get { return true; } }

        public IEffectEditor clone() { return new RandomEffectEditor(); }

        public bool manages(IEffect c)
        {
            return c.GetType() == effect.GetType();
        }
    }
}