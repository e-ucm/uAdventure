using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace uAdventure.Editor
{
    public abstract class ReorderableListEditorWindowExtension : ButtonMenuEditorWindowExtension
    {

        protected ReorderableList reorderableList;
        protected List<string> options;
        protected List<string> CreationOptions {
            get
            {
                return options;
            }
            set
            {
                if(value == null || value.Count <= 1)
                {
                    reorderableList.onAddDropdownCallback = null;
                    reorderableList.onAddCallback = OnAdd;
                }
                else
                {
                    reorderableList.onAddDropdownCallback = OnAddDropdown;
                    reorderableList.onAddCallback = null;
                }
                options = value;
            }

        } 
        public float ElementHeight { get
            {
                return reorderableList.elementHeight;
            }
            set
            {
                reorderableList.elementHeight = value;
                this.MenuHeight = ElementHeight * reorderableList.list.Count;
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

        protected ReorderableListEditorWindowExtension(Rect rect, params GUILayoutOption[] options) : this(rect, null, null, options) { }
        protected ReorderableListEditorWindowExtension(Rect rect, GUIContent content, params GUILayoutOption[] options) : this(rect, content, null, options) { }
        protected ReorderableListEditorWindowExtension(Rect rect, GUIStyle style, params GUILayoutOption[] options) : this(rect, null, style, options) { }
        protected ReorderableListEditorWindowExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            elements = new List<string>();

            reorderableList = new ReorderableList(elements, typeof(string));

            reorderableList.headerHeight = 0;
            reorderableList.drawHeaderCallback += DrawHeader;

            reorderableList.footerHeight = 10;

            reorderableList.elementHeight = 30;
            reorderableList.drawElementCallback += DrawElement;

            reorderableList.onSelectCallback += OnSelect;
			reorderableList.onRemoveCallback += removeCallback;
            reorderableList.onReorderCallback += OnReorder;

            CreationOptions = new List<string>();
            OnSelect(reorderableList);
        }

        public override bool DrawButton(Rect rect, GUIStyle style)
        {
            if (style == null) style = "Button";
            var r = GUI.Button(rect, ButtonContent, style);
            if (r) OnRequestMainView(this);
            return r;
        }

        public override bool LayoutDrawButton(GUIStyle style, params GUILayoutOption[] options)
        {
            if (style == null) style = "Button";

            var r = GUILayout.Button(ButtonContent, style, options);
            if (r) OnRequestMainView(this);
            return r;
        }

        public override void DrawMenu(Rect rect, GUIStyle style)
        {
            reorderableList.DoList(rect);
        }
        

        public override void LayoutDrawMenu(GUIStyle style, params GUILayoutOption[] options)
        {
            OnUpdateList(reorderableList);
            try
            {
                reorderableList.DoLayoutList();
            }catch(System.Exception e)
            {
                Debug.Log(e);
            }
        }

        // ---------------------------------------
        //           List operations
        // ----------------------------------------
        #region listOperations
        protected virtual void DrawHeader(Rect rect){}
        protected virtual void DrawFooter(Rect rect){}

		int editingIndex = -1;
		bool flushed = true;
		string editingName;
        protected virtual void DrawElement(Rect rect, int index, bool active, bool focused)
        {
			if (index == -1)
            {
                return;
            }

			// Backup for selection change
			if (flushed && reorderableList.index == index)
            {
				editingIndex = index;
				flushed = false;
				editingName = reorderableList.list [editingIndex] as string;
			}
			 
			if (index == editingIndex)
			{
				GUI.SetNextControlName ("editingField");
				editingName = EditorGUI.TextField(rect, editingName);
				if (GUI.GetNameOfFocusedControl () == "editingField" 
					&& !(Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return))
                {
					// If the field is selected
					flushed = false;
				}
                else if (!flushed)
                {
					// Name changing
					flushed = true;
					if(editingName != (string) reorderableList.list[editingIndex])
                    {
                        OnElementNameChanged(reorderableList, editingIndex, editingName);
                    }
					editingIndex = -1;
				}
            }
            else
			{
				GUI.Label(rect, reorderableList.list[index] as string);
            }
        }
        protected abstract void OnElementNameChanged(ReorderableList r, int index, string newName);

        protected abstract void OnAdd(ReorderableList r);
        protected abstract void OnUpdateList(ReorderableList r);
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
		private void removeCallback(ReorderableList r){
			OnRemove (r);
			var tmp = r.index;
			if (tmp < r.list.Count - 1)
            {
				r.index = -1;
				OnSelect (r);
				r.index = tmp;
				OnSelect (r);
			}

		}
        private void optionCallback(object option) { OnAddOption(reorderableList, (string)option); }
        #endregion
    }
}
