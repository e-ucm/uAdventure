using UnityEngine;
using System.Collections;

using uAdventure.Editor;
using System;
using uAdventure.Core;
using System.Collections.Generic;
using UnityEditor;

namespace uAdventure.Geo
{
    public class NavigationControlEffectEditor : IEffectEditor
    {
        // --------------------
        // Attributes
        // ----------------------
        private NavigationControlEffect effect;
        private Rect window = new Rect(0, 0, 300, 0);
        private List<string> possibleReferences;

        // ----------------------
        // Constructor
        // ---------------------
        public NavigationControlEffectEditor()
        {
            effect = new NavigationControlEffect();
            possibleReferences = new List<string>();
            
            Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>().ForEach(g => possibleReferences.Add(g.getId()));
            Controller.Instance.SelectedChapterDataControl.getItemsList().getItems().ForEach(i => possibleReferences.Add(i.getId()));
            Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList().ForEach(a => possibleReferences.Add(a.getId()));
            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs().ForEach(npc => possibleReferences.Add(npc.getId()));

        }

        // -------------------------
        // Properties
        // --------------------------
        public bool Collapsed { get; set; }
        public string EffectName { get { return "Navigation control"; } }
        public Rect Window
        {
            get
            {
                if (Collapsed) return new Rect(window.x, window.y, 50, 30);
                else return window;
            }
            set
            {
                if (Collapsed) window = new Rect(value.x, value.y, window.width, window.height);
                else window = value;
            }
        }

        public IEffect Effect
        {
            get { return effect; }
            set { effect = value as NavigationControlEffect; }
        }


        // ----------------------------
        // Methods
        //--------------------------

        public IEffectEditor clone()
        {
            return new NavigationControlEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c is NavigationControlEffect;
        }

        private Dictionary<string, string> getObjectIDReferences()
        {
            Dictionary<string, string> objects = new Dictionary<string, string>();
            // TODO extend here
            return objects;
        }

        // -------------------------
        // Draw method
        // ---------------------
        public void draw()
        {
            effect.Type = (NavigationControlEffect.ControlType)EditorGUILayout.EnumPopup("Control type", effect.Type);

            switch (effect.Type)
            {
                case NavigationControlEffect.ControlType.Next:
                    break;
                case NavigationControlEffect.ControlType.Previous:
                    break;
                case NavigationControlEffect.ControlType.Index:
                    var newValue = EditorGUILayout.IntField("Move to index", effect.Index);
                    effect.Index = newValue < 0 ? 0 : newValue;
                    break;
                case NavigationControlEffect.ControlType.ReferenceId:
                    var selected = possibleReferences.IndexOf(effect.Reference);
                    effect.Reference = possibleReferences[EditorGUILayout.Popup(selected == -1 ? 0 : selected, possibleReferences.ToArray())];
                    break;
                default:
                    break;
            }
        }
        public bool Usable { get { return true; } }
    }
}