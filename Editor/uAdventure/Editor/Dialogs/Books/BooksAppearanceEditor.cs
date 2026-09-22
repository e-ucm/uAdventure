using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class BooksAppearanceEditor : BaseAreaEditablePopup
    {
        private BookDataControl bookRef;

        private Texture2D backgroundPreviewTex = null;
        private Texture2D leftArrowTex = null;
        private Texture2D rightArrowTex = null;

        private Rect imageBackgroundRect, leftArrowRect, rightArrowRect;
        private Vector2 scrollPosition;

        private Vector2 defaultPreviousPageArrowPosition, defaultNextPageArrowPosition;

        private float x_prev, y_prev, x_next, y_next;

        private const float MARGIN = 20.0f;
        private bool dragging;
        private Vector2 startPos;
        private Vector2 currentPos;
        private ArrowType arrowDragged = ArrowType.None;

        private enum ArrowType
        {
            None,
            Left,
            Right
        };


        public void Init(DialogReceiverInterface e, BookDataControl book)
        {
            bookRef = book;

            string backgroundPath = book.getPreviewImage();

            backgroundPreviewTex =
                (Texture2D)Resources.Load(backgroundPath.Substring(0, backgroundPath.LastIndexOf(".")), typeof(Texture2D));

            string leftNormalArrowPath = book.getArrowImagePath_WithDefault(BookDataControl.ARROW_LEFT,
                BookDataControl.ARROW_NORMAL);

            leftArrowTex =
                (Texture2D)
                    Resources.Load(leftNormalArrowPath.Substring(0, leftNormalArrowPath.LastIndexOf(".")),
                        typeof(Texture2D));

            string rightNormalArrowPath = book.getArrowImagePath_WithDefault(BookDataControl.ARROW_RIGHT,
                BookDataControl.ARROW_NORMAL);

            rightArrowTex =
                (Texture2D)
                    Resources.Load(rightNormalArrowPath.Substring(0, rightNormalArrowPath.LastIndexOf(".")),
                        typeof(Texture2D));

            imageBackgroundRect = new Rect(0f, 0f, backgroundPreviewTex.width, backgroundPreviewTex.height);

            defaultPreviousPageArrowPosition = new Vector2(MARGIN,
                backgroundPreviewTex.height - leftArrowTex.height - MARGIN);
            defaultNextPageArrowPosition = new Vector2(backgroundPreviewTex.width - rightArrowTex.width - MARGIN,
                backgroundPreviewTex.height - rightArrowTex.height - MARGIN);

            if (bookRef.getPreviousPagePosition() == Vector2.zero && bookRef.getNextPagePosition() == Vector2.zero)
                SetDefaultArrowsPosition();

            CalculateArrowsPosition();

            base.Init(e, backgroundPreviewTex.width, backgroundPreviewTex.height);
        }

        void OnGUI()
        {
            // Dragging events
            if (Event.current.type == EventType.MouseDrag)
            {
                if (leftArrowRect.Contains(Event.current.mousePosition) ||
                    rightArrowRect.Contains(Event.current.mousePosition))
                {
                    if (!dragging)
                    {
                        dragging = true;
                        startPos = currentPos;
                        if (leftArrowRect.Contains(Event.current.mousePosition))
                            arrowDragged = ArrowType.Left;
                        else
                            arrowDragged = ArrowType.Right;
                    }

                }
                currentPos = Event.current.mousePosition;
            }

            if (Event.current.type == EventType.MouseUp)
            {
                dragging = false;
                arrowDragged = ArrowType.None;
            }

            if (dragging)
            {
                OnBeingDragged();
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUI.DrawTexture(imageBackgroundRect, backgroundPreviewTex);

            GUI.DrawTexture(leftArrowRect, leftArrowTex);
            GUI.DrawTexture(rightArrowRect, rightArrowTex);

            GUILayout.EndScrollView();

            // Default arrow positions
            if (GUILayout.Button(TC.get("Behaviour.Normal")))
            {
                OnDefaultClicked();
            }



            GUILayout.BeginHorizontal();
            GUILayout.Box(TC.get("Book.Previous"), GUILayout.Width(0.5f * backgroundPreviewTex.width));
            GUILayout.Box(TC.get("Book.Next"), GUILayout.Width(0.5f * backgroundPreviewTex.width));
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

            GUILayout.Box("X", GUILayout.Width(0.15f * backgroundPreviewTex.width));
            x_prev = EditorGUILayout.IntField((int)bookRef.getPreviousPagePosition().x,
                GUILayout.Width(0.35f * backgroundPreviewTex.width));

            GUILayout.Box("X", GUILayout.Width(0.15f * backgroundPreviewTex.width));
            x_next = EditorGUILayout.IntField((int)bookRef.getNextPagePosition().x,
                GUILayout.Width(0.35f * backgroundPreviewTex.width));

            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();

            GUILayout.Box("Y", GUILayout.Width(0.15f * backgroundPreviewTex.width));
            y_prev = EditorGUILayout.IntField((int)bookRef.getPreviousPagePosition().y,
                GUILayout.Width(0.35f * backgroundPreviewTex.width));
            GUILayout.Box("Y", GUILayout.Width(0.15f * backgroundPreviewTex.width));
            y_next = EditorGUILayout.IntField((int)bookRef.getNextPagePosition().y,
                GUILayout.Width(0.35f * backgroundPreviewTex.width));

            GUILayout.EndHorizontal();

            bookRef.setPreviousPagePosition(new Vector2(x_prev, y_prev));
            bookRef.setNextPagePosition(new Vector2(x_next, y_next));
            CalculateArrowsPosition();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("End"))
            {
                reference.OnDialogOk("Applied");
                this.Close();
            }
            GUILayout.EndHorizontal();
        }

        private void OnBeingDragged()
        {
            float x, y;
            switch (arrowDragged)
            {
                case ArrowType.Left:
                    x = (int)currentPos.x - (int)(0.5f * leftArrowTex.width);
                    y = (int)currentPos.y - (int)(0.5f * leftArrowTex.height);
                    bookRef.setPreviousPagePosition(new Vector2(x, y));
                    break;
                case ArrowType.Right:
                    x = (int)currentPos.x - (int)(0.5f * rightArrowTex.width);
                    y = (int)currentPos.y - (int)(0.5f * rightArrowTex.height);
                    bookRef.setNextPagePosition(new Vector2(x, y));
                    break;
            }
            CalculateArrowsPosition();
            Repaint();
        }

        private void CalculateArrowsPosition()
        {
            leftArrowRect = new Rect(bookRef.getPreviousPagePosition().x, bookRef.getPreviousPagePosition().y,
                leftArrowTex.width, leftArrowTex.height);
            rightArrowRect = new Rect(bookRef.getNextPagePosition().x, bookRef.getNextPagePosition().y, rightArrowTex.width,
                rightArrowTex.height);
        }

        private void SetDefaultArrowsPosition()
        {
            bookRef.setPreviousPagePosition(defaultPreviousPageArrowPosition);
            bookRef.setNextPagePosition(defaultNextPageArrowPosition);

            CalculateArrowsPosition();
        }

        void OnDefaultClicked()
        {
            SetDefaultArrowsPosition();
        }
    }
}