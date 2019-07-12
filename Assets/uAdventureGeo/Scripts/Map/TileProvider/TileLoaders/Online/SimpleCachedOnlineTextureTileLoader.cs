using System;
using System.IO;
using UnityEngine;

namespace uAdventure.Geo
{
    public class SimpleCachedOnlineTextureTileLoader : ITileLoader
    {
        private readonly ITileMeta tileMeta;
        private readonly long cacheDuration;

        public SimpleCachedOnlineTextureTileLoader(ITileMeta tileMeta, long cacheDuration)
        {
            this.tileMeta = tileMeta;
            this.cacheDuration = cacheDuration;

            TileProvider.Instance.PublishMeta(tileMeta);
        }

        public bool CanLoadTile(Vector3d tile, ITileMeta tileMeta)
        {
            return tileMeta == this.tileMeta || tileMeta.Identifier == this.tileMeta.Identifier ||
                   tileMeta.GetType() == this.tileMeta.GetType();
        }

        public ITilePromise LoadTile(Vector3d tile, ITileMeta tileMeta, Action<ITilePromise> callback)
        {
            var urlTemplate = (string)tileMeta["url-template"];
            var url = string.Format(urlTemplate, tile.z, tile.x, tile.y);

            var cachePath = GetTilePath(url);
            if (TextureExistsInCache(cachePath))
            {
                if (!HasExpired(cachePath, cacheDuration))
                {
                    var cachePromise = new CachedTextureTilePromise(tile, tileMeta, cachePath, callback);
                    cachePromise.Load();
                    return cachePromise;
                }

                // If has expired we delete the file
                File.Delete(cachePath);
            }

            // Otherwise we load it from online
            var onlinePromise = new OnlineTextureTilePromise(tile, tileMeta, url, (tilePromise) =>
            {
                var texture = tilePromise.Data as Texture2D;
                if (tilePromise.Loaded && texture != null)
                {
                    SavePNG(texture, GetTilePath(url));
                }
                callback(tilePromise);
            });

            onlinePromise.Load();
            return onlinePromise;
        }

        private static bool TextureExistsInCache(string tilePath)
        {
            return File.Exists(tilePath);
        }

        private static bool HasExpired(string path, long duration)
        {
            DateTime creation = File.GetCreationTime(path);
            var lifeTime = (DateTime.Now.Millisecond / 1000) - (creation.Millisecond / 1000);
            return lifeTime > duration;
        }

        private static string GetTilePath(string url)
        {
            var path = url;
            var protocol = path.IndexOf("://");
            if (protocol != -1)
            {
                path = path.Substring(protocol + 3);
            }

            var firstBar = path.IndexOf("/");
            var folder = path.Remove(firstBar).Replace(".", "_");
            var tile = path.Substring(firstBar + 1).Replace("/", "_");

            return Application.persistentDataPath + "/" + folder + "/" + tile;
        }

        public static void SavePNG(Texture2D texture, string filePath)
        {
            var bytes = texture.EncodeToPNG();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var file = File.Open(filePath, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
        }
    }
}