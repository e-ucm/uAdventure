
using System;
using UnityEngine;

namespace uAdventure.Geo
{
    public abstract class TextureTilePromise : ITilePromise
    {
        private static Texture2D loadingTexture;

        // Zoom in X, Tile in Y and Z
        protected Texture2D texture;
        protected readonly Action<ITilePromise> callback;
        protected bool loading = false;

        public Vector3d Tile { get; protected set; }
        public ITileMeta Meta { get; protected set; }
        public object Data { get { return Loaded ? texture : loadingTexture; } protected set { texture = value as Texture2D; } }
        public bool Loaded { get; protected set; }

        protected TextureTilePromise(Vector3d tile, ITileMeta meta, Action<ITilePromise> callback)
        {
            if (!loadingTexture)
            {
                loadingTexture = Resources.Load<Texture2D>("tile_loading");
            }

            this.Tile = tile;
            this.Meta = meta;
            this.callback = callback;
        }

        public void Load()
        {
            if (loading)
            {
                return;
            }

            loading = true;

            LoadTile();
        }

        public abstract bool Update();

        protected abstract void LoadTile();
    }
}
