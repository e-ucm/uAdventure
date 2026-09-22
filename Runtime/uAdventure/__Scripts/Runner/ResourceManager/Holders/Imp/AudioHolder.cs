using UnityEngine;
using System.IO;

namespace uAdventure.Runner
{
    public class AudioHolder : Resource
    {
        private readonly AudioClip audioClip;
        
        private readonly bool loaded = false;

        public AudioClip AudioClip
        {
            get
            {
                return audioClip;
            }
        }

        public AudioHolder(string path, ResourceManager.LoadingType type)
        {
            loaded = true;
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    audioClip = LoadAudio(path, type);
                    break;
                case ResourceManager.LoadingType.SystemIO:
                    Debug.LogError("Not yet implemented");
                    break;
            }

            if (audioClip)
            {
                loaded = true;
            }
        }

        public bool Loaded()
        {
            return loaded;
        }

        private static AudioClip LoadAudio(string path, ResourceManager.LoadingType type)
        {
            AudioClip loadedClip = null;
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    loadedClip = Resources.Load<AudioClip>(path);
                    if (loadedClip == null)
                    {
                        Debug.Log("Couldn't load: " + path);
                    }

                    break;
                case ResourceManager.LoadingType.SystemIO:
                    Debug.LogError("Not yet implemented");
                    break;
            }

            return loadedClip;
        }
    }
}