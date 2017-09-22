using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BooksWindowAppearance : PreviewLayoutWindow
    {
        private BookDataControl workingBook;

        private Texture2D clearImg = null;
        
        private static Rect tableRect;
        private static Rect previewRect;
        private static Rect infoPreviewRect;

        private Texture2D backgroundPreview;
        private Texture2D leftNormalArrow, rightNormalArrow, leftOverArrow, rightOverArrow;

        private AppearanceEditor appearanceEditor;

        private FileChooser background, left, left_over, right, right_over;


        public BooksWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;


            background = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionBookBackground"),
                FileType = BaseFileOpenDialog.FileType.SCENE_BACKGROUND
            };

            left = new FileChooser()
            {
                Label = TC.get("Resources.ArrowLeftNormal"),
                FileType = BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_NORMAL
            };

            left_over = new FileChooser()
            {
                Label = TC.get("Resources.ArrowLeftOver"),
                FileType = BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_OVER
            };

            right = new FileChooser()
            {
                Label = TC.get("Resources.ArrowRightNormal"),
                FileType = BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_NORMAL
            };

            right_over = new FileChooser()
            {
                Label = TC.get("Resources.ArrowRightOver"),
                FileType = BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_OVER
            };

        }

        private void DoArrowField(FileChooser arrow, int arrowOrientation, int arrowState)
        {
            EditorGUI.BeginChangeCheck();
            arrow.Path = workingBook.getArrowImagePath(arrowOrientation, arrowState);
            arrow.DoLayout(GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingBook.setArrowImagePath(arrowOrientation, arrowState, arrow.Path);
            }
        }

        private Texture2D LoadArrowTexture(BookDataControl book,int arrowType, int arrowMode)
        {
            var path = book.getArrowImagePath(arrowType, arrowMode);
            return !string.IsNullOrEmpty(path) ? AssetsController.getImage(path).texture : null;
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var book = (BookDataControl)data;
            
            var backgroundPath = book.getPreviewImage();
            backgroundPreview = !string.IsNullOrEmpty(backgroundPath) ? AssetsController.getImage(backgroundPath).texture : null;

            leftNormalArrow = LoadArrowTexture(book,    BookDataControl.ARROW_LEFT, BookDataControl.ARROW_NORMAL);
            rightNormalArrow = LoadArrowTexture(book,   BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_NORMAL);
            leftOverArrow = LoadArrowTexture(book,      BookDataControl.ARROW_LEFT, BookDataControl.ARROW_OVER);
            rightOverArrow = LoadArrowTexture(book,     BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_OVER);
        }

        protected override void DrawInspector()
        {
            workingBook = Controller.Instance.SelectedChapterDataControl.getBooksList().getBooks()[GameRources.GetInstance().selectedBookIndex];

            // Appearance table
            appearanceEditor.Data = workingBook;
            appearanceEditor.OnInspectorGUI();

            GUILayout.Space(10);

            // Background
            EditorGUI.BeginChangeCheck();
            background.Path = workingBook.getPreviewImage();
            background.DoLayout();
            if (EditorGUI.EndChangeCheck())
                workingBook.setPreviewImage(background.Path);

            // Arrows
            GUILayout.BeginHorizontal();
            DoArrowField(left, BookDataControl.ARROW_LEFT, BookDataControl.ARROW_NORMAL);
            DoArrowField(right, BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_NORMAL);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            DoArrowField(left_over, BookDataControl.ARROW_LEFT, BookDataControl.ARROW_OVER);
            DoArrowField(right_over, BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_OVER);
            GUILayout.EndHorizontal();
        }

        protected override void DrawPreview(Rect rect)
        {
            GUI.DrawTexture(rect, backgroundPreview, ScaleMode.ScaleToFit);
        }

        protected override bool HasToDrawPreviewInspector()
        {
            return true;
        }

        protected override void DrawPreviewInspector()
        {
            GUILayout.Label("Hola mundo");
        }
    }
}