using UnityEditor;
using UnityEngine;

public class ChaptersMenu : WindowMenuContainer
{
    private AddChapterMenuItem add;
    private DeleteChapterMenuItem delete;
    private ImportChapterMenuItem import;
    private MoveUpChapterMenuItem moveUp;
    private MoveDownChapterMenuItem moveDown;
    private EditFlagsVariablesMenuItem variablesFlags;

    private static ChaptersMenu instance;

    public ChaptersMenu()
    {
        SetMenuItems();
        instance = this;
    }

    public static ChaptersMenu getInstance()
    {
        return instance;
    }

    public void RefreshMenuItems()
    {
        SetMenuItems();
    }


    protected override void Callback(object obj)
    {
        if ((obj as AddChapterMenuItem) != null)
            add.OnCliked();
        else if ((obj as DeleteChapterMenuItem) != null)
            delete.OnCliked();
        else if ((obj as ImportChapterMenuItem) != null)
            import.OnCliked();
        else if ((obj as MoveUpChapterMenuItem) != null)
            moveUp.OnCliked();
        else if ((obj as MoveDownChapterMenuItem) != null)
            moveDown.OnCliked();
        else if ((obj as EditFlagsVariablesMenuItem) != null)
            variablesFlags.OnCliked();
        else if ((obj is int))
        {
            if ((int) obj != Controller.getInstance().getCharapterList().getSelectedChapter())
            {
                Controller.getInstance().getCharapterList().setSelectedChapterInternal((int)obj);
                SetMenuItems();
                EditorWindowBase.RefreshChapter();
            }
        }
    }

    protected override void SetMenuItems()
    {
        menu = new GenericMenu();

        add = new AddChapterMenuItem(TC.get("MenuChapters.AddChapter"));
        delete = new DeleteChapterMenuItem(TC.get("MenuChapters.DeleteChapter"));
        import = new ImportChapterMenuItem(TC.get("MenuChapters.ImportChapter"));
        variablesFlags = new EditFlagsVariablesMenuItem(TC.get("MenuChapters.Flags"));
        moveUp = new MoveUpChapterMenuItem(TC.get("MenuChapters.MoveChapterUp"));
        moveDown = new MoveDownChapterMenuItem(TC.get("MenuChapters.MoveChapterDown"));

        menu.AddItem(new GUIContent(add.Label), false, Callback, add);
        //Delte button is only visible for more than 1 chapter
        if(Controller.getInstance().getCharapterList().getChaptersCount()>1)
             menu.AddItem(new GUIContent(delete.Label), false, Callback, delete);
        menu.AddItem(new GUIContent(import.Label), false, Callback, import);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent(moveUp.Label), false, Callback, moveUp);
        menu.AddItem(new GUIContent(moveDown.Label), false, Callback, moveDown);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent(variablesFlags.Label), false, Callback, variablesFlags);
        for (int i = 0; i < Controller.getInstance().getCharapterList().getChaptersCount(); i++)
        {
            bool selected = (Controller.getInstance().getCharapterList().getSelectedChapter() == i);

            menu.AddItem(new GUIContent(Controller.getInstance().getCharapterList().getChapterTitles()[i]), selected, Callback, i);
        }
    }
}
