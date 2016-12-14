using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Editor;
using System;
using UnityEditorInternal;

using uAdventure.Geo;

public class MapSceneWindow : ReorderableListEditorWindowExtension {

    private int selectedElement;

    public MapSceneWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
    {
        var bc = new GUIContent();
        bc.image = (Texture2D)Resources.Load("EAdventureData/img/icons/mapscenes", typeof(Texture2D));
        bc.text = "MapScenes";  //TC.get("Element.Name1");
        ButtonContent = bc;
    }

    public override void Draw(int aID)
    {
        base.Draw(aID);
    }

    protected override void OnAdd(ReorderableList r)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().Add(new MapScene("newMapscene"));
    }

    protected override void OnAddOption(ReorderableList r, string option){}

    protected override void OnButton()
    {
        reorderableList.index = -1;
    }

    protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>()[index].Id = newName;
    }

    protected override void OnRemove(ReorderableList r)
    {
        Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().RemoveAt(r.index);
    }

    protected override void OnReorder(ReorderableList r)
    {
    }

    protected override void OnSelect(ReorderableList r)
    {
        selectedElement = r.index;
    }

    protected override void OnUpdateList(ReorderableList r)
    {
        r.list = Controller.getInstance().getSelectedChapterDataControl().getObjects<MapScene>().ConvertAll(s => s.getId());
    }
}
