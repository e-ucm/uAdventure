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

        private Texture2D imageTex = null;

        private FileChooser image;
        
        private AppearanceEditor appearanceEditor;

        private static List<ResourcesDataControl> emptyList;

        public SetItemsWindowApperance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;

            // File selectors

            image = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImage"),
                FileType = BaseFileOpenDialog.FileType.SET_ITEM_IMAGE
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

            string previousValue = image.Path = workingAtrezzo.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image.Path) workingAtrezzo.setImage(image.Path);


            if (EditorGUI.EndChangeCheck())
            {
                RefreshPathInformation(workingAtrezzo);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var elem = Target != null ? Target as AtrezzoDataControl : Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];
            var imagePath = elem.getPreviewImage();
            var imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;

            GUI.DrawTexture(rect, imageTex, ScaleMode.ScaleToFit);
        }

        public override void OnRender(Rect viewport)
        {
            var imagePath = (Target as AtrezzoDataControl).getPreviewImage();
            var imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;

            var rect = GetViewportRect(new Rect(new Vector2(-0.5f * imageTex.width, -imageTex.height), new Vector2(imageTex.width, imageTex.height)), viewport);
            GUI.DrawTexture(rect, imageTex, ScaleMode.ScaleToFit);
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var imagePath = (data as AtrezzoDataControl).getPreviewImage();
            imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
        }
    }
}