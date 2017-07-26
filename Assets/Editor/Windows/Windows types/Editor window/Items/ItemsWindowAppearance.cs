using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class ItemsWindowAppearance : LayoutWindow
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
        private DataControlList appearanceList;
        
        private static List<ResourcesDataControl> emptyList;

        public ItemsWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            conditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/conditions-24x24", typeof(Texture2D));
            noConditionsTex = (Texture2D)Resources.Load("EAdventureData/img/icons/no-conditions-24x24", typeof(Texture2D));

            appearanceList = new DataControlList()
            {
                headerHeight = 20,
                footerHeight = 20,
                Columns = new List<ColumnList.Column>()
                {
                    new ColumnList.Column(){
                        Text = TC.get("Item.LookPanelTitle"),
                        SizeOptions = new GUILayoutOption[]
                        {
                            GUILayout.ExpandWidth(true)
                        }
                    },
                    new ColumnList.Column(){
                        Text = TC.get("Conditions.Title"),
                        SizeOptions = new GUILayoutOption[]
                        {
                            GUILayout.ExpandWidth(true)
                        }
                    }
                },
                drawCell = (rect, index, col, isActive, isFocused) =>
                {
                    var resources = workingItem.getResources()[index];
                    switch (col)
                    {
                        case 0:
                            if (index == appearanceList.index)
                            {
                                resources.renameElement(EditorGUI.TextField(rect, resources.getName()));
                            }
                            else
                            {
                                EditorGUI.LabelField(rect, resources.getName());
                            }
                            break;
                        case 1:
                            if (GUI.Button(rect, resources.getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex))
                            {
                                ConditionEditorWindow window =
                                     (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                                window.Init(resources.getConditions());
                            }
                            break;
                    }
                },
                onSelectCallback = (list) =>
                {
                    if (list.index == -1) list.index = 0;
                    workingItem.setSelectedResources(list.index);
                    RefreshPathInformation();
                }
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
            var previousWorkingItem = workingItem;
            workingItem = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex];
            
            if (previousWorkingItem != workingItem)
            {
                appearanceList.SetData(workingItem, (data) => (data as DataControlWithResources).getResources().ConvertAll(r => (DataControl)r));
                appearanceList.index = workingItem.getSelectedResources();
                RefreshPathInformation();
            }

            if (workingItem == null)
            {
                appearanceList.list = emptyList;
                return;
            }
            else
            {
                appearanceList.list = workingItem.getResources();
            }
            
            // Appearance table
            appearanceList.DoList(200f);

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            string previousValue = image.Path = workingItem.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if(previousValue != image.Path) workingItem.setPreviewImage(image.Path);

            previousValue = icon.Path = workingItem.getIconImage();
            icon.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != icon.Path) workingItem.setIconImage(icon.Path);

            previousValue = image_over.Path = workingItem.getMouseOverImage();
            image_over.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image_over.Path) workingItem.setMouseOverImage(image_over.Path);

            if (EditorGUI.EndChangeCheck())
            {
                RefreshPathInformation();
            }

            GUILayout.Space(10);

            var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    rect.x += 30;
                    rect.width -= 60;
                    rect.y += 30;
                    rect.height -= 60;

                    GUI.DrawTexture(rect, rect.Contains(Event.current.mousePosition) && imageOverTex ? imageOverTex : imageTex, ScaleMode.ScaleToFit);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void RefreshPathInformation()
        {
            Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].setSelectedResources(appearanceList.index);

            imagePath =
                  Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].getPreviewImage();

            imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;

            inventoryIconPath =
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getIconImage();

            iconTex = string.IsNullOrEmpty(inventoryIconPath) ? null :AssetsController.getImage(inventoryIconPath).texture;

            imageWhenOverPath =
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getMouseOverImage();
            
            imageOverTex = string.IsNullOrEmpty(imageWhenOverPath) ? null : AssetsController.getImage(imageWhenOverPath).texture;
        }
    }
}