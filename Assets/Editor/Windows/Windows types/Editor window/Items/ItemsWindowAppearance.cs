using UnityEngine;
using System.Collections;

using uAdventure.Core;

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

        private int selectedAppearance;

        private string apperanceName = "", apperanceNameLast = "";

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

            selectedAppearance = -1;
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            appearanceTableRect = new Rect(0f, 0.1f * windowHeight, 0.9f * windowWidth, 0.2f * windowHeight);
            rightPanelRect = new Rect(0.9f * windowWidth, 0.1f * windowHeight, 0.08f * windowWidth, 0.2f * windowHeight);
            propertiesTable = new Rect(0f, 0.3f * windowHeight, 0.95f * windowWidth, 0.3f * windowHeight);
            previewRect = new Rect(0f, 0.6f * windowHeight, windowWidth, windowHeight * 0.35f);

            GUILayout.BeginArea(appearanceTableRect);
            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("Item.LookPanelTitle"), GUILayout.Width(windowWidth * 0.44f));
            GUILayout.Box(TC.get("Conditions.Title"), GUILayout.Width(windowWidth * 0.44f));
            GUILayout.EndHorizontal();
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            // Appearance table
            for (int i = 0; i < Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[GameRources.GetInstance().selectedItemIndex].getResourcesCount(); i++)
            {
                if (i == selectedAppearance)
                    GUI.skin = selectedAreaSkin;
                else
                    GUI.skin = noBackgroundSkin;

                tmpTex = (Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                         GameRources.GetInstance().selectedItemIndex].getResources()[i].getConditions().getBlocksCount() > 0 ? conditionsTex : noConditionsTex);

                GUILayout.BeginHorizontal();

                if (i == selectedAppearance)
                {
                    if (GUILayout.Button(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                     GameRources.GetInstance().selectedItemIndex].getResources()[i].getName(), GUILayout.Width(windowWidth * 0.44f)))
                    {
                        OnAppearanceSelectionChange(i);
                    }
                    if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth * 0.44f)))
                    {
                        ConditionEditorWindow window =
                             (ConditionEditorWindow)ScriptableObject.CreateInstance(typeof(ConditionEditorWindow));
                        window.Init(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                   GameRources.GetInstance().selectedItemIndex].getResources()[i].getConditions());
                    }
                }
                else
                {
                    if (GUILayout.Button(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                   GameRources.GetInstance().selectedItemIndex].getResources()[i].getName(), GUILayout.Width(windowWidth * 0.44f)))
                    {
                        OnAppearanceSelectionChange(i);
                    }
                    if (GUILayout.Button(tmpTex, GUILayout.Width(windowWidth * 0.44f)))
                    {
                        OnAppearanceSelectionChange(i);
                    }
                }
                GUILayout.EndHorizontal();
                GUI.skin = defaultSkin;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            /*
            * Right panel
            */
            GUILayout.BeginArea(rightPanelRect);
            GUI.skin = noBackgroundSkin;
            if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].addElement(Controller.RESOURCES, "");
            }
            if (GUILayout.Button(duplicateTex, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                  GameRources.GetInstance().selectedItemIndex].duplicateElement(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].getResources()[selectedAppearance]);
            }
            if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f * windowWidth)))
            {
                Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                  GameRources.GetInstance().selectedItemIndex].deleteElement(Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].getResources()[selectedAppearance], false);
            }
            GUI.skin = defaultSkin;
            GUILayout.EndArea();


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
            if (workingObject is BaseFileOpenDialog.FileType)
            {
                switch ((BaseFileOpenDialog.FileType)workingObject)
                {
                    case BaseFileOpenDialog.FileType.ITEM_IMAGE:
                        imagePath = message;
                        Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex].setPreviewImage(imagePath);
                        break;
                    case BaseFileOpenDialog.FileType.ITEM_ICON:
                        inventoryIconPath = message;
                        Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex].setIconImage(inventoryIconPath);
                        break;
                    case BaseFileOpenDialog.FileType.ITEM_IMAGE_OVER:
                        imageWhenOverPath = message;
                        Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                            GameRources.GetInstance().selectedItemIndex].setMouseOverImage(imageWhenOverPath);
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
            selectedAppearance = i;
            apperanceName =
                apperanceNameLast = Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                    GameRources.GetInstance().selectedItemIndex].getResources()[selectedAppearance].getName();
            RefreshPathInformation();
        }

        private void RefreshPathInformation()
        {
            Controller.Instance.SelectedChapterDataControl.getItemsList().getItems()[
                      GameRources.GetInstance().selectedItemIndex].setSelectedResources(selectedAppearance);

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