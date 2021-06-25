using System;
using UnityEngine;
using UnityEngine.Networking;

namespace uAdventure.Geo
{
    class OnlineTextureTilePromise : TextureTilePromise
    {
        private static readonly int MaxTries = 5;

        private readonly string url;
        private UnityWebRequestAsyncOperation request;
        private bool completed;
        private int tries = 0;

        public OnlineTextureTilePromise(Vector3d tile, ITileMeta tileMeta, string url, Action<ITilePromise> callback) : base(tile, tileMeta, callback)
        {
            this.url = url;
        }

        protected override void LoadTile()
        {
            tries++;
            completed = false;
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, false);
            request = www.SendWebRequest();
            request.completed += Request_completed;
        }

        public override bool Update()
        {
            if (request != null && request.isDone && !completed)
            {
                Request_completed(request);
            }
            return completed;
        }

        private void Request_completed(AsyncOperation obj)
        {
            if (completed)
            {
                return;
            }

            completed = true;
            var currRequest = obj as UnityWebRequestAsyncOperation;
            if (currRequest.webRequest.isNetworkError || currRequest.webRequest.isHttpError )
            {
                if (tries < MaxTries)
                {
                    Debug.Log("Retrying download tile " + Tile + " at " + url + ": (" + currRequest.webRequest.responseCode + ") " + currRequest.webRequest.error);
                    LoadTile();
                }
                else
                {
                    Data = null;
                    Loaded = true;
                    callback(this);
                }
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(currRequest.webRequest);
                var reducedSize = new Texture2D(128, 128, TextureFormat.RGBA32, false);
                Graphics.ConvertTexture(texture, reducedSize);
                Texture2D.DestroyImmediate(texture);
                Data = reducedSize;
                Loaded = true;
                callback(this);
            }
        }
    }
}
