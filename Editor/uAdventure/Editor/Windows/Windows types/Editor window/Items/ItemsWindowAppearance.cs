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
        private Texture2D imageOverTex = null;

        private readonly FileChooser image, icon, image_over;

        private ItemDataControl workingItem;
        private readonly ResourcesList appearanceEditor;

        public ItemsWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            appearanceEditor = ScriptableObject.CreateInstance<ResourcesList>();
            appearanceEditor.Height = 160;
            appearanceEditor.onResourceSelected = RefreshResources;

            PreviewTitle = "Item.Preview".Traslate();
            // File selectors

            image = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImage"),
                FileType = FileType.ITEM_IMAGE,
                Empty = SpecialAssetPaths.ASSET_EMPTY_IMAGE
            };

            icon = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemIcon"),
                FileType = FileType.ITEM_ICON,
                Empty = SpecialAssetPaths.ASSET_EMPTY_ICON
            };

            image_over = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImageOver"),
                FileType = FileType.ITEM_IMAGE_OVER
            };
        }

        protected override void DrawInspector()
        {
            workingItem = Target as ItemDataControl ?? Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];

            // Appearance table
            appearanceEditor.Data = workingItem;
            appearanceEditor.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            string previousValue = image.Path = workingItem.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image.Path)
            {
                workingItem.setPreviewImage(image.Path);
            }

            previousValue = icon.Path = workingItem.getIconImage();
            icon.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != icon.Path)
            {
                workingItem.setIconImage(icon.Path);
            } 

            previousValue = image_over.Path = workingItem.getMouseOverImage();
            image_over.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image_over.Path)
            {
                workingItem.setMouseOverImage(image_over.Path);
            }

            if (EditorGUI.EndChangeCheck())
            {
                RefreshResources(workingItem);
            }
        }

        private void RefreshResources(DataControlWithResources dataControl)
        {
            var item = dataControl as ItemDataControl;
            if(item == null)
            {
                return;
            }

            var imagePath         = item.getPreviewImage();
            var imageWhenOverPath = item.getMouseOverImage();

            imageTex     = string.IsNullOrEmpty(imagePath)         ? null : Controller.ResourceManager.getImage(imagePath);
            imageOverTex = string.IsNullOrEmpty(imageWhenOverPath) ? null : Controller.ResourceManager.getImage(imageWhenOverPath);
        }

        public override void DrawPreview(Rect rect)
        {
            var item = Target as ItemDataControl ?? Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];
            RefreshResources(item);

            if (imageTex == null)
            {
                return;
            }

            GUI.DrawTexture(rect, rect.Contains(Event.current.mousePosition) && imageOverTex ? imageOverTex : imageTex, ScaleMode.ScaleToFit);
        }

        public override void OnRender()
        {
            var item = Target as ItemDataControl;
            RefreshResources(item);

            if (imageTex == null)
            {
                return;
            }

            var rect = new RectD(new Vector2d(-0.5f * imageTex.width, -imageTex.height),
                new Vector2d(imageTex.width, imageTex.height));
            var adaptedRect = ComponentBasedEditor.Generic.ToRelative(rect.ToPoints()).ToRectD().ToRect();
            GUI.DrawTexture(adaptedRect, rect.Contains(Event.current.mousePosition.ToVector2d()) && imageOverTex ? imageOverTex : imageTex, ScaleMode.ScaleToFit);
        }


    }
}