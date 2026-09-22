using System;
using System.IO;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Geo
{
    public class ResourcesTilePromise : TextureTilePromise
    {
        private readonly string path;

        public ResourcesTilePromise(Vector3d tile, ITileMeta tileMeta, string path, Action<ITilePromise> callback) : base(tile,
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

        private static Texture2D LoadPNG(string resourcePath)
        {
            return Resources.Load<Texture2D>(resourcePath);
        }
    }
}
