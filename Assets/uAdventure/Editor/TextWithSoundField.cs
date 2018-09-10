using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    public class TextWithSoundField : FileChooser
    {

        private readonly Texture2D audioTex, noAudioTex;

        public string Content { get; set; }

        public TextWithSoundField() : base()
        {
            noAudioTex = Resources.Load<Texture2D>("EAdventureData/img/icons/noAudio");
            audioTex = Resources.Load<Texture2D>("EAdventureData/img/icons/audio");
        }

        public override void DoLayout(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            {
                Content = EditorGUILayout.TextField(Label, Content, GUILayout.ExpandWidth(true));
                DrawAudioPath();
                DrawSelectLayout();
                DrawClearLayout();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected void DrawAudioPath()
        {
            GUI.DrawTexture(GUILayoutUtility.GetRect(0, 0, GUILayout.Height(audioTex.height), GUILayout.Width(audioTex.width)), string.IsNullOrEmpty(Path) ? noAudioTex : audioTex);
            EditorGUILayout.LabelField(string.IsNullOrEmpty(Path) ? TC.get("Conversations.NoAudio") : Path, GUILayout.Width(120));
        }
    }
}
