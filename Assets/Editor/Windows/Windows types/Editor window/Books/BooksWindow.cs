using UnityEngine;
using System.Collections;

public class BooksWindow : LayoutWindow
{
    private enum BookWindowType { Appearance, Content, Documentation}

    private static BookWindowType openedWindow = BookWindowType.Appearance;
    private static BooksWindowAppearance booksWindowAppearance;
    private static BooksWindowContents booksWindowContents;
    private static BooksWindowDocumentation booksWindowDocumentation;

    private static float windowWidth, windowHeight;

    private static Rect thisRect;

    private static GUISkin selectedButtonSkin;
    private static GUISkin defaultSkin;


    // Flag determining visibility of concrete item information
    private bool isConcreteItemVisible = false;

    public BooksWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        booksWindowAppearance = new BooksWindowAppearance(aStartPos, new GUIContent(TC.get("Book.App")), "Window");
        booksWindowContents = new BooksWindowContents(aStartPos, new GUIContent(TC.get("Book.Contents")), "Window");
        booksWindowDocumentation = new BooksWindowDocumentation(aStartPos, new GUIContent(TC.get("Book.Documentation")), "Window");

        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        thisRect = aStartPos;
        selectedButtonSkin = (GUISkin)Resources.Load("Editor/ButtonSelected", typeof(GUISkin));
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
                    booksWindowAppearance.Draw(aID);
                    break;
                case BookWindowType.Documentation:
                    booksWindowDocumentation.Draw(aID);
                    break;
                case BookWindowType.Content:
                    booksWindowContents.Draw(aID);
                    break;
            }
        }
        else
        {
            GUILayout.Space(30);
            for (int i = 0; i < Controller.getInstance().getCharapterList().getSelectedChapterData().getBooks().Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Box(Controller.getInstance().getCharapterList().getSelectedChapterData().getBooks()[i].getId(), GUILayout.Width(windowWidth * 0.75f));
                if (GUILayout.Button(TC.get("GeneralText.Edit"), GUILayout.MaxWidth(windowWidth * 0.2f)))
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

        // Reload windows for newly selected book
        booksWindowAppearance = new BooksWindowAppearance(thisRect, new GUIContent(TC.get("Book.App")), "Window");
        booksWindowContents = new BooksWindowContents(thisRect, new GUIContent(TC.get("Book.Contents")), "Window");
        booksWindowDocumentation = new BooksWindowDocumentation(thisRect, new GUIContent(TC.get("Book.Documentation")), "Window");
    }
}