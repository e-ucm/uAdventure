using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BooksWindowAppearance : LayoutWindow, DialogReceiverInterface
    {

        private Texture2D clearImg = null;

        private Texture2D backgroundPreview = null;
        private static Rect tableRect;
        private static Rect previewRect;
        private static Rect infoPreviewRect;

        private string backgroundPath = "";
        private string leftNormalArrowPath = "", rightNormalArrowPath = "";
        private string leftOverArrowPath = "", rightOverArrowPath = "";


        public BooksWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            clearImg = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));

            if (GameRources.GetInstance().selectedBookIndex >= 0)
            {
                backgroundPath =
                    Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getPreviewImage();

                leftNormalArrowPath = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].getArrowImagePath(BookDataControl.ARROW_LEFT,
                        BookDataControl.ARROW_NORMAL);

                rightNormalArrowPath = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].getArrowImagePath(BookDataControl.ARROW_RIGHT,
                        BookDataControl.ARROW_NORMAL);

                leftOverArrowPath = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].getArrowImagePath(BookDataControl.ARROW_LEFT,
                        BookDataControl.ARROW_OVER);

                rightOverArrowPath = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].getArrowImagePath(BookDataControl.ARROW_RIGHT,
                        BookDataControl.ARROW_OVER);
            }
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreview = AssetsController.getImage(backgroundPath).texture;

        }

        public override void Draw(int aID)
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;

            tableRect = new Rect(0f, 0.1f * windowHeight, windowWidth, windowHeight * 0.33f);
            infoPreviewRect = new Rect(0f, 0.45f * windowHeight, windowWidth, windowHeight * 0.05f);
            previewRect = new Rect(0f, 0.5f * windowHeight, windowWidth, windowHeight * 0.45f);

            /**
            * TABLE
            */
            GUILayout.BeginArea(tableRect);

            GUILayout.Label(TC.get("Resources.DescriptionBookBackground"));
            GUILayout.BeginHorizontal();
            GUILayout.Box(backgroundPath, GUILayout.MaxWidth(0.85f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.15f * windowWidth)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_IMAGE_PARAGRAPH);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            // NORMAL ARROWS PART
            GUILayout.Label(TC.get("Resources.ArrowLeftNormal"), GUILayout.MaxWidth(0.5f * windowWidth));
            GUILayout.Label(TC.get("Resources.ArrowRightNormal"), GUILayout.MaxWidth(0.5f * windowWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.05f * windowWidth)))
            {
                //TODO: clear
            }
            GUILayout.Box(leftNormalArrowPath, GUILayout.MaxWidth(0.3f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.1f * windowWidth)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_NORMAL);
            }

            GUILayout.Space(0.05f * windowWidth);

            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.05f * windowWidth)))
            {
                //TODO: clear

            }
            GUILayout.Box(rightNormalArrowPath, GUILayout.MaxWidth(0.3f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.1f * windowWidth)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_NORMAL);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // OVER ARROWS PART
            GUILayout.BeginHorizontal();
            GUILayout.Label(TC.get("Resources.ArrowLeftOver"), GUILayout.MaxWidth(0.5f * windowWidth));
            GUILayout.Label(TC.get("Resources.ArrowRightOver"), GUILayout.MaxWidth(0.5f * windowWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.05f * windowWidth)))
            {

                //TODO: clear
            }
            GUILayout.Box(leftOverArrowPath, GUILayout.MaxWidth(0.3f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.1f * windowWidth)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_OVER);
            }

            GUILayout.Space(0.05f * windowWidth);

            if (GUILayout.Button(clearImg, GUILayout.MaxWidth(0.05f * windowWidth)))
            {

                //TODO: clear
            }
            GUILayout.Box(rightOverArrowPath, GUILayout.MaxWidth(0.3f * windowWidth));
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.1f * windowWidth)))
            {
                ImageFileOpenDialog imageDialog =
                    (ImageFileOpenDialog)ScriptableObject.CreateInstance(typeof(ImageFileOpenDialog));
                imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_OVER);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();


            if (backgroundPath != "")
            {

                /**
                * PREVIEW BUTTON
                */
                GUILayout.BeginArea(infoPreviewRect);
                if (GUILayout.Button(TC.get("GeneralText.Edit")))
                {
                    BooksAppearanceEditor window =
                      (BooksAppearanceEditor)ScriptableObject.CreateInstance(typeof(BooksAppearanceEditor));
                    window.Init(this, Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex]);
                }
                GUILayout.EndArea();


                /**
                * PREVIEW TEXTURE
                */
                GUI.DrawTexture(previewRect, backgroundPreview, ScaleMode.ScaleToFit);

            }
            else
            {
                GUILayout.BeginArea(infoPreviewRect);
                GUILayout.Button("No background!");
                GUILayout.EndArea();
            }
        }

        public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {

            if (workingObject is BaseFileOpenDialog.FileType)
            {
                switch ((BaseFileOpenDialog.FileType)workingObject)
                {
                    case BaseFileOpenDialog.FileType.BOOK_IMAGE_PARAGRAPH:
                        OnBackgroundChange(message);
                        break;
                    case BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_NORMAL:
                        OnArrowLeftNormalChange(message);
                        break;
                    case BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_NORMAL:
                        OnArrowRightNormalChange(message);
                        break;
                    case BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_OVER:
                        OnArrowLeftOverChange(message);
                        break;
                    case BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_OVER:
                        OnArrowRightOverChange(message);
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

        private void OnBackgroundChange(string val)
        {
            backgroundPath = val;
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                    GameRources.GetInstance().selectedBookIndex].setPreviewImage(val);
            if (backgroundPath != null && !backgroundPath.Equals(""))
                backgroundPreview =
                    AssetsController.getImage(backgroundPath).texture;
        }
        private void OnArrowLeftNormalChange(string val)
        {
            leftNormalArrowPath = val;
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                   GameRources.GetInstance().selectedBookIndex].setArrowImagePath(BookDataControl.ARROW_LEFT,
                       BookDataControl.ARROW_NORMAL, val);

        }
        private void OnArrowRightNormalChange(string val)
        {
            rightNormalArrowPath = val;
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                   GameRources.GetInstance().selectedBookIndex].setArrowImagePath(BookDataControl.ARROW_RIGHT,
                       BookDataControl.ARROW_NORMAL, val);
        }
        private void OnArrowLeftOverChange(string val)
        {
            leftOverArrowPath = val;
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                   GameRources.GetInstance().selectedBookIndex].setArrowImagePath(BookDataControl.ARROW_LEFT,
                       BookDataControl.ARROW_OVER, val);
        }
        private void OnArrowRightOverChange(string val)
        {
            rightOverArrowPath = val;
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                   GameRources.GetInstance().selectedBookIndex].setArrowImagePath(BookDataControl.ARROW_RIGHT,
                       BookDataControl.ARROW_OVER, val);
        }
    }
}