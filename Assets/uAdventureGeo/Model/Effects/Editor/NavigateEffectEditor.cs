using UnityEngine;
using System.Collections;

using uAdventure.Editor;
using System;
using uAdventure.Core;
using UnityEditorInternal;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Geo
{

    public class NavigateEffectEditor : IEffectEditor
    {
        // --------------------
        // Attributes
        // ----------------------
        private NavigateEffect effect;
        private Rect window = new Rect(0, 0, 300, 0);
        private ReorderableList mapElementsReorderableList;

        // ----------------------
        // Constructor
        // ---------------------
        public NavigateEffectEditor()
        {
            effect = new NavigateEffect();

            mapElementsReorderableList = new ReorderableList(effect.Steps, typeof(NavigationStep));

            mapElementsReorderableList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50, 0)), "Reference Id");
                EditorGUI.LabelField(new Rect(rect.position + new Vector2(rect.size.x - 50, 0), new Vector2(50, rect.size.y)), "Locks");
            };

            mapElementsReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUI.LabelField(new Rect(rect.position, rect.size - new Vector2(50,0)), effect.Steps[index].Reference);
                effect.Steps[index].LockNavigation = GUI.Toggle(
                    new Rect(rect.position + new Vector2(rect.size.x - 50, 0), new Vector2(50, rect.size.y)), effect.Steps[index].LockNavigation, "");
            };
            
            mapElementsReorderableList.onAddDropdownCallback = (rect, r) =>
            {
                var elems = getObjectIDReferences();

                var menu = new GenericMenu();
                foreach(var elem in elems) menu.AddItem(new GUIContent(elem.Key), false, (v) => effect.Steps.Add(new NavigationStep(v as string)), elem.Value);
                menu.ShowAsContext();
            };

        }

        // -------------------------
        // Properties
        // --------------------------
        public bool Collapsed { get; set; }
        public string EffectName { get { return "Navigate"; } }
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
            set { effect = value as NavigateEffect; }
        }


        // ----------------------------
        // Methods
        //--------------------------

        public IEffectEditor clone()
        {
            return new NavigateEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c is NavigateEffect;
        }

        private Dictionary<string, string> getObjectIDReferences()
        {
            Dictionary<string, string> objects = new Dictionary<string, string>();
            // TODO extend here

            Controller.Instance.SelectedChapterDataControl.getObjects<GeoElement>().ForEach(g => objects.Add("GeoElement/" + g.getId(), g.getId()));
            Controller.Instance.SelectedChapterDataControl.getItemsList().getItems().ForEach(i => objects.Add("Item/" + i.getId(), i.getId()));
            Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList().ForEach(a => objects.Add("Atrezzo/" + a.getId(), a.getId()));
            Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs().ForEach(npc => objects.Add("Character/" + npc.getId(), npc.getId()));

            return objects;
        }

        // -------------------------
        // Draw method
        // ---------------------
        public void draw()
        {
            effect.NavigationType = (NavigationType)EditorGUILayout.EnumPopup("Navigation type", effect.NavigationType);

            mapElementsReorderableList.list = effect.Steps;
            mapElementsReorderableList.DoLayoutList();
        }
        public bool Usable { get { return true; } }


    }
}