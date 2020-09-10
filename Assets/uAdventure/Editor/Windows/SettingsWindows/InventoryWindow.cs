using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class InventoryWindow : DefaultButtonMenuEditorWindowExtension
    {
        private const string INVENTORY_STRING = "Inventory.Type";
        private InventoryPreview inventoryPreview;

        public InventoryWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
            : base(rect, content, style)
        {
            Options = options;
            ButtonContent = new GUIContent("Inventory");
            inventoryPreview = new InventoryPreview(rect, content, style, options);
            inventoryPreview.OnRequestRepaint += () => Repaint();
            inventoryPreview.EndWindows = () => EndWindows();
            inventoryPreview.BeginWindows = () => BeginWindows();
        }

        public override void Draw(int aID)
        {
            GUILayout.Label("GeneralText.Information".Traslate(), EditorStyles.boldLabel);
            GUILayout.Label("Inventory.Information".Traslate());
            GUILayout.Space(10);

            var adventureData = Controller.Instance.AdventureData;

            EditorGUI.BeginChangeCheck();

            var newInventoryPosition = EditorGUILayout.Popup(adventureData.getInventoryPosition(), DescriptorData.getInventoryTypes().Select(t => TC.get(INVENTORY_STRING + t)).ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                adventureData.setInventoryPosition(newInventoryPosition);
            }
            var lastRect = GUILayoutUtility.GetLastRect();

            if(Event.current.type == EventType.Repaint)
            {
                var previewRect = Rect;
                previewRect.y = lastRect.y + lastRect.height + 20;
                previewRect.height -= (lastRect.y + lastRect.height + 20);
                inventoryPreview.Rect = previewRect;
            }
            inventoryPreview.Draw(aID);

        }

        protected override void OnButton()
        {
        }

        private class InventoryPreview : PreviewLayoutWindow 
        {
            private Texture2D defaultBackground;
            private FileChooser iconChooser;
            private Texture2D icon;

            public InventoryPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
                : base(rect, content, style, options)
            {
                iconChooser = new FileChooser
                {
                    FileType = FileType.BUTTON,
                    Empty = SpecialAssetPaths.ASSET_DEFAULT_INVENTORY,
                    Label = TC.get("Inventory.Image")
                };
                defaultBackground = Controller.ResourceManager.getImage(SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                ReloadIcon();
            }
            protected override void DrawPreviewHeader()
            {
                base.DrawPreviewHeader();
                previewHeight = Rect.height;
            }

            public override void DrawPreview(Rect rect)
            {
                base.DrawPreview(rect);
                var adventureData = Controller.Instance.AdventureData;
                if(defaultBackground != null)
                {
                    GUI.DrawTexture(rect, defaultBackground, ScaleMode.ScaleToFit);
                }

                if(icon != null)
                {
                    var scale = adventureData.getInventoryScale();
                    var pos = adventureData.getInventoryCoords();
                    var inventoryRect = new Rect(pos.x + icon.width * scale/2f, pos.y + icon.height, icon.width * scale, icon.height * scale);
                    GUI.DrawTexture(inventoryRect, icon);

                    EditorGUI.BeginChangeCheck();
                    var inventoryId = GUIUtility.GetControlID(FocusType.Passive, inventoryRect);
                    var newRect = HandleUtil.HandleFixedRatioRect(inventoryId, inventoryRect, icon.width/(float)icon.height, 10f,
                        (polygon, over, active) => HandleUtil.DrawPolyLine(polygon, true, Color.red, over || active ? 4f : 2f),
                        (point, over, active) => HandleUtil.DrawPoint(point, 4.5f, Color.blue, over || active ? 2f : 1f, over || active ? Color.red : Color.black));

                    if (EditorGUI.EndChangeCheck())
                    {
                        var newScale = newRect.width / (float)icon.width;
                        var newCoords = rect.center + new Vector2(0, rect.height / 2f);

                        adventureData.setInventoryCoords(newCoords);
                        adventureData.setInventoryScale(newScale);
                    }
                }

            }

            protected override bool HasToDrawPreviewInspector()
            {
                var adventureData = Controller.Instance.AdventureData;
                return adventureData.getInventoryPosition() == DescriptorData.INVENTORY_ICON_FREEPOS;
            }

            protected override void DrawPreviewInspector()
            {
                base.DrawPreviewInspector();
                var adventureData = Controller.Instance.AdventureData;
                EditorGUI.BeginChangeCheck();
                iconChooser.Path = string.IsNullOrEmpty(adventureData.getInventoryImage()) ? null : adventureData.getInventoryImage();
                iconChooser.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setInventoryImage(iconChooser.Path);
                    ReloadIcon();
                }

                EditorGUI.BeginChangeCheck();
                var newPosition = EditorGUILayout.Vector2Field(TC.get("Inventory.Coords"), adventureData.getInventoryCoords());
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setInventoryCoords(newPosition);
                }

                EditorGUI.BeginChangeCheck();
                var newScale = EditorGUILayout.FloatField(TC.get("Inventory.Scale"), adventureData.getInventoryScale());
                if (EditorGUI.EndChangeCheck())
                {
                    adventureData.setInventoryScale(newScale);
                }
            }

            private void ReloadIcon()
            {
                if (string.IsNullOrEmpty(iconChooser.Path))
                {
                    icon = Controller.ResourceManager.getImage(iconChooser.Path);
                }
                else
                {
                    icon = Controller.ResourceManager.getImage(iconChooser.Empty);
                }
            }
        }
    }
}
