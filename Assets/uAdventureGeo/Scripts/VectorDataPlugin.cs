using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MapzenGo.Helpers;
using UniRx;
using UnityEngine;
using MapzenGo.Models.Plugins;
using System;
using MapzenGo.Models.Factories;
using UnityEngine.Networking;

namespace MapzenGo.Models
{
    public class VectorDataPlugin : Plugin
    {
        public string RelativeCachePath = "../CachedTileData/{0}/";
        protected string CacheFolderPath;

        private List<Plugin> _plugins;

        protected readonly string _mapzenUrl = "http://tile.mapzen.com/mapzen/vector/v1/{0}/{1}/{2}/{3}.{4}?api_key={5}";
        [SerializeField]
        protected string _key = "vector-tiles-5sBcqh6"; //try getting your own key if this doesn't work
        protected string _mapzenLayers;

        protected readonly string _mapzenFormat = "json";

        void Awake()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            CacheFolderPath = Path.Combine(Application.persistentDataPath, RelativeCachePath);
#else
            CacheFolderPath = Path.Combine(Application.dataPath, RelativeCachePath);
#endif

            InitFactories();
            InitLayers();

        }

        private void InitFactories()
        {
            _plugins = new List<Plugin>();
            foreach (var plugin in GetComponentsInChildren<Plugin>())
            {
                _plugins.Add(plugin);
            }
        }

        private void InitLayers()
        {
            var layers = new List<string>();
            foreach (var plugin in _plugins.OfType<Factory>())
            {
                if (layers.Contains(plugin.XmlTag)) continue;
                layers.Add(plugin.XmlTag);
            }
            _mapzenLayers = string.Join(",", layers.ToArray());
        }



        protected override IEnumerator CreateRoutine(Tile tile, Action<bool> finished)
        {
            var url = string.Format(_mapzenUrl, _mapzenLayers, tile.Zoom, tile.TileTms.x, tile.TileTms.y, _mapzenFormat, _key);
            //this is temporary (hopefully), cant just keep adding stuff to filenames

            var zoomFolder = CacheFolderPath.Format(tile.Zoom);
            if (!Directory.Exists(zoomFolder))
                Directory.CreateDirectory(zoomFolder);

            var tilePath = Path.Combine(zoomFolder, _mapzenLayers.Replace(',', '_') + "_" + tile.TileTms.x + "_" + tile.TileTms.y);
            if (File.Exists(tilePath))
            {
                using (var r = new StreamReader(tilePath, Encoding.Default))
                {
                    var mapData = r.ReadToEnd();
                    ConstructTile(mapData, tile, finished);
                }
            }
            else
            {
                var uwr = UnityWebRequest.Get(url);
                yield return uwr.SendWebRequest();

                if (uwr.isHttpError || uwr.isNetworkError)
                {
                    Debug.LogError(uwr.error);
                    finished(false);
                }
                else
                {
                    var sr = File.CreateText(tilePath);
                    sr.Write(uwr.downloadHandler.text);
                    sr.Close();
                    ConstructTile(uwr.downloadHandler.text, tile, finished);
                }
            }

            yield return null;
        }

        protected void ConstructTile(string text, Tile tile, Action<bool> finished)
        {
            var heavyMethod = Observable.Start(() => new JSONObject(text));
            
            heavyMethod.ObserveOnMainThread().Subscribe(mapData =>
            {
                if (tile) // checks if tile still exists and haven't destroyed yet
                    tile.Data = mapData;
                
                finished(true);
            });
        }
    }
}
