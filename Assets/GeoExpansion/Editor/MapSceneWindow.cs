using UnityEngine;
using System.Collections;

using uAdventure.Editor;
using System;
using UnityEditorInternal;

public class MapSceneWindow : ReorderableListEditorWindowExtension {

    public MapSceneWindow(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
    {
    }

    protected override void OnAdd(ReorderableList r)
    {
        throw new NotImplementedException();
    }

    protected override void OnAddOption(ReorderableList r, string option)
    {
        throw new NotImplementedException();
    }

    protected override void OnButton()
    {
        throw new NotImplementedException();
    }

    protected override void OnElementNameChanged(ReorderableList r, int index, string newName)
    {
        throw new NotImplementedException();
    }

    protected override void OnRemove(ReorderableList r)
    {
        throw new NotImplementedException();
    }

    protected override void OnReorder(ReorderableList r)
    {
        throw new NotImplementedException();
    }

    protected override void OnSelect(ReorderableList r)
    {
        throw new NotImplementedException();
    }

    protected override void OnUpdateList(ReorderableList r)
    {
        throw new NotImplementedException();
    }
}
