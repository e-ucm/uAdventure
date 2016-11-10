using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class BooksWindowContents : LayoutWindow, DialogReceiverInterface
{
    private static float windowWidth, windowHeight;

    private Texture2D addTex = null;
    private Texture2D moveUpTex, moveDownTex = null;
    private Texture2D clearTex = null;

    private Texture2D titleParagraphTex = null;
    private Texture2D textParagraphTex = null;
    private Texture2D bulletParagraphTex = null;
    private Texture2D imageParagraphTex = null;

    // Variables for storing paragraph type information
    private Texture2D tmpTexture = null;
    private string tmpParagraphTypeName = "";

    private static Vector2 scrollPosition;

    private static GUISkin selectedElementSkin;
    private static GUISkin defaultSkin;
    private static GUISkin noBackgroundSkin;

    private static Rect tableRect;
    private static Rect previewRect;
    private Rect rightPanelRect;

    private int selectedElement;

    private string editableFieldContent = "";

    private AddParagraphMenu addMenu;

    private int tmpType;

    public BooksWindowContents(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
        : base(aStartPos, aContent, aStyle, aOptions)
    {
        windowWidth = aStartPos.width;
        windowHeight = aStartPos.height;

        clearTex = (Texture2D) Resources.Load("EAdventureData/img/icons/deleteContent", typeof (Texture2D));
        addTex = (Texture2D) Resources.Load("EAdventureData/img/icons/addNode", typeof (Texture2D));
        moveUpTex = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeUp", typeof (Texture2D));
        moveDownTex = (Texture2D) Resources.Load("EAdventureData/img/icons/moveNodeDown", typeof (Texture2D));

        titleParagraphTex =
            (Texture2D) Resources.Load("EAdventureData/img/icons/titleBookParagraph", typeof (Texture2D));
        textParagraphTex =
            (Texture2D) Resources.Load("EAdventureData/img/icons/bulletBookParagraph", typeof (Texture2D));
        bulletParagraphTex =
            (Texture2D) Resources.Load("EAdventureData/img/icons/bulletBookParagraph", typeof (Texture2D));
        imageParagraphTex =
            (Texture2D) Resources.Load("EAdventureData/img/icons/imageBookParagraph", typeof (Texture2D));

        selectedElementSkin = (GUISkin) Resources.Load("Editor/EditorLeftMenuItemSkinConcreteOptions", typeof (GUISkin));
        noBackgroundSkin = (GUISkin) Resources.Load("Editor/EditorNoBackgroundSkin", typeof (GUISkin));

        tableRect = new Rect(0f, 0.1f*windowHeight, 0.9f*windowWidth, windowHeight*0.33f);
        rightPanelRect = new Rect(0.9f*windowWidth, 0.1f*windowHeight, 0.08f*windowWidth, 0.33f*windowHeight);
        previewRect = new Rect(0f, 0.5f*windowHeight, windowWidth, windowHeight*0.45f);

        selectedElement = -1;

        addMenu = new AddParagraphMenu();
    }

    public override void Draw(int aID)
    {
        GUILayout.BeginArea(tableRect);
        GUILayout.BeginHorizontal();
        GUILayout.Box(TC.get("BookParagraphsList.ParagraphType"), GUILayout.Width(windowWidth*0.19f));
        GUILayout.Box(TC.get("BookParagraphsList.Content"), GUILayout.Width(windowWidth*0.69f));
        GUILayout.EndHorizontal();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0;
            i <
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs().Count;
            i++)
        {
            GUI.skin = noBackgroundSkin;
            if (i == selectedElement)
                GUI.skin = selectedElementSkin;

            GUILayout.BeginHorizontal();
            tmpType = Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                    i].getType();
            switch (tmpType)
            {
                case Controller.BOOK_TITLE_PARAGRAPH:
                    tmpTexture = titleParagraphTex;
                    tmpParagraphTypeName = TC.get("Element.Name14");
                    break;
                case Controller.BOOK_BULLET_PARAGRAPH:
                    tmpTexture = bulletParagraphTex;
                    tmpParagraphTypeName = TC.get("Element.Name16");
                    break;
                case Controller.BOOK_TEXT_PARAGRAPH:
                    tmpTexture = textParagraphTex;
                    tmpParagraphTypeName = TC.get("Element.Name15");
                    break;
                case Controller.BOOK_IMAGE_PARAGRAPH:
                    tmpTexture = imageParagraphTex;
                    tmpParagraphTypeName = TC.get("Element.Name1");
                    break;
            }

            if (GUILayout.Button(new GUIContent(tmpParagraphTypeName, tmpTexture),
                GUILayout.Width(windowWidth*0.19f), GUILayout.MaxHeight(0.05f*windowHeight)))
            {
                selectedElement = i;
            }

            if (selectedElement != i)
            {

                if (
                    GUILayout.Button(
                        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()
                            [
                                i].getParagraphContent(),
                        GUILayout.Width(windowWidth*0.69f)))
                {
                    selectedElement = i;
                }
            }
            else
            {
                if (tmpType == Controller.BOOK_IMAGE_PARAGRAPH)
                {
                    if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.09f*windowWidth)))
                    {
                    }
                    if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.MaxWidth(0.20f*windowWidth)))
                    {
                        ImageFileOpenDialog imageDialog =
                            (ImageFileOpenDialog) ScriptableObject.CreateInstance(typeof (ImageFileOpenDialog));
                        imageDialog.Init(this, BaseFileOpenDialog.FileType.BOOK_IMAGE_PARAGRAPH);
                    }
                    GUILayout.Box(Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                            i].getParagraphContent(), GUILayout.MaxWidth(0.4f*windowWidth));
                }
                else
                {
                    editableFieldContent =
                        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                                i].getParagraphContent();
                    editableFieldContent = GUILayout.TextField(editableFieldContent, GUILayout.Width(0.69f*windowWidth));
                    Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                            i].setParagraphTextContent(editableFieldContent);
                }

            }

            GUILayout.EndHorizontal();
            GUI.skin = defaultSkin;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        /*
        * Right panel
        */
        GUILayout.BeginArea(rightPanelRect);
        GUI.skin = noBackgroundSkin;
        if (GUILayout.Button(addTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            addMenu.menu.ShowAsContext();
        }
        if (GUILayout.Button(moveUpTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().moveElementUp(
                    Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                            selectedElement]);
        }
        if (GUILayout.Button(moveDownTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().moveElementDown(
                    Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                            selectedElement]);
        }
        if (GUILayout.Button(clearTex, GUILayout.MaxWidth(0.08f*windowWidth)))
        {
            Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().deleteElement(
                    Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                        GameRources.GetInstance().selectedBookIndex].getBookParagraphsList().getBookParagraphs()[
                            selectedElement], false);
        }
        GUI.skin = defaultSkin;
        GUILayout.EndArea();
    }

    public void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        if (workingObject is BaseFileOpenDialog.FileType)
        {
            switch ((BaseFileOpenDialog.FileType) workingObject)
            {
                case BaseFileOpenDialog.FileType.BOOK_IMAGE_PARAGRAPH:
                    Controller.getInstance()
                        .addTool(
                            new ChangeParagraphContentTool(
                                Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
                                    GameRources.GetInstance().selectedBookIndex].getBookParagraphsList()
                                    .getBookParagraphsList()[selectedElement], message));
                    break;
            }
        }
    }

    public void OnDialogCanceled(object workingObject = null)
    {
    }
}


