using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public abstract class PreviewLayoutWindow : LayoutWindow
    {
        public PreviewLayoutWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
        }

        public override void Draw(int aID)
        {
            DrawInspector();

            DrawPreviewHeader();
            var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    rect.x += 30; rect.width -= 60;
                    rect.y += 30; rect.height -= 60;

                    DrawPreview(rect);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        protected abstract void DrawInspector();

        protected virtual void DrawPreviewHeader()
        {
            GUILayout.Space(10);
            GUILayout.Label(TC.get("ImageAssets.Preview"), "preToolbar", GUILayout.ExpandWidth(true));
        }

        protected virtual void DrawPreview(Rect rect)
        {

        }
    }
}