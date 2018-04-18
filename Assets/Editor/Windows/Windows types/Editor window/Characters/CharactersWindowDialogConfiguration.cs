using UnityEngine;
using System.Collections;
using UnityEditor;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    [EditorComponent(typeof(NPCDataControl), Name = "NPC.DialogPanelTitle", Order = 15)]
    public class CharactersWindowDialogConfiguration : AbstractEditorComponentWithPreview
    {
        private NPCDataControl workingCharacter;

        private Color fontFrontColor, fontBorderColor, bubbleBcgColor, bubbleBorderColor;

        private bool shouldShowSpeechBubble;

        private GUISkin skinDefault;
        private GUIStyle previewTextStyle;

        private Texture2D bckImage = null;

        public CharactersWindowDialogConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            bckImage = Resources.Load<Texture2D>("EAdventureData/img/TextBubble");
            previewTextStyle = new GUIStyle();
            previewTextStyle.fontSize = 24;
            previewTextStyle.alignment = TextAnchor.MiddleCenter;
            previewTextStyle.border = new RectOffset(32, 32, 32, 32);
            previewTextStyle.padding = new RectOffset(32, 32, 32, 32);
        }

        protected override void DrawInspector()
        {
            workingCharacter = Target != null ? Target as NPCDataControl : Controller.Instance.SelectedChapterDataControl.getNPCsList().getNPCs()[GameRources.GetInstance().selectedCharacterIndex];

            GUILayout.Label(TC.get("Player.TextColor"));

            GUILayout.Space(20);

            // Use Bubbles
            EditorGUI.BeginChangeCheck();
            shouldShowSpeechBubble = GUILayout.Toggle(shouldShowSpeechBubble, TC.get("Player.ShowsSpeechBubble"));
            if (EditorGUI.EndChangeCheck())
                workingCharacter.setShowsSpeechBubbles(shouldShowSpeechBubble);
            

            // Font Color
            EditorGUI.BeginChangeCheck();
            fontFrontColor = EditorGUILayout.ColorField(TC.get("Player.FrontColor"), workingCharacter.getTextFrontColor());
            if (EditorGUI.EndChangeCheck())
            {
                workingCharacter.setTextFrontColor(fontFrontColor);
            }

            // Font Border Color
            EditorGUI.BeginChangeCheck();
            fontBorderColor = EditorGUILayout.ColorField(TC.get("Player.BorderColor"), workingCharacter.getTextBorderColor());
            if (EditorGUI.EndChangeCheck())
            {
                workingCharacter.setTextBorderColor(fontBorderColor);
            }


            using (new EditorGUI.DisabledScope(!shouldShowSpeechBubble))
            {
                // Background color
                EditorGUI.BeginChangeCheck();
                bubbleBcgColor = EditorGUILayout.ColorField(TC.get("Player.BubbleBkgColor"), workingCharacter.getBubbleBkgColor());
                if (EditorGUI.EndChangeCheck())
                {
                    workingCharacter.setBubbleBkgColor(bubbleBcgColor);
                }

                // Border Color
                EditorGUI.BeginChangeCheck();
                bubbleBorderColor = EditorGUILayout.ColorField(TC.get("Player.BubbleBorderColor"), workingCharacter.getBubbleBorderColor());
                if (EditorGUI.EndChangeCheck())
                {
                    workingCharacter.setBubbleBorderColor(bubbleBorderColor);
                }
            }
        }

        private static void DrawPreview(GUIContent content, bool showBackground, Texture2D background, Color backgroundColor, Color borderColor, Color fontColor, Color fontBorder, GUIStyle style)
        {
            var size = style.CalcSize(content);
            var rect = GUILayoutUtility.GetRect(size.x, size.y, style);

            if (showBackground)
            {
                DrawBackgroundBorder(rect, content, background, borderColor, style);
                DrawBackground(rect, content, background, backgroundColor, style);
            }

            DrawTextBorder(rect, content, fontBorder, style);
            DrawText(rect, content, fontColor, style);
        }

        private static void DrawBackgroundBorder(Rect rect, GUIContent content, Texture2D texture, Color borderColor,  GUIStyle style)
        {
            var preColor = GUI.backgroundColor;
            GUI.backgroundColor = borderColor;
            style.normal.background = texture;
            style.normal.textColor = new Color(0, 0, 0, 0);

            if(Event.current.type == EventType.Repaint)
            {
                style.Draw(new Rect(rect.x, rect.y - 3, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x - 3, rect.y, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x, rect.y + 3, rect.width, rect.height), content, false, false, false, false);
                style.Draw(new Rect(rect.x + 3, rect.y, rect.width, rect.height), content, false, false, false, false);
            }
            
            GUI.backgroundColor = preColor;
            style.normal.background = null;
        }

        private static void DrawBackground(Rect rect, GUIContent content, Texture2D texture, Color backgroundColor, GUIStyle style)
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


        private static void DrawTextBorder(Rect rect, GUIContent content, Color fontBorder, GUIStyle style)
        {
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
        }

        private static void DrawText(Rect rect, GUIContent content, Color fontColor, GUIStyle style)
        {
            style.normal.textColor = fontColor;

            if (Event.current.type == EventType.Repaint)
            {
                style.Draw(rect, content, false, false, false, false);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(); GUILayout.FlexibleSpace();
            DrawPreview(new GUIContent(TC.get("GeneralText.PreviewText")), shouldShowSpeechBubble, bckImage, bubbleBcgColor, bubbleBorderColor, fontFrontColor, fontBorderColor, previewTextStyle);
            GUILayout.FlexibleSpace(); GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}