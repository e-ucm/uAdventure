using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(ItemDataControl), Name = "Item.LookPanelTitle", Order = 5)]
    public class ItemsWindowAppearance : AbstractEditorComponentWithPreview
    {

        private Texture2D imageTex = null;
        private Texture2D iconTex = null;
        private Texture2D imageOverTex = null;

        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;

        private string imagePath = "";
        private string inventoryIconPath = "";
        private string imageWhenOverPath = "";

        private FileChooser image, icon, image_over;

        private ItemDataControl workingItem;
        private AppearanceEditor appearanceEditor;

        public ItemsWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;
            // File selectors

            image = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImage"),
                FileType = BaseFileOpenDialog.FileType.ITEM_IMAGE
            };

            icon = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemIcon"),
                FileType = BaseFileOpenDialog.FileType.ITEM_ICON
            };

            image_over = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImageOver"),
                FileType = BaseFileOpenDialog.FileType.ITEM_IMAGE_OVER
            };
        }

        protected override void DrawInspector()
        {
            var previousWorkingItem = workingItem;
            workingItem = Target != null ? Target as ItemDataControl : Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];

            // Appearance table
            appearanceEditor.Data = workingItem;
            appearanceEditor.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            string previousValue = image.Path = workingItem.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image.Path) workingItem.setPreviewImage(image.Path);

            previousValue = icon.Path = workingItem.getIconImage();
            icon.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != icon.Path) workingItem.setIconImage(icon.Path);

            previousValue = image_over.Path = workingItem.getMouseOverImage();
            image_over.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image_over.Path) workingItem.setMouseOverImage(image_over.Path);

            if (EditorGUI.EndChangeCheck())
            {
                RefreshPathInformation(workingItem);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var item = Target != null ? Target as ItemDataControl : Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];
            imagePath = item.getPreviewImage();
            var imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
            imagePath = item.getIconImage();
            var iconTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
            imagePath = item.getMouseOverImage();
            var imageOverTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;

            if (imageTex == null)
                return;

            GUI.DrawTexture(rect, rect.Contains(Event.current.mousePosition) && imageOverTex ? imageOverTex : imageTex, ScaleMode.ScaleToFit);
        }

        private void RefreshPathInformation(DataControlWithResources dataControl)
        {
            var item = dataControl as ItemDataControl;

            imagePath           = item.getPreviewImage();
            inventoryIconPath   = item.getIconImage();
            imageWhenOverPath   = item.getMouseOverImage();

            imageTex        = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
            iconTex         = string.IsNullOrEmpty(inventoryIconPath) ? null :AssetsController.getImage(inventoryIconPath).texture;
            imageOverTex    = string.IsNullOrEmpty(imageWhenOverPath) ? null : AssetsController.getImage(imageWhenOverPath).texture;
        }

        public override void OnRender(Rect viewport)
        {
            var item = Target as ItemDataControl;
            imagePath = item.getPreviewImage();
            var imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
            imagePath = item.getIconImage();
            var iconTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
            imagePath = item.getMouseOverImage();
            var imageOverTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;

            if (imageTex == null)
                return;
            
            var rect = GetViewportRect(new Rect(new Vector2(-0.5f * imageTex.width, -imageTex.height), new Vector2(imageTex.width, imageTex.height)), viewport);
            GUI.DrawTexture(rect, rect.Contains(Event.current.mousePosition) && imageOverTex ? imageOverTex : imageTex, ScaleMode.ScaleToFit);
        }


    }
}