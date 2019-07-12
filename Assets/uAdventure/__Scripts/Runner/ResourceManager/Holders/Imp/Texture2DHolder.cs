using UnityEngine;
using System.IO;

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
            loaded = true;
            this.type = type;
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    this.path = path;
                    tex = LoadTexture();
                    break;
                case ResourceManager.LoadingType.SystemIO:
                    this.path = path;
                    this.fileData = LoadBytes(this.path);

                    if (this.fileData == null)
                    {
                        this.fileData = LoadBytes(this.path);

                        if (this.fileData == null)
                        {
                            loaded = false;
                            Debug.Log("No se pudo cargar: " + this.path);
                        }
                    }
                    break;
            }
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
            Texture2D tex = new Texture2D(1, 1);
            switch (type)
            {
                case ResourceManager.LoadingType.ResourcesLoad:
                    tex = Resources.Load(path) as Texture2D;
                    if (tex == null)
                    {
                        loaded = false;
                        Debug.Log("No se pudo cargar: " + this.path);
                    }
                    else
                        loaded = true;

                    break;
                case ResourceManager.LoadingType.SystemIO:
                    if (!Path.HasExtension(path))
                    {
                        foreach(var extension in extensions)
                        {
                            if(File.Exists(path + "." + extension))
                            {
                                path = path + "." + extension;
                                break;
                            }
                        }
                    }

                    tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                    tex.LoadImage(fileData);
                    this.fileData = null;
                    break;
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