using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;
using uAdventure.Core;
using System;
using System.Text.RegularExpressions;

namespace uAdventure.Runner
{
    public sealed class ResourceManagerFactory
    {
        public static ResourceManager CreateLocal(string resourcesFolder = "CurrentGame/", ResourceManager.LoadingType loadingType = ResourceManager.LoadingType.RESOURCES_LOAD)
        {
            var resourceManager = new ResourceManager(loadingType);
            if(loadingType == ResourceManager.LoadingType.SYSTEM_IO)
            {
                resourceManager.Path = resourceManager.getCurrentDirectory() + resourcesFolder;
            }
            else
            {
                resourceManager.Path = resourcesFolder;
            }
            return resourceManager;
        }

        public static ResourceManager CreateExternal(string path)
        {
            var resourceManager = new ResourceManager(ResourceManager.LoadingType.SYSTEM_IO);
            resourceManager.Path = path;
            return resourceManager;
        }

    }

    public sealed class ResourceManager
    {

        
        public enum LoadingType
        {
            SYSTEM_IO,
            RESOURCES_LOAD
        }

        //##################################################
        //################# IMPLEMENTATION #################
        //##################################################

        private string path = "";
        LoadingType type = LoadingType.RESOURCES_LOAD;
        private readonly Dictionary<string, Texture2DHolder> images;
        private readonly Dictionary<string, AudioHolder> audios;
        private readonly Dictionary<string, eAnim> animations;
        private readonly Dictionary<string, Core.Animation> otherAnimations;

        public string Path
        {
            get
            {
                string ret = "";

                switch (type)
                {
                    case LoadingType.SYSTEM_IO:
                        ret = path;
                        break;
                    case LoadingType.RESOURCES_LOAD:
                        ret = "CurrentGame/";
                        break;
                }

                return ret;
            }
            set
            {
                path = value;
            }
        }

        public Dictionary<string, Core.Animation> getAnimationsCache()
        {
            return otherAnimations;
        }

        internal ResourceManager(LoadingType loadingType)
        {
            this.images = new Dictionary<string, Texture2DHolder>();
            this.audios = new Dictionary<string, AudioHolder>();
            this.animations = new Dictionary<string, eAnim>();
            this.otherAnimations = new Dictionary<string, Core.Animation>();

            type = loadingType;
        }

        public LoadingType getLoadingType()
        {
            return type;
        }

        public Sprite getSprite(string uri)
        {
            if (uri == null)
                return null;

            if (images.ContainsKey(uri))
                return images[uri].Sprite;
            else
            {
                Texture2DHolder holder = new Texture2DHolder(fixPath(uri), type);
                if (holder.Loaded())
                {
                    images.Add(uri, holder);
                    return holder.Sprite;
                }
                else
                {
                    // Load from defaults
                    holder = new Texture2DHolder(defaultPath(uri), type);
                    if (holder.Loaded())
                    {
                        Debug.Log(uri + " loaded from defaults...");
                        images.Add(uri, holder);
                        return holder.Sprite;
                    }
                    else
                    {
                        Debug.LogWarning("Unable to load " + uri);
                        return null;
                    }
                }
            }
        }

        public Texture2D getImage(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            if (images.ContainsKey(uri))
            {
                return images[uri].Texture;
            }
            else
            {
                Texture2DHolder holder = new Texture2DHolder(fixPath(uri), type);
                if (holder.Loaded())
                {
                    images.Add(uri, holder);
                    return holder.Texture;
                }
                else
                {
                    // Load from defaults
                    holder = new Texture2DHolder(defaultPath(uri), type);
                    if (holder.Loaded())
                    {
                        Debug.Log(uri + " loaded from defaults...");
                        images.Add(uri, holder);
                        return holder.Texture;
                    }
                    else
                    {
                        Debug.LogWarning("Unable to load " + uri);
                        return null;
                    }
                }
            }
        }

        public AudioClip getAudio(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return null;
            }

            if (audios.ContainsKey(uri))
            {
                return audios[uri].AudioClip;
            }
            else
            {
                AudioHolder holder = new AudioHolder(fixPath(uri), type);
                if (holder.Loaded())
                {
                    audios.Add(uri, holder);
                    return holder.AudioClip;
                }
                else
                {
                    // Load from defaults
                    holder = new AudioHolder(defaultPath(uri), type);
                    if (holder.Loaded())
                    {
                        Debug.Log(uri + " loaded from defaults...");
                        audios.Add(uri, holder);
                        return holder.AudioClip;
                    }
                    else
                    {
                        Debug.LogWarning("Unable to load " + uri);
                        return null;
                    }
                }
            }
        }

