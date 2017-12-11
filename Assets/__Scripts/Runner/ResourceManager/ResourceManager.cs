using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;
using uAdventure.Core;
using System;
using System.Text.RegularExpressions;

namespace uAdventure.Runner
{
    public sealed class ResourceManager
    {

        //#############################################
        //################# SINGLETON #################
        //#############################################

        public enum LoadingType
        {
            SYSTEM_IO,
            RESOURCES_LOAD
        }

        static ResourceManager instance;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        //##################################################
        //################# IMPLEMENTATION #################
        //##################################################

        private string path = "";
        private string name = "";
        LoadingType type = LoadingType.SYSTEM_IO;
        private Dictionary<string, Texture2DHolder> images;
        private Dictionary<string, eAnim> animations;
        private Dictionary<string, MovieHolder> videos;

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
                        ret = name;
                        break;
                }

                return ret;
            }
            set
            {
                switch (type)
                {
                    case LoadingType.SYSTEM_IO:
                        path = value;
                        break;
                    case LoadingType.RESOURCES_LOAD:
                        name = value + "/";
                        break;
                }
            }
        }

        private ResourceManager()
        {
            this.images = new Dictionary<string, Texture2DHolder>();
            this.animations = new Dictionary<string, eAnim>();
            this.videos = new Dictionary<string, MovieHolder>();

            if (Game.Instance != null)
            {
                type = Game.Instance.getLoadingType();
            }
            else
                type = LoadingType.SYSTEM_IO;
        }

        public LoadingType getLoadingType()
        {
            return type;
        }

        public class ResourceImageLoader : ImageLoaderFactory
        {
            private string path_;

            private static byte[] LoadBytes(string filePath)
            {
                byte[] fileData = null;

#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
                if (System.IO.File.Exists(filePath))
                    fileData = System.IO.File.ReadAllBytes(filePath);
#endif

                return fileData;
            }

            public Sprite getImageFromPath(string uri)
            {
                Sprite img = null;
                switch (Instance.getLoadingType())
                {
                    default:
                    case ResourceManager.LoadingType.RESOURCES_LOAD:
                        if (uri.StartsWith("Assets/Resources/"))
                            uri = uri.Remove(0, 17);

                        var parts = uri.Split('.');
                        uri = parts[0];
                        img = Resources.Load<Sprite>(uri);
                        break;
                    case ResourceManager.LoadingType.SYSTEM_IO:
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
                        var bytes = LoadBytes(uri);
                        if (bytes != null)
                        {
                            var tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                            tex.LoadImage(bytes);
                            img = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                        }
#endif
                        break;
                }
                return img;
            }

            public void showErrorDialog(string title, string message)
            {
                throw new NotImplementedException();
            }
        }

        public Texture2D getImage(string uri)
        {
            if (uri == null)
                return null;

            if (images.ContainsKey(uri))
                return images[uri].Texture;
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
                    holder = new Texture2DHolder(Path.Replace("CurrentGame/", "") + uri, type);
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


        public eAnim getAnimation(string uri)
        {
            if (animations.ContainsKey(uri))
                return animations[uri];
            else
            {
                eAnim animation = new eAnim(fixPath(uri), type);
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
            if (videos.ContainsKey(uri))
                return videos[uri];
            else
            {
                MovieHolder holder = new MovieHolder(fixPath(uri), type);
                if (holder.Loaded())
                {
                    videos.Add(uri, holder);
                    return holder;
                }
                else
                    return null;
            }
        }

        public string getText(string uri)
        {
            string xml = "";

            uri = fixPath(uri);
            
            if (uri.EndsWith(".eaa"))
                uri += ".xml";
            else if (!uri.EndsWith(".eaa.xml"))
                uri += ".eaa.xml";

            switch (ResourceManager.Instance.getLoadingType())
            {
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    
                    if (uri.EndsWith(".xml"))
                        uri = uri.Substring(0, uri.Length - 4);

                    TextAsset ta = Resources.Load<TextAsset>(uri);
                    if (ta == null)
                    {
                        Debug.Log("Can't load Descriptor file: " + uri);
                        return "";
                    }
                    else
                        xml = ta.text;
                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
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
                if (uri.StartsWith("Assets/Resources/"))
                    uri = uri.Remove(0, "Assets/Resources/".Length);
                if (uri.StartsWith("Resources/"))
                    uri = uri.Remove(0, "Resources/".Length);
            }

            return uri;
        }

        public bool extracted = false;
        public void extractFile(string file)
        {
            extracted = false;
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
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
#endif
        }

        public string getCurrentDirectory()
        {
            string ret = "";
#if UNITY_ANDROID
		ret = "/mnt/sdcard/uAdventure";//Application.persistentDataPath;
#elif UNITY_IPHONE
		ret = "";
#else
            ret = System.IO.Directory.GetCurrentDirectory();
#endif
            return ret;
        }

        public string getStoragePath()
        {
            string ret = "";
#if UNITY_ANDROID
		ret = "/mnt/sdcard";
#elif UNITY_IPHONE
		ret = "";
#else
            ret = System.IO.Directory.GetCurrentDirectory();
#endif
            return ret;
        }
    }
}