#region Add paragraphs options
class AddParagraphMenu : WindowMenuContainer
{
    private AddTitleParagraph titleParagraph;
    private AddBulletParagraph bulletParagraph;
    private AddTextParagraph textParagraph;
    private AddImageParagraph imageParagraph;

    public AddParagraphMenu()
    {
        SetMenuItems();
    }

    protected override void Callback(object obj)
    {
        if ((obj as AddTitleParagraph) != null)
            titleParagraph.OnCliked();
        else if ((obj as AddBulletParagraph) != null)
            bulletParagraph.OnCliked();
        else if ((obj as AddTextParagraph) != null)
            textParagraph.OnCliked();
        else if ((obj as AddImageParagraph) != null)
            imageParagraph.OnCliked();
    }

    protected override void SetMenuItems()
    {
        menu = new GenericMenu();

        titleParagraph = new AddTitleParagraph(TC.get("TreeNode.AddElement14"));
        bulletParagraph = new AddBulletParagraph(TC.get("TreeNode.AddElement16"));
        textParagraph = new AddTextParagraph(TC.get("TreeNode.AddElement15"));
        imageParagraph = new AddImageParagraph(TC.get("TreeNode.AddElement17"));

        menu.AddItem(new GUIContent(titleParagraph.Label), false, Callback, titleParagraph);
        menu.AddItem(new GUIContent(bulletParagraph.Label), false, Callback, bulletParagraph);
        menu.AddItem(new GUIContent(textParagraph.Label), false, Callback, textParagraph);
        menu.AddItem(new GUIContent(imageParagraph.Label), false, Callback, imageParagraph);
    }
}

public class AddTitleParagraph : IMenuItem
{
    public AddTitleParagraph(string name_)
    {
        this.Label = name_;
    }

    public string Label
    {
        get; set;
    }

    public void OnCliked()
    {
        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList()
            .addElement(Controller.BOOK_TITLE_PARAGRAPH, String.Empty);
    }
}

public class AddBulletParagraph : IMenuItem
{
    public AddBulletParagraph(string name_)
    {
        this.Label = name_;
    }

    public string Label
    {
        get; set;
    }

    public void OnCliked()
    {
        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList()
            .addElement(Controller.BOOK_BULLET_PARAGRAPH, String.Empty);
    }
}

public class AddTextParagraph : IMenuItem
{
    public AddTextParagraph(string name_)
    {
        this.Label = name_;
    }

    public string Label
    {
        get; set;
    }

    public void OnCliked()
    {
        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList()
            .addElement(Controller.BOOK_TEXT_PARAGRAPH, String.Empty);
    }
}

public class AddImageParagraph : IMenuItem
{
    public AddImageParagraph(string name_)
    {
        this.Label = name_;
    }

    public string Label { get; set; }

    public void OnCliked()
    {
        Controller.getInstance().getSelectedChapterDataControl().getBooksList().getBooks()[
            GameRources.GetInstance().selectedBookIndex].getBookParagraphsList()
            .addElement(Controller.BOOK_IMAGE_PARAGRAPH, String.Empty);
    }
}

#endregion