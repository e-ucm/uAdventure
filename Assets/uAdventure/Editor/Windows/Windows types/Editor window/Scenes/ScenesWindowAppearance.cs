using UnityEngine;

using uAdventure.Core;
using UnityEditor;
using System;
using System.Linq;

namespace uAdventure.Editor
{
    public class ScenesWindowAppearance : SceneEditorWindow
    {
        private SceneDataControl workingScene;

        private readonly AppearanceEditor appearanceEditor;
        private readonly FileChooser background, foreground, music;

        public ScenesWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle, SceneEditor sceneEditor, params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, sceneEditor, aOptions)
        {
            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = sceneEditor.RefreshSceneResources;
            PreviewTitle = "Scene.Preview".Traslate();

            // Fields
            background = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneBackground"),
                FileType = FileType.SCENE_BACKGROUND,
                Empty = SpecialAssetPaths.ASSET_EMPTY_BACKGROUND
            };

            foreground = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneForeground"),
                FileType = FileType.SCENE_FOREGROUND
            };

            music = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneMusic"),
                FileType = FileType.SCENE_MUSIC
            };
        }


        protected override void DrawInspector()
        {
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
                sceneEditor.RefreshSceneResources(workingScene);
            }

            EditorGUI.BeginChangeCheck();
            foreground.Path = workingScene.getPreviewForeground();
            foreground.DoLayout();
            if (EditorGUI.EndChangeCheck())
            {
                workingScene.setPreviewForeground(foreground.Path);
                sceneEditor.RefreshSceneResources(workingScene);
            }

            EditorGUI.BeginChangeCheck();
            music.Path = workingScene.getPreviewMusic();
            music.DoLayout();
            if (EditorGUI.EndChangeCheck())
                workingScene.setPreviewMusic(music.Path);
        }
    }
}