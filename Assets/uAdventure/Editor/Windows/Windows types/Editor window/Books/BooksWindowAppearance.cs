using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditor;

namespace uAdventure.Editor
{
    public class BooksWindowAppearance : PreviewLayoutWindow
    {
        private BookDataControl workingBook;

        private Texture2D backgroundPreview;
        private Texture2D leftNormalArrow, rightNormalArrow, leftOverArrow, rightOverArrow;

        private readonly ResourcesList appearanceEditor;
        private readonly FileChooser background, left, left_over, right, right_over;

        private int SelectedArrow = -1;

        public BooksWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            appearanceEditor = ScriptableObject.CreateInstance<ResourcesList>();
            appearanceEditor.Height = 160;
            appearanceEditor.onResourceSelected = RefreshPathInformation;

            PreviewTitle = "BackgroundAssets.Preview".Traslate();

            background = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionBookBackground"),
                FileType = FileType.SCENE_BACKGROUND,
                Empty = SpecialAssetPaths.ASSET_EMPTY_IMAGE
            };

            left = new FileChooser()
            {
                Label = TC.get("Resources.ArrowLeftNormal"),
                FileType = FileType.BOOK_ARROW_LEFT_NORMAL,
                Empty = SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL
            };

            left_over = new FileChooser()
            {
                Label = TC.get("Resources.ArrowLeftOver"),
                FileType = FileType.BOOK_ARROW_LEFT_OVER,
                Empty = SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER
            };

            right = new FileChooser()
            {
                Label = TC.get("Resources.ArrowRightNormal"),
                FileType = FileType.BOOK_ARROW_RIGHT_NORMAL,
                Empty = SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL_RIGHT
            };

            right_over = new FileChooser()
            {
                Label = TC.get("Resources.ArrowRightOver"),
                FileType = FileType.BOOK_ARROW_RIGHT_OVER,
                Empty = SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER_RIGHT
            };

        }

        private void DoArrowField(FileChooser arrow, int arrowOrientation, int arrowState)
        {
            EditorGUI.BeginChangeCheck();
            arrow.Path = workingBook.getArrowImagePath(arrowOrientation, arrowState);
            if (string.IsNullOrEmpty(arrow.Path))
            {
                arrow.Path = arrow.Empty;
                workingBook.setArrowImagePath(arrowOrientation, arrowState, arrow.Path);
                RefreshPathInformation(workingBook);
            }

            arrow.DoLayout(GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                workingBook.setArrowImagePath(arrowOrientation, arrowState, arrow.Path);
                RefreshPathInformation(workingBook);
            }
        }

        private Texture2D LoadArrowTexture(BookDataControl book,int arrowType, int arrowMode)
        {
            var path = book.getArrowImagePath(arrowType, arrowMode);
            return !string.IsNullOrEmpty(path) ? Controller.ResourceManager.getImage(path) : null;
        }

        private void RefreshPathInformation(DataControlWithResources data)
        {
            var book = (BookDataControl)data;
            
            var backgroundPath = book.getPreviewImage();
            backgroundPreview = !string.IsNullOrEmpty(backgroundPath) ? Controller.ResourceManager.getImage(backgroundPath) : null;

            leftNormalArrow  = LoadArrowTexture(book, BookDataControl.ARROW_LEFT, BookDataControl.ARROW_NORMAL);
            rightNormalArrow = LoadArrowTexture(book, BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_NORMAL);
            leftOverArrow    = LoadArrowTexture(book, BookDataControl.ARROW_LEFT, BookDataControl.ARROW_OVER);
            rightOverArrow   = LoadArrowTexture(book, BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_OVER);
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
            {
                workingBook.setPreviewImage(background.Path);
            }

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


        private static Vector2 DoArrow(Vector2 position, Texture2D image, Texture2D imageOver, Rect viewport, bool flip, out bool selected, int controlId)
        {
            var imageSize = new Vector2(image.width, image.height);
            var buttonRect = new Rect(position, imageSize);
            buttonRect = buttonRect.AdjustToViewport(800, 600, viewport);
            var mouseRect = buttonRect;
            if (flip)
            {
                buttonRect.x += buttonRect.width;
                buttonRect.width = -buttonRect.width;
            }

            var isOver = mouseRect.Contains(Event.current.mousePosition);
            GUI.DrawTexture(buttonRect, isOver ? imageOver : image);

            selected = false;
            switch (Event.current.type)
            {
                case EventType.MouseDown:

                    if (isOver && Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        GUIUtility.hotControl = controlId;
                        selected = true;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (image && GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;

                case EventType.MouseDrag:
                    if (image && GUIUtility.hotControl == controlId)
                    {
                        buttonRect.position += Event.current.delta;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                    break;
            }

            if (flip)
            {
                buttonRect.width = -buttonRect.width;
                buttonRect.x -= buttonRect.width;
            }

            buttonRect = buttonRect.ViewportToScreen(800, 600, viewport);
            return buttonRect.position;
        }

        public override void DrawPreview(Rect rect)
        {
            // We first fix the ratio of the rect
            var viewport = rect.AdjustToRatio(800f / 600f);
            GUI.DrawTexture(viewport, backgroundPreview, ScaleMode.ScaleToFit);
            
            bool selected;
            if(Event.current.type == EventType.MouseDown)
            {
                SelectedArrow = -1;
            }

            // Draw the left arrow
            if (leftNormalArrow)
            {
                EditorGUI.BeginChangeCheck();
                var leftArrowPos = DoArrow(workingBook.getPreviousPagePosition(), leftNormalArrow, leftOverArrow, viewport, false, out selected, "Arrow left".GetHashCode());
                if (EditorGUI.EndChangeCheck())
                {
                    workingBook.setPreviousPagePosition(leftArrowPos);
                }
                if (selected)
                {
                    SelectedArrow = BookDataControl.ARROW_LEFT;
                }
            }

            if (rightNormalArrow)
            {
                EditorGUI.BeginChangeCheck();
                var isDefault = workingBook.getArrowImagePath(BookDataControl.ARROW_RIGHT, BookDataControl.ARROW_NORMAL) == SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL_RIGHT;
                var rightArrowPos = DoArrow(workingBook.getNextPagePosition(), rightNormalArrow, rightOverArrow, viewport, isDefault, out selected, "Arrow right".GetHashCode());
                if (EditorGUI.EndChangeCheck())
                {
                    workingBook.setNextPagePosition(rightArrowPos);
                }
                if (selected)
                {
                    SelectedArrow = BookDataControl.ARROW_RIGHT;
                }
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