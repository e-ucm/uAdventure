using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using uAdventure.Core;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace uAdventure.Runner
{
    public sealed class ResourceManagerFactory
    {
        public static ResourceManager CreateLocal(string resourcesFolder = "CurrentGame/", ResourceManager.LoadingType loadingType = ResourceManager.LoadingType.ResourcesLoad)
        {
            var resourceManager = new ResourceManager(loadingType);
            if(loadingType == ResourceManager.LoadingType.SystemIO)
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
            return new ResourceManager(ResourceManager.LoadingType.SystemIO) { Path = path };
        }

    }

    public sealed class ResourceManager
    {

        
        public enum LoadingType
        {
            SystemIO,
            ResourcesLoad
        }

        //##################################################
        //################# IMPLEMENTATION #################
        //##################################################

        private string path = "";
        private readonly LoadingType type = LoadingType.ResourcesLoad;
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
                    case LoadingType.SystemIO:
                        ret = path;
                        break;
                    case LoadingType.ResourcesLoad:
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
            {
                return null;
            }

            if (images.ContainsKey(uri))
            {
                return images[uri].Sprite;
            }
            else
            {
                var holder = new Texture2DHolder(fixPath(uri), type);
                holder.Load();
                if (holder.Loaded())
                {
                    //images.Add(uri, holder);
                    return holder.Sprite;
                }
                else
                {
                    // Load from defaults
                    holder = new Texture2DHolder(defaultPath(uri), type);
                    holder.Load();
                    if (holder.Loaded())
                    {
                        Debug.Log(uri + " loaded from defaults...");
                        //images.Add(uri, holder);
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

        public IAsyncOperation<Sprite> getSpriteAsync(string uri, bool loadFromDefaults)
        {
            var result = new AsyncCompletionSource<Sprite>();
            if (string.IsNullOrEmpty(uri))
            {
                result.SetResult(null);
            }
            else
            {
                if (images.ContainsKey(uri))
                {
                    result.SetResult(images[uri].Sprite);
                }
                else
                {
                    var holder = new Texture2DHolder(fixPath(uri), type);
                    holder.LoadAsync()
                        .Then(done =>
                        {
                            if (done)
                            {
                                images.Add(uri, holder);
                                result.SetResult(holder.Sprite);
                            }
                            else if (loadFromDefaults)
                            {
                                // Load from defaults
                                holder = new Texture2DHolder(defaultPath(uri), type);
                                if (holder.Loaded())
                                {
                                    Debug.Log(uri + " loaded from defaults...");
                                    images.Add(uri, holder);
                                    result.SetResult(holder.Sprite);
                                }
                                else
                                {
                                    Debug.LogWarning("Unable to load " + uri);
                                    result.SetResult(null);
                                }
                            }
                            else
                            {
                                result.SetResult(null);
                            }
                        });
                }
            }
            return result;
        }

        public Texture2D getImage(string uri)
        {
            return getImage(uri, true);
        }

        public IAsyncOperation<Texture2D> getImageAsync(string uri)
        {
            return getImageAsync(uri, true);
        }

        public Texture2D getImage(string uri, bool loadFromDefaults)
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
                var holder = new Texture2DHolder(fixPath(uri), type);
                holder.Load();
                if (holder.Loaded())
                {
                    images.Add(uri, holder);
                    return holder.Texture;
                }
                else if(loadFromDefaults)
                {
                    // Load from defaults
                    holder = new Texture2DHolder(defaultPath(uri), type);
                    holder.Load();
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
                else
                {
                    return null;
                }
            }
        }

        public IAsyncOperation<Texture2D> getImageAsync(string uri, bool loadFromDefaults)
        {
            var result = new AsyncCompletionSource<Texture2D>();
            if (string.IsNullOrEmpty(uri))
            {
                result.SetResult(null);
            }
            else
            {
                if (images.ContainsKey(uri))
                {
                    result.SetResult(images[uri].Texture);
                }
                else
                {
                    var holder = new Texture2DHolder(fixPath(uri), type);
                    holder.LoadAsync()
                        .Then(done =>
                        {
                            Debug.Log("Done loading " + uri);
                            if (done)
                            {
                                if (!images.ContainsKey(uri))
                                {
                                    images.Add(uri, holder);
                                }
                                result.SetResult(holder.Texture);
                            }
                            else if (loadFromDefaults)
                            {
                                // Load from defaults
                                holder = new Texture2DHolder(defaultPath(uri), type);
                                if (holder.Loaded())
                                {
                                    Debug.Log(uri + " loaded from defaults...");
                                    if (!images.ContainsKey(uri))
                                    {
                                        images.Add(uri, holder); 
                                    }
                                    result.SetResult(holder.Texture);
                                }
                                else
                                {
                                    Debug.LogWarning("Unable to load " + uri);
                                    result.SetResult(null);
                                }
                            }
                            else
                            {
                                result.SetResult(null);
                            }
                        });
                }
            }
            return result;
        }

        public void ClearImage(string uri)
        {
            if (images.ContainsKey(uri))
            {
                Texture2D.Destroy(images[uri].Sprite);
                Resources.UnloadAsset(images[uri].Texture);
                images[uri].Texture = new Texture2D(1, 1);
                images.Remove(uri);
               
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
                var holder = new AudioHolder(fixPath(uri), type);
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
            {
                return null;
            }

            if (animations.ContainsKey(uri))
            {
                return animations[uri];
            }
            else
            {
                eAnim animation = new eAnim(uri, type);
                if (animation.Loaded())
                {
                    animations.Add(uri, animation);
                    return animation;
                }
                else
                {
                    animations.Add(uri, null);
                    return null;
                }
            }
        }

        /*public IAsyncOperation<eAnim> getAnimationAsync(string uri)
        {
            var request = new AsyncCompletionSource<eAnim>();

            if (string.IsNullOrEmpty(uri))
            {
                request.SetResult(null);
            }

            if (animations.ContainsKey(uri))
            {
                request.SetResult(animations[uri]);
            }
            else
            {
                eAnim animation = new eAnim(uri, type);
                if (animation.Loaded())
                {
                    animations.Add(uri, animation);
                    return animation;
                }
                else
                {
                    return null;
                }
            }

            return request;
        }*/

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
                case LoadingType.ResourcesLoad:
                    
                    var ta = Resources.Load<TextAsset>(uri); 
                    if (ta == null)
                    {
                        Debug.Log("Can't load Descriptor file: " + uri);
                        return "";
                    }
                    else
                    {
                        xml = ta.text;
                    }
                    break;
                case LoadingType.SystemIO:
                    if(System.IO.File.Exists(uri))
                    {
                        xml = System.IO.File.ReadAllText(uri);
                    }

                    break;
            }

            return xml;
        }

        public IAsyncOperation<string> getTextAsync(string uri)
        {
            UnityEngine.Debug.Log("Getting Text Async");
            var result = new AsyncCompletionSource<string>();

            uri = fixPath(uri);

            switch (getLoadingType())
            {
                case LoadingType.ResourcesLoad:

                    var resourceRequest = Resources.LoadAsync<TextAsset>(uri);
                    resourceRequest.completed += (done) =>
                    {
                        UnityEngine.Debug.Log("Done Getting Text Async");
                        if (resourceRequest.asset == null)
                        {
                            Debug.Log("Can't load Descriptor file: " + uri);
                            result.SetResult(null);
                        }
                        else
                        {
                            result.SetResult((resourceRequest.asset as TextAsset).text);
                        }
                    };
                    
                    break;
                case LoadingType.SystemIO:
                    if (System.IO.File.Exists(uri))
                    {
                        var xml = System.IO.File.ReadAllText(uri);
                        result.SetResult(xml);
                    }
                    else
                    {
                        result.SetResult(null);
                    }

                    break;
            }

            return result;
        }

        private string fixPath(string uri)
        {
            var pattern = new Regex("[óñ]");
            uri = pattern.Replace(uri, "+¦");

            if (!uri.StartsWith(Path))
            {
                uri = Path + uri;
            }

            if(type == LoadingType.ResourcesLoad)
            {
                if (uri.StartsWith("Assets/uAdventure/Resources/"))
                {
                    uri = uri.Remove(0, "Assets/uAdventure/Resources/".Length);
                }

                if (uri.StartsWith("Assets/Resources/"))
                {
                    uri = uri.Remove(0, "Assets/Resources/".Length);
                }

                if (uri.StartsWith("Resources/"))
                {
                    uri = uri.Remove(0, "Resources/".Length);
                }

                if (System.IO.Path.HasExtension(uri))
                {
                    uri = uri.RemoveFromEnd(System.IO.Path.GetExtension(uri));
                }
            }

            return uri;
        }

        private string defaultPath(string uri)
        {
            var pattern = new Regex("[óñ]");
            uri = pattern.Replace(uri, "+¦");

            if(type == LoadingType.SystemIO)
            {
                // Default asset location for SystemIO
                uri = getCurrentDirectory() + "Assets/uAdventure/Resources/EAdventureData/" + uri;
            }

            if (type == LoadingType.ResourcesLoad)
            {
                if (uri.StartsWith("Assets/uAdventure/Resources/"))
                {
                    uri = uri.Remove(0, "Assets/uAdventure/Resources/".Length);
                }

                if (uri.StartsWith("Assets/Resources/"))
                {
                    uri = uri.Remove(0, "Assets/Resources/".Length);
                }

                if (uri.StartsWith("Resources/"))
                {
                    uri = uri.Remove(0, "Resources/".Length);
                }

                if (System.IO.Path.HasExtension(uri))
                {
                    uri = uri.RemoveFromEnd(System.IO.Path.GetExtension(uri));
                }
            }

            return uri;
        }

        public string getCurrentDirectory()
        {
            string ret = "";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    ret = "/mnt/sdcard/uAdventure"; //Application.persistentDataPath;
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

        public IAsyncOperation CacheAssets(IEnumerable<string> assets)
        {
            Debug.Log("CachingAssets: " + assets.Count());
            var result = new AsyncCompletionSource();

            var i = 0;
            var total = assets.Count();
            System.Action step = () =>
            {
                Debug.Log("Step: " + i + " (" + total + ")");
                i++;
                result.SetProgress(Mathf.Clamp01(i / (float) total));
                if (i == total)
                {
                    result.SetCompleted();
                }
            };

            foreach(var asset in assets)
            {
                if (asset.EndsWith("eaa.xml") || asset.EndsWith(".eaa"))
                {
                    Debug.Log("Loading Animation Async: " + asset);
                    Loader.LoadAnimationAsync(asset, this, new List<Incidence>())
                        .Then(anim =>
                        {
                            total += anim.getFrames().Count;
                            foreach(var frame in anim.getFrames())
                            {
                                getImageAsync(frame.getImageAbsolutePath()).Then(step);
                            }
                            step();
                        });
                }
                else if (asset.EndsWith(".png") || asset.EndsWith(".jpg") || asset.EndsWith(".ico"))
                {
                    Debug.Log("Loading Image Async: " + asset);
                    getImageAsync(asset).Then(step);
                }
                else
                {
                    step();
                }
                //TODO end the rest of file types
                
            }

            if(total == 0)
            {
                result.SetCompleted();
            }

            return result;
        }
    }
}