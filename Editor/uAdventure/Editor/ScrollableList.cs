using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ScrollableList
{
    public float footerHeight;
    public float headerHeight;
    public bool displayHeader, displayAddButton, displayRemoveButton;

    public ReorderableList reorderableList;

    public HeaderCallbackDelegate drawHeaderCallback;
    public ChangedCallbackDelegate onChangedCallback;
    public SelectCallbackDelegate onMouseUpCallback;
    public bool displayRemove;
    public AddDropdownCallbackDelegate onAddDropdownCallback;
    public AddCallbackDelegate onAddCallback;
    public SelectCallbackDelegate onSelectCallback;
    public ReorderCallbackDelegate onReorderCallback;
    public ElementHeightCallbackDelegate elementHeightCallback;
    public ElementCallbackDelegate drawElementBackgroundCallback;
    public ElementCallbackDelegate drawElementCallback;
    public FooterCallbackDelegate drawFooterCallback;
    public RemoveCallbackDelegate onRemoveCallback;

    #region Constructors

    public ScrollableList(SerializedObject serializedObject, SerializedProperty elements)
    {
        reorderableList = new ReorderableList(serializedObject, elements);
        InitList(reorderableList);

        displayHeader = displayAddButton = displayRemoveButton = true;
    }

    public ScrollableList(IList elements, Type elementType)
    {
        reorderableList = new ReorderableList(elements, elementType);
        InitList(reorderableList);

        displayHeader = displayAddButton = displayRemoveButton = true;
    }

    public ScrollableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
    {
        reorderableList = new ReorderableList(serializedObject, elements, draggable, false, false, false);
        InitList(reorderableList);

        this.displayHeader = displayHeader;
        this.displayAddButton = displayAddButton;
        this.displayRemoveButton = displayRemoveButton;
    }
    public ScrollableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton)
    {
        reorderableList = new ReorderableList(elements, elementType, draggable, false, false, false);
        InitList(reorderableList);

        this.displayHeader = displayHeader;
        this.displayAddButton = displayAddButton;
        this.displayRemoveButton = displayRemoveButton;
    }

    private void InitList(ReorderableList reorderableList)
    {
        reorderableList.headerHeight = 0;
        reorderableList.footerHeight = 0;

        reorderableList.drawFooterCallback = null;
        reorderableList.drawElementCallback = OnElementCallback;
        reorderableList.onCanRemoveCallback = null;
        reorderableList.onReorderCallback = OnReorderCallback;
        reorderableList.onSelectCallback = OnSelectCallback;
        reorderableList.onAddCallback = OnAddCallback;
        reorderableList.onAddDropdownCallback = OnAddDropdownCallback;
        reorderableList.onRemoveCallback = OnRemoveCallback;
        reorderableList.onChangedCallback = OnChangedCallback;
        reorderableList.drawHeaderCallback = null;
        reorderableList.elementHeightCallback = OnElementHeightCallback;
    }

    private void UpdateCallbacks(ReorderableList reorderableList)
    {
        if (onReorderCallback != null) reorderableList.onReorderCallback = OnReorderCallback;
        else reorderableList.onReorderCallback = null;

        if (onSelectCallback != null) reorderableList.onSelectCallback = OnSelectCallback;
        else reorderableList.onSelectCallback = null;

        if (onAddDropdownCallback != null) reorderableList.onAddDropdownCallback = OnAddDropdownCallback;
        else reorderableList.onAddDropdownCallback = null;

        if (onChangedCallback != null) reorderableList.onChangedCallback = OnChangedCallback;
        else reorderableList.onChangedCallback = null;
        
        if (elementHeightCallback != null) reorderableList.elementHeightCallback = OnElementHeightCallback;
        else reorderableList.elementHeightCallback = null;
        
        if (drawElementCallback != null) reorderableList.drawElementCallback = OnElementCallback;
        else reorderableList.drawElementCallback = null;
    }

    #endregion
    

    public float elementHeight { get { return reorderableList.elementHeight; } set { reorderableList.elementHeight = value; } }
    public static ReorderableList.Defaults defaultBehaviours { get { return ReorderableList.defaultBehaviours; } }
    public SerializedProperty serializedProperty { get { return reorderableList.serializedProperty; } set { reorderableList.serializedProperty = value; } }
    public int count { get { return reorderableList.count; } }
    public bool draggable { get { return reorderableList.draggable; } set { reorderableList.draggable = value; } }
    public int index { get { return reorderableList.index; } set { reorderableList.index = value; } }
    public IList list { get { return reorderableList.list; } set { reorderableList.list = value; } }

    private Vector2 scrollPos;

    public virtual void DoList(float height)
    {
        UpdateCallbacks(reorderableList);

        // Header
        DoListHeader(GUILayoutUtility.GetRect(0, headerHeight, GUILayout.ExpandWidth(true)));
        
        // Scroll calculation
        var scrollRect = GUILayoutUtility.GetRect(0, height - (headerHeight + footerHeight), GUILayout.ExpandWidth(true));
        scrollRect.y -= 2;
        var desiredHeight = Mathf.Max(elementHeight, elementHeight * reorderableList.list.Count);
        var insideRect = new Rect(0, 0, scrollRect.width - (desiredHeight > scrollRect.height ? 15 : 0), desiredHeight);

        // Background correction
        if (Event.current.type == EventType.Repaint && desiredHeight > scrollRect.height)
        {
            var backgroundRect = new Rect(scrollRect);
            backgroundRect.height += 3f;
            ScrollableList.defaultBehaviours.boxBackground.Draw(backgroundRect, false, false, false, false);
        }

        // Scroll List
        var wasType = Event.current.type;
        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, insideRect);
        reorderableList.DoList(insideRect);
        GUI.EndScrollView();

        if(wasType == EventType.ScrollWheel && Event.current.type == EventType.Used)
        {
            bool hasScrolled = true;

            // Detect if scroll has happened
            if (scrollRect.height >= insideRect.height)
                hasScrolled = false;
            else if (scrollPos.y == insideRect.height - scrollRect.height && Event.current.delta.y > 0)
                hasScrolled = false;
            else if (scrollPos.y == 0 && Event.current.delta.y < 0)
                hasScrolled = false;

            // If no real scroll has been performed, we release the event
            if(!hasScrolled) { Event.current.type = wasType; }
        }

        // Background correction
        if (Event.current.type == EventType.Repaint && desiredHeight <= scrollRect.height)
        {
            var backgroundRect = new Rect(scrollRect);
            backgroundRect.y += desiredHeight + 3f;
            backgroundRect.height -= desiredHeight;
            ScrollableList.defaultBehaviours.boxBackground.Draw(backgroundRect, false, false, false, false);
        }
        
        // Footer
        DoListFooter(GUILayoutUtility.GetRect(0, footerHeight, GUILayout.ExpandWidth(true)));
    }

    private void DoListHeader(Rect headerRect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            ReorderableList.defaultBehaviours.DrawHeaderBackground(headerRect);
        }

        headerRect.xMin += 6f;
        headerRect.xMax -= 6f;
        headerRect.height -= 2f;
        headerRect.y += 1f;

        if (this.drawHeaderCallback != null)
        {
            this.drawHeaderCallback(headerRect);
        }
        else if (this.displayHeader)
        {
            ReorderableList.defaultBehaviours.DrawHeader(headerRect, reorderableList.serializedProperty != null ? reorderableList.serializedProperty.serializedObject : null, reorderableList.serializedProperty, reorderableList.list);
        }
    }

    private void DoListFooter(Rect footerRect)
    {
        if (this.drawFooterCallback != null)
        {
            this.drawFooterCallback(footerRect);
        }
        else if (this.displayAddButton || this.displayRemoveButton)
        {
            reorderableList.displayAdd = displayAddButton;
            reorderableList.displayRemove = displayRemoveButton;
            ReorderableList.defaultBehaviours.DrawFooter(footerRect, reorderableList);

            reorderableList.displayAdd = false;
            reorderableList.displayRemove = false;
        }
    }

    public float GetHeight() { return reorderableList.GetHeight(); }
    public void GrabKeyboardFocus() { reorderableList.GrabKeyboardFocus(); }
    public bool HasKeyboardControl() { return reorderableList.HasKeyboardControl(); }
    public void ReleaseKeyboardFocus() { reorderableList.ReleaseKeyboardFocus(); }

    public delegate void FooterCallbackDelegate(Rect rect);
    public delegate void ElementCallbackDelegate(Rect rect, int index, bool isActive, bool isFocused);
    public delegate void ReorderCallbackDelegate(ReorderableList list);
    public delegate void SelectCallbackDelegate(ReorderableList list);
    public delegate void AddCallbackDelegate(ReorderableList list);
    public delegate void AddDropdownCallbackDelegate(Rect buttonRect, ReorderableList list);
    public delegate void RemoveCallbackDelegate(ReorderableList list);
    public delegate void ChangedCallbackDelegate(ReorderableList list);
    public delegate void HeaderCallbackDelegate(Rect rect);
    public delegate float ElementHeightCallbackDelegate(int index);


    private void OnFooterCallback(Rect rect) { drawFooterCallback(rect); }
    private void OnElementCallback(Rect rect, int index, bool isActive, bool isFocused) { drawElementCallback(rect, index, isActive, isFocused); }
    private void OnReorderCallback(ReorderableList list) { onReorderCallback(list); }
    private void OnSelectCallback(ReorderableList list) { onSelectCallback(list); }
    private void OnAddCallback(ReorderableList list) { onAddCallback(list); }
    private void OnAddDropdownCallback(Rect buttonRect, ReorderableList list) { onAddDropdownCallback(buttonRect, list); }
    private void OnRemoveCallback(ReorderableList list) { onRemoveCallback(list); }
    private void OnChangedCallback(ReorderableList list) { onChangedCallback(list); }
    private void OnHeaderCallback(Rect rect) { drawHeaderCallback(rect); }
    private float OnElementHeightCallback(int index) { return elementHeightCallback(index); }
}