        public eAnim getAnimation(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return null;
            /*if (uri.EndsWith(".eaa"))
                uri += ".xml";
            else if (!uri.EndsWith(".eaa.xml"))
                uri += ".eaa.xml";*/
            
            if (animations.ContainsKey(uri))
                return animations[uri];
            else
            {
                eAnim animation = new eAnim(uri, type);
                if (animation.Loaded())
                {
                    animations.Add(uri, animation);
                    return animation;
                }
                else
                    return null;
            }
        }

        public MovieHolder getVideo(string uri)
        {
            return new MovieHolder(fixPath(uri), type);
        }

        public string getText(string uri)
        {
            string xml = null;

            uri = fixPath(uri); 

            switch (getLoadingType())
            {
                case LoadingType.RESOURCES_LOAD:
                    
                    TextAsset ta = Resources.Load<TextAsset>(uri); 
                    if (ta == null)
                    {
                        Debug.Log("Can't load Descriptor file: " + uri);
                        return "";
                    }
                    else
                        xml = ta.text;
                    break;
                case LoadingType.SYSTEM_IO:
                    if(System.IO.File.Exists(uri))
                        xml = System.IO.File.ReadAllText(uri);
                    break;
            }

            return xml;
        }

        private string fixPath(string uri)
        {
            Regex pattern = new Regex("[óñ]");
            uri = pattern.Replace(uri, "+¦");

            if (!uri.StartsWith(Path))
                uri = Path + uri;

            if(type == LoadingType.RESOURCES_LOAD)
            {
                if (uri.StartsWith("Assets/uAdventure/Resources/"))
                    uri = uri.Remove(0, "Assets/uAdventure/Resources/".Length);
                if (uri.StartsWith("Assets/Resources/"))
                    uri = uri.Remove(0, "Assets/Resources/".Length);
                if (uri.StartsWith("Resources/"))
                    uri = uri.Remove(0, "Resources/".Length);

                if (System.IO.Path.HasExtension(uri))
                {
                    uri = uri.RemoveFromEnd(System.IO.Path.GetExtension(uri));
                }
            }

            return uri;
        }

        private string defaultPath(string uri)
        {
            Regex pattern = new Regex("[óñ]");
            uri = pattern.Replace(uri, "+¦");

            if(type == LoadingType.SYSTEM_IO)
            {
                // Default asset location for SYSTEM_IO
                uri = getCurrentDirectory() + "Assets/uAdventure/Resources/EAdventureData/" + uri;
            }

            if (type == LoadingType.RESOURCES_LOAD)
            {
                if (uri.StartsWith("Assets/uAdventure/Resources/"))
                    uri = uri.Remove(0, "Assets/uAdventure/Resources/".Length);
                if (uri.StartsWith("Assets/Resources/"))
                    uri = uri.Remove(0, "Assets/Resources/".Length);
                if (uri.StartsWith("Resources/"))
                    uri = uri.Remove(0, "Resources/".Length);

                if (System.IO.Path.HasExtension(uri))
                {
                    uri = uri.RemoveFromEnd(System.IO.Path.GetExtension(uri));
                }
            }

            return uri;
        }

        public bool extracted = false;
        public void extractFile(string file)
        {
            extracted = false;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Debug.LogWarning("Extraction is not allowed in browser!");
                return;
            }


            string[] dir = file.Split(System.IO.Path.DirectorySeparatorChar);
            string filename = dir[dir.Length - 1].Split('.')[0];

            string exportLocation = getCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar + filename;

            ZipUtil.Unzip(file, exportLocation);

            foreach (string f in System.IO.Directory.GetFiles(exportLocation))
            {
                if (!f.Contains(".xml"))
                    System.IO.File.Delete(f);
            }

            string[] tmp;
            foreach (string f in System.IO.Directory.GetDirectories(exportLocation))
            {
                tmp = f.Split(System.IO.Path.DirectorySeparatorChar);
                if (tmp[tmp.Length - 1] != "assets" && tmp[tmp.Length - 1] != "gui")
                    System.IO.Directory.Delete(f, true);
            }

            VideoConverter converter = new VideoConverter();
            foreach (string video in System.IO.Directory.GetFiles(exportLocation + "/assets/video/"))
            {
                converter.Convert(video);
            }
            extracted = true;
        }

        public string getCurrentDirectory()
        {
            string ret = "";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ret = "/mnt/sdcard/uAdventure";//Application.persistentDataPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ret = "";
                    break;
                default:
                    ret = System.IO.Directory.GetCurrentDirectory();
                    break;
            }
            return ret;
        }

        public string getStoragePath()
        {
            string ret = "";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ret = "/mnt/sdcard";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    ret = "";
                    break;
                default:
                    ret = System.IO.Directory.GetCurrentDirectory();
                    break;
            }
            return ret;
        }
    }
}