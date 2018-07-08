using UnityEngine;
using System.IO;

namespace uAdventure.Runner
{
    public class AudioHolder : Resource
    {
        private readonly string path;
        private readonly AudioClip audioClip;
        private readonly ResourceManager.LoadingType type;
        
        private bool loaded = false;

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
            this.type = type;
            switch (type)
            {
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    this.path = path;
                    audioClip = LoadAudio(path, type);
                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
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
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    loadedClip = Resources.Load<AudioClip>(path);
                    if (loadedClip == null)
                    {
                        Debug.Log("Couldn't load: " + path);
                    }

                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
                    Debug.LogError("Not yet implemented");
                    break;
            }

            return loadedClip;
        }
    }
}