using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace uAdventure.Geo
{
    public class SimpleCachedOnlineTextureTileLoader : ITileLoader
    {
        private readonly ITileMeta tileMeta;
        private readonly long cacheDuration;

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GetProtocol();
#else
        private static string GetProtocol()
        {
            return "https";
        }
#endif

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
            var url = string.Format(urlTemplate, GetProtocol(), tile.z, tile.x, tile.y);

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
            var readable = CreateReadableTexture(texture);
            var bytes = readable.EncodeToPNG();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            var file = File.Open(filePath, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
            if(Application.isEditor && !Application.isPlaying)
            {
                Texture2D.DestroyImmediate(readable);
            }
            else
            {
                Texture2D.Destroy(readable);
            }
        }

        public static Texture2D CreateReadableTexture(Texture2D texture)
        {
            Debug.Log("Generated Readable: " + texture.name);
            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(texture, tmp);
            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;
            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
            // Create a new readable Texture2D to copy the pixels to it
            Texture2D readableVersion = new Texture2D(texture.width, texture.height);
            // Copy the pixels from the RenderTexture to the new Texture
            readableVersion.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableVersion.Apply();
            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            tmp.Release();
            RenderTexture.ReleaseTemporary(tmp);

            return readableVersion;
        }
    }
}