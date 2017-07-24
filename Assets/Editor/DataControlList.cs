using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DataControlList {

    private DataControl control;
    private List<DataControl> childs;

    private ButtonList buttonList;
    private GetNestedElements getChilds;

    public delegate List<DataControl> GetNestedElements(DataControl datacontrol);

    public DataControlList(DataControl control, GetNestedElements getChilds)
    {
        this.control = control;
        this.getChilds = getChilds;
        childs = getChilds(control);

        // ----------------
        // List config
        // ----------------
        buttonList = new ButtonList(childs, typeof(DataControl), true, true);
        buttonList.onSelectCallback += OnSelect;
        buttonList.onReorderCallback += OnReorder;

        // ----------------
        // Add button
        // ----------------
        var buttonAdd = new ButtonList.Button();
        var addTex = (Texture2D)Resources.Load("EAdventureData/img/icons/addNode", typeof(Texture2D));
        buttonAdd.content = new GUIContent(addTex);
        // Can add
        buttonAdd.onButtonEnabledCallback = (list) => control.canAddElements() && control.getAddableElements().ToList().Any(e => control.canAddElement(e));
        // Do add
        buttonAdd.onButtonPressedCallback = (rect, list) =>
        {
            var addable = control.getAddableElements().ToList().FindAll(e => control.canAddElement(e));

            if (addable.Count == 1)
            {
                OnAdd(addable[0]);
            }
            else
            {
                var menu = new GenericMenu();
                addable.ForEach(a => menu.AddItem(new GUIContent(TC.get("TreeNode.AddElement"+a)), false, OnAdd, a));
                menu.ShowAsContext();
            }
        };

        buttonList.buttons.Add(buttonAdd);

        // ----------------
        // Remove button
        // ----------------
        var buttonDel = new ButtonList.Button();
        var delTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        buttonDel.content = new GUIContent(delTex);
        // Can remove
        buttonDel.onButtonEnabledCallback = (list) => list.index > 0 && childs[list.index].canBeDeleted();
        // DoRemove
        buttonDel.onButtonPressedCallback = (rect, list) => control.deleteElement(childs[list.index], false);

        buttonList.buttons.Add(buttonDel);

        // ----------------
        // Duplicate button
        // ----------------
        var buttonDup = new ButtonList.Button();
        var dupTex = (Texture2D)Resources.Load("EAdventureData/img/icons/duplicateNode", typeof(Texture2D));
        buttonDup.content = new GUIContent(dupTex);
        // Can duplicate
        buttonDup.onButtonEnabledCallback = (list) => list.index > 0 && childs[list.index].canBeDuplicated();
        // Do Duplicate
        buttonDup.onButtonPressedCallback = (rect, list) => control.duplicateElement(childs[list.index]);
    }

    protected void OnAdd(object type)
    {
        control.addElement((int)type, control.getDefaultId((int)type));
        OnChanged(buttonList.reorderableList);
    }

    protected void OnChanged(ReorderableList list)
    {
        buttonList.list = childs = getChilds(control);
    }

    int previousSelection = -1;
    protected void OnSelect(ReorderableList r)
    {
        if (r.index == previousSelection)
        {
            r.index = -1;
        } 
    }

    protected void OnReorder(ReorderableList r)
    {
        var list = getChilds(control);

        var toPos = r.index;
        var fromPos = list.FindIndex(i => i == r.list[r.index]);

        control.MoveElement(list[fromPos], fromPos, toPos);
    }

}
