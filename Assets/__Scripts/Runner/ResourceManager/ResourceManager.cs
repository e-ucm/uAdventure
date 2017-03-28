using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ionic.Zip;

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

        public Texture2D getImage(string uri)
        {
            if (images.ContainsKey(uri))
                return images[uri].Texture;
            else
            {
                Texture2DHolder holder = new Texture2DHolder(Path + uri, type);
                if (holder.Loaded())
                {
                    images.Add(uri, holder);
                    return holder.Texture;
                }
                else
                    return null;
            }
        }

        public LoadingType getLoadingType()
        {
            return type;
        }

        public eAnim getAnimation(string uri)
        {
            if (animations.ContainsKey(uri))
                return animations[uri];
            else
            {
                eAnim animation = new eAnim(Path + uri, type);
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
                MovieHolder holder = new MovieHolder(uri, type);
                if (holder.Loaded())
                {
                    videos.Add(uri, holder);
                    return holder;
                }
                else
                    return null;
            }
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