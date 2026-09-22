using UnityEngine;
using System.IO;
using System.Linq;
using UnityFx.Async;
using UnityFx.Async.Promises;

namespace uAdventure.Runner
{
    public class Texture2DHolder : Resource
    {
        private static string[] extensions = { ".png", ".jpg", ".jpeg" };

        byte[] fileData;
        string path;
        private Texture2D tex;
        private Sprite sprite;

        bool loaded = false;
        ResourceManager.LoadingType type;
        public Sprite Sprite
        {
            get
            {
                if (!sprite && tex || (sprite && sprite.texture != tex))
                { 
                    sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0.5f, 0.5f));
                }

                return sprite;
            }
        }
        public Texture2D Texture
        {
            get
            {
                if (!tex)
                {
                    tex = LoadTexture();
                }

                return tex;
            }
            set { tex = value; }
        }

        // ##################################################
        // ################## CONSTRUCTORS ##################
        // ##################################################

        public Texture2DHolder(byte[] data)
        {
            this.fileData = data;
        }

        public Texture2DHolder(string path, ResourceManager.LoadingType type)
        {
            this.type = type;
            this.path = path;
        }

        public bool Load()
        {
            tex = LoadTexture();
            loaded = tex != null;
            return loaded;
        }

        public IAsyncOperation<bool> LoadAsync()
        {
            var result = new AsyncCompletionSource<bool>();
            LoadTextureAsync()
                .Then(texture =>
                {
                    tex = texture;
                    loaded = tex != null;
                    result.SetResult(tex);
                });
            return result;
        }

        public bool Loaded()
        {
            return loaded;
        }

        // #####################################################
        // ################## LOADING METHODS ##################
        // #####################################################

        private Texture2D LoadTexture()
        {
            Texture2D tex = null;
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    tex = Resources.Load(path) as Texture2D;
                    if (tex == null)
                    {
                        Debug.Log("No se pudo cargar: " + this.path);
                    }

                    break;

                case ResourceManager.LoadingType.SystemIO:
                    tex = ReadFromFile(path);

                    break;
            }

            return tex;
        }

        private IAsyncOperation<Texture2D> LoadTextureAsync()
        {
            var result = new AsyncCompletionSource<Texture2D>();
            Texture2D tex = null;
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    var resourceRequest = Resources.LoadAsync<Texture2D>(path);
                    resourceRequest.completed += done =>
                    {
                        tex = resourceRequest.asset as Texture2D;
                        if (tex == null)
                        {
                            Debug.Log("No se pudo cargar: " + this.path);
                        }
                        result.SetResult(tex);
                    };

                    break;

                case ResourceManager.LoadingType.SystemIO:
                    tex = ReadFromFile(path);
                    result.SetResult(tex);

                    break;
            }

            return result;
        }

        private Texture2D ReadFromFile(string path)
        {
            if (!Path.HasExtension(path))
            {
                path = extensions
                    .Select(ex => path + "." + ex)
                    .Where(File.Exists)
                    .FirstOrDefault();
            }

            this.fileData = LoadBytes(this.path);
            if (this.fileData == null)
            {
                Debug.Log("No se pudo cargar: " + this.path);
            }
            else
            {
                tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                tex.LoadImage(fileData);
                this.fileData = null;
            }

            return tex;
        }

        private static byte[] LoadBytes(string filePath)
        {
            byte[] fileData = null;

            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (System.IO.File.Exists(filePath))
                    fileData = System.IO.File.ReadAllBytes(filePath);
            }

            return fileData;
        }
    }
}