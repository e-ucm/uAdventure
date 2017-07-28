using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ScenesWindowAppearance : LayoutWindow
    {
        private SceneDataControl workingScene;

        private Texture2D backgroundPreview = null, foregroundPreview = null;

        private AppearanceEditor appearanceEditor;
        private FileChooser background, foreground, music;

        public ScenesWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshResources;

            // Fields
            background = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneBackground"),
                FileType = BaseFileOpenDialog.FileType.SCENE_BACKGROUND
            };

            foreground = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneForeground"),
                FileType = BaseFileOpenDialog.FileType.SCENE_FOREGROUND
            };

            music = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneMusic"),
                FileType = BaseFileOpenDialog.FileType.SCENE_MUSIC
            };
        }


        public override void Draw(int aID)
        {

            var previousWorkingItem = workingScene;
            workingScene = Controller.Instance.SelectedChapterDataControl.getScenesList().getScenes()[GameRources.GetInstance().selectedSceneIndex];

            // Appearance table
            appearanceEditor.Data = workingScene;
            appearanceEditor.OnInspectorGUI();

            GUILayout.Space(10);

            EditorGUI.BeginChangeCheck();
            background.Path = workingScene.getPreviewBackground();
            background.DoLayout();
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setPreviewBackground(background.Path);
                RefreshResources(workingScene);
            }

            EditorGUI.BeginChangeCheck();
            foreground.Path = workingScene.getPreviewForeground();
            foreground.DoLayout();
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setPreviewForeground(foreground.Path);
                RefreshResources(workingScene);
            }

            EditorGUI.BeginChangeCheck();
            music.Path = workingScene.getPreviewMusic();
            music.DoLayout();
            if (EditorGUI.EndChangeCheck())
                workingScene.setPreviewMusic(music.Path);

            GUILayout.Space(10);
            GUILayout.Label(TC.get("ImageAssets.Preview"), "preToolbar", GUILayout.ExpandWidth(true));
            var rect = EditorGUILayout.BeginVertical("preBackground", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            {
                GUILayout.BeginHorizontal();
                {
                    rect.x += 30; rect.width -= 60;
                    rect.y += 30; rect.height -= 60;

                    if (backgroundPreview)
                        GUI.DrawTexture(rect, backgroundPreview, ScaleMode.ScaleToFit);
                    if (foregroundPreview)
                        GUI.DrawTexture(rect, foregroundPreview, ScaleMode.ScaleToFit);
                }
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void RefreshResources(DataControlWithResources resources)
        {
            var back = workingScene.getPreviewBackground();
            var fore = workingScene.getPreviewForeground();

            backgroundPreview = string.IsNullOrEmpty(back) ? null : AssetsController.getImage(back).texture;
            foregroundPreview = string.IsNullOrEmpty(fore) ? null : AssetsController.getImage(fore).texture;

            if (backgroundPreview && foregroundPreview)
                foregroundPreview = CreateMask(backgroundPreview, foregroundPreview);
        }

        private Texture2D CreateMask(Texture2D background, Texture2D foreground)
        {
            Texture2D toReturn = new Texture2D(background.width, background.height, background.format, false);
            var foregroundPixels = foreground.GetPixels();
            toReturn.SetPixels(background.GetPixels().Select((color, i) => new Color(color.r, color.g, color.b, foregroundPixels[i].r)).ToArray());
            toReturn.Apply();
            return toReturn;
        }
    }
}