using System;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

public class FileChooser : DialogReceiverInterface
{
    protected Texture2D delTex;

    [System.ComponentModel.DefaultValue("")]
    public string Label { get; set; }
    [System.ComponentModel.DefaultValue(true)]
    public bool ShowClear { get; set; }
    [System.ComponentModel.DefaultValue(false)]
    public bool AllowEditingPath { get; set; }
    [System.ComponentModel.DefaultValue("")]
    public string Path { get; set; }
    
    public BaseFileOpenDialog.FileType FileType { get; set; }

    public FileChooser()
    {
        delTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
        Path = "";
        AllowEditingPath = false;
        ShowClear = true;
        Label = "";
    }

    public virtual void DoLayout(params GUILayoutOption[] options)
    {
        var rect = EditorGUILayout.BeginHorizontal(options);
        {
            drawPath();
            drawSelect();
            drawClear();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected virtual void drawPath()
    {
        EditorGUILayout.LabelField(Label, GUILayout.MaxWidth(250));
        using (new EditorGUI.DisabledScope(!AllowEditingPath))
        {
            Path = EditorGUILayout.TextField(Path, GUILayout.ExpandWidth(true));
        }
    }

    protected virtual void drawSelect()
    {
        if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x)))
        {
            ShowAssetChooser(FileType);
        }
    }

    protected virtual void drawClear()
    {
        if (ShowClear)
        {
            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(Path)))
            {
                if (GUILayout.Button(delTex, GUILayout.Width(delTex.width + 10f)))
                {
                    Path = string.Empty;
                }
            }
        }
    }

    void ShowAssetChooser(BaseFileOpenDialog.FileType type)
    {
        BaseFileOpenDialog fileDialog = null;

        switch (type)
        {
            case BaseFileOpenDialog.FileType.PATH:
            case BaseFileOpenDialog.FileType.SCENE_BACKGROUND:
            case BaseFileOpenDialog.FileType.SCENE_FOREGROUND:
            case BaseFileOpenDialog.FileType.EXIT_ICON:
            case BaseFileOpenDialog.FileType.FRAME_IMAGE:
            case BaseFileOpenDialog.FileType.ITEM_ICON:
            case BaseFileOpenDialog.FileType.ITEM_IMAGE:
            case BaseFileOpenDialog.FileType.ITEM_IMAGE_OVER:
            case BaseFileOpenDialog.FileType.SET_ITEM_IMAGE:
            case BaseFileOpenDialog.FileType.BOOK_IMAGE_PARAGRAPH:
            case BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_NORMAL:
            case BaseFileOpenDialog.FileType.BOOK_ARROW_LEFT_OVER:
            case BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_NORMAL:
            case BaseFileOpenDialog.FileType.BOOK_ARROW_RIGHT_OVER:
            case BaseFileOpenDialog.FileType.BUTTON:
            case BaseFileOpenDialog.FileType.BUTTON_OVER:
                fileDialog = ScriptableObject.CreateInstance<ImageFileOpenDialog>();
                break;
            case BaseFileOpenDialog.FileType.SCENE_MUSIC:
            case BaseFileOpenDialog.FileType.CUTSCENE_MUSIC:
            case BaseFileOpenDialog.FileType.EXIT_MUSIC:
            case BaseFileOpenDialog.FileType.FRAME_MUSIC:
            case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_NAME_SOUND:
            case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_DETAILED_SOUND:
            case BaseFileOpenDialog.FileType.NPC_DESCRIPTION_BRIEF_SOUND:
            case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_NAME_SOUND:
            case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_DETAILED_SOUND:
            case BaseFileOpenDialog.FileType.ITEM_DESCRIPTION_BRIEF_SOUND:
            case BaseFileOpenDialog.FileType.PLAY_SOUND_EFFECT:
            case BaseFileOpenDialog.FileType.BUTTON_SOUND:
                fileDialog = ScriptableObject.CreateInstance<MusicFileOpenDialog>();
                break;
            case BaseFileOpenDialog.FileType.CHARACTER_ANIM:
            case BaseFileOpenDialog.FileType.CUTSCENE_SLIDES:
                fileDialog = ScriptableObject.CreateInstance<AnimationFileOpenDialog>();
                break;
            case BaseFileOpenDialog.FileType.CUTSCENE_VIDEO:
                fileDialog = ScriptableObject.CreateInstance<VideoFileOpenDialog>();
                break;
        }

        if (fileDialog)
        {
            fileDialog.Init(this, type);
        }
        else
        {
            Debug.LogError("No window popup for filetype: " + type);
        }

    }

    public void OnDialogCanceled(object workingObject = null)
    {

    }

    public virtual void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
    {
        Path = message;        
    }
}
