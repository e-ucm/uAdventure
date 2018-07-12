using uAdventure.Core;
using uAdventure.Editor;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
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
        public string Empty { get; set; }

        public FileType FileType { get; set; }

        public FileChooser()
        {
            delTex = (Texture2D)Resources.Load("EAdventureData/img/icons/deleteContent", typeof(Texture2D));
            Path = "";
            AllowEditingPath = false;
            ShowClear = true;
            Label = "";
            Empty = string.Empty;
        }

        public virtual void DoLayout(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            {
                DrawPath();
                DrawSelect();
                DrawClear();
            }
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void DrawPath()
        {
            EditorGUILayout.LabelField(Label, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));
            using (new EditorGUI.DisabledScope(!AllowEditingPath))
            {
                Path = EditorGUILayout.TextField(Path, GUILayout.ExpandWidth(true));
            }
        }

        protected virtual void DrawSelect()
        {
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x)))
            {
                ShowAssetChooser(FileType);
            }
        }

        protected virtual void DrawClear()
        {
            if (ShowClear)
            {
                using (new EditorGUI.DisabledScope(Empty.Equals(Path)))
                {
                    if (GUILayout.Button(delTex, GUILayout.Width(delTex.width + 10f)))
                    {
                        Path = Empty;
                    }
                }
            }
        }

        void ShowAssetChooser(FileType type)
        {
            BaseFileOpenDialog fileDialog = null;

            switch (type)
            {
                case FileType.PATH:
                    fileDialog = ScriptableObject.CreateInstance<PathFileOpenDialog>();
                    break;
                case FileType.SCENE_BACKGROUND:
                case FileType.SCENE_FOREGROUND:
                case FileType.EXIT_ICON:
                case FileType.FRAME_IMAGE:
                case FileType.ITEM_ICON:
                case FileType.ITEM_IMAGE:
                case FileType.ITEM_IMAGE_OVER:
                case FileType.SET_ITEM_IMAGE:
                case FileType.BOOK_IMAGE_PARAGRAPH:
                case FileType.BOOK_ARROW_LEFT_NORMAL:
                case FileType.BOOK_ARROW_LEFT_OVER:
                case FileType.BOOK_ARROW_RIGHT_NORMAL:
                case FileType.BOOK_ARROW_RIGHT_OVER:
                case FileType.BUTTON:
                case FileType.BUTTON_OVER:
                    fileDialog = ScriptableObject.CreateInstance<ImageFileOpenDialog>();
                    break;
                case FileType.SCENE_MUSIC:
                case FileType.CUTSCENE_MUSIC:
                case FileType.EXIT_MUSIC:
                case FileType.FRAME_MUSIC:
                case FileType.NPC_DESCRIPTION_NAME_SOUND:
                case FileType.NPC_DESCRIPTION_DETAILED_SOUND:
                case FileType.NPC_DESCRIPTION_BRIEF_SOUND:
                case FileType.ITEM_DESCRIPTION_NAME_SOUND:
                case FileType.ITEM_DESCRIPTION_DETAILED_SOUND:
                case FileType.ITEM_DESCRIPTION_BRIEF_SOUND:
                case FileType.PLAY_SOUND_EFFECT:
                case FileType.BUTTON_SOUND:
                    fileDialog = ScriptableObject.CreateInstance<MusicFileOpenDialog>();
                    break;
                case FileType.PLAY_ANIMATION_EFFECT:
                case FileType.CHARACTER_ANIM:
                case FileType.CUTSCENE_SLIDES:
                    fileDialog = ScriptableObject.CreateInstance<AnimationFileOpenDialog>();
                    break;
                case FileType.CUTSCENE_VIDEO:
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

}