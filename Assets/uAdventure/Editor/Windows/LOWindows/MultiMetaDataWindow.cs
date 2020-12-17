using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace uAdventure.Editor
{
	public class MultiMetaDataWindow : ReorderableListEditorWindowExtension
	{

        public delegate void DrawDelegate(int id, SerializedProperty property);
        public DrawDelegate onDraw;
        public SerializedProperty property;

        public MultiMetaDataWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options)
			: base(rect, content, style)
		{
			Options = options;
		}

		public override void Draw(int id)
        {
            if (reorderableList.index == -1)
            {
                GUILayout.Label("Select an element");
                return;
            }

            var prop = property.GetArrayElementAtIndex(reorderableList.index);
            onDraw(id, prop);
        }

        protected override void OnElementNameChanged(UnityEditorInternal.ReorderableList r, int index, string newName)
        {
        }

        protected override void OnAdd(UnityEditorInternal.ReorderableList r)
        {
			property.InsertArrayElementAtIndex(property.arraySize);

		}

        protected override void OnUpdateList(UnityEditorInternal.ReorderableList r)
        {
			var list = new List<string>();
			for (int i = 1; i <= property.arraySize; i++)
            {
				list.Add(property.displayName + "#" + i);
            }
			r.list = list;
        }

        protected override void OnAddOption(UnityEditorInternal.ReorderableList r, string option)
        {
        }

        protected override void OnSelect(UnityEditorInternal.ReorderableList r)
        {
        }

        protected override void OnRemove(UnityEditorInternal.ReorderableList r)
        {
			property.DeleteArrayElementAtIndex(r.index);
			r.index = -1;
        }

        protected override void OnReorder(UnityEditorInternal.ReorderableList r)
        {
        }

        protected override void OnButton()
        {
        }
    }
}