using UnityEngine;
using System.Collections;

using uAdventure.Core;
using uAdventure.Editor;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditorInternal;
using Action = uAdventure.Core.Action;

namespace uAdventure.QR
{
    public class QRScannerEffectEditor : IEffectEditor
    {
        // --------------------
        // Attributes
        // ----------------------
        private QRScannerEffect effect;
        private Rect window = new Rect(0, 0, 300, 0);
        private List<string> qrIds;
        private readonly List<string> effectIds;
        private readonly ReorderableList qrIdReorderableList;
        private readonly string[] listTypeNames;

        // ----------------------
        // Constructor
        // ---------------------
        public QRScannerEffectEditor()
        {
            effect = new QRScannerEffect();
            effectIds = new List<string>();

            listTypeNames = Enum.GetValues(typeof(QRScannerEffect.ListType))
                .Cast<QRScannerEffect.ListType>()
                .Select(v => TC.get("QR.QRScannerEffect.ListType." + v.ToString()))
                .ToArray();

            Action<ReorderableList> listChangedCallback = (r) =>
            {
                Controller.Instance.AddTool(new ChangeValueTool<QRScannerEffect, List<string>>(effect, effectIds.ToList(), "ValidIds"));
            };

            GenericMenu.MenuFunction2 toAddSelected = (s) =>
            {
                effectIds.Add((string)s);
                listChangedCallback(qrIdReorderableList);
            };

            qrIdReorderableList = new ReorderableList(effectIds, typeof(string))
            {
                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.LabelField(rect, (string)qrIdReorderableList.list[index]);
                },
                onAddDropdownCallback = (rect, list) =>
                {
                    var genericMenu = new GenericMenu();
                    foreach(var qr in qrIds)
                    {
                        genericMenu.AddItem(new GUIContent(qr), false, toAddSelected, qr);
                    }
                    genericMenu.ShowAsContext();
                },
                onRemoveCallback = (r) =>
                {
                    effectIds.RemoveAt(r.index);
                    listChangedCallback(r);
                },
                onReorderCallback = (r) => listChangedCallback(r)
            };

        }

        // -------------------------
        // Properties
        // --------------------------
        public bool Collapsed { get; set; }
        public string EffectName { get { return "QR.QRScannerEffect.Title".Traslate(); } }
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
            set { effect = value as QRScannerEffect; }
        }

        // ----------------------------
        // Methods
        //--------------------------

        public IEffectEditor clone()
        {
            return new QRScannerEffectEditor();
        }

        public bool manages(IEffect c)
        {
            return c is QRScannerEffect;
        }

        // -------------------------
        // Draw method
        // ---------------------
        public void draw()
        {
            if (!Usable)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            var newScannerMessage = EditorGUILayout.TextField("QR.QRScannerEffectEditor.Message".Traslate(), effect.ScannerMessage, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Controller.Instance.AddTool(new ChangeValueTool<QRScannerEffect, string>(effect, newScannerMessage, "ScannerMessage"));
            }
            EditorGUI.BeginChangeCheck();
            var newSelectionType = (QRScannerEffect.ListType) EditorGUILayout.Popup("QR.QRScannerEffectEditor.ListType".Traslate(), (int) effect.SelectionType, listTypeNames);
            if (EditorGUI.EndChangeCheck())
            {
                Controller.Instance.AddTool(ChangeEnumValueTool.Create(effect, newSelectionType, "SelectionType"));
            }

            UpdateEffectQRIds();
            UpdateAvailableQRIds();
            qrIdReorderableList.DoLayoutList();
        }

        // -------------------------------
        // AUX METHODS
        // ----------------------------

        private void UpdateAvailableQRIds()
        {
            qrIds = Controller.Instance.IdentifierSummary
                .getIds<QR>()
                .Except(effectIds)
                .ToList();
            qrIdReorderableList.displayAdd = qrIds.Count > 0;
        }

        private void UpdateEffectQRIds()
        {
            effectIds.Clear();
            foreach (var qr in effect.ValidIds)
            {
                effectIds.Add(qr);
            }
        }

        public bool Usable { get { return Controller.Instance.IdentifierSummary.getIds<QR>().Length > 0; } }
    }
}