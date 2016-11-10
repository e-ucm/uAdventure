using UnityEngine;
using UnityEditor;

public class EditMenu : WindowMenuContainer
{
    private UndoMenuItem undo;
    private RedoMenuItem redo;
    private SearchMenuItem search;

    public EditMenu()
    {
        SetMenuItems();
    }

    protected override void Callback(object obj)
    {
        if ((obj as UndoMenuItem) != null)
            undo.OnCliked();
        else if ((obj as RedoMenuItem) != null)
            redo.OnCliked();
        else if ((obj as SearchMenuItem) != null)
            search.OnCliked();
    }

    protected override void SetMenuItems()
    {
        menu = new GenericMenu();

        undo = new UndoMenuItem("UNDO");
        redo = new RedoMenuItem("REDO");
        search = new SearchMenuItem("SEARCH");

        menu.AddItem(new GUIContent(Language.GetText(undo.Label)), false, Callback, undo);
        menu.AddItem(new GUIContent(Language.GetText(redo.Label)), false, Callback, redo);
        menu.AddItem(new GUIContent(Language.GetText(search.Label)), false, Callback, search);
    }
}
