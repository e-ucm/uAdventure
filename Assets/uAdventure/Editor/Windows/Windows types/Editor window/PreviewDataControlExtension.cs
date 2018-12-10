using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class PreviewDataControlExtension : TabsEditorWindowExtension
    {

        private readonly GUIContent[] displayModes;
        private static int selectedDisplayMode = 0;

        private Vector2 scroll;

        private readonly GUIStyle scrollBackground;

        protected PreviewDataControlExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            var size0Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size0", typeof(Texture2D));
            var size1Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size1", typeof(Texture2D));
            var size2Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size2", typeof(Texture2D));

            displayModes = new GUIContent[] { new GUIContent(size0Img), new GUIContent(size1Img), new GUIContent(size2Img) };

            Tabs = new List<KeyValuePair<string, Enum>>();
            Childs = new Dictionary<Enum, LayoutWindow>();

            scrollBackground = new GUIStyle(GUI.skin.box);
        }

        protected override void OnDrawMainView(int aID)
        {
            GUILayout.Space(10);
            // Tabs menu
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            selectedDisplayMode = GUILayout.Toolbar(selectedDisplayMode, displayModes);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            var oldBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
            scroll = EditorGUILayout.BeginScrollView(scroll, scrollBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.backgroundColor = oldBackgroundColor;
            switch (selectedDisplayMode)
            {
                case 0: Display(1, false); break;
                case 1: Display(4, true); break;
                case 2: Display(2, true); break;
            }
            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.EndScrollView();
            GUI.backgroundColor = oldBackgroundColor;
        }
        

        private Dictionary<int, Rect> rects;

        private Rect GetPreviewRect(int index)
        {
            if (rects == null)
                rects = new Dictionary<int, Rect>();

            if (rects.ContainsKey(index))
                return rects[index];

            return Rect.zero;
        }

        private void SetPreviewRect(int index, Rect rect)
        {
            if (rects == null)
                rects = new Dictionary<int, Rect>();

            rects[index] = rect;
        }

        private const float previewHeight = 440;
        private void Display(int columns, bool preview)
        {
            var rect = EditorGUILayout.BeginVertical(GUILayout.Width(m_Rect.width - 32));
            {
                if (Event.current.type == EventType.Repaint) SetPreviewRect(-1, rect);
                rect = GetPreviewRect(-1);

                float columnWidth = Mathf.RoundToInt(rect.width / (float)columns);
                float columnHeight;

                for (int index = 0; index < dataControlList.list.Count; index++)
                {
                    if(index % columns == 0) EditorGUILayout.BeginHorizontal();
                    {
                        if (preview)
                        {
                            columnHeight = previewHeight / columns + 25f;
                            EditorGUILayout.BeginVertical(GUILayout.Width(columnWidth));
                            GUILayout.Label(GetElementName(index), "preToolbar", GUILayout.Width(columnWidth));
                            var previewRect = GUILayoutUtility.GetRect(columnWidth, columnHeight - 25, "preBackground", GUILayout.MaxWidth(columnWidth));
                            // If its an repaint event we store the rect
                            previewRect.height += 25f;
                            if (Event.current.type == EventType.Repaint) SetPreviewRect(index, previewRect);
                            previewRect = GetPreviewRect(index);
                            GUILayout.BeginArea(previewRect, new GUIStyle("preBackground"));
                            {
                                var viewPort = new Rect(previewRect);
                                viewPort.position = new Vector2(0, 5f);
                                viewPort.height -= 30f;
                                OnDrawMainPreview(viewPort, index);
                            }
                            GUILayout.EndArea();
                            if (GUILayout.Button("Edit", EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).button))
                            {
                                dataControlList.index = index;
                                OnSelect(dataControlList.reorderableList);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            GUILayout.Box(GetElementName(index), GUILayout.ExpandWidth(true));
                            if (GUILayout.Button("Edit", GUILayout.Width(150)))
                            {
                                dataControlList.index = index;
                                OnSelect(dataControlList.reorderableList);
                            }
                        }
                    }
                    if (index % columns == columns - 1 || index == dataControlList.count - 1)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }

            }
            EditorGUILayout.EndVertical();
        }

        private string GetElementName(int index)
        {
            var element = dataControlList.list[index] as DataControl;
            var content = element.getContent() as HasId;
            if (content == null)
            {
                return "---";
            }

            return content.getId();
        }

        protected abstract void OnDrawMainPreview(Rect rect, int index);

        public override void SelectElement(List<Searchable> path)
        {
            // On button forces data to refresh
            OnButton();

            var toSelect = path[0];
            for (int i = 0; i < dataControlList.list.Count; i++)
            {
                if(dataControlList.list[i] == toSelect)
                {
                    dataControlList.index = i;
                    break;
                }
            }

            OnSelect(dataControlList.reorderableList);
        }
    }
}

