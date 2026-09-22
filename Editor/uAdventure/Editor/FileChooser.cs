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
            delTex = Resources.Load<Texture2D>("EAdventureData/img/icons/deleteContent");
            Path = "";
            AllowEditingPath = false;
            ShowClear = true;
            Label = "";
            Empty = string.Empty;
        }

        public virtual void DoLayout(params GUILayoutOption[] options)
        {
            using (new EditorGUILayout.HorizontalScope(options))
            {
                DrawPathLayout();
                DrawSelectLayout();
                DrawClearLayout();
            }
        }

        public virtual void Do(Rect rect)
        {
            var selectWidth = GUI.skin.button.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x;
            var clearWidth  = delTex.width + 10f;
            var pathWidth   = rect.width - selectWidth - clearWidth;

            var pathRect    = new Rect(rect.x, rect.y, pathWidth, rect.height);
            var selectRect  = new Rect(pathRect.xMax, rect.y, selectWidth, rect.height);
            var clearRect   = new Rect(selectRect.xMax, rect.y, clearWidth, rect.height);

            DrawPath(pathRect);
            DrawSelect(selectRect);
            DrawClear(clearRect);
        }

        protected virtual void DrawPathLayout()
        {
            EditorGUILayout.LabelField(Label, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));
            using (new EditorGUI.DisabledScope(!AllowEditingPath))
            {
                Path = EditorGUILayout.TextField(Path, GUILayout.ExpandWidth(true));
            }
        }

        protected virtual void DrawSelectLayout()
        {
            if (GUILayout.Button(TC.get("Buttons.Select"), GUILayout.Width(GUI.skin.button.CalcSize(new GUIContent(TC.get("Buttons.Select"))).x)))
            {
                ShowAssetChooser(FileType);
            }
        }

        protected virtual void DrawClearLayout()
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

        protected virtual void DrawPath(Rect rect)
        {
            var labelRect = new Rect(rect.x, rect.y, string.IsNullOrEmpty(Label) ? 0 : EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.LabelField(labelRect, Label);
            using (new EditorGUI.DisabledScope(!AllowEditingPath))
            {
                var pathRect = new Rect(labelRect.xMax, rect.y, rect.width, rect.height);
                Path = EditorGUI.TextField(pathRect, Path);
            }
        }

        protected virtual void DrawSelect(Rect rect)
        {
            if (GUI.Button(rect, TC.get("Buttons.Select")))
            {
                ShowAssetChooser(FileType);
            }
        }

        protected virtual void DrawClear(Rect rect)
        {
            if (ShowClear)
            {
                using (new EditorGUI.DisabledScope(Empty.Equals(Path)))
                {
                    if (GUI.Button(rect, delTex))
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
                case FileType.CURSOR:
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
            // Nothing to do on cancel
        }

        public virtual void OnDialogOk(string message, object workingObject = null, object workingObjectSecond = null)
        {
            Path = message;
        }
    }

}