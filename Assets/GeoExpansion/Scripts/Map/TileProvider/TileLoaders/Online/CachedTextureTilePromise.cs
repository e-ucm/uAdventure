using System;
using System.IO;
using UnityEngine;

namespace uAdventure.Geo
{
    public class CachedTextureTilePromise : TextureTilePromise
    {
        private readonly string path;

        public CachedTextureTilePromise(Vector3d tile, ITileMeta tileMeta, string path, Action<ITilePromise> callback) : base(tile,
            tileMeta, callback)
        {
            this.path = path;
        }

        protected override void LoadTile()
        {
            Data = LoadPNG(path);
            Loaded = true;
            callback(this);
        }

        public override bool Update()
        {
            return true;
        }

        private static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            return tex;
        }
    }
}
