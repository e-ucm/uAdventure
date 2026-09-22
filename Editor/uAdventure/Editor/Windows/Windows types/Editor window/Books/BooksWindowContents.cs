using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;
using System.Linq;
using uAdventure.Runner;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    public class BooksWindowContents : AbstractEditorComponentWithPreview, IUnclippedDrawReceiver
    {
        private readonly Dictionary<int, Texture2D> elementIcons;
        private readonly Dictionary<int, string> elementTitles;

        private readonly DataControlList bookContentsList;

        private readonly FileChooser imageChooser;
        private readonly BookDrawer bookDrawer;

        private Rect previewRect;

        public BooksWindowContents(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            PreviewTitle = "Book.Preview".Traslate();
            elementTitles = new Dictionary<int, string>
            {
                { Controller.BOOK_TITLE_PARAGRAPH,  "Element.Name14" },
                { Controller.BOOK_TEXT_PARAGRAPH,   "Element.Name15" },
                { Controller.BOOK_BULLET_PARAGRAPH, "Element.Name16" },
                { Controller.BOOK_IMAGE_PARAGRAPH,  "Element.Name17" }
            };

            elementIcons = new Dictionary<int, Texture2D>
            {
                { Controller.BOOK_TITLE_PARAGRAPH,  Resources.Load<Texture2D>("EAdventureData/img/icons/titleBookParagraph") },
                { Controller.BOOK_TEXT_PARAGRAPH,   Resources.Load<Texture2D>("EAdventureData/img/icons/textBookParagraph") },
                { Controller.BOOK_BULLET_PARAGRAPH, Resources.Load<Texture2D>("EAdventureData/img/icons/bulletBookParagraph") },
                { Controller.BOOK_IMAGE_PARAGRAPH,  Resources.Load<Texture2D>("EAdventureData/img/icons/imageBookParagraph") }
            };

            imageChooser = new FileChooser
            {
                Label = "",
                FileType = FileType.BOOK_IMAGE_PARAGRAPH
            };

            bookContentsList = new DataControlList
            {
                Columns =
                {
                    new ColumnList.Column
                    {
                        Text = "BookParagraphsList.ParagraphType",
                        SizeOptions = new GUILayoutOption[] { GUILayout.Width(150) }
                    },
                    new ColumnList.Column
                    {
                        Text = "BookParagraphsList.Content",
                        SizeOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) }
                    }
                },
                drawCell = (rect, row, column, isActive, isFocused) =>
                {
                    var content = bookContentsList.list[row] as BookParagraphDataControl;
                    switch (column)
                    {
                        default:
                            Debug.LogError("Column number out of bounds");
                            break;
                        case 0:
                            {
                                var height = rect.height;
                                var icon = elementIcons[content.getType()];
                                var text = elementTitles[content.getType()].Traslate();
                                var imageRect = new Rect(1, 1, icon.width, icon.height).GUIAdapt(rect);
                                var titleRect = new Rect(height, 0, rect.width - height, height).GUIAdapt(rect);

                                GUI.DrawTexture(imageRect, icon, ScaleMode.ScaleAndCrop);
                                GUI.Label(titleRect, text);
                            }
                            break;
                        case 1:
                            {
                                if (isActive)
                                {
                                    var isImage = content.getType() == Controller.BOOK_IMAGE_PARAGRAPH;
                                    if (isImage)
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        imageChooser.Path = content.getParagraphContent();
                                        imageChooser.Do(rect);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            content.setParagraphImagePath(imageChooser.Path);
                                        }
                                    }
                                    else
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var newContent = GUI.TextArea(rect, content.getParagraphContent());
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            content.setParagraphTextContent(newContent);
                                        }
                                    }
                                }
                                else
                                {
                                    GUI.Label(rect, content.getParagraphContent());
                                }
                            }
                            break;
                    }
                }
            };

            bookDrawer = new BookDrawer(Controller.ResourceManager);
        }

        protected override void DrawInspector()
        {
            bookContentsList.DoList(240);
        }

        protected override bool HasToDrawPreviewInspector() { return false; }

        public override void DrawPreview(Rect rect)
        {
            previewRect = rect;

            previewRect.position = GUIUtility.GUIToScreenPoint(rect.position);
        }
        
        protected override void OnTargetChanged()
        {
            if (Target != null || GameRources.GetInstance().selectedBookIndex >= 0)
            {
                var workingBook = Target as BookDataControl ?? Controller.Instance.SelectedChapterDataControl
                    .getBooksList().getBooks()[GameRources.GetInstance().selectedBookIndex];

                bookDrawer.Book = workingBook.getContent() as Book;
                bookContentsList.SetData(workingBook.getBookParagraphsList(),
                    (content) => content != null ? (content as BookParagraphsListDataControl).getBookParagraphs().Cast<DataControl>().ToList() : new List<DataControl>());
            }
            else
            {
                bookDrawer.Book = null;
                bookContentsList.SetData(null, _ =>  new List<DataControl>());
            }
        }

        public void UnclippedDraw(Rect rect)
        {
            previewRect.position = previewRect.position - GUIUtility.GUIToScreenPoint(Vector2.zero);
            bookDrawer.Draw(previewRect);
        }
    }
}