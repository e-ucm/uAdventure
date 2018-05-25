using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Editor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace uAdventure.QR
{
    public class QRPromtEffectEditor : EffectEditor
    {
        // --------------------
        // Attributes
        // ----------------------
        private QRPromptEffect effect;
        private Rect window = new Rect(0, 0, 300, 0);
        private List<string> qrIds;
        private ReorderableList qrIdReorderableList;

        // ----------------------
        // Constructor
        // ---------------------
        public QRPromtEffectEditor()
        {
            effect = new QRPromptEffect();
            UpdateQRIds();

            qrIdReorderableList = new ReorderableList(effect.ValidIds, typeof(string));
            qrIdReorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                // TODO optimize this
                var elemList = qrIds.Except(effect.ValidIds).ToList();
                elemList.Add(effect.ValidIds[index]);
                elemList.Sort();

                EditorGUI.BeginChangeCheck();
                var selected = elemList.BinarySearch(effect.ValidIds[index]);
                var newSelected = EditorGUI.Popup(rect, selected, elemList.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    effect.ValidIds.Remove(elemList[selected]);
                    effect.ValidIds.Add(elemList[newSelected]);
                }
            };

            qrIdReorderableList.displayAdd = qrIds.Count > effect.ValidIds.Count;
            qrIdReorderableList.onAddCallback = (r) =>
            {
                effect.ValidIds.Add(qrIds.Except(effect.ValidIds).ElementAt(0));
                qrIdReorderableList.displayAdd = qrIds.Count > effect.ValidIds.Count;
            };
            
        }

        // -------------------------
        // Properties
        // --------------------------
        public bool Collapsed { get; set; }
        public string EffectName { get { return "Open QR Prompt"; } }
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
            set { effect = value as QRPromptEffect; }
        }

        // ----------------------------
        // Methods
        //--------------------------

        public EffectEditor clone()
        {
            return new QRPromtEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c is QRPromptEffect;
        }

        // -------------------------
        // Draw method
        // ---------------------
        public void draw()
        {
            UpdateQRIds();
            effect.PromptMessage = EditorGUILayout.TextField("Prompt message", effect.PromptMessage, new GUILayoutOption[0]);
            effect.SelectionType = (QRPromptEffect.ListType) EditorGUILayout.EnumPopup("List type", effect.SelectionType);
            qrIdReorderableList.list = effect.ValidIds;
            qrIdReorderableList.DoLayoutList();
        }

        // -------------------------------
        // AUX METHODS
        // ----------------------------

        private void UpdateQRIds()
        {
            var controllerQRList = Controller.Instance.SelectedChapterDataControl.getObjects<QR>();
            qrIds = controllerQRList.ConvertAll(qr => qr.Id);
            qrIds.Sort();
        }
        public bool Usable { get { return qrIds.Count > 0; } }
    }
}