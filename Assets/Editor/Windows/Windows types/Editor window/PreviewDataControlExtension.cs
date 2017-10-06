using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class PreviewDataControlExtension : DataControlListEditorWindowExtension {

        protected List<KeyValuePair<string, Enum>> Tabs;
        protected Dictionary<Enum, LayoutWindow> Childs;

        protected Enum OpenedWindow;

        protected Enum DefaultOpenedWindow;

        private GUIContent[] displayModes;
        private static int selectedDisplayMode = 0;

        private Vector2 scroll;

        private GUIStyle scrollBackground;
        
        protected void AddTab(string name, Enum identifier, LayoutWindow window)
        {
            Tabs.Add(new KeyValuePair<string, Enum> (name, identifier));
            Childs.Add(identifier, window);
        }

        public PreviewDataControlExtension(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            var size0Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size0", typeof(Texture2D));
            var size1Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size1", typeof(Texture2D));
            var size2Img = (Texture2D)Resources.Load("EAdventureData/img/icons/size2", typeof(Texture2D));

            displayModes = new GUIContent[] { new GUIContent(size0Img), new GUIContent(size1Img), new GUIContent(size2Img) };

            Tabs = new List<KeyValuePair<string, Enum>>();
            Childs = new Dictionary<Enum, LayoutWindow>();

            scrollBackground = new GUIStyle(GUI.skin.box);
            //scrollBackground.normal.background.
            //scrollBackground.normal.background = new Texture2D(1,1);
            //scrollBackground.normal.background.SetPixel(0, 0, Color.black);
        }

        public override void Draw(int aID)
        {
            if (dataControlList.index < 0)
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
                GUI.backgroundColor = new Color(0.5f,0.5f,0.5f);
                scroll = EditorGUILayout.BeginScrollView(scroll, scrollBackground, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUI.backgroundColor = oldBackgroundColor;
                switch (selectedDisplayMode)
                {
                    case 0: Display(1, false); break;
                    case 1: Display(4, true); break;
                    case 2: Display(2, true); break;
                };
                GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
                EditorGUILayout.EndScrollView();
                GUI.backgroundColor = oldBackgroundColor;
            }
            else
            {
                // Tabs menu
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                OpenedWindow = Tabs[GUILayout.Toolbar(Tabs.FindIndex(t => t.Value == OpenedWindow), Tabs.ConvertAll(t => t.Key).ToArray(), GUILayout.ExpandWidth(false))].Value;
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                // Display Window
                var window = Childs[OpenedWindow];
                window.Rect = this.Rect;
                window.Draw(aID);
            }
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
                float columnHeight = 25f;
                for (int index = 0; index < dataControlList.list.Count; index++)
                {
                    if(index % columns == 0) EditorGUILayout.BeginHorizontal();
                    {
                        if (preview)
                        {
                            columnHeight = previewHeight / columns + 25f;
                            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(columnWidth));
                            GUILayout.Label(GetElementName(index), "preToolbar", GUILayout.ExpandWidth(true));
                            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(columnWidth));
                            var previewRect = GUILayoutUtility.GetRect(columnWidth, columnHeight - 25, "preBackground", GUILayout.MaxWidth(columnWidth));
                            // If its an repaint event we store the rect
                            previewRect.height += 25f;
                            if (Event.current.type == EventType.Repaint) SetPreviewRect(index, previewRect);
                            previewRect = GetPreviewRect(index);
                            GUILayout.BeginArea(previewRect, new GUIStyle("preBackground"));
                            {
                                var viewPort = previewRect;
                                viewPort.height -= 25f;
                                OnDrawMainPreview(viewPort, index);
                            }
                            GUILayout.EndArea();
                            if (GUILayout.Button("Edit"))
                            {
                                dataControlList.index = index;
                                OnSelect(dataControlList.reorderableList);
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Box(GetElementName(index), GUILayout.ExpandWidth(true));
                            if (GUILayout.Button("Edit", GUILayout.Width(150)))
                            {
                                dataControlList.index = index;
                                OnSelect(dataControlList.reorderableList);
                            }
                            EditorGUILayout.EndHorizontal();
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
            return "Placeholder";
        }

        public override void OnDrawMoreWindows()
        {
            if (OpenedWindow != null)
            {
                // Display Window
                var window = Childs[OpenedWindow];
                window.Rect = this.Rect;
                window.OnDrawMoreWindows();
            }
        }

        protected override void OnButton()
        {
            dataControlList.index = -1;
            OpenedWindow = DefaultOpenedWindow != null ? DefaultOpenedWindow : Tabs[0].Value;
        }

        protected override void OnSelect(ReorderableList r)
        {
        }

        protected abstract void OnDrawMainPreview(Rect rect, int index);
    }
}

