using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ItemsWindowAppearance : LayoutWindow
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

        private FileChooser image, icon, image_over;

        private ItemDataControl workingItem;
        private DataControlList appearanceList;

        private string apperanceName = "", apperanceNameLast = "";
        private static List<ResourcesDataControl> emptyList;

        public ItemsWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));
            

            noBackgroundSkin = (GUISkin)Resources.Load("Editor/EditorNoBackgroundSkin", typeof(GUISkin));
            selectedAreaSkin = (GUISkin)Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof(GUISkin));
            
            appearanceList = new DataControlList()
            {
                headerHeight = 20,
                footerHeight = 20
            };
            appearanceList.drawHeaderCallback += (rect) =>
            {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 2f, rect.height), TC.get("Item.LookPanelTitle"));
                EditorGUI.LabelField(new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height), TC.get("Conditions.Title"));
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


        public override void Draw(int aID)
        {
            workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];
            appearanceList.SetData(workingItem, (data) => (data as DataControlWithResources).getResources().ConvertAll(r => (DataControl)r));

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
            
            // Appearance table
            appearanceList.DoList(200f);

            GUILayout.Space(10);

            string previousValue = image.Path = workingItem.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if(previousValue != image.Path) workingItem.setPreviewImage(image.Path);

            previousValue = icon.Path = workingItem.getIconImage();
            icon.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != icon.Path) workingItem.setIconImage(icon.Path);

            previousValue = image_over.Path = workingItem.getMouseOverImage();
            image_over.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image_over.Path) workingItem.setMouseOverImage(image_over.Path);

            GUILayout.Space(10);

            var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    GUI.DrawTexture(previewRect, imageTex, ScaleMode.ScaleToFit);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            if (imagePath != "")
            {
            }
        }


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