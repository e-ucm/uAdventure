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
            inventoryPreview = new InventoryPreview(rect, content, style, options)
            {
                Resizable = false,
                EndWindows = () => EndWindows(),
                BeginWindows = () => BeginWindows(),
                OnRequestRepaint = () => Repaint()
            };
        }

        public override void Draw(int aID)
        {
            inventoryPreview.Rect = Rect;
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
            private bool growing;
            private float scale;
            private float speed = 30; // Pixels per second
            private float progressTop, progressBottom;

            public InventoryPreview(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
                : base(rect, content, style, options)
            {
                iconChooser = new FileChooser
                {
                    FileType = FileType.BUTTON,
                    Empty = SpecialAssetPaths.ASSET_DEFAULT_INVENTORY,
                    Label = TC.get("Inventory.Image")
                };
                var adventureData = Controller.Instance.AdventureData;
                iconChooser.Path = string.IsNullOrEmpty(adventureData.getInventoryImage()) ? iconChooser.Empty : adventureData.getInventoryImage();
                defaultBackground = Controller.ResourceManager.getImage(SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                ReloadIcon();
                EditorApplication.update += () =>
                {
                    if (growing)
                    {
                        if(scale > 0)
                        {
                            progressTop += (speed / 100f) * Time.deltaTime;
                            progressBottom -= (speed / 100f) * Time.deltaTime;
                        }
                        else
                        {
                            progressTop -= (speed / 100f) * Time.deltaTime;
                            progressBottom += (speed / 100f) * Time.deltaTime;
                        }
                    }
                    else
                    {
                        progressTop -= (speed / 100f) * Time.deltaTime;
                        progressBottom -= (speed / 100f) * Time.deltaTime;
                    }
                    Repaint();
                };
            }

            public override void DrawPreview(Rect rect)
            {
                base.DrawPreview(rect);
                var adventureData = Controller.Instance.AdventureData;
                var backgroundScale = 1f;
                if (defaultBackground != null)
                {
                    GUI.DrawTexture(rect, defaultBackground, ScaleMode.ScaleToFit);
                    backgroundScale = Mathf.Min(rect.width / defaultBackground.width, rect.height / defaultBackground.height);
                }

                var matrix = Matrix4x4.TRS(rect.center - (new Vector2(defaultBackground.width, defaultBackground.height) * backgroundScale / 2f),
                    Quaternion.identity, new Vector3(backgroundScale, backgroundScale));
                if(Event.current.type == EventType.Repaint)
                {
                    growing = false;
                }
                switch (adventureData.getInventoryPosition())
                {
                    case DescriptorData.INVENTORY_NONE:
                        GUI.Label(rect, "NO INVENTORY");
                        break;
                    case DescriptorData.INVENTORY_TOP_BOTTOM:
                        progressTop = DrawInventory(adventureData, matrix, new Vector2(0, 0), new Vector2(800, 600), 1, progressTop);
                        progressBottom = DrawInventory(adventureData, matrix, new Vector2(0, 600), new Vector2(800, 600), -1, progressBottom);
                        break;
                    case DescriptorData.INVENTORY_TOP:
                        progressTop = DrawInventory(adventureData, matrix, new Vector2(0, 0), new Vector2(800, 600), 1, progressTop);
                        break;
                    case DescriptorData.INVENTORY_BOTTOM:
                        progressBottom = DrawInventory(adventureData, matrix, new Vector2(0, 600), new Vector2(800, 600), -1, progressBottom);
                        break;
                    case DescriptorData.INVENTORY_FIXED_TOP:
                        DrawInventory(adventureData, matrix, new Vector2(0, 0), new Vector2(800, 600), 1, 1);
                        break;
                    case DescriptorData.INVENTORY_FIXED_BOTTOM:
                        DrawInventory(adventureData, matrix, new Vector2(0, 600), new Vector2(800, 600), -1, 1);
                        break;
                    case DescriptorData.INVENTORY_ICON_FREEPOS:
                        DrawIconInventory(adventureData, matrix);
                        break;
                }


            }

            private float DrawInventory(AdventureDataControl adventureData, Matrix4x4 matrix, Vector2 corner, Vector2 backgroundSize, float scale, float progress)
            {
                corner = matrix.MultiplyPoint(corner);
                backgroundSize = matrix.MultiplyVector(backgroundSize);

                var rectOpen = new Rect(corner, new Vector2(backgroundSize.x, scale * 100f * 1));
                var rect = new Rect(corner, new Vector2(backgroundSize.x, scale * 100 * progress));

                if(rectOpen.height < 0)
                {
                    rectOpen.y += rectOpen.height;
                    rectOpen.height = -rectOpen.height;
                }

                Handles.DrawSolidRectangleWithOutline(rect, new Color(0.1f, 0.1f, 0.1f, 0.2f), new Color(0.1f, 0.1f, 0.1f, 0.8f));
                if(Event.current.type == EventType.Repaint)
                {
                    if (rectOpen.Contains(Event.current.mousePosition))
                    {
                        growing = true;
                        this.scale = scale;
                    }
                }
                return Mathf.Clamp01(progress);
            }

            private void DrawIconInventory(AdventureDataControl adventureData, Matrix4x4 matrix)
            {
                if (icon != null)
                {
                    var scale = adventureData.getInventoryScale();
                    var pos = matrix.MultiplyPoint3x4(adventureData.getInventoryCoords());

                    var textureSize = matrix.MultiplyVector(new Vector2(icon.width, icon.height));
                    var inventorySize = textureSize * scale;
                    var inventoryCorner = pos - new Vector3(inventorySize.x / 2f, inventorySize.y);

                    var inventoryRect = new Rect(inventoryCorner, inventorySize);
                    GUI.DrawTexture(inventoryRect, icon);

                    EditorGUI.BeginChangeCheck();
                    var inventoryId = GUIUtility.GetControlID("SizeId".GetHashCode(), FocusType.Passive, inventoryRect);
                    var newRect = HandleUtil.HandleFixedRatioRect(inventoryId, inventoryRect, icon.width / (float)icon.height, 10f,
                        (polygon, over, active) => HandleUtil.DrawPolyLine(polygon, true, Color.red, over || active ? 4f : 2f),
                        (point, over, active) => HandleUtil.DrawPoint(point, 4.5f, Color.blue, over || active ? 2f : 1f, over || active ? Color.red : Color.black));

                    if (EditorGUI.EndChangeCheck())
                    {
                        var newScale = newRect.width / (float)textureSize.x;
                        var newCoords = InverseMultiplyPoint(matrix, newRect.center + new Vector2(0, newRect.height / 2f));

                        adventureData.setInventoryCoords(newCoords);
                        adventureData.setInventoryScale(newScale);
                    }

                    EditorGUI.BeginChangeCheck();
                    var inventoryMovementId = GUIUtility.GetControlID("MovementId".GetHashCode(), FocusType.Passive, inventoryRect);
                    newRect = HandleUtil.HandleRectMovement(inventoryMovementId, newRect);
                    if (EditorGUI.EndChangeCheck())
                    {

                        var newCoords = InverseMultiplyPoint(matrix, newRect.center + new Vector2(0, newRect.height / 2f));
                        adventureData.setInventoryCoords(newCoords);
                    }
                }
            }

            private Vector2 InverseMultiplyPoint(Matrix4x4 matrix, Vector2 point)
            {
                var zero = matrix.MultiplyPoint(Vector3.zero);
                var scale = matrix.MultiplyVector(Vector3.one);
                var absolutePoint = new Vector3(point.x, point.y) - zero;
                var unscaledPoint = new Vector2(absolutePoint.x / scale.x, absolutePoint.y / scale.y);
                return unscaledPoint;
            }

            protected override void DrawInspector()
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
                iconChooser.Path = string.IsNullOrEmpty(adventureData.getInventoryImage()) ? iconChooser.Empty : adventureData.getInventoryImage();
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
                if (!string.IsNullOrEmpty(iconChooser.Path))
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
