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

        private int SelectedArrow = -1;

        private Vector2 offset;

        public override void DrawPreview(Rect rect)
        {
            // We first fix the ratio of the rect
            var viewport = rect.AdjustToRatio(800f / 600f);
            GUI.DrawTexture(viewport, backgroundPreview, ScaleMode.ScaleToFit);

            Rect leftArrowRect = Rect.zero, rightArrowRect = Rect.zero;

            // Draw the left arrow
            if (leftNormalArrow)
            {
                leftArrowRect = new Rect(workingBook.getPreviousPagePosition(), new Vector2(leftNormalArrow.width, leftNormalArrow.height)).AdjustToViewport(800f, 600f, viewport);
                GUI.DrawTexture(leftArrowRect, leftNormalArrow, ScaleMode.ScaleToFit);
            }

            // Draw the right arrow
            if (rightNormalArrow)
            {
                rightArrowRect = new Rect(workingBook.getNextPagePosition(), new Vector2(rightNormalArrow.width, rightNormalArrow.height)).AdjustToViewport(800f, 600f, viewport);
                GUI.DrawTexture(rightArrowRect, rightNormalArrow, ScaleMode.ScaleToFit);
            }

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (leftArrowRect.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl = leftNormalArrow.GetInstanceID();
                        SelectedArrow = BookDataControl.ARROW_LEFT;

                    }else if (rightArrowRect.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl = rightNormalArrow.GetInstanceID();
                        SelectedArrow = BookDataControl.ARROW_RIGHT;
                    }
                    else
                    {
                        SelectedArrow = -1;
                    }
                    break;

                case EventType.mouseUp:
                    if (GUIUtility.hotControl == leftNormalArrow.GetInstanceID())
                    {
                        GUIUtility.hotControl = 0;
                    }
                    if (rightNormalArrow && GUIUtility.hotControl == rightNormalArrow.GetInstanceID())
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;

                case EventType.mouseDrag:
                    if (GUIUtility.hotControl == leftNormalArrow.GetInstanceID())
                    {
                        leftArrowRect.position += Event.current.delta;
                        workingBook.setPreviousPagePosition(rect.ViewportToScreen(800f, 600f, SceneEditor.Current.Viewport).position);
                    }
                    if (rightNormalArrow && GUIUtility.hotControl == rightNormalArrow.GetInstanceID())
                    {
                        leftArrowRect.position += Event.current.delta;
                    }
                    break;
            }


        }

        protected override bool HasToDrawPreviewInspector()
        {
            return SelectedArrow != -1;
        }

        protected override void DrawPreviewInspector()
        {
            switch (SelectedArrow)
            {
                case BookDataControl.ARROW_LEFT:
                    {
                        GUILayout.Label("Left arrow properties");
                        EditorGUI.BeginChangeCheck();
                        var newPos = EditorGUILayout.Vector2Field("Position", workingBook.getPreviousPagePosition());
                        if (EditorGUI.EndChangeCheck())
                        {
                            workingBook.setPreviousPagePosition(newPos);
                        }
                    }
                    break;
                case BookDataControl.ARROW_RIGHT:
                    {
                        GUILayout.Label("Right arrow properties");
                        EditorGUI.BeginChangeCheck();
                        var newPos = EditorGUILayout.Vector2Field("Position", workingBook.getNextPagePosition());
                        if (EditorGUI.EndChangeCheck())
                        {
                            workingBook.setNextPagePosition(newPos);
                        }
                    }
                    break;
            }
        }
    }
}