using UnityEngine;
using System.Collections;

using uAdventure.Core;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class SetItemsWindowApperance : LayoutWindow
    {

        private Texture2D imageTex = null;
        private Texture2D iconTex = null;
        private Texture2D imageOverTex = null;

        private Texture2D conditionsTex = null;
        private Texture2D noConditionsTex = null;

        private FileChooser image;

        private AtrezzoDataControl workingAtrezzo;
        private DataControlList appearanceList;

        private static List<ResourcesDataControl> emptyList;

        public SetItemsWindowApperance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
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
                    var resources = workingAtrezzo.getResources()[index];
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
                    workingAtrezzo.setSelectedResources(list.index);
                    RefreshPathInformation();
                }
            };
            // File selectors

            image = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionItemImage"),
                FileType = BaseFileOpenDialog.FileType.SET_ITEM_IMAGE
            };
        }


        public override void Draw(int aID)
        {
            var previousWorkingItem = workingAtrezzo;
            workingAtrezzo = Controller.Instance.SelectedChapterDataControl.getAtrezzoList().getAtrezzoList()[GameRources.GetInstance().selectedSetItemIndex];

            if (previousWorkingItem != workingAtrezzo)
            {
                appearanceList.SetData(workingAtrezzo, (data) => (data as DataControlWithResources).getResources().ConvertAll(r => (DataControl)r));
                appearanceList.index = workingAtrezzo.getSelectedResources();
                RefreshPathInformation();
            }

            if (workingAtrezzo == null)
            {
                appearanceList.list = emptyList;
                return;
            }
            else
            {
                appearanceList.list = workingAtrezzo.getResources();
            }

            // Appearance table
            appearanceList.DoList(200f);

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            string previousValue = image.Path = workingAtrezzo.getPreviewImage();
            image.DoLayout(GUILayout.ExpandWidth(true));
            if (previousValue != image.Path) workingAtrezzo.setImage(image.Path);
            

            if (EditorGUI.EndChangeCheck())
            {
                RefreshPathInformation();
            }

            GUILayout.Space(10);

            GUILayout.Label(TC.get("ImageAssets.Preview"));
            var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    rect.x += 30; rect.width -= 60;
                    rect.y += 30; rect.height -= 60;

                    GUI.DrawTexture(rect, imageTex, ScaleMode.ScaleToFit);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void RefreshPathInformation()
        {
            var imagePath = workingAtrezzo.getPreviewImage();
            imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
        }
    }
}