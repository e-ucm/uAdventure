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
        private GUISkin skin;

        public PreviewLayoutWindow(Rect rect, GUIContent content, GUIStyle style, params GUILayoutOption[] options) : base(rect, content, style, options)
        {
            skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        }

        private Rect windowRect;
        private Rect previewRect;

        public override void OnDrawMoreWindows()
        {

            if (HasToDrawPreviewInspector())
            {
                var prevSkin = GUI.skin;
                GUI.skin = skin;

                windowRect = GUILayout.Window(9999, windowRect, (i) =>
                {
                    //GUI.DragWindow();
                    DrawPreviewInspector();
                }, "Properties", skin.window);

                GUI.BringWindowToFront(9999);
                
                windowRect.center = previewRect.position + previewRect.size - (windowRect.size/2f) - (Vector2.one * 10);

                GUI.skin = prevSkin;
            }

        }

        public override void Draw(int aID)
        {
            DrawInspector();

            DrawPreviewHeader();
            var auxRect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    previewRect.x += 30; previewRect.width -= 60;
                    previewRect.y += 30; previewRect.height -= 60;

                    DrawPreview(auxRect);
                }
                GUILayout.EndHorizontal();
            }

            if (auxRect.x != 0)
            {
                previewRect = auxRect;
                previewRect.center += m_Rect.position;
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

        protected virtual bool HasToDrawPreviewInspector()
        {
            return false;
        }

        protected virtual void DrawPreviewInspector() { }
    }
}