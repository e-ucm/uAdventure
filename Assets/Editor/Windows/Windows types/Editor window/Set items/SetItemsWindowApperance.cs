using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace uAdventure.Editor
{
    public class SetItemsWindowApperance : PreviewLayoutWindow
    {

        private Texture2D imageTex = null;

        private FileChooser image;

        private AtrezzoDataControl workingAtrezzo;
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
            workingAtrezzo = Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];

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

        protected override void DrawPreview(Rect rect)
        {
            GUI.DrawTexture(rect, imageTex, ScaleMode.ScaleToFit);
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var imagePath = (data as AtrezzoDataControl).getPreviewImage();
            imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
        }
    }
}