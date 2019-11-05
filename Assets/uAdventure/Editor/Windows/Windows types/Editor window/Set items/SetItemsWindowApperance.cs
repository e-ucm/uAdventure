using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(AtrezzoDataControl), Name = "Atrezzo.LookPanelTitle", Order = 10)]
    public class SetItemsWindowApperance : AbstractEditorComponentWithPreview
    {
        private readonly FileChooser image;
        private readonly ResourcesList appearanceEditor;

        public SetItemsWindowApperance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            appearanceEditor = ScriptableObject.CreateInstance<ResourcesList>();
            appearanceEditor.Height = 160;

            PreviewTitle = "Atrezzo.Preview".Traslate();

            // File selectors

            image = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImage"),
                FileType = FileType.SET_ITEM_IMAGE
            };
        }

        protected override void DrawInspector()
        {
            var workingAtrezzo = Target != null ? Target as AtrezzoDataControl : Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];

            // Appearance table
            appearanceEditor.Data = workingAtrezzo;
            appearanceEditor.OnInspectorGUI();

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            image.Path = workingAtrezzo.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingAtrezzo.setImage(image.Path);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var imageTex = GetAtrezzoImage();
            if (imageTex)
            {
                GUI.DrawTexture(rect, imageTex, ScaleMode.ScaleToFit);
            }
        }

        public override void OnRender()
        {
            var imageTex = GetAtrezzoImage();
            if (!imageTex)
            {
                return;
            }

            var rect = new Rect(new Vector2(-0.5f * imageTex.width, -imageTex.height),
                new Vector2(imageTex.width, imageTex.height));
            var adaptedRect = ComponentBasedEditor.Generic.ToRelative(rect.ToRectD().ToPoints()).ToRectD().ToRect();
            GUI.DrawTexture(adaptedRect, imageTex, ScaleMode.ScaleToFit);

        }

        private AtrezzoDataControl GetAtrezzo()
        {
            var atrezzo = Target as AtrezzoDataControl;
            if(atrezzo == null)
            {
                atrezzo = Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];
            }
            return atrezzo;
        }

        private Texture2D GetAtrezzoImage()
        {
            var atrezzo = GetAtrezzo();
            if (atrezzo != null)
            {
                var imagePath = atrezzo.getPreviewImage();
                if (!string.IsNullOrEmpty(imagePath))
                {
                    return Controller.ResourceManager.getImage(imagePath);
                }
            }
            return null;
        }
    }
}