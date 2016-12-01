using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;

namespace uAdventure.Editor
{
    public abstract class ReorderableListEditorWindowExtension : EditorWindowExtension
    {
        public GUIContent ButtonContent { get; set; }

        protected ReorderableList reorderableList;
        protected List<string> options;
        protected List<string> Options {
            get
            {
                return options;
            }
            set
            {
                if(value == null || value.Count <= 1)
                {
                    reorderableList.onAddDropdownCallback = null;
                    reorderableList.onAddCallback += OnAdd;
                }
                else
                {
                    reorderableList.onAddDropdownCallback += OnAddDropdown;
                    reorderableList.onAddCallback = null;
                }
            }

        } 
        public float ElementHeith { get
            {
                return reorderableList.elementHeight;
            }
            set
            {
                reorderableList.elementHeight = value;
                this.MenuHeight = ElementHeith * reorderableList.list.Count;
            }
        }

        protected List<string> elements;
        public List<string> Elements
        {
            get { return elements; }
            set {
                elements.Clear();
                elements.AddRange(value);
            }
        }

        public ReorderableListEditorWindowExtension(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions) : base(aStartPos, aContent, aStyle, aOptions)
        {

            elements = new List<string>();

            reorderableList = new ReorderableList(elements, typeof(string));

            reorderableList.headerHeight = 0;
            reorderableList.drawHeaderCallback += DrawHeader;

            reorderableList.footerHeight = 10;
            //reorderableList.drawFooterCallback += DrawFooter;

            reorderableList.elementHeight = 30;
            reorderableList.drawElementCallback += DrawElement;

            reorderableList.onSelectCallback += OnSelect;
            reorderableList.onRemoveCallback += OnRemove;
            reorderableList.onReorderCallback += OnReorder;

            OnSelect(reorderableList);

            Options = new List<string>();
        }

        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            Selected = GUI.Button(rect, ButtonContent, style);
            return Selected;
        }

        public override void DrawMenu(Rect rect, GUIStyle style)
        {
            reorderableList.DoList(rect);
        }

        public override bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options)
        {
            Selected = GUILayout.Button(ButtonContent, style, options);
            return Selected;
        }

        public override void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options)
        {
            reorderableList.DoLayoutList();
        }

        // ---------------------------------------
        //           List operations
        // ----------------------------------------
        #region listOperations
        protected virtual void DrawHeader(Rect rect){}
        protected virtual void DrawFooter(Rect rect){}

        protected virtual void DrawElement(Rect rect, int index, bool b, bool b2)
        {
            var oldName = reorderableList.list[index] as string;
            var newName = GUI.TextField(rect, oldName);
            if (oldName != newName) OnElementNameChanged(reorderableList, index, newName);
        }
        protected abstract void OnElementNameChanged(ReorderableList r, int index, string newName);

        protected abstract void OnButton();
        protected abstract void OnAdd(ReorderableList r);
        protected abstract void OnAddOption(ReorderableList r, string option);
        protected abstract void OnSelect(ReorderableList r);
        protected abstract void OnRemove(ReorderableList r);
        protected abstract void OnReorder(ReorderableList r);

        protected virtual void OnAddDropdown(Rect r, ReorderableList rl)
        {
            var menu = new GenericMenu();
            options.ForEach( s => menu.AddItem(new GUIContent(s), false, optionCallback, s));
            menu.ShowAsContext();
        }

        // Helper
        private void optionCallback(object option) { OnAddOption(reorderableList, (string)option); }
        #endregion
    }
}
