using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ItemsWindowAppearance : LayoutWindow, DialogReceiverInterface
    {

        private enum AssetType
        {
            ITEM,
            ICON,
            ITEM_OVER
        };

        private Texture2D addTex = null;
        private Texture2D duplicateTex = null;
        private Texture2D clearTex = null;

        private Texture2D imageTex = null;

        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;
        private Texture2D tmpTex = null;
        
        private static Rect previewRect, appearanceTableRect, propertiesTable, rightPanelRect;

        private static GUISkin defaultSkin;
        private static GUISkin noBackgroundSkin;
        private static GUISkin selectedAreaSkin;

        private Vector2 scrollPosition;

        private string imagePath = "";
        private string inventoryIconPath = "";
        private string imageWhenOverPath = "";

        private ItemDataControl workingItem;
        private ScrollableList appearanceList;

        private string apperanceName = "", apperanceNameLast = "";
        private static List<ResourcesDataControl> emptyList;

        public ItemsWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
            duplicateTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            if (GameRources.GetInstance().selectedItemIndex >= 0)
            {
                imagePath =
                    Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getPreviewImage();
                inventoryIconPath =
                    Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getIconImage();
                imageWhenOverPath =
                    Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                        GameRources.GetInstance().selectedItemIndex].getMouseOverImage();
            }

            if (imagePath != null && !imagePath.Equals(""))
                imageTex = AssetsController.getImage(imagePath).texture;

            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
            selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));

            appearanceList = new ScrollableList(emptyList = new List<ResourcesDataControl>(), typeof(ResourcesDataControl));

            appearanceList.headerHeight = 20;
            appearanceList.footerHeight = 20;

            appearanceList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), TC.get("Item.LookPanelTitle"));
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height), TC.get("Conditions.Title"));
            };

            appearanceList.drawFooterCallback += (rect) =>
            {
                float xMax = rect.xMax;
                float num = xMax - 83f;

                rect = new Rect(num, rect.y, xMax - num, rect.height);
                Rect addRect = new Rect(num + 4f, rect.y - 1f, 22f, 13f);
                Rect dupRect = new Rect(addRect.x + addRect.width + 4f, rect.y - 1f, 22f, 13f);
                Rect delRect = new Rect(dupRect.x + dupRect.width + 4f, rect.y - 1f, 22f, 13f);

                if (Event.current.type == EventType.Repaint)
                {
                    ScrollableList.defaultBehaviours.footerBackground.Draw(rect, false, false, false, false);
                }

                var buttonStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).button);
                buttonStyle.margin = new RectOffset(3, 0, 0, 0);
                buttonStyle.padding = new RectOffset(3, 0, 0, 0);
                buttonStyle.normal.background = buttonStyle.onNormal.background = null;
                buttonStyle.active.background = buttonStyle.onActive.background = null;
                buttonStyle.focused.background = buttonStyle.onFocused.background = null;
                buttonStyle.hover.background = buttonStyle.onHover.background = null;

                // Add
                if (GUI.Button(addRect, addTex, buttonStyle))
                {
                    if (appearanceList.onAddCallback == null)
                    {
                        ScrollableList.defaultBehaviours.DoAddButton(appearanceList.reorderableList);
                    }
                    else
                    {
                        appearanceList.onAddCallback(appearanceList.reorderableList);
                    }
                    if (appearanceList.onChangedCallback != null)
                    {
                        appearanceList.onChangedCallback(appearanceList.reorderableList);
                    }
                }

                // Duplicate
                using (new EditorGUI.DisabledScope(appearanceList.index < 0))
                {
                    if (GUI.Button(dupRect, duplicateTex, buttonStyle))
                    {
                        if (appearanceList.onRemoveCallback == null)
                        {
                            ScrollableList.defaultBehaviours.DoRemoveButton(appearanceList.reorderableList);
                        }
                        else
                        {
                            appearanceList.onRemoveCallback(appearanceList.reorderableList);
                        }
                        if (appearanceList.onChangedCallback != null)
                        {
                            appearanceList.onChangedCallback(appearanceList.reorderableList);
                        }
                    }
                }

                // Remove
                using (new EditorGUI.DisabledScope(appearanceList.index < 0 || appearanceList.index >= appearanceList.count || (appearanceList.onCanRemoveCallback != null && !appearanceList.onCanRemoveCallback(appearanceList.reorderableList))))
                {
                    if (GUI.Button(delRect, clearTex, buttonStyle))
                    {
                        if (appearanceList.onRemoveCallback == null)
                        {
                            ScrollableList.defaultBehaviours.DoRemoveButton(appearanceList.reorderableList);
                        }
                        else
                        {
                            appearanceList.onRemoveCallback(appearanceList.reorderableList);
                        }
                        if (appearanceList.onChangedCallback != null)
                        {
                            appearanceList.onChangedCallback(appearanceList.reorderableList);
                        }
                    }
                }

                /*GUI.skin = noBackgroundSkin;
                if (GUI.Button(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), addTex))
                {
                }
                if (GUI.Button(new Rect(rect.x + rect.width / 3f, rect.y, rect.width / 3f, rect.height), duplicateTex))
                {
                    workingItem.duplicateElement(workingItem.getResources()[appearanceList.index]);
                }
                if (GUI.Button(new Rect(rect.x + 2f * rect.width / 3f, rect.y, rect.width / 3f, rect.height), clearTex))
                {
                }
                GUI.skin = defaultSkin;*/
            };

            appearanceList.onAddCallback += (list) =>
            {
                workingItem.addElement(Controller.RESOURCES, "");
            };
            
            appearanceList.onRemoveCallback += (list) =>
            {
                workingItem.deleteElement(workingItem.getResources()[list.index], false);
            };

            appearanceList.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                var resources = workingItem.getResources()[index];
                var leftRect = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
                var rightRect = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);

                GUILayout.BeginHorizontal(); 
                if (index == appearanceList.index)
                {
                    resources.renameElement(EditorGUI.TextField(leftRect, resources.getName()));
                }
                else
                {
                    EditorGUI.LabelField(leftRect, resources.getName());
                }

                if (GUI.Button(rightRect, resources.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                {
                    ConditionEditorWindow window =
                         (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                    window.Init(resources.getConditions());
                }
                GUILayout.EndHorizontal();
            };
        }


        public override void Draw(int aID)
        {
            workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];
            if (workingItem == null)
            {
                appearanceList.list = emptyList;
                return;
            }
            else
            {
                appearanceList.list = workingItem.getResources();
            }


            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            appearanceTableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, 0.2f * windowHeight);
            rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.2f * windowHeight);
            propertiesTable = new Rect(0f, 0.3f * windowHeight, 0.95f * windowWidth, 0.3f * windowHeight);
            previewRect = new Rect(0f, 0.6f * windowHeight, windowWidth, windowHeight * 0.35f);
            
            // Appearance table
            appearanceList.DoList(200f);


            /*for (int i = 0; i < selectedItem.getResourcesCount(); i++)
            {
                if (i == selectedAppearance)
                    GUI.skin = selectedAreaSkin;
                else
                    GUI.skin = noBackgroundSkin;

                tmpTex = (selectedItem.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex);

                GUI.skin = defaultSkin; 
            }*/

            /*
            * Right panel
            */


            GUILayout.Space(30);


            GUILayout.BeginArea(propertiesTable);
            // Background chooser
            GUILayout.Label(TC.get("Resources.DescriptionItemImage"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearTex, GUILayout.Width(0.05f * windowWidth)))
            {
                imagePath = "";
            }
            GUILayout.Box(imagePath, GUILayout.Width(0.7f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.19f * windowWidth)))
            {
                ShowAssetChooser(AssetType.ITEM);
            }
            GUILayout.EndHorizontal();

            // Icon chooser
            GUILayout.Label(TC.get("Resources.DescriptionItemIcon"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearTex, GUILayout.Width(0.05f * windowWidth)))
            {
                inventoryIconPath = "";
            }
            GUILayout.Box(inventoryIconPath, GUILayout.Width(0.7f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.19f * windowWidth)))
            {
                ShowAssetChooser(AssetType.ICON);
            }
            GUILayout.EndHorizontal();

            // Image over chooser
            GUILayout.Label(TC.get("Resources.DescriptionItemImageOver"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearTex, GUILayout.Width(0.05f * windowWidth)))
            {
                imageWhenOverPath = "";
            }
            GUILayout.Box(imageWhenOverPath, GUILayout.Width(0.7f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.19f * windowWidth)))
            {
                ShowAssetChooser(AssetType.ITEM_OVER);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            if (imagePath != "")
            {
                GUI.DrawTexture(previewRect, imageTex, ScaleMode.ScaleToFit);
            }
        }

        void ShowAssetChooser(AssetType type)
        {
            switch (type)
            {
                case AssetType.ITEM:
                    ImageFileOpenDialog itemDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                    itemDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_IMAGE);
                    break;
                case AssetType.ICON:
                    ImageFileOpenDialog iconDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                    iconDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_ICON);
                    break;
                case AssetType.ITEM_OVER:
                    ImageFileOpenDialog itemOverDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                    itemOverDialog.Init(this, BaseFileOpenDialog.FileType.ITEM_IMAGE_OVER);
                    break;
            }

        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            var selectedItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex];

            if (workingObject is BaseFileOpenDialog.FileType)
            {
                switch ((BaseFileOpenDialog.FileType)workingObject)
                {
                    case BaseFileOpenDialog.FileType.ITEM_IMAGE:
                        imagePath = message;
                        selectedItem.setPreviewImage(imagePath);
                        break;
                    case BaseFileOpenDialog.FileType.ITEM_ICON:
                        inventoryIconPath = message;
                        selectedItem.setIconImage(inventoryIconPath);
                        break;
                    case BaseFileOpenDialog.FileType.ITEM_IMAGE_OVER:
                        imageWhenOverPath = message;
                        selectedItem.setMouseOverImage(imageWhenOverPath);
                        break;
                    default:
                        break;
                }
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log("Wiadomość nie OK");
        }

        //private void OnAppearanceNameChange(string val)
        //{
        //    apperanceNameLast = val;
        //    Controller.getInstance().getSelectedChapterDataControl().getItemsList().getItems()[
        //            GameRources.GetInstance().selectedItemIndex].getResources()[selectedAppearance].
        //}


        private void OnAppearanceSelectionChange(int i)
        {
            apperanceName =
                apperanceNameLast = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getResources()[appearanceList.index].getName();
            RefreshPathInformation();
        }

        private void RefreshPathInformation()
        {
            Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].setSelectedResources(appearanceList.index);

            imagePath =
                  Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].getPreviewImage();
            inventoryIconPath =
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getIconImage();
            imageWhenOverPath =
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getMouseOverImage();
        }
    }
}