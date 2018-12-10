using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace uAdventure.Editor
{
    [EditorWindowExtension(80, typeof(BookDataControl))]
    public class BooksWindow : PreviewDataControlExtension, IUnclippedDrawReceiver
    {
        private enum BookWindowType { Appearance, Content, Documentation }
        
        private static BooksWindowContents booksWindowContents;

        public BooksWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name11")), aStyle, aOptions)
        {
            ButtonContent = new GUIContent()
            {
                image = Resources.Load<Texture2D>("EAdventureData/img/icons/books"),
                text = "Element.Name11"
            };

            var booksWindowAppearance = new BooksWindowAppearance(aStartPos, new GUIContent(TC.get("Book.App")), "Window");
            booksWindowContents = new BooksWindowContents(aStartPos, new GUIContent(TC.get("Book.Contents")), "Window");
            var booksWindowDocumentation = new BooksWindowDocumentation(aStartPos, new GUIContent(TC.get("Book.Documentation")), "Window");

            AddTab(TC.get("Book.App"), BookWindowType.Appearance, booksWindowAppearance);
            AddTab(TC.get("Book.Contents"), BookWindowType.Content, booksWindowContents);
            AddTab(TC.get("Book.Documentation"), BookWindowType.Documentation, booksWindowDocumentation);
        }

        protected override void OnSelect(ReorderableList r)
        {
            booksWindowContents.Target = Controller.Instance.SelectedChapterDataControl.getBooksList().getBooks()[r.index];
            GameRources.GetInstance().selectedBookIndex = r.index;
        }


        protected override void OnButton()
        {
            base.OnButton();
            dataControlList.SetData(Controller.Instance.SelectedChapterDataControl.getBooksList(),
                booksList => (booksList as BooksListDataControl).getBooks().Cast<DataControl>().ToList());
            GameRources.GetInstance().selectedBookIndex = -1;
        }

        protected override void OnDrawMainPreview(Rect rect, int index)
        {
            booksWindowContents.Target = dataControlList.list[index] as DataControl;
            booksWindowContents.DrawPreview(rect);
            booksWindowContents.Target = null;
        }

        public void UnclippedDraw(Rect rect)
        {
            if((BookWindowType)OpenedWindow == BookWindowType.Content)
            {
                booksWindowContents.UnclippedDraw(rect);
            }
        }
    }
}