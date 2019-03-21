using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public class DataControlList : ButtonList
    {

        private DataControl dataControl;
        private GetNestedElements getChilds;
        private List<DataControl> childs;

        public delegate List<DataControl> GetNestedElements(DataControl datacontrol);

        public DataControl DataControl { get { return dataControl; } }
        public GetNestedElements GetChilds { get { return getChilds; } }

        /// <summary>
        /// Data setting and nested childs accessor delegate.
        /// </summary>
        /// <param name="dataControl">DataControl to to manipulate in the list.</param>
        /// <param name="getChilds">Child accessor.</param>
        public void SetData(DataControl dataControl, GetNestedElements getChilds)
        {
            this.dataControl = dataControl;
            this.getChilds = getChilds;
            this.childs = getChilds(dataControl);

            this.list = this.childs;
            if (this.list.Count <= this.index)
            {
                this.index = -1;
            }
        }

        public DataControlList() : base(new List<DataControl>(), typeof(DataControl), true, true)
        {
            // ----------------
            // List config
            // ----------------
            onSelectCallback += OnSelect;
            onReorderCallback += OnReorder;

            // ----------------
            // Add button
            // ----------------
            var buttonAdd = new ButtonList.Button();
            var addTex = Resources.Load<Texture2D>("EAdventureData/img/icons/addNode");
            buttonAdd.content = new GUIContent(addTex);
            // Can add
            buttonAdd.onButtonEnabledCallback = (list) => dataControl != null && dataControl.canAddElements() && dataControl.getAddableElements().ToList().Any(e => dataControl.canAddElement(e));
            // Do add
            buttonAdd.onButtonPressedCallback = (rect, list) =>
            {
                var addable = dataControl.getAddableElements().ToList().FindAll(e => dataControl.canAddElement(e));

                if (addable.Count == 1)
                {
                    OnAdd(addable[0]);
                }
                else
                {
                    var menu = new GenericMenu();
                    addable.ForEach(a => menu.AddItem(new GUIContent(TC.get("TreeNode.AddElement" + a)), false, OnAdd, a));
                    menu.ShowAsContext();
                }
            };

            buttons.Add(buttonAdd);

            // ----------------
            // Remove button
            // ----------------
            var buttonDel = new ButtonList.Button();
            var delTex = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteContent");
            buttonDel.content = new GUIContent(delTex);
            // Can remove
            buttonDel.onButtonEnabledCallback = (list) => dataControl != null && list.index >= 0 && childs[list.index].canBeDeleted();
            // DoRemove
            buttonDel.onButtonPressedCallback = (rect, list) => OnRemove();

            buttons.Add(buttonDel);

            // ----------------
            // Duplicate button
            // ----------------
            var buttonDup = new ButtonList.Button();
            var dupTex = Resources.Load<Texture2D>("EAdventureData/img/icons/duplicateNode");
            buttonDup.content = new GUIContent(dupTex);
            // Can duplicate
            buttonDup.onButtonEnabledCallback = (list) => dataControl != null && list.index >= 0 && childs[list.index].canBeDuplicated();
            // Do Duplicate
            buttonDup.onButtonPressedCallback = (rect, list) => OnDuplicate();
            buttons.Add(buttonDup);
        }

        protected virtual void OnAdd(object type)
        {
            dataControl.addElement((int)type, null);
            OnChanged(reorderableList);
        }

        protected virtual void OnRemove()
        {
            dataControl.deleteElement(childs[index], false);
            reorderableList.index = -1;
            OnSelect(reorderableList);
            if(onRemoveCallback != null)
            {
                onRemoveCallback(this.reorderableList);
            }
            OnChanged(reorderableList);
        }

        protected virtual void OnDuplicate()
        {
            dataControl.duplicateElement(childs[index]);
            OnChanged(reorderableList);
        }

        protected void OnChanged(ReorderableList list)
        {
            this.list = childs = getChilds(dataControl);
            while (this.index >= this.list.Count)
                this.index--;
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
            var list = getChilds(dataControl);

            var toPos = r.index;
            var fromPos = list.FindIndex(i => i == r.list[r.index]);

            dataControl.MoveElement(list[fromPos], fromPos, toPos);
        }

        public override void DoList(float height)
        {
            if(dataControl != null)
            {
                var content = dataControl.getContent();
                if (content != null)
                {
                    if (content is Array)
                    {
                        if (((Array)content).Length != reorderableList.list.Count)
                            OnChanged(reorderableList);
                    }
                    else if (content is IList)
                    {
                        if (((IList)content).Count != reorderableList.list.Count)
                            OnChanged(reorderableList);
                    }
                    /* This has obvious performance issues
                    else if (content is IEnumerable)
                    {
                        var count = 0;
                        foreach (var _ in (IEnumerable)content)
                            ++count;

                        if (count != reorderableList.list.Count)
                            OnChanged(reorderableList);
                    }*/
                }
            }

            base.DoList(height);
        }

    }

}