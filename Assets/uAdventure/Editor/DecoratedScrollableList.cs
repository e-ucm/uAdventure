using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class DecoratedScrollableList : ScrollableList
{

    FieldInfo boxField, footerField, headerField;

    #region Constructors

    public DecoratedScrollableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
    {
        InitFields();
    }

    public DecoratedScrollableList(IList elements, Type elementType) : base(elements, elementType)
    {
        InitFields();
    }

    public DecoratedScrollableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : 
        base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
    {
        InitFields();
    }
    public DecoratedScrollableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : 
        base (elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
    {
        InitFields();
    }

    void InitFields()
    {
        boxField =
            typeof(ReorderableList.Defaults)
               .GetField("boxBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        footerField =
            typeof(ReorderableList.Defaults)
               .GetField("footerBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        headerField =
            typeof(ReorderableList.Defaults)
               .GetField("headerBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    #endregion

    public override void DoList(float height)
    {
        var prevSkin = GUI.skin;
        GUIStyle boxBackground = null, footerBackground = null, headerBackground = null;
        bool changed = false;
        if (ReorderableList.defaultBehaviours != null)
        {
            boxBackground = ReorderableList.defaultBehaviours.boxBackground; 
            footerBackground = ReorderableList.defaultBehaviours.footerBackground;
            headerBackground = ReorderableList.defaultBehaviours.headerBackground;

            GUI.skin = Resources.Load<GUISkin>("Skin/uASkin");
            boxField.SetValue(ReorderableList.defaultBehaviours, (GUIStyle)"uaLstBgn");
            footerField.SetValue(ReorderableList.defaultBehaviours, (GUIStyle)"uaLstFtr");
            headerField.SetValue(ReorderableList.defaultBehaviours, (GUIStyle)"uaLstHdr");

            changed = true;
        }

        GUI.skin = prevSkin;


        base.DoList(height);

        if (changed)
        {
            boxField.SetValue(ReorderableList.defaultBehaviours, boxBackground);
            footerField.SetValue(ReorderableList.defaultBehaviours, footerBackground);
            headerField.SetValue(ReorderableList.defaultBehaviours, headerBackground);
        }
    }

}
