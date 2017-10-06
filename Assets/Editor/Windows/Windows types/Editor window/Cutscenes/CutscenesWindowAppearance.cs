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

        private Texture2D clearImg = null;
        private Texture2D slidesPreview = null;
        private Texture2D slidePreviewMovie = null;
        private static Rect previewRect;

        private AppearanceEditor appearanceEditor;
        private CutsceneDataControl workingCutscene;

        private bool canSkipVideo, canSkipVideoLast;

        private FileChooser video, music;
        private AnimationField slides;

        public DataControl Target { get; set; }

        public CutscenesWindowAppearance(Rect aStartPos, GUIContent aContent, GUIStyle aStyle,
            params GUILayoutOption[] aOptions)
            : base(aStartPos, aContent, aStyle, aOptions)
        {
            appearanceEditor = ScriptableObject.CreateInstance<AppearanceEditor>();
            appearanceEditor.height = 160;
            appearanceEditor.onAppearanceSelected = RefreshPathInformation;
            
            slidePreviewMovie = (Texture2D)Resources.Load("EAdventureData/img/icons/video", typeof(Texture2D));

            video = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionVideoscenes"),
                FileType = BaseFileOpenDialog.FileType.CUTSCENE_VIDEO
            };

            slides = new AnimationField()
            {
                Label = TC.get("Resources.DescriptionSlidesceneSlides"),
                FileType = BaseFileOpenDialog.FileType.CUTSCENE_SLIDES
            };

            music = new FileChooser()
            {
                Label = TC.get("Resources.DescriptionSceneMusic"),
                FileType = BaseFileOpenDialog.FileType.CUTSCENE_MUSIC
            };
        }

        protected override void DrawInspector()
        {
            var windowWidth = m_Rect.width;
            var windowHeight = m_Rect.height;
            previewRect = new Rect(0f, 0.5f * windowHeight, windowWidth, windowHeight * 0.45f);
            workingCutscene = Controller.Instance.SelectedChapterDataControl.getCutscenesList().getCutscenes()[GameRources.GetInstance().selectedCutsceneIndex];

            appearanceEditor.Data = workingCutscene;
            appearanceEditor.OnInspectorGUI();

            /*
            * View for videoscene
            */
            if (workingCutscene.isVideoscene())
            {
                // Background chooser
                EditorGUI.BeginChangeCheck();
                video.Path = workingCutscene.getPathToVideo();
                video.DoLayout();
                if (EditorGUI.EndChangeCheck())
                    workingCutscene.setPathToVideo(video.Path);

                EditorGUI.BeginChangeCheck();
                var canSkipVideo = EditorGUILayout.Toggle(new GUIContent(TC.get("Videoscene.Skip.border"), TC.get("Videoscene.Skip.label")), workingCutscene.getCanSkip());
                if (EditorGUI.EndChangeCheck())
                    workingCutscene.setCanSkip(canSkipVideo);
            }
            /*
            * View for slidescene
            */
            else
            {
                // Background chooser
                EditorGUI.BeginChangeCheck();
                slides.Path = workingCutscene.getPathToSlides();
                slides.DoLayout();
                if (EditorGUI.EndChangeCheck())
                    workingCutscene.setPathToSlides(slides.Path);

                // Music chooser
                EditorGUI.BeginChangeCheck();
                music.Path = workingCutscene.getPathToMusic();
                music.DoLayout();
                if (EditorGUI.EndChangeCheck())
                    workingCutscene.setPathToMusic(music.Path);
            }
        }

        public override void DrawPreview(Rect rect)
        {
            var slidesPreview = this.slidesPreview;
            if(Target != null)
            {
                var cutscene = Target as CutsceneDataControl;

                if (cutscene.isVideoscene()) slidesPreview = slidePreviewMovie;
                else
                {
                    var pathPreview = cutscene.getPreviewImage();
                    slidesPreview = pathPreview == null ? null : AssetsController.getImage(pathPreview).texture;
                }
            }

            GUI.DrawTexture(rect, slidesPreview, ScaleMode.ScaleToFit);
        }

        private void RefreshPathInformation(DataControlWithResources dataControl)
        {
            var cutscene = dataControl as CutsceneDataControl;
            
            if (cutscene.isVideoscene())
            {
                slidesPreview = slidePreviewMovie;
            }
            else
            {
                var pathPreview = cutscene.getPreviewImage();
                slidesPreview = pathPreview == null ? null : AssetsController.getImage(pathPreview).texture;
            }
        }
    }
}