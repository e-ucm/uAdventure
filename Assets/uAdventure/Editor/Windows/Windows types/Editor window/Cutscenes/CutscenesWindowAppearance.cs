using UnityEngine;
using System.Collections;

using uAdventure.Core;
using System.Linq;
using System;
using UnityEditor;

namespace uAdventure.Editor
{
    public class CutscenesWindowAppearance : PreviewLayoutWindow
    {
        private enum AssetType
        {
            SELECT_SLIDES,
            MUSIC,
            VIDEOSCENE
        };

        private readonly ResourcesList appearanceEditor;

        private readonly FileChooser video, music;
        private readonly AnimationField slides;

        public DataControl Target { get; set; }

        public CutscenesWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            appearanceEditor = ScriptableObject.CreateInstance<ResourcesList>();
            appearanceEditor.Height = 160;

            video = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionVideoscenes"),
                FileType = FileType.CUTSCENE_VIDEO
            };

            slides = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionSlidesceneSlides"),
                FileType = FileType.CUTSCENE_SLIDES
            };

            music = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneMusic"),
                FileType = FileType.CUTSCENE_MUSIC
            };
        }

        protected override void DrawInspector()
        {
            var cutscene = GetCurrentCutscene();

            appearanceEditor.Data = cutscene;
            appearanceEditor.OnInspectorGUI();

            /*
            * View for videoscene
            */
            if (cutscene.isVideoscene())
            {
                PreviewTitle = "Videoscenes.Preview".Traslate();
                // Background chooser
                EditorGUI.BeginChangeCheck();
                video.Path = cutscene.getPathToVideo();
                video.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    cutscene.setPathToVideo(video.Path);
                }

                EditorGUI.BeginChangeCheck();
                var canSkipVideo = EditorGUILayout.Toggle(new GUIContent(TC.get("Videoscene.Skip.border"), TC.get("Videoscene.Skip.label")), cutscene.getCanSkip());
                if (EditorGUI.EndChangeCheck())
                {
                    cutscene.setCanSkip(canSkipVideo);
                }
            }
            /*
            * View for slidescene
            */
            else
            {
                PreviewTitle = "Slidescenes.Preview".Traslate();
                // Background chooser
                EditorGUI.BeginChangeCheck();
                slides.Path = cutscene.getPathToSlides();
                slides.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    cutscene.setPathToSlides(slides.Path);
                }

                // Music chooser
                EditorGUI.BeginChangeCheck();
                music.Path = cutscene.getPathToMusic();
                music.DoLayout();
                if (EditorGUI.EndChangeCheck())
                {
                    cutscene.setPathToMusic(music.Path);
                }
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var cutscene = GetCurrentCutscene();
            var previewPath = cutscene.getPreviewImage();
            if (!string.IsNullOrEmpty(previewPath))
            {
                var previewToDraw = Controller.ResourceManager.getImage(previewPath);
                if (previewToDraw)
                {
                    GUI.DrawTexture(rect, previewToDraw, ScaleMode.ScaleToFit);
                }
            }
        }

        protected CutsceneDataControl GetCurrentCutscene()
        {
            var cutscene = Target as CutsceneDataControl;
            if (cutscene == null)
            {
                cutscene = Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];
            }
            return cutscene;
        }
    }
}