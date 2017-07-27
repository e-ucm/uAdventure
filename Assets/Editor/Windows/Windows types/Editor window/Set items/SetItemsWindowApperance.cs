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
        private AppearanceEditor appearanceEditor;

        private static List<ResourcesDataControl> emptyList;

        public SetItemsWindowApperance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
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
                FileType = BaseFileOpenDialog.FileType.SET_ITEM_IMAGE
            };
        }


        public override void Draw(int aID)
        {
            var previousWorkingItem = workingAtrezzo;
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

            GUILayout.Space(10);
            GUILayout.Label(TC.get("ImageAssets.Preview"), "preToolbar", GUILayout.ExpandWidth(true));
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

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var imagePath = (data as AtrezzoDataControl).getPreviewImage();
            imageTex = string.IsNullOrEmpty(imagePath) ? null : AssetsController.getImage(imagePath).texture;
        }
    }
}