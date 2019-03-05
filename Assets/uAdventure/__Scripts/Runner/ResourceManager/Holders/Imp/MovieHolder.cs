﻿using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using System.IO;

namespace uAdventure.Runner
{
    public class MovieHolder
    {
        private static string[] extensions = { ".asf",".avi",".dv",".m4v",".mov",".mp4",".mpg",".mpeg",".ogv",".vp8",".webm",".wmv" };

        public RenderTexture Movie { get { return videoPlayer.targetTexture; } }
        private string error = string.Empty;
        public string Error { get { return error; } }

        private VideoPlayer videoPlayer;
        
        string path;
        private ResourceManager.LoadingType type;

        // ##################################################
        // ################## CONSTRUCTORS ##################
        // ##################################################

        public MovieHolder(string path, ResourceManager.LoadingType type)
        {
            this.type = type;

            var videoPlayerHolder = new GameObject("VideoPlayer-" + System.IO.Path.GetFileNameWithoutExtension(path));
            videoPlayer = videoPlayerHolder.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
            videoPlayer.errorReceived += VideoPlayer_errorReceived;

            this.path = path;
            LoadVideo();
        }

        private void VideoPlayer_errorReceived(VideoPlayer source, string message)
        {
            error = message;
            videoPlayer.errorReceived -= VideoPlayer_errorReceived;
        }

        public void Clean()
        {
            videoPlayer.Stop();
            videoPlayer.targetTexture.Release();
            GameObject.Destroy(videoPlayer.gameObject);
        }

        public bool Loaded()
        {
            return videoPlayer.isPrepared;
        }

        // #####################################################
        // ################## CONTROL METHODS ##################
        // #####################################################


        public bool isPlaying()
        {
            return videoPlayer.isPlaying;
        }

        public bool isError()
        {
            return !string.IsNullOrEmpty(error);
        }

        public void Play()
        {
            videoPlayer.Play();
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        // #####################################################
        // ################## LOADING METHODS ##################
        // #####################################################
        

        private void LoadVideo()
        {
            switch (type)
            {
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        videoPlayer.source = VideoSource.Url;
                        videoPlayer.url = Application.streamingAssetsPath + "/" + path + ".webm";
                        videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RGB565);
                    }
                    else
                    {
                        videoPlayer.source = VideoSource.VideoClip;
                        videoPlayer.clip = Resources.Load<VideoClip>(path);
                        if (videoPlayer.clip)
                        {
                            videoPlayer.targetTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 16, RenderTextureFormat.RGB565);
                        }
                        else
                        {
                            videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RGB565);
                        }
                    }

                    if (videoPlayer.source == VideoSource.VideoClip && videoPlayer.clip == null)
                    {
                        error = "Video not found: " + this.path;
                        Debug.Log("No se pudo cargar: " + this.path);
                    }

                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
                    if (!Path.HasExtension(path))
                    {
                        foreach (var extension in extensions)
                        {
                            if (File.Exists(path + "." + extension))
                            {
                                path = path + "." + extension;
                                break;
                            }
                        }
                    }
                    
                    videoPlayer.source = VideoSource.Url;
                    videoPlayer.url = "file://" + path;
                    videoPlayer.targetTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.RGB565);
                    break;
            }

            videoPlayer.targetTexture.Create();
            videoPlayer.Prepare();
        }
    }
}