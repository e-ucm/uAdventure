using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using UnityEditor;
using UnityEngine;

public class TextWithSoundField : FileChooser {

    private Texture2D audioTex, noAudioTex;

    public string Content { get; set; }

    public TextWithSoundField() : base()
    {
        noAudioTex = Resources.Load<Texture2D>("EAdventureData/img/icons/noAudio");
        audioTex = Resources.Load<Texture2D>("EAdventureData/img/icons/audio");
    }

    public override void DoLayout(params GUILayoutOption[] options)
    {
        var rect = EditorGUILayout.BeginHorizontal(options);
        {
            Content = EditorGUILayout.TextField(Label, Content, GUILayout.ExpandWidth(true));
            drawAudioPath();
            drawSelect();
            drawClear();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void drawAudioPath()
    {
        GUI.DrawTexture(GUILayoutUtility.GetRect(0, 0, GUILayout.ExpandHeight(true), GUILayout.Width(audioTex.width)), string.IsNullOrEmpty(Path) ? noAudioTex : audioTex);
        EditorGUILayout.LabelField(string.IsNullOrEmpty(Path) ? TC.get("Conversations.NoAudio") : Path, GUILayout.Width(120));
    }
}
