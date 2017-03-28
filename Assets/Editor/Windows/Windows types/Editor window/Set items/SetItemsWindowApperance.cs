using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{

    public class SetItemsWindowApperance : LayoutWindow, DialogReceiverInterface
    {
        private Texture2D clearImg = null;
        private Texture2D atrezzoImg = null;
        private static Rect previewRect;
        
        private string pathToImg = "";

        public SetItemsWindowApperance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            
            if (GameRources.GetInstance().selectedSetItemIndex >= 0)
            {
                pathToImg =
                    Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList()[
                        GameRources.GetInstance().selectedSetItemIndex].getPreviewImage();
            }


            if (pathToImg != null && !pathToImg.Equals(""))
                atrezzoImg = AssetsController.getImage(pathToImg).texture;
        }


        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;
            previewRect = new Rect(0f, 0.5f * windowHeight, windowWidth, windowHeight * 0.45f);

            // Background chooser
            GUILayout.Label(TC.get("Resources.DescriptionItemImage"));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearImg, GUILayout.Width(0.1f * windowWidth)))
            {
                OnImageChanged("");
            }
            GUILayout.Box(pathToImg, GUILayout.Width(0.6f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(0.1f * windowWidth)))
            {
                ImageFileOpenDialog imgDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imgDialog.Init(this, BaseFileOpenDialog.FileType.SET_ITEM_IMAGE);
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label(TC.get("ImageAssets.Preview"));

			if (pathToImg != "" && atrezzoImg)
            {
                GUI.DrawTexture(previewRect, atrezzoImg, ScaleMode.ScaleToFit);
            }

        }

        void OnImageChanged(string val)
        {
            Debug.Log("PATH: " + val + "\n " + Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList()[
                GameRources.GetInstance().selectedSetItemIndex].getPreviewImage());
            pathToImg = val;
            Controller.getInstance().getSelectedChapterDataControl().getAtrezzoList().getAtrezzoList()[
                GameRources.GetInstance().selectedSetItemIndex].setImage(val);
            if (pathToImg != null && !pathToImg.Equals(""))
                atrezzoImg = AssetsController.getImage(pathToImg).texture;
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            if (workingObject is BaseFileOpenDialog.FileType)
            {
                switch ((BaseFileOpenDialog.FileType)workingObject)
                {
                    case BaseFileOpenDialog.FileType.SET_ITEM_IMAGE:
                        OnImageChanged(message);
                        break;
                }
            }
        }

        public void OnDialogCanceled(object workingObject = null)
        {
            Debug.Log("Wiadomość nie OK");
        }
    }
}