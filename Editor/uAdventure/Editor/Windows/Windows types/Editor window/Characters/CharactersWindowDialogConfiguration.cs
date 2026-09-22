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
        private readonly GUIStyle previewTextStyle;

        private readonly Texture2D bckImage = null;

        public CharactersWindowDialogConfiguration(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {

            bckImage = Resources.Load<Texture2D>("EAdventureData/img/TextBubble");
            previewTextStyle = new GUIStyle();
            previewTextStyle.fontSize = 24;
            previewTextStyle.alignment = TextAnchor.MiddleCenter;
            previewTextStyle.border = new RectOffset(32, 32, 32, 32);
            previewTextStyle.padding = new RectOffset(32, 32, 32, 32);
            PreviewTitle = "FormattedTextAssets.Preview".Traslate();
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

        public static void DrawPreview(GUIContent content, bool showBackground, Texture2D background, Color backgroundColor, Color borderColor, Color fontColor, Color fontBorder, GUIStyle style)
        {
            var size = style.CalcSize(content);
            var rect = GUILayoutUtility.GetRect(size.x, size.y, style);

            if (showBackground)
            {
                GUIUtil.DrawBackgroundBorder(rect, content, background, borderColor, style);
                GUIUtil.DrawBackground(rect, content, background, backgroundColor, style);
            }

            GUIUtil.DrawTextBorder(rect, content, fontBorder, style);
            GUIUtil.DrawText(rect, content, fontColor, style);
        }

        public override void DrawPreview(Rect rect)
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    DrawPreview(new GUIContent(TC.get("GeneralText.PreviewText")), shouldShowSpeechBubble, bckImage, bubbleBcgColor, bubbleBorderColor, fontFrontColor, fontBorderColor, previewTextStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.FlexibleSpace();
            }
        }
    }
}