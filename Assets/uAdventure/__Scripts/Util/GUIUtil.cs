using System;
using UnityEngine;

namespace uAdventure.Core
{
    public class GUIUtil
    {
        public class SkinScope : IDisposable
        {
            private readonly GUISkin previousSkin;

            public SkinScope(GUISkin skin)
            {
                previousSkin = GUI.skin;
                GUI.skin = skin;
            }

            ~SkinScope()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    GUI.skin = previousSkin;
                }
            }
        }



        public static void DrawBackgroundBorder(Rect rect, GUIContent content, Texture2D texture, Color borderColor, GUIStyle style)
        {
            var preColor = GUI.backgroundColor;
            GUI.backgroundColor = borderColor;
            style.normal.background = texture;
            style.normal.textColor = new Color(0, 0, 0, 0);

            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(new Rect(rect.x, rect.y - 3, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x - 3, rect.y, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x, rect.y + 3, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x + 3, rect.y, rect.width, rect.height), content, false, false, false, false);
            }

            GUI.backgroundColor = preColor;
            style.normal.background = null;
        }

        public static void DrawBackground(Rect rect, GUIContent content, Texture2D texture, Color backgroundColor, GUIStyle style)
        {
            var preColor = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;

            style.normal.background = texture;
            style.normal.textColor = new Color(0, 0, 0, 0);
            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(rect, content, false, false, false, false);
            }

            GUI.backgroundColor = preColor;
            style.normal.background = null;
        }


        public static void DrawTextBorder(Rect rect, GUIContent content, Color fontBorder, GUIStyle style)
        {
            var prevColor = style.normal.textColor;
            style.normal.textColor = fontBorder;

            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(new Rect(rect.x, rect.y - 2, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x - 2, rect.y, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x, rect.y + 2, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x + 2, rect.y, rect.width, rect.height), content, false, false, false, false);

                style.Draw(new Rect(rect.x - 1, rect.y - 1, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x + 1, rect.y - 1, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x - 1, rect.y + 1, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), content, false, false, false, false);
            }
            style.normal.textColor = prevColor;
        }

        public static void DrawText(Rect rect, GUIContent content, Color fontColor, GUIStyle style)
        {
            var prevColor = style.normal.textColor;
            style.normal.textColor = fontColor;

            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(rect, content, false, false, false, false);
            }
            style.normal.textColor = prevColor;
        }

    }
}
