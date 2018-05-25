using UnityEngine;

using uAdventure.Core;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace uAdventure.Editor
{
    //[EditorWindowExtension(80, typeof(Book))]
    public class BooksWindow : ReorderableListEditorWindowExtension
    {
        private enum BookWindowType { Appearance, Content, Documentation }

        private static BookWindowType openedWindow = BookWindowType.Appearance;
        private static BooksWindowAppearance booksWindowAppearance;
        private static BooksWindowContents booksWindowContents;
        private static BooksWindowDocumentation booksWindowDocumentation;

        private static GUISkin selectedButtonSkin;
        private static GUISkin defaultSkin;


        // Flag determining visibility of concrete item information
        private bool isConcreteItemVisible = false;

        public BooksWindow(Rect aStartPos, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, new GUIContent(TC.get("Element.Name11")), aStyle, aOptions)
        {
            var c = new GUIContent();
            c = new GUIContent();
            c.image = (Texture2D)Resources.Load("EAdventureData/img/icons/books", typeof(Texture2D)); ;
            c.text = TC.get("Element.Name11");

            ButtonContent = c;

            booksWindowAppearance = new BooksWindowAppearance(aStartPos, new GUIContent(TC.get("Book.App")), "Window");
            booksWindowContents = new BooksWindowContents(aStartPos, new GUIContent(TC.get("Book.Contents")), "Window");
            booksWindowDocumentation = new BooksWindowDocumentation(aStartPos, new GUIContent(TC.get("Book.Documentation")), "Window");

            VoidMethodDelegate requestRepaint = () => OnRequestRepaint();
            booksWindowAppearance.OnRequestRepaint += requestRepaint;
            booksWindowContents.OnRequestRepaint += requestRepaint;
            booksWindowDocumentation.OnRequestRepaint += requestRepaint;

            selectedButtonSkin = Resources.Load<GUISkin>("EAdventureData/skin/ButtonSelected");
        }


        public override void Draw(int aID)
        {
            // Show information of concrete item
            if (isConcreteItemVisible)
            {
                /**
                UPPER MENU
                */
                GUILayout.BeginHorizontal();
                if (openedWindow == BookWindowType.Appearance)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Book.App")))
                {
                    OnWindowTypeChanged(BookWindowType.Appearance);
                }
                if (openedWindow == BookWindowType.Appearance)
                    GUI.skin = defaultSkin;

                if (openedWindow == BookWindowType.Documentation)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Book.Documentation")))
                {
                    OnWindowTypeChanged(BookWindowType.Documentation);
                }
                if (openedWindow == BookWindowType.Documentation)
                    GUI.skin = defaultSkin;

                if (openedWindow == BookWindowType.Content)
                    GUI.skin = selectedButtonSkin;
                if (GUILayout.Button(TC.get("Book.Contents")))
                {
                    OnWindowTypeChanged(BookWindowType.Content);
                }
                if (openedWindow == BookWindowType.Content)
                    GUI.skin = defaultSkin;

                GUILayout.EndHorizontal();

                switch (openedWindow)
                {
                    case BookWindowType.Appearance:
                        booksWindowAppearance.Rect = Rect;
                        booksWindowAppearance.Draw(aID);
                        break;
                    case BookWindowType.Documentation:
                        booksWindowDocumentation.Rect = Rect;
                        booksWindowDocumentation.Draw(aID);
                        break;
                    case BookWindowType.Content:
                        booksWindowContents.Rect = Rect;
                        booksWindowContents.Draw(aID);
                        break;
                }
            }
            else
            {
                GUILayout.Space(30);
                for (int i = 0; i < Controller.Instance.ChapterList.getSelectedChapterData().getBooks().Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Box(Controller.Instance.ChapterList.getSelectedChapterData().getBooks()[i].getId(), GUILayout.Width(m_Rect.width * 0.75f));
                    if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(m_Rect.width * 0.2f)))
                    {
                        ShowItemWindowView(i);
                    }

                    GUILayout.EndHorizontal();

                }
            }
        }

        void OnWindowTypeChanged(BookWindowType type_)
        {
            openedWindow = type_;
        }

        // Two methods responsible for showing right window content 
        // - concrete item info or base window view
        public void ShowBaseWindowView()
        {
            isConcreteItemVisible = false;
            GameRources.GetInstance().selectedBookIndex = -1;
        }

        public void ShowItemWindowView(int o)
        {
            isConcreteItemVisible = true;
            GameRources.GetInstance().selectedBookIndex = o;
        }

        protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
        {
			Controller.Instance.ChapterList.getSelectedChapterDataControl().getBooksList().getBooks ()[index].renameElement(newName);
        }

        protected override void OnAdd(ReorderableList r)
        {
            if (r.index != -1 && r.index < r.list.Count)
            {
                Controller.Instance                           .ChapterList                           .getSelectedChapterDataControl()
                           .getBooksList()
                           .duplicateElement(
                               Controller.Instance                                   .ChapterList                                   .getSelectedChapterDataControl()
                                   .getBooksList()
                                   .getBooks()[r.index]);
            }
            else
            {
                Controller.Instance.SelectedChapterDataControl.getBooksList().addElement(Controller.BOOK, "newBook");
            }

        }

        protected override void OnAddOption(ReorderableList r, string option)
        {
            // No options
        }

        protected override void OnRemove(ReorderableList r)
        {
            if (r.index != -1)
            {
                Controller.Instance                              .ChapterList                              .getSelectedChapterDataControl()
                              .getBooksList()
                              .deleteElement(
                                  Controller.Instance                                      .ChapterList                                      .getSelectedChapterDataControl()
                                      .getBooksList()
                                      .getBooks()[r.index], false);

                ShowBaseWindowView();
            }
        }

        protected override void OnSelect(ReorderableList r)
        {
            ShowItemWindowView(r.index);
        }

        protected override void OnReorder(ReorderableList r)
        {
			var dataControlList = Controller.Instance 				.ChapterList .getSelectedChapterDataControl ().getBooksList ();

			var toPos = r.index;
			var fromPos = dataControlList.getBooks ().FindIndex (i => i.getId () == r.list [r.index] as string);

			dataControlList.MoveElement (dataControlList.getBooks ()[fromPos], fromPos, toPos);
        }

        protected override void OnButton()
        {
            ShowBaseWindowView();
            reorderableList.index = -1;
        }

        protected override void OnUpdateList(ReorderableList r)
        {
			Elements = Controller.Instance.ChapterList.getSelectedChapterDataControl().getBooksList ().getBooks().ConvertAll(s => s.getId());
        }
    }
}