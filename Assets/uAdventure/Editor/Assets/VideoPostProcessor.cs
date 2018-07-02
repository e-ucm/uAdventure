using UnityEngine;
using UnityEditor;
using System.Linq;

namespace uAdventure.Editor
{
    public class VideoPostProcessor : AssetPostprocessor
    {
        private static readonly string[] categories = { "/video/" };
        
        void OnPreprocessAsset()
        {
            string lowerCaseAssetPath = assetPath.ToLower();
            bool isuAdventureAsset = lowerCaseAssetPath.Contains("/uadventure/");
            bool shouldInitVideoClipSettings = isuAdventureAsset && categories.Any(c => lowerCaseAssetPath.Contains(c));

            if (shouldInitVideoClipSettings)
            {
                var videoClipImporter = assetImporter as VideoClipImporter;
                if(videoClipImporter == null)
                {
                    return;
                }

                var width = videoClipImporter.GetResizeWidth(VideoResizeMode.OriginalSize);
                var height = videoClipImporter.GetResizeHeight(VideoResizeMode.OriginalSize);
                var ratio = width / (float)height;

                // Default for standalone
                videoClipImporter.defaultTargetSettings = new VideoImporterTargetSettings()
                {
                    enableTranscoding = true,
                    resizeMode = width <= 1920 || height <= 1080 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                    customWidth = ratio > 1 ? 1920 : Mathf.FloorToInt(1080 * ratio),
                    customHeight = ratio < 1 ? 1080 : Mathf.FloorToInt(1920 / ratio),
                    aspectRatio = VideoEncodeAspectRatio.NoScaling,
                    codec = VideoCodec.Auto,
                    spatialQuality = VideoSpatialQuality.HighSpatialQuality,
                    bitrateMode = VideoBitrateMode.High
                };


                // Valid names: 'Default', 'Web', 'Standalone', 'iOS', 'Android', 'WebGL', 'PS4', 'PSP2', 'XBox360', 'XboxOne', 'WP8', or 'WSA'
                // From: https://github.com/Unity-Technologies/UnityCsReference/blob/11bcfd801fccd2a52b09bb6fd636c1ddcc9f1705/artifacts/generated/bindings_old/common/Editor/VideoImporterBindings.gen.cs#L224

                // Android
                videoClipImporter.SetTargetSettings("Android", new VideoImporterTargetSettings()
                {
                    enableTranscoding = true,
                    resizeMode = width <= 1280 || height <= 720 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                    customWidth = ratio > 1 ? 1280 : Mathf.FloorToInt(720 * ratio),
                    customHeight = ratio < 1 ? 720 : Mathf.FloorToInt(1280 / ratio),
                    aspectRatio = VideoEncodeAspectRatio.NoScaling,
                    codec = VideoCodec.Auto,
                    spatialQuality = VideoSpatialQuality.LowSpatialQuality,
                    bitrateMode = VideoBitrateMode.Low
                });

                // iOS
                videoClipImporter.SetTargetSettings("iOS", new VideoImporterTargetSettings()
                {
                    enableTranscoding = true,
                    resizeMode = width <= 1280 || height <= 720 ? VideoResizeMode.OriginalSize : VideoResizeMode.CustomSize,
                    customWidth = ratio > 1 ? 1280 : Mathf.FloorToInt(720 * ratio),
                    customHeight = ratio < 1 ? 720 : Mathf.FloorToInt(1280 / ratio),
                    aspectRatio = VideoEncodeAspectRatio.NoScaling,
                    codec = VideoCodec.Auto,
                    spatialQuality = VideoSpatialQuality.MediumSpatialQuality,
                    bitrateMode = VideoBitrateMode.Medium
                });
            }
        }
    }
}